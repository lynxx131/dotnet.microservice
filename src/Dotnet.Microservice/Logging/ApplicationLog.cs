using System;
using System.Collections.Generic;
using System.Threading;

namespace Dotnet.Microservice.Logging
{
    /// <summary>
    /// Static helper class to wrap around Microsoft.Framework.Logging
    /// </summary>
    public class ApplicationLog
    {
        internal static readonly List<IApplicationLogProvider> Providers = new List<IApplicationLogProvider>();

        /// <summary>
        /// Add console logger with a defined minimum log level
        /// <param name="minLevel">The minimum message level to log</param>
        /// </summary>
        public static void AddConsole(LogLevel minLevel)
        {
            Providers.Add(new ConsoleLog(minLevel));
        }

        /// <summary>
        /// Add console logger with a default log level of INFO
        /// </summary>
        public static void AddConsole()
        {
            Providers.Add(new ConsoleLog());
        }

        /// <summary>
        /// Create a logger instance for the specified type
        /// </summary>
        public static Logger CreateLogger<T>()
        {
            return new Logger(typeof(T).ToString());
        }

        /// <summary>
        /// Create a logger instance with the specified name
        /// <param name="name">Name to assign to this logger</param>
        /// </summary>
        public static Logger CreateLogger(string name)
        {
            return new Logger(name);
        }

        /// <summary>
        /// Add a file logger with a defined minimum log level
        /// <param name="minLevel">The minimum message level to log</param>
        /// </summary>
        public static void AddFile(string path, LogLevel minLevel)
        {
            Providers.Add(new FileLog(path, minLevel));
        }

        /// <summary>
        /// Create a logger instance that logs to a file with a default level of INFO
        /// </summary>
        public static void AddFile(string path)
        {
            Providers.Add(new FileLog(path));
        }

        /// <summary>
        /// Format a log message and return it as a string
        /// <param name="logLevel">Log level of the message</param>
        /// <param name="message">The log message to format</param>
        /// <param name="loggerName">The name given of this logger</param>
        /// <returns>A string containing the formatted log message</returns>
        /// </summary>
        internal static string FormatLogMessage(LogLevel logLevel, string message, string loggerName)
        {
            // Format the message time
            string formattedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss,fff");
            
            // Get a string representation of the log level
            var logLevelString = GetLogLevelString(logLevel);

            // Return the formatted message
            return $"{formattedDate} [Thread-{Thread.CurrentThread.ManagedThreadId}] {logLevelString}  {loggerName} - {message}";
        }

        /// <summary>
        /// Get a string representation of a specified log level
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <returns>Returns a string representation of the specified log level</returns>
        internal static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Info:
                    return "INFO";
                case LogLevel.Notice:
                    return "NOTICE";
                case LogLevel.Warn:
                    return "WARN";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Critical:
                    return "CRIT";
                default:
                    return "UNKNOWN";
            }
        }

    }
}
