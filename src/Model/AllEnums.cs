namespace Hygrometer.InfluxDB.Collector.Model
{
    public enum OutputSettingEnum
    {
        Console,
        Influx
    }

    public enum SensorType
    {
        TEST,
        Aht20,
        BME280,
        BMP280,
        Mcp9808,
        SHT3x,
        SHT4x,
        SI7021
    }
}
