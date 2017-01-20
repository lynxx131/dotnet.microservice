using System;
using Npgsql;

namespace Dotnet.Microservice.Health.Checks
{
    public class PostgresqlHealthCheck
    {
        /// <summary>
        /// Check that a connection can be established to Postgresql and return the server version
        /// </summary>
        /// <param name="connectionString">An Npgsql connection string</param>
        /// <returns>A <see cref="HealthResponse"/> object that contains the return status of this health check</returns>
        public static HealthResponse CheckHealth(string connectionString)
        {
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectionString);
                NpgsqlConnection.ClearPool(conn);
                conn.Open();
                string host = conn.Host;
                string version = conn.PostgreSqlVersion.ToString();
                int port = conn.Port;
                conn.Close();
                conn.Dispose();
                return HealthResponse.Healthy(new { host = host, port = port , version = version});
            }
            catch (Exception e)
            {
                return HealthResponse.Unhealthy(e);
            }
        }
    }
}
