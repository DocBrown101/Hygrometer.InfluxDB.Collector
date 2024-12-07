using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace Hygrometer.InfluxDB.Collector.Metrics
{
    public class PayloadClient
    {
        private readonly MetricsConfiguration configuration;
        private readonly InfluxDBClient influxDBClient;
        private readonly List<PointData> pointDataList;

        public PayloadClient(MetricsConfiguration configuration)
        {
            this.configuration = configuration;
            this.pointDataList = new List<PointData>();

            var builder = new InfluxDBClientOptions.Builder();
            builder.Url(configuration.InfluxDbUrl);
            builder.Org(configuration.InfluxDbOrg);
            builder.Bucket(configuration.InfluxDbBucket);
            builder.AuthenticateToken(configuration.InfluxDbAuthenticateToken);

            this.influxDBClient = new InfluxDBClient(builder.Build());
        }

        public async Task AddAndTrySendPayload(SensorData sensorData)
        {
            await Task.CompletedTask;

            this.AddPayload(sensorData);
            this.TrySendPayload();
        }

        private void AddPayload(SensorData sensorData)
        {
            var payloadDateTime = DateTime.UtcNow;

            switch (sensorData.SensorType)
            {
                case SensorType.BME280:
                case SensorType.BMP280:
                    this.pointDataList.Add(PointData.Measurement($"{this.configuration.InfluxDbMeasurement}")
                        .Tag("device", this.configuration.Device)
                        .Tag("sensor", sensorData.SensorType.ToString())
                        .Field("temperature_C", sensorData.DegreesCelsius)
                        .Field("hectopascal_H", sensorData.Hectopascals)
                        .Timestamp(payloadDateTime, WritePrecision.Ms));
                    break;
                case SensorType.DHT22:
                    this.pointDataList.Add(PointData.Measurement($"{this.configuration.InfluxDbMeasurement}")
                        .Tag("device", this.configuration.Device)
                        .Tag("sensor", sensorData.SensorType.ToString())
                        .Field("temperature_C", sensorData.DegreesCelsius)
                        .Field("humidity_P", sensorData.HumidityInPercent)
                        .Timestamp(payloadDateTime, WritePrecision.Ms));
                    break;
            }
        }

        private void TrySendPayload()
        {
            if (this.pointDataList.Count >= this.configuration.MinimumDataPoints)
            {
                try
                {
                    using (var writeApi = this.influxDBClient.GetWriteApi())
                    {
                        writeApi.WritePoints(this.pointDataList);
                        writeApi.Flush();

                        this.pointDataList.Clear();
                        ConsoleLogger.Debug("InfluxDb write operation completed successfully");
                    }
                }
                catch (Exception ex)
                {
                    ConsoleLogger.Error(ex.Message);

                    if (ex.InnerException != null)
                    {
                        ConsoleLogger.Error(ex.InnerException.Message);
                    }
                }
            }
        }
    }
}
