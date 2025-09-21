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
        private readonly string sensorName;

        public Sht4xSensorReader(int busId = 1, int deviceAddress = Sht4x.DefaultI2cAddress) // Default in HEX: 44
        {
            var device = I2cDevice.Create(new I2cConnectionSettings(busId, deviceAddress));
            this.sensor = new Sht4x(device);
            this.sensor.Reset();
            this.sensorName = this.sensor.GetType().Name;
        }

        public async Task<SensorData> GetSensorData()
        {
            RelativeHumidity? humidity;
            Temperature? temperature;

            do
            {
                (humidity, temperature) = await this.sensor.ReadHumidityAndTemperatureAsync().ConfigureAwait(false);

                if (!humidity.HasValue || !temperature.HasValue)
                {
                    ConsoleLogger.Debug($"Warning, sensor data from {this.sensorName} could not be read! Trying again...");
                    await Task.Delay(200).ConfigureAwait(false);
                }
            } while (!humidity.HasValue || !temperature.HasValue);

            return new SensorData(this.sensorName)
            {
                DegreesCelsius = temperature.Value.DegreesCelsius,
                HumidityInPercent = humidity.Value.Percent
            };
        }
    }
}
