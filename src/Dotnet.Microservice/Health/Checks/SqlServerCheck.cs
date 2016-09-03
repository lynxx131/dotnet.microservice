using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Dotnet.Microservice.Health.Checks
{
    public class SqlServerCheck
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
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string host = conn.DataSource;
                string version = conn.ServerVersion;
                conn.Close();
                conn.Dispose();
                return HealthResponse.Healthy(new { host = host, version = version });
            }
            catch (Exception e)
            {
                return HealthResponse.Unhealthy(e);
            }
        }
    }
}
