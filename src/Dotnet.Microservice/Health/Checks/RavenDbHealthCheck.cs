using Raven.Abstractions.Data;
using System;

namespace Dotnet.Microservice.Health.Checks
{
    public class RavenDbHealthCheck
    {
        public static HealthResponse CheckHealth(string connectionString)
        {
            try
            {
                ConnectionStringParser<RavenConnectionStringOptions> parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);
                parser.Parse();
                var store = new Raven.Client.Document.DocumentStore
                {
                    Url = parser.ConnectionStringOptions.Url,
                    DefaultDatabase = parser.ConnectionStringOptions.DefaultDatabase
                };
                store.Initialize();
                
                return HealthResponse.Healthy(new { server = store.Url, database = store.DefaultDatabase });
            }
            catch (Exception ex)
            {
                return HealthResponse.Unhealthy(ex);
            }
        }
    }
}
