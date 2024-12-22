using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Bmxx80.ReadResult;
using UnitsNet;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Bme280SensorReader : ISensorReader
    {
        private readonly Bme280 sensor;

        /// <summary>
        /// Required parameters for an I2C communication
        /// </summary>
        /// <param name="busId">Default busId</param>
        /// <param name="deviceAddress">Default device I2C address (i2cdetect -y 1)</param>
        public Bme280SensorReader(int busId = 1, int deviceAddress = Bme280.DefaultI2cAddress)
        {
            var i2cSettings = new I2cConnectionSettings(busId, deviceAddress);
            var i2cDevice = I2cDevice.Create(i2cSettings);
            this.sensor = new Bme280(i2cDevice)
            {
                PressureSampling = Sampling.Standard,
                HumiditySampling = Sampling.Standard,
                TemperatureSampling = Sampling.UltraHighResolution
            };
        }

        public async Task<SensorData> GetSensorData()
        {
            this.sensor.SetPowerMode(Bmx280PowerMode.Forced);

            Temperature? temperature = null;
            RelativeHumidity? humidity = null;
            Pressure? pressure = null;
            bool isLastReadSuccessful;

            do
            {
                var result = await this.sensor.ReadAsync().ConfigureAwait(false);
                isLastReadSuccessful = IsLastReadSuccessful(result);

                if (isLastReadSuccessful)
                {
                    temperature = result.Temperature.Value;
                    humidity = result.Humidity.Value;
                    pressure = result.Pressure.Value;
                    break;
                }
                else
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

            } while (!isLastReadSuccessful);

            return new SensorData(SensorType.BME280) {
                DegreesCelsius = temperature.Value.DegreesCelsius,
                HumidityInPercent = humidity.Value.Percent, 
                Hectopascals = pressure.Value.Hectopascals };
        }

        private static bool IsLastReadSuccessful(Bme280ReadResult result)
        {
            if (result.Temperature.HasValue && result.Humidity.HasValue && result.Pressure.HasValue)
            {
                return true;
            }

            return false;
        }
    }
}
