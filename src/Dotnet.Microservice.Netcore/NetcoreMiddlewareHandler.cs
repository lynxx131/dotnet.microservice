using System.Reflection;
using Dotnet.Microservice.Health;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnet.Microservice.Netcore
{
    public static class NetcoreMiddlewareHandler
    {
        public static void UseHealthEndpoint(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/health"))
                {
                    // Perform IP access check
                    if (MicroserviceConfiguration.AllowedIpAddresses != null && context.Request.HttpContext.Connection.RemoteIpAddress != null)
                    {
                        if (!MicroserviceConfiguration.AllowedIpAddresses.Contains(context.Request.HttpContext.Connection.RemoteIpAddress))
                        {
                            context.Response.StatusCode = 403;
                            await next();
                        }
                    }

                    HealthCheckRegistry.HealthStatus status = await Task.Run(() => HealthCheckRegistry.GetStatus());

                    if (!status.IsHealthy)
                    {
                        // Return a service unavailable status code if any of the checks fail
                        context.Response.StatusCode = 503;
                    }

                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(status));
                }
                else
                {
                    await next();
                }
            });
        }

        public static void UseEnvironmentEndpoint(this IApplicationBuilder app, bool includeEnvVars = false)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/env"))
                {
                    // Perform IP access check
                    if (MicroserviceConfiguration.AllowedIpAddresses != null && context.Request.HttpContext.Connection.RemoteIpAddress != null)
                    {
                        if (!MicroserviceConfiguration.AllowedIpAddresses.Contains(context.Request.HttpContext.Connection.RemoteIpAddress))
                        {
                            context.Response.StatusCode = 403;
                            await next();
                        }
                    }

                    // Get current application environment
                    ApplicationEnvironment env = ApplicationEnvironment.GetApplicationEnvironment(includeEnvVars);

                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(env, new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii }));
                }
                else
                {
                    await next();
                }
            });
        }

        // For DNX running on the full CLR we can use the GetExecutingAssembly() method to get our entry assembly
#if !NETSTANDARD1_6
        public static void UseInfoEndpoint(this IApplicationBuilder app)
        {
            Assembly entryAssembly = Assembly.GetCallingAssembly();
            UseInfoEndpoint(app, entryAssembly.GetName());
        }
#endif

        // .NET core does not contain the GetExecutingAssembly() method so we must pass in a reference to the entry assembly
        public static void UseInfoEndpoint(this IApplicationBuilder app, AssemblyName entryAssembly)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/info"))
                {
                    // Perform IP access check
                    if (MicroserviceConfiguration.AllowedIpAddresses != null && context.Request.HttpContext.Connection.RemoteIpAddress != null)
                    {
                        if (!MicroserviceConfiguration.AllowedIpAddresses.Contains(context.Request.HttpContext.Connection.RemoteIpAddress))
                        {
                            context.Response.StatusCode = 403;
                            await next();
                        }
                    }

                    var appInfo = new
                    {
                        Name = entryAssembly.Name,
                        Version = entryAssembly.Version.ToString(3)
                    };

                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(appInfo));
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
