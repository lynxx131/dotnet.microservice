namespace Dotnet.Microservice.Logging
{
    public interface IApplicationLogProvider
    {
        void WriteLog(string message, LogLevel level);
        bool IsEnabled(LogLevel level);
    }
}
