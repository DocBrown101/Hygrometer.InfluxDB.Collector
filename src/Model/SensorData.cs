using System;

namespace Hygrometer.InfluxDB.Collector.Model
{
    public class SensorData(string sensorType)
    {
        public string SensorType { get; } = sensorType;

        public double DegreesCelsius { get; set; }

        public double? HumidityInPercent { get; set; }

        public double? Hectopascals { get; set; }
    }
}
