using Hygrometer.InfluxDB.Collector;
using Hygrometer.InfluxDB.Collector.Metrics;
using Hygrometer.InfluxDB.Collector.Model;
using Hygrometer.InfluxDB.Collector.Sensors;
using McMaster.Extensions.CommandLineUtils;
using System;

var app = new CommandLineApplication();
app.HelpOption();

var appConfiguration = new AppConfiguration(app);

app.OnExecuteAsync(async cancellationToken =>
{
    ConsoleLogger.SetDebugOutput(appConfiguration.DebugOutput);

    try
    {
        var config = new MetricsConfiguration()
        {
            Device = appConfiguration.Device,
            // SensorTypes = [SensorType.BMP280, SensorType.DHT22],
            // SensorTypes = [SensorType.Si7021, SensorType.BME280],
            SensorTypes = [SensorType.Sht4x],
            IntervalSeconds = appConfiguration.IntervalSeconds,
            MinimumDataPoints = appConfiguration.MinimumDataPoints,
            InfluxDbUrl = appConfiguration.InfluxDbUrl,
            InfluxDbOrg = appConfiguration.InfluxDbOrg,
            InfluxDbBucket = appConfiguration.InfluxDbBucket,
            InfluxDbMeasurement = appConfiguration.InfluxDbMeasurement,
            InfluxDbAuthenticateToken = appConfiguration.InfluxDbAuthenticateToken
        };

        ConsoleLogger.Info("Current Version: 1.0.0");
        ConsoleLogger.Debug($"Current output setting: {appConfiguration.OutputSetting}");
        ConsoleLogger.Debug($"InfluxDb {config.InfluxDbUrl}");
        ConsoleLogger.Debug($"Interval {config.IntervalSeconds} seconds");
        ConsoleLogger.Debug($"MinimumDataPoints {config.MinimumDataPoints}");
        ConsoleLogger.Debug($"Sensor {config.SensorTypes}");

        var metricsCompositor = new MetricsCompositor(SensorFactory.GetSensors(config.SensorTypes), config);

        ConsoleLogger.Info($"Collect Metrics ...");

        switch (appConfiguration.OutputSetting)
        {
            case OutputSettingEnum.Console:
                await metricsCompositor.WriteSensorDataToConsole(cancellationToken).ConfigureAwait(false);
                break;
            case OutputSettingEnum.Influx:
                await metricsCompositor.StartMetricCollectionLoop(cancellationToken).ConfigureAwait(false);
                break;
            default:
                throw new System.ComponentModel.InvalidEnumArgumentException(nameof(appConfiguration.OutputSetting));
        }
    }
    catch (Exception e)
    {
        ConsoleLogger.Error(e.Message);

        if (e.InnerException != null)
        {
            ConsoleLogger.Error(e.InnerException.Message);
        }

        Environment.Exit(1);
    }
});

return app.Execute(args);