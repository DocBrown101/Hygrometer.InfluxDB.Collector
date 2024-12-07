using System;

namespace Hygrometer.InfluxDB.Collector.Model
{
    public class SensorData(SensorType sensorType)
    {
        public SensorType SensorType { get; } = sensorType;

        private double degreesCelsius;
        public double DegreesCelsius
        {
            get => this.degreesCelsius;
            set => this.degreesCelsius = Math.Round(value, 1);
        }

        private double? humidityInPercent;
        public double? HumidityInPercent
        {
            get => this.humidityInPercent;
            set => this.humidityInPercent = value.HasValue ? Math.Round(value.Value, 1) : null;
        }

        private double? hectopascals;
        public double? Hectopascals
        {
            get => this.hectopascals;
            set => this.hectopascals = value.HasValue ? Math.Round(value.Value, 1) : null;
        }
    }
}
