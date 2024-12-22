using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;

namespace Hygrometer.InfluxDB.Collector.Sensors
{
    public interface ISensorReader
    {
        Task<SensorData> GetSensorData();
    }
}
