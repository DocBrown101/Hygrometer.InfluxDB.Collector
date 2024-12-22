using System;
using System.Collections.Generic;
using Hygrometer.InfluxDB.Collector.Model;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public static class SensorFactory
    {
        public static IList<ISensorReader> GetSensors(IList<SensorType> sensorTypes)
        {
            var sensorList = new List<ISensorReader>();

            foreach (SensorType sensorType in sensorTypes)
            {
                sensorList.AddRange(GetSensor(sensorType));
            }

            return sensorList;
        }

        private static IList<ISensorReader> GetSensor(SensorType sensorType)
        {
            try
            {
                return sensorType switch
                {
                    SensorType.TEST => [new TestSensorReader()],
                    SensorType.BME280 => [new Bme280SensorReader()],
                    SensorType.BMP280 => [new Bmp280SensorReader()],
                    SensorType.DHT22 => [new Dht22SensorReader()],
                    SensorType.SHT4x => [new Sht4xSensorReader()],
                    SensorType.SI7021 => [new Si7021SensorReader()],
                    _ => throw new ArgumentException("Not supported!"),
                };
            }
            catch
            {
                ConsoleLogger.Error($"SensorFactory for {sensorType}");
                throw;
            }
        }
    }
}
