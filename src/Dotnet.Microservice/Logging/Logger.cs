using System.Collections.Generic;

namespace Dotnet.Microservice.Logging
{
    public class Logger
    {
        private readonly string _name;

        public Logger(string name)
        {
            _name = name;
        }

        public void Debug(string message)
        {
            LogMessage(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            LogMessage(message, LogLevel.Info);
        }

        public void Notice(string message)
        {
            LogMessage(message, LogLevel.Notice);
        }

        public void Warn(string message)
        {
            LogMessage(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            LogMessage(message, LogLevel.Error);
        }

        public void Critical(string message)
        {
            LogMessage(message, LogLevel.Critical);
        }

        internal void LogMessage(string message, LogLevel level)
        {
            string msg = ApplicationLog.FormatLogMessage(level, message, _name);
            LogMessageQueue.HandleMessage(new KeyValuePair<string, LogLevel>(msg, level));
        }

    }
}
