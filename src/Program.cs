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
            SensorTypes = appConfiguration.GetSensorTypes(),
            IntervalSeconds = appConfiguration.IntervalSeconds,
            MinimumDataPoints = appConfiguration.MinimumDataPoints,
            InfluxDbUrl = appConfiguration.InfluxDbUrl,
            InfluxDbOrg = appConfiguration.InfluxDbOrg,
            InfluxDbBucket = appConfiguration.InfluxDbBucket,
            InfluxDbMeasurement = appConfiguration.InfluxDbMeasurement,
            InfluxDbAuthenticateToken = appConfiguration.InfluxDbAuthenticateToken
        };

        ConsoleLogger.Info("Current Version: 1.3.0");
        ConsoleLogger.Debug($"Current output setting: {appConfiguration.OutputSetting}");
        ConsoleLogger.Debug($"InfluxDb {config.InfluxDbUrl}");
        ConsoleLogger.Debug($"Interval {config.IntervalSeconds} seconds");
        ConsoleLogger.Debug($"MinimumDataPoints {config.MinimumDataPoints}");
        ConsoleLogger.Debug($"Sensor count {config.SensorTypes.Count}");

        var metricsCompositor = new MetricsCompositor(SensorFactory.GetSensors(config.SensorTypes), config);

        ConsoleLogger.Info($"Collect Metrics ...");

        await metricsCompositor.StartMetricCollectionLoop(appConfiguration.OutputSetting, cancellationToken).ConfigureAwait(false);
    }
    catch (Exception e)
    {
        ConsoleLogger.Exception(e);
        Environment.Exit(1);
    }
});

return app.Execute(args);