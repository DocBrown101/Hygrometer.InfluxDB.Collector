using System.Device.I2c;
using System.Threading.Tasks;
using Hygrometer.InfluxDB.Collector.Model;
using Iot.Device.Mcp9808;

namespace Hygrometer.InfluxDB.Collector.Sensors;

internal class Mcp9808SensorReader : ISensorReader
{
    private readonly Mcp9808 sensor;

    public Mcp9808SensorReader(int i2cBusId = 1, int deviceAddress = Mcp9808.DefaultI2cAddress) // Default in HEX: 18
    {
        var i2cSettings = new I2cConnectionSettings(i2cBusId, deviceAddress);
        var i2cDevice = I2cDevice.Create(i2cSettings);

        this.sensor = new Mcp9808(i2cDevice);
        this.sensor.Wake();
    }

    public Task<SensorData> GetSensorData()
    {
        var sensorData = new SensorData(nameof(this.sensor))
        {
            DegreesCelsius = this.sensor.Temperature.DegreesCelsius
        };

        return Task.FromResult(sensorData);
    }
}
