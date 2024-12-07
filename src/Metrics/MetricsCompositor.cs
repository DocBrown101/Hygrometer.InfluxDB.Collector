using Hygrometer.InfluxDB.Collector.Model;
using Hygrometer.InfluxDB.Collector.Sensors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hygrometer.InfluxDB.Collector.Metrics
{
    public class MetricsCompositor
    {
        private readonly IList<ISensorReader> sensorReaders;
        private readonly MetricsConfiguration configuration;
        private readonly PayloadClient payloadClient;

        public MetricsCompositor(IList<ISensorReader> sensorReaders, MetricsConfiguration configuration)
        {
            this.sensorReaders = sensorReaders;
            this.configuration = configuration;
            this.payloadClient = new PayloadClient(configuration);
        }

        public async Task StartMetricCollectionLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var tasks = new List<Task> { Task.Delay(TimeSpan.FromSeconds(this.configuration.IntervalSeconds), ct) };

                foreach (var sensorReader in this.sensorReaders)
                {
                    tasks.Add(this.CreateSensorPayloadTask(sensorReader));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        public async Task WriteSensorDataToConsole(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var tasks = new List<Task> { Task.Delay(TimeSpan.FromSeconds(1), ct) };

                foreach (var sensorReader in this.sensorReaders)
                {
                    tasks.Add(this.CreateSensorConsoleTask(sensorReader));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        private async Task<Task> CreateSensorPayloadTask(ISensorReader sensorReader)
        {
            var sensorData = await sensorReader.GetSensorData().ConfigureAwait(false);

            return this.payloadClient.AddAndTrySendPayload(sensorData);
        }

        private async Task CreateSensorConsoleTask(ISensorReader sensorReader)
        {
            var sensorData = await sensorReader.GetSensorData().ConfigureAwait(false);
            var sb = new StringBuilder($"{sensorData.SensorType} -> Temperature: {sensorData.DegreesCelsius} °C");

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
