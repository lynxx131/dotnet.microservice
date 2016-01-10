using System;
using System.Collections.Generic;
using Dotnet.Microservice.Health;
using Dotnet.Microservice.Logging;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

#if DNXCORE50
using System.Reflection;
#endif 

namespace Dotnet.Microservice.Dnx.Sample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appenv)
        {
            _env = env;
            _appenv = appenv;
        }

        private IHostingEnvironment _env;
        private IApplicationEnvironment _appenv;

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app)
        {
            // Add logging
            ApplicationLog.AddConsole();
            ApplicationLog.AddFile("Test.log");
            Logger logger = ApplicationLog.CreateLogger<Startup>();
            logger.Info("Initializing service");

            // Build an IConfiguration instance using the ConfigurationBuilder as normal
            Dictionary<string, string> collection = new Dictionary<string, string>() { { "key1", "value1" }, { "key2", "value2" } };
            var config1 = new ConfigurationBuilder().AddInMemoryCollection(collection).Build();
            var config2 = new ConfigurationBuilder().AddIniFile("hosting.ini").Build();
            var config3 = new ConfigurationBuilder().AddJsonFile("config.json").Build();

            // AppConfig is a static class that groups together instances of IConfiguration and makes them available statically anywhere in the application
            AppConfig.AddConfigurationObject(config1, "memorySource");
            AppConfig.AddConfigurationObject(config2, "iniSource");
            AppConfig.AddConfigurationObject(config3, "jsonSource");

            // The above configuration sources can now be referenced easily with a static helper function
            Console.WriteLine("key1 key in memorySource: "+ AppConfig.Get("memorySource", "key1"));
            Console.WriteLine("server.urls key in iniSource: " + AppConfig.Get("iniSource", "server.urls"));

            // Runtime configuration can be updated easily as well
            AppConfig.Set("iniSource", "server.urls", "http://localhost:5001");
            Console.WriteLine("Modified server.urls key in iniSource: " + AppConfig.Get("iniSource", "server.urls"));

            /*
             *   Health checks are simply functions that return either healthy or unhealthy with an optional message string
             */
            HealthCheckRegistry.RegisterHealthCheck("MyCustomMonitor", () => HealthResponse.Healthy("Test Message"));
            HealthCheckRegistry.RegisterHealthCheck("MyCustomMonitor2", () => HealthResponse.Healthy("Test Message2"));
            HealthCheckRegistry.RegisterHealthCheck("SampleOperation", () => SampleHealthCheckOperation());

            /*
                Uncomment the below line to only allow access to the actuator endpoints from localhost

                Allowed patterns are:

                1. CIDR range: "192.168.0.0/24", "fe80::/10"
                2. Single address: "127.0.0.1", ":;1"
                3. Begin end range: "169.258.0.0-169.258.0.255"
                4. Bit mask range: "192.168.0.0/255.255.255.0"

                NOTE: Currently this feature is not supported under Kestrel self-host as it does not set the client's IP address in HttpContext
            */

            //MicroserviceBootstrap.AllowedIpAddresses = IpAddressRange.Parse("127.0.0.0/8");

            // Activate /health endpoint
            app.UseHealthEndpoint();

            /* 
             * Activate /env endpoint
             *
             * The ApplicationConfiguration element of the env endpoint will only contain data if the AppConfig helper class is 
             * used to manage application configuration
             */
            app.UseEnvironmentEndpoint();

            /*
             *   The compiler directive below is only required if you plan to target .NET core as well as the full CLR
             *   If you don't target dnxcore50 in your project.json you can remove the below #if and just call UseInfoEndpoint()
             *   without any parameters
             */

            // Activate /info endpoint
#if DNXCORE50
            // Required for .NET Core until the relevant APIs are added
            app.UseInfoEndpoint(typeof(Startup).GetTypeInfo().Assembly.GetName());
#else
            app.UseInfoEndpoint();
#endif

        }

        public static HealthResponse SampleHealthCheckOperation()
        {
            return HealthResponse.Unhealthy("Sample operation failed");
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

    }

}
