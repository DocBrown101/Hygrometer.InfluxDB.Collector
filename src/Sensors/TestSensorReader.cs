using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public class TestSensorReader : ISensorReader
    {
        public async Task<SensorData> GetSensorData()
        {
            await Task.Delay(1000);

            return new SensorData("TEST")
            {
                DegreesCelsius = 22.818,
                HumidityInPercent = 45.111,
                Hectopascals = 653.321
            };
        }
    }
}
