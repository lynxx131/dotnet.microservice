using System;
using System.Collections.Generic;
using Dotnet.Microservice.Health;
using Dotnet.Microservice.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.Hosting;
using Owin;

namespace Dotnet.Microservice.Owin.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Bootstrap and start the OWIN host
            string baseUri = "http://localhost:5000";
            Args = args;
            WebApp.Start<Startup>(baseUri);
            Logger logger = ApplicationLog.CreateLogger<Program>();
            logger.Info($"Server running at {baseUri} - press Enter to quit. ");
            Console.ReadLine();
        }

        public static string[] Args;

    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Setup logging
            ApplicationLog.AddConsole();
            Logger logger = ApplicationLog.CreateLogger<Startup>();
            logger.Info("Initializing service");

            // Build an IConfiguration instance using the ConfigurationBuilder as normal
            Dictionary<string, string> collection = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" } };
            var config1 = new ConfigurationBuilder().AddInMemoryCollection(collection).Build();
            var config3 = new ConfigurationBuilder().AddJsonFile("config.json").Build();

            // AppConfig is a static class that groups together instances of IConfiguration and makes them available statically anywhere in the application
            AppConfig.AddConfigurationObject(config1, "memorySource");
            AppConfig.AddConfigurationObject(config3, "jsonSource");

            // The above configuration sources can now be referenced easily with a static helper function
            Console.WriteLine("key1 key in memorySource: " + AppConfig.Get("memorySource", "key1"));
            Console.WriteLine("config:setting key in jsonSource: " + AppConfig.Get("jsonSource", "config:setting"));

            // Runtime configuration can be updated easily as well
            AppConfig.Set("jsonSource", "config:setting", "http://localhost:5001");
            Console.WriteLine("Modified config:setting key in jsonSource: " + AppConfig.Get("jsonSource", "config:setting"));

            // Redis health check (Requires StackExchange.Redis)
            //HealthCheckRegistry.RegisterHealthCheck("Redis", () => RedisHealthCheck.CheckHealth("localhost"));
            // PostgreSQL health check (Requires Npgsql)
            //HealthCheckRegistry.RegisterHealthCheck("Postgresql", () => PostgresqlHealthCheck.CheckHealth("Host=localhost;Username=postgres;Password=postgres;Database=postgres"));

            /*
             *   Health checks are simply functions that return either healthy or unhealthy with an optional message string
             */
            HealthCheckRegistry.RegisterHealthCheck("MyCustomMonitor", () => HealthResponse.Healthy("Test Message"));
            HealthCheckRegistry.RegisterHealthCheck("MyCustomMonitor2", () => HealthResponse.Healthy("Test Message2"));
            HealthCheckRegistry.RegisterHealthCheck("SampleOperation", () => SampleHealthCheckOperation());

            // Activate the info endpoint
            app.UseInfoEndpoint();

            // Activate the environment endpoint
            app.UseEnvironmentEndpoint();

            // Activate the health endpoint
            app.UseHealthEndpoint();
        }

        public static HealthResponse SampleHealthCheckOperation()
        {
            return HealthResponse.Unhealthy("Sample operation failed");
        }

    }
}
