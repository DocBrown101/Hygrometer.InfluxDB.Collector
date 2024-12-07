namespace Hygrometer.InfluxDB.Collector
{
    using McMaster.Extensions.CommandLineUtils;

    internal class AppConfiguration
    {
        public OutputSettingEnum OutputSetting => this.outputSetting.ParsedValue;
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
        private readonly CommandOption<string> device;
        private readonly CommandOption<int> intervalSeconds;
        private readonly CommandOption<int> minimumDataPoints;
        private readonly CommandOption<bool> debugOutput;
        private readonly CommandOption<string> influxDbUrl;
        private readonly CommandOption<string> influxDbOrg;
        private readonly CommandOption<string> influxDbBucket;
        private readonly CommandOption<string> influxDbMeasurement;
        private readonly CommandOption<string> influxDbAuthenticateToken;

        public AppConfiguration(CommandLineApplication app)
        {
            this.outputSetting = app.Option<OutputSettingEnum>("-o|--output", "Console or Influx.", CommandOptionType.SingleValue);
            this.outputSetting.DefaultValue = OutputSettingEnum.Console;

            this.device = app.Option<string>("-d|--device", "InfluxDB Tag.", CommandOptionType.SingleValue);
            this.device.DefaultValue = "Solar battery";

            this.intervalSeconds = app.Option<int>("-i|--interval", "The interval in seconds to request metrics.", CommandOptionType.SingleValue).Accepts(v => v.Range(10, 60));
            this.intervalSeconds.DefaultValue = 30;

            this.minimumDataPoints = app.Option<int>("--minimumDataPoints", "Minimum number of data points for transmission.", CommandOptionType.SingleValue).Accepts(v => v.Range(1, 100));
            this.minimumDataPoints.DefaultValue = 2;

            this.debugOutput = app.Option<bool>("--debugOutput", "Any debug output?", CommandOptionType.SingleValue);
            this.debugOutput.DefaultValue = false;

            this.influxDbUrl = app.Option<string>("--influxDbUrl", "The InfluxDb Url. E.g. http://192.168.0.220:8088", CommandOptionType.SingleValue);
            this.influxDbUrl.DefaultValue = "http://192.168.0.220:8088";
            this.influxDbOrg = app.Option<string>("--influxDbOrg", "The InfluxDb Org name.", CommandOptionType.SingleValue);
            this.influxDbOrg.DefaultValue = "home";
            this.influxDbBucket = app.Option<string>("--influxDbBucket", "The InfluxDb Bucket name.", CommandOptionType.SingleValue);
            this.influxDbBucket.DefaultValue = "solar";
            this.influxDbMeasurement = app.Option<string>("--influxDbMeasurement", "Prefix all metrics pushed into the InfluxDb.", CommandOptionType.SingleValue);
            this.influxDbMeasurement.DefaultValue = "environment";
            this.influxDbAuthenticateToken = app.Option<string>("--influxDbToken", "The InfluxDb Token.", CommandOptionType.SingleValue);
            this.influxDbAuthenticateToken.DefaultValue = "cx8rDqWJ3FrDhCei9onSGpndAQzEhSYPSjApXzCJK40hUIY6rYro_yrav18JNalQF25eBG3baR6fys9WPQio6w==";
        }
    }
}
