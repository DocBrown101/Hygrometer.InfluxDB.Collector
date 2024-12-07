namespace Hygrometer.InfluxDB.Collector
{
    public enum OutputSettingEnum
    {
        Console,
        Influx
    }

    public enum SensorType
    {
        BME280,
        BMP280,
        DHT22,
        Si7021
    }
}
