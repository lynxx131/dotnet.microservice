using System.Reflection;
using Dotnet.Microservice.Health;
using Newtonsoft.Json;
using Owin;

namespace Dotnet.Microservice.Owin
{
    public static class OwinMiddlewareHandler
    {
        public static void UseInfoEndpoint(this IAppBuilder app)
        {
            AssemblyName entryAssembly = Assembly.GetCallingAssembly().GetName();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/info"))
                {
                    var appInfo = new
                    {
                        Name = entryAssembly.Name,
                        Version = entryAssembly.Version.ToString(3)
                    };
                    context.Response.Headers.Set("Content-Type", "application/json");
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(appInfo));
                }
                else
                {
                    await next();
                }
            });
        }

        public static void UseHealthEndpoint(this IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/health"))
                {
                    HealthCheckRegistry.HealthStatus status = HealthCheckRegistry.GetStatus();

                    if (!status.IsHealthy)
                    {
                        // Return a service unavailable status code if any of the checks fail
                        context.Response.StatusCode = 503;
                    }

                    context.Response.Headers.Set("Content-Type", "application/json");
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(status));
                }
                else
                {
                    await next();
                }
            });
        }

        public static void UseEnvironmentEndpoint(this IAppBuilder app, bool includeEnvironmentVariables = false)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/env"))
                {
                    // Get current application environment
                    ApplicationEnvironment env = ApplicationEnvironment.GetApplicationEnvironment(includeEnvironmentVariables);

                    context.Response.Headers.Set("Content-Type", "application/json");
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(env, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
