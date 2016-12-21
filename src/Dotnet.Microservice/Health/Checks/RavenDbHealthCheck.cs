using Raven.Abstractions.Data;
using System;

namespace Dotnet.Microservice.Health.Checks
{
    public class RavenDbHealthCheck
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">RavenDB connection string</param>
        /// <param name="database">Optional : database name to connect to</param>
        /// <returns></returns>
        public static HealthResponse CheckHealth(string connectionString , string database = null)
        {
            try
            {
                ConnectionStringParser<RavenConnectionStringOptions> parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);
                parser.Parse();
                var store = new Raven.Client.Document.DocumentStore
                {
                    Url = parser.ConnectionStringOptions.Url,
                    DefaultDatabase = database == null ? parser.ConnectionStringOptions.DefaultDatabase : database
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
