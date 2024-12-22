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
        BME280,
        BMP280,
        DHT22,
        SHT3x,
        SHT4x,
        SI7021
    }
}
