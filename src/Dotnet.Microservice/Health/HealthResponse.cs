using System;

namespace Dotnet.Microservice.Health
{
    /// <summary>
    /// Represents a health check response
    /// </summary>
    public struct HealthResponse
    {
        public readonly bool IsHealthy;
        public readonly string Message;

        private HealthResponse(bool isHealthy, string statusMessage)
        {
            IsHealthy = isHealthy;
            Message = statusMessage;
        }

        public static HealthResponse Healthy()
        {
            return Healthy("OK");
        }

        public static HealthResponse Healthy(string message)
        {
            return new HealthResponse(true, message);
        }

        public static HealthResponse Unhealthy()
        {
            return Unhealthy("FAILED");
        }

        public static HealthResponse Unhealthy(string message)
        {
            return new HealthResponse(false, message);
        }

        public static HealthResponse Unhealthy(Exception exception)
        {
            var message = $"EXCEPTION: {exception.GetType().Name}, {exception.Message}";
            return new HealthResponse(false, message);
        }

    }
}
