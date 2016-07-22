using System;
using StackExchange.Redis;

namespace Dotnet.Microservice.Health.Checks
{
    public class RedisHealthCheck
    {
        /// <summary>
        /// Check that a connection to Redis can be established and attempt to pull back server statistics
        /// </summary>
        /// <param name="connectionString">A StackExchange.Redis connection string</param>
        /// <returns>A <see cref="HealthResponse"/> object that contains the return status of this health check</returns>
        public static HealthResponse CheckHealth(string connectionString)
        {
            try
            {
                ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(connectionString);
                string[] server = conn.GetStatus().Split(';');
                conn.Close();
                return HealthResponse.Healthy(server[0]);
            }
            catch (Exception e)
            {
                return HealthResponse.Unhealthy(e);
            }
        }
    }
}
