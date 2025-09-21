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

        public Sht4xSensorReader(int busId = 1, int deviceAddress = Sht4x.DefaultI2cAddress) // Default in HEX: 44
        {
            var device = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress));
            this.sensor = new Sht4x(device);
            this.sensor.Reset();
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
                    await Task.Delay(200).ConfigureAwait(false);
                }

            } while (!isLastReadSuccessful);

            return new SensorData(nameof(this.sensor))
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
