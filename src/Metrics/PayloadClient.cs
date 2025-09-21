using System;
using System.Collections.Generic;
using System.Linq;
using Hygrometer.InfluxDB.Collector.Model;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

namespace Hygrometer.InfluxDB.Collector.Metrics
{
    public class PayloadClient
    {
        private readonly CollectorConfiguration configuration;
        private readonly InfluxDBClient influxDBClient;
        private readonly List<PointData> pointDataList;

        public PayloadClient(CollectorConfiguration configuration)
        {
            this.configuration = configuration;
            this.pointDataList = [];

            var builder = new InfluxDBClientOptions.Builder();
            builder.Url(configuration.InfluxDbUrl);
            builder.Org(configuration.InfluxDbOrg);
            builder.Bucket(configuration.InfluxDbBucket);
            builder.AuthenticateToken(configuration.InfluxDbAuthenticateToken);

            this.influxDBClient = new InfluxDBClient(builder.Build());
        }

        public void AddAndTrySendPayload(IEnumerable<SensorData> sensorDataEnum)
        {
            var sensorDataList = sensorDataEnum.ToList();
            this.AddPayload(sensorDataList);
            this.TrySendPayload(sensorDataList.Count);
        }

        private void AddPayload(List<SensorData> sensorDataList)
        {
            foreach (var sensorData in sensorDataList)
            {
                var pointData = PointData.Measurement($"{this.configuration.InfluxDbMeasurement}")
                                        .Tag("sensor", sensorData.SensorName)
                                        .Field("temperature_C", sensorData.DegreesCelsius)
                                        .Timestamp(DateTime.UtcNow, WritePrecision.S);

                if (sensorData.Hectopascals.HasValue)
                {
                    pointData = pointData.Field("hectopascal_H", sensorData.Hectopascals.Value);
                }

                if (sensorData.HumidityInPercent.HasValue)
                {
                    pointData = pointData.Field("humidity_P", sensorData.HumidityInPercent.Value);
                }

                this.pointDataList.Add(pointData);
            }
        }

        private void TrySendPayload(int sensorCount)
        {
            if (this.pointDataList.Count >= (this.configuration.MinimumDataPoints * sensorCount))
            {
                try
                {
                    using var writeApi = this.influxDBClient.GetWriteApi();
                    writeApi.WritePoints(this.pointDataList);
                    writeApi.Flush();

                    this.pointDataList.Clear();
                    ConsoleLogger.Debug("InfluxDb write operation completed successfully");
                }
                catch (Exception e)
                {
                    ConsoleLogger.Error(e);
                }
            }
        }
    }
}
