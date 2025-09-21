using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;

namespace Hygrometer.InfluxDB.Collector.Model
{
    public class CollectorConfiguration
    {
        public OutputSettingEnum OutputSetting => this.outputSetting.ParsedValue;
        public IList<SensorType> Sensors => this.GetSensorTypes();
        public string Device => this.device.ParsedValue;
        public int IntervalSeconds => this.intervalSeconds.ParsedValue;
        public int MinimumDataPoints => this.minimumDataPoints.ParsedValue;
        public bool DebugOutput => this.debugOutput.ParsedValue;
        public string InfluxDbUrl => this.influxDbUrl.ParsedValue;
        public string InfluxDbOrg => this.influxDbOrg.ParsedValue;
        public string InfluxDbBucket => this.influxDbBucket.ParsedValue;
        public string InfluxDbMeasurement => this.influxDbMeasurement.ParsedValue;
        public string InfluxDbAuthenticateToken => this.influxDbAuthenticateToken.ParsedValue;

        private readonly CommandOption<OutputSettingEnum> outputSetting;
        private readonly CommandOption sensors;
        private readonly CommandOption<string> device;
        private readonly CommandOption<int> intervalSeconds;
        private readonly CommandOption<int> minimumDataPoints;
        private readonly CommandOption<bool> debugOutput;
        private readonly CommandOption<string> influxDbUrl;
        private readonly CommandOption<string> influxDbOrg;
        private readonly CommandOption<string> influxDbBucket;
        private readonly CommandOption<string> influxDbMeasurement;
        private readonly CommandOption<string> influxDbAuthenticateToken;

        public CollectorConfiguration(CommandLineApplication app)
        {
            this.outputSetting = app.Option<OutputSettingEnum>("-o|--output", "Console or Influx.", CommandOptionType.SingleValue);
            this.sensors = app.Option("-s|--sensor <SENSOR>", "All required sensors.", CommandOptionType.MultipleValue).Accepts(v => v.Values(Enum.GetNames<SensorType>()));
            this.device = app.Option<string>("-d|--device", "InfluxDB Tag.", CommandOptionType.SingleValue);
            this.intervalSeconds = app.Option<int>("-i|--interval", "The interval in seconds to request metrics.", CommandOptionType.SingleValue).Accepts(v => v.Range(10, 60));
            this.minimumDataPoints = app.Option<int>("--minimumDataPoints", "Minimum number of data points for transmission.", CommandOptionType.SingleValue).Accepts(v => v.Range(1, 100));
            this.debugOutput = app.Option<bool>("--debugOutput", "Any debug output?", CommandOptionType.NoValue);

            this.influxDbUrl = app.Option<string>("--influxDbUrl", "InfluxDb Url to be used.", CommandOptionType.SingleValue);
            this.influxDbOrg = app.Option<string>("--influxDbOrg", "InfluxDb Org name to be used.", CommandOptionType.SingleValue);
            this.influxDbBucket = app.Option<string>("--influxDbBucket", "InfluxDb Bucket name to be used.", CommandOptionType.SingleValue);
            this.influxDbMeasurement = app.Option<string>("--influxDbMeasurement", "Prefix all metrics pushed into the InfluxDb.", CommandOptionType.SingleValue);
            this.influxDbAuthenticateToken = app.Option<string>("--influxDbToken", "The InfluxDb Token.", CommandOptionType.SingleValue);

            this.SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            this.outputSetting.DefaultValue = OutputSettingEnum.Console;
            this.device.DefaultValue = "Solar battery";
            this.intervalSeconds.DefaultValue = 60;
            this.minimumDataPoints.DefaultValue = 5;
            this.debugOutput.DefaultValue = false;

            this.influxDbUrl.DefaultValue = "http://192.168.0.220:8088";
            this.influxDbOrg.DefaultValue = "home";
            this.influxDbBucket.DefaultValue = "minisolar";
            this.influxDbMeasurement.DefaultValue = "environment";
            this.influxDbAuthenticateToken.DefaultValue = "token";
        }

        private List<SensorType> GetSensorTypes()
        {
            var parsedSensors = new List<SensorType>();

            foreach (var sensor in this.sensors.Values)
            {
                if (Enum.TryParse(sensor, true, out SensorType sensorType))
                {
                    parsedSensors.Add(sensorType);
                }
            }

            return parsedSensors;
        }
    }
}
