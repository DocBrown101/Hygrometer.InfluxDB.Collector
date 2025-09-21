using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Sht3x;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Sht3xSensorReader : ISensorReader
    {
        private readonly Sht3x sensor;

        public Sht3xSensorReader(int busId = 1, byte deviceAddress = (byte)I2cAddress.AddrLow) // Default in HEX: 44
        {
            var settings = new I2cConnectionSettings(busId, deviceAddress);
            var device = I2cDevice.Create(settings);
            this.sensor = new Sht3x(device)
            {
                Resolution = Resolution.High
            };
        }

        public Task<SensorData> GetSensorData()
        {
            var sensorData = new SensorData(nameof(this.sensor))
            {
                DegreesCelsius = this.sensor.Temperature.DegreesCelsius,
                HumidityInPercent = this.sensor.Humidity.Percent
            };

            return Task.FromResult(sensorData);
        }
    }
}
