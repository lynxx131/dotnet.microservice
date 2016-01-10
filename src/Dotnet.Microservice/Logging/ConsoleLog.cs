using System;

namespace Dotnet.Microservice.Logging
{
    public class ConsoleLog : IApplicationLogProvider
    {
        private readonly LogLevel _minLevel;
        public ConsoleLog(LogLevel minLevel = LogLevel.Info)
        {
            _minLevel = minLevel;
        }

        public void WriteLog(string message, LogLevel level)
        {
            // Check if the specified log level is enabled
            if (!IsEnabled(level))
            {
                return;
            }

            // Don't bother with empty messages
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            Console.WriteLine(message);

        }

        public bool IsEnabled(LogLevel level)
        {
            return (level >= _minLevel);
        }
    }
}
