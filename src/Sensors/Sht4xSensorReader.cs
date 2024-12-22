using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Sht4x;
using UnitsNet;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Sht4xSensorReader : ISensorReader
    {
        private readonly Sht4x sensor;

        public Sht4xSensorReader(int busId = 1, int deviceAddress = Sht4x.DefaultI2cAddress)
        {
            var settings = new I2cConnectionSettings(busId, deviceAddress);
            var device = I2cDevice.Create(settings);
            this.sensor = new Sht4x(device);
        }

        public async Task<SensorData> GetSensorData()
        {
            Temperature? temperature = null;
            RelativeHumidity? humidity = null;
            bool isLastReadSuccessful;

            do
            {
                (var hum, var temp) = await this.sensor.ReadHumidityAndTemperatureAsync().ConfigureAwait(false);
                isLastReadSuccessful = IsLastReadSuccessful(hum, temp);

                if (isLastReadSuccessful)
                {
                    temperature = temp.Value;
                    humidity = hum.Value;
                    break;
                }
                else
                {
                    await Task.Delay(250).ConfigureAwait(false);
                }

            } while (!isLastReadSuccessful);

            return new SensorData(SensorType.SHT4x)
            {
                DegreesCelsius = temperature.Value.DegreesCelsius,
                HumidityInPercent = humidity.Value.Percent
            };
        }

        private static bool IsLastReadSuccessful(RelativeHumidity? humidity, Temperature? temperature)
        {
            return humidity.HasValue && temperature.HasValue;
        }
    }
}
