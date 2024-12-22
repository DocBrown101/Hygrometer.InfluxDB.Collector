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
        SHT4x,
        SI7021
    }
}
