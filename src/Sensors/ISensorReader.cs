using Hygrometer.InfluxDB.Collector.Model;
using System.Threading.Tasks;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public interface ISensorReader
    {
        Task<SensorData> GetSensorData();
    }
}