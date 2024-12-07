using System;

namespace Hygrometer.InfluxDB.Collector
{
    public static class ConsoleLogger
    {
        private static bool writeDebug;

        public static void SetDebugOutput(bool enableDebug)
        {
            writeDebug = enableDebug;

            if (writeDebug)
            {
                Debug("Debug output has been enabled!");
            }
        }

        public static void Info(string message)
        {
            var oldForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now:o} - {message}");
            Console.ForegroundColor = oldForeground;
        }

        public static void Debug(string message)
        {
            if (!writeDebug)
            {
                return;
            }

            var oldForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now:o} DEBUG {message}");
            Console.ForegroundColor = oldForeground;
        }

        public static void Error(string message)
        {
            var oldForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"{DateTime.Now:o} ERROR {message}");
            Console.ForegroundColor = oldForeground;
        }
    }
}
