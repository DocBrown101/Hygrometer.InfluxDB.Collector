using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Si7021;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Si7021SensorReader : ISensorReader
    {
        private readonly Si7021 sensor;
        private readonly string sensorName;

        public Si7021SensorReader()
        {
            var i2cSettings = new I2cConnectionSettings(1, Si7021.DefaultI2cAddress); // Default in HEX: 40
            var i2cDevice = I2cDevice.Create(i2cSettings);
            this.sensor = new Si7021(i2cDevice, Resolution.Resolution1);
            this.sensorName = this.sensor.GetType().Name;
        }

        public Task<SensorData> GetSensorData()
        {
            var sensorData = new SensorData(this.sensorName)
            {
                DegreesCelsius = this.sensor.Temperature.DegreesCelsius,
                HumidityInPercent = this.sensor.Humidity.Percent
            };
            return Task.FromResult(sensorData);
        }
    }
}
