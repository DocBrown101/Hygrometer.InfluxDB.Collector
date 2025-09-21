using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Bmxx80.ReadResult;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Bme280SensorReader : ISensorReader
    {
        private readonly Bme280 sensor;
        private readonly string sensorName;

        public Bme280SensorReader(int busId = 1, int deviceAddress = Bme280.DefaultI2cAddress) // Default in HEX: 77
        {
            var i2cSettings = new I2cConnectionSettings(busId, deviceAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            this.sensor = new Bme280(i2cDevice)
            {
                PressureSampling = Sampling.Standard,
                HumiditySampling = Sampling.Standard,
                TemperatureSampling = Sampling.Standard
            };
            this.sensor.SetPowerMode(Bmx280PowerMode.Normal);
            this.sensorName = this.sensor.GetType().Name;
        }

        public async Task<SensorData> GetSensorData()
        {
            Bme280ReadResult result;

            do
            {
                result = await this.sensor.ReadAsync().ConfigureAwait(false);

                if (!result.Temperature.HasValue || !result.Humidity.HasValue || !result.Pressure.HasValue)
                {
                    ConsoleLogger.Debug($"Warning, sensor data from {this.sensorName} could not be read! Trying again...");
                    await Task.Delay(100).ConfigureAwait(false);
                }
            } while (!result.Temperature.HasValue || !result.Humidity.HasValue || !result.Pressure.HasValue);

            return new SensorData(this.sensorName)
            {
                DegreesCelsius = result.Temperature.Value.DegreesCelsius + 0.7,
                HumidityInPercent = result.Humidity.Value.Percent + 3.5,
                Hectopascals = result.Pressure.Value.Hectopascals
            };
        }
    }
}
