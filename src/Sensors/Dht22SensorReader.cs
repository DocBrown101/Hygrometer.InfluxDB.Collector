using System.Device.Gpio;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.DHTxx;
using UnitsNet;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class Dht22SensorReader : ISensorReader
    {
        private readonly Dht22 sensor;

        /// <summary>
        /// Required parameters for an GPIO communication
        /// </summary>
        /// <param name="gpioPin">Default GPIO pin</param>
        public Dht22SensorReader(int gpioPin = 23)
        {
            this.sensor = new Dht22(gpioPin, PinNumberingScheme.Logical);
        }

        public async Task<SensorData> GetSensorData()
        {
            Temperature temperature;
            RelativeHumidity humidity;
            bool isLastReadSuccessful;

            do
            {
                var success1 = this.sensor.TryReadTemperature(out temperature);
                var success2 = this.sensor.TryReadHumidity(out humidity);

                isLastReadSuccessful = IsLastReadSuccessful(temperature, humidity);

                if (success1 && success2 && isLastReadSuccessful)
                {
                    break;
                }
                else
                {
                    await Task.Delay(100).ConfigureAwait(false);
                }

            } while (!isLastReadSuccessful);

            return new SensorData(SensorType.DHT22)
            {
                DegreesCelsius = temperature.DegreesCelsius,
                HumidityInPercent = humidity.Percent
            };
        }

        private static bool IsLastReadSuccessful(Temperature temperature, RelativeHumidity humidity)
        {
            if (temperature.DegreesCelsius > 0 && humidity.Percent <= 100)
            {
                return true;
            }

            return false;
        }
    }
}
