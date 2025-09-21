using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Bmxx80.ReadResult;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Bmp280SensorReader : ISensorReader
    {
        private readonly Bmp280 sensor;
        private readonly string sensorName;

        public Bmp280SensorReader(int busId = 1, int deviceAddress = Bmp280.DefaultI2cAddress) // Default in HEX: 77
        {
            var i2cSettings = new I2cConnectionSettings(busId, deviceAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            this.sensor = new Bmp280(i2cDevice)
            {
                PressureSampling = Sampling.Standard,
                TemperatureSampling = Sampling.UltraHighResolution
            };
            this.sensor.SetPowerMode(Bmx280PowerMode.Normal);
            this.sensorName = this.sensor.GetType().Name;
        }

        public async Task<SensorData> GetSensorData()
        {
            Bmp280ReadResult result;

            do
            {
                result = await this.sensor.ReadAsync().ConfigureAwait(false);

                if (!result.Temperature.HasValue || !result.Pressure.HasValue)
                {
                    ConsoleLogger.Debug($"Warning, sensor data from {this.sensorName} could not be read! Trying again...");
                    await Task.Delay(100).ConfigureAwait(false);
                }
            } while (!result.Temperature.HasValue || !result.Pressure.HasValue);

            return new SensorData(this.sensorName)
            {
                DegreesCelsius = result.Temperature.Value.DegreesCelsius,
                Hectopascals = result.Pressure.Value.Hectopascals
            };
        }
    }
}
