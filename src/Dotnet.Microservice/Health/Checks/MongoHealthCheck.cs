using System;
using MongoDB.Driver;

namespace Dotnet.Microservice.Health.Checks
{
    public class MongoHealthCheck
    {
        public static HealthResponse CheckHealth(string connectionString)
        {
            try
            {
                var client = new MongoClient(connectionString);
                return HealthResponse.Healthy(new { databases = client.ListDatabases().ToList()  , server = client.Settings.Server });
            }
            catch (Exception ex)
            {

                return HealthResponse.Unhealthy(ex);
            }
        }
    }
}
