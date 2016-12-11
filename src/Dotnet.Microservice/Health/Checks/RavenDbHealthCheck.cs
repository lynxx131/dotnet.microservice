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
                // Client doesn't seem to throw an exception until we try to do something so let's just do something simple and get the build number of the server.
                var build = store.DatabaseCommands.GlobalAdmin.GetBuildNumber();
                // Dispose the store object
                store.Dispose();
                
                return HealthResponse.Healthy(new { server = store.Url, database = store.DefaultDatabase, serverBuild = build.BuildVersion });
            }
            catch (Exception ex)
            {
                return HealthResponse.Unhealthy(ex);
            }
        }
    }
}
