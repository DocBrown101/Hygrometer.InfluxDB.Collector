using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Bmxx80.ReadResult;
using System.Device.I2c;
using System.Threading.Tasks;
using UnitsNet;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Bmp280SensorReader : ISensorReader
    {
        private readonly Bmp280 sensor;

        /// <summary>
        /// Required parameters for an I2C communication
        /// </summary>
        /// <param name="busId">Default busId</param>
        /// <param name="deviceAddress">Default device I2C address (i2cdetect -y 1)</param>
        public Bmp280SensorReader(int busId = 1, int deviceAddress = Bmp280.DefaultI2cAddress)
        {
            var i2cSettings = new I2cConnectionSettings(busId, deviceAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            this.sensor = new Bmp280(i2cDevice)
            {
                PressureSampling = Sampling.Standard,
                TemperatureSampling = Sampling.UltraHighResolution
            };
        }

        public async Task<SensorData> GetSensorData()
        {
            this.sensor.SetPowerMode(Bmx280PowerMode.Forced);

            Temperature? temperature = null;
            Pressure? pressure = null;
            bool isLastReadSuccessful;

            do
            {
                var result = await this.sensor.ReadAsync().ConfigureAwait(false);
                isLastReadSuccessful = IsLastReadSuccessful(result);

                if (isLastReadSuccessful)
                {
                    temperature = result.Temperature.Value;
                    pressure = result.Pressure.Value;
                    break;
                }
                else
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

            } while (!isLastReadSuccessful);

            return new SensorData(SensorType.BMP280)
            {
                DegreesCelsius = temperature.Value.DegreesCelsius,
                Hectopascals = pressure.Value.Hectopascals
            };
        }

        private static bool IsLastReadSuccessful(Bmp280ReadResult result)
        {
            if (result.Temperature.HasValue && result.Pressure.HasValue)
            {
                return true;
            }

            return false;
        }
    }
}
