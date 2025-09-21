using System;
using Hygrometer.InfluxDB.Collector;
using Hygrometer.InfluxDB.Collector.Metrics;
using Hygrometer.InfluxDB.Collector.Model;
using Hygrometer.InfluxDB.Collector.Sensors;
using McMaster.Extensions.CommandLineUtils;

var app = new CommandLineApplication();
app.HelpOption();

var config = new CollectorConfiguration(app);

app.OnExecuteAsync(async cancellationToken =>
{
    try
    {
        ConsoleLogger.Init(config.DebugOutput, "1.5.0");
        ConsoleLogger.Debug($"Current output setting: {config.OutputSetting}");
        ConsoleLogger.Debug($"InfluxDb {config.InfluxDbUrl}");
        ConsoleLogger.Debug($"Interval {config.IntervalSeconds} seconds");
        ConsoleLogger.Debug($"MinimumDataPoints {config.MinimumDataPoints}");
        ConsoleLogger.Debug($"Sensor count {config.Sensors.Count}");

        var metricsCompositor = new MetricsCompositor(SensorFactory.GetSensors(config.Sensors), config);

        await metricsCompositor.StartMetricCollectionLoop(config.OutputSetting, cancellationToken).ConfigureAwait(false);
    }
    catch (Exception e)
    {
        ConsoleLogger.Error(e);
        Environment.Exit(1);
    }
});

return app.Execute(args);
