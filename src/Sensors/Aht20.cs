using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Ahtxx;

namespace Hygrometer.InfluxDB.Collector.Sensors;

public class Aht20SensorReader : ISensorReader
{
    private readonly Aht20 sensor;
    private readonly string sensorName;

    public Aht20SensorReader(int i2cBusId = 1, int deviceAddress = Aht20.DefaultI2cAddress)
    {
        var i2cSettings = new I2cConnectionSettings(i2cBusId, deviceAddress);
        var i2cDevice = I2cDevice.Create(i2cSettings);

        this.sensor = new Aht20(i2cDevice);
        this.sensorName = this.sensor.GetType().Name;
    }

    public Task<SensorData> GetSensorData()
    {
        var sensorData = new SensorData(this.sensorName)
        {
            DegreesCelsius = this.sensor.GetTemperature().DegreesCelsius,
            HumidityInPercent = this.sensor.GetHumidity().Percent
        };

        return Task.FromResult(sensorData);
    }
}
