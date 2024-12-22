using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Si7021;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Si7021SensorReader : ISensorReader
    {
        private readonly Si7021 sensor;

        public Si7021SensorReader()
        {
            var i2cSettings = new I2cConnectionSettings(1, Si7021.DefaultI2cAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            this.sensor = new Si7021(i2cDevice, Resolution.Resolution1);
        }

        public Task<SensorData> GetSensorData()
        {
            var sensorData = new SensorData(SensorType.SI7021)
            {
                DegreesCelsius = this.sensor.Temperature.DegreesCelsius,
                HumidityInPercent = this.sensor.Humidity.Percent
            };
            return Task.FromResult(sensorData);
        }
    }
}
