using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Hygrometer.InfluxDB.Collector.Sensors;

namespace Hygrometer.InfluxDB.Collector.Metrics
{
    public class MetricsCompositor(IList<ISensorReader> sensorReaders, CollectorConfiguration configuration)
    {
        private readonly IList<ISensorReader> sensorReaders = sensorReaders;
        private readonly CollectorConfiguration configuration = configuration;
        private readonly PayloadClient payloadClient = new(configuration);

        public async Task StartMetricCollectionLoop(OutputSettingEnum output, CancellationToken ct)
        {
            ConsoleLogger.Info($"Collect Metrics ...");

            while (!ct.IsCancellationRequested)
            {
                var delayTasks = Task.Delay(TimeSpan.FromSeconds(this.configuration.IntervalSeconds), ct);
                var sensorTasks = new List<Task<SensorData>>();

                foreach (var sensorReader in this.sensorReaders)
                {
                    sensorTasks.Add(sensorReader.GetSensorData());
                }

                var sensorData = await Task.WhenAll(sensorTasks).ConfigureAwait(false);

                ConsoleLogger.Debug($"Sensor data received!");

                if (output == OutputSettingEnum.Influx)
                {
                    this.payloadClient.AddAndTrySendPayload(sensorData);
                }
                else
                {
                    WriteToConsole(sensorData);
                }

                await delayTasks.ConfigureAwait(false);
            }
        }

        private static void WriteToConsole(IEnumerable<SensorData> sensorDataList)
        {
            var sb = new StringBuilder();

            foreach (var sensorData in sensorDataList)
            {
                sb.Append($"{sensorData.SensorType} -> Temperature: {sensorData.DegreesCelsius:F1} °C");

                if (sensorData.HumidityInPercent.HasValue)
                {
                    sb.Append($", Humidity: {sensorData.HumidityInPercent:F0} %");
                }
                if (sensorData.Hectopascals.HasValue)
                {
                    sb.Append($", Pressure: {sensorData.Hectopascals} hPa");
                }
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
            Console.WriteLine("---");
        }
    }
}
