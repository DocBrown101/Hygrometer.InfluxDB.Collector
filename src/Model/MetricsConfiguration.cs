﻿namespace Hygrometer.InfluxDB.Collector.Model
{
    using System.Collections.Generic;

    public class MetricsConfiguration
    {
        public string Device { get; set; }

        public string InfluxDbUrl { get; set; }

        public string InfluxDbOrg { get; set; }

        public string InfluxDbBucket { get; set; }

        public string InfluxDbMeasurement { get; set; }

        public string InfluxDbAuthenticateToken { get; set; }

        public int IntervalSeconds { get; set; }

        public int MinimumDataPoints { get; set; }

        public IList<SensorType> SensorTypes { get; set; }
    }
}
