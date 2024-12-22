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
                var tasks = new List<Task> { Task.Delay(TimeSpan.FromSeconds(this.configuration.IntervalSeconds), ct) };

                foreach (var sensorReader in this.sensorReaders)
                {
                    if (output == OutputSettingEnum.Influx)
                    {
                        tasks.Add(this.CreateSensorPayloadTask(sensorReader));
                    }
                    else
                    {
                        tasks.Add(CreateSensorConsoleTask(sensorReader));
                    }
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        private async Task CreateSensorPayloadTask(ISensorReader sensorReader)
        {
            var sensorData = await sensorReader.GetSensorData().ConfigureAwait(false);

            ConsoleLogger.Debug($"Sensor data received: {sensorData.SensorType}");

            this.payloadClient.AddAndTrySendPayload(sensorData);
        }

        private static async Task CreateSensorConsoleTask(ISensorReader sensorReader)
        {
            var sensorData = await sensorReader.GetSensorData().ConfigureAwait(false);
            var sb = new StringBuilder($"{sensorData.SensorType} -> Temperature: {sensorData.DegreesCelsius} °C");

            ConsoleLogger.Debug($"Sensor data received: {sensorData.SensorType}");

            if (sensorData.Hectopascals.HasValue)
            {
                sb.Append($", Pressure: {sensorData.Hectopascals} hPa");
            }
            if (sensorData.HumidityInPercent.HasValue)
            {
                sb.Append($", Humidity: {sensorData.HumidityInPercent} %");
            }

            Console.WriteLine(sb.ToString());
            Console.WriteLine("---");
        }
    }
}
