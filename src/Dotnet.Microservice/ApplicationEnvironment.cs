using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

#if DNXCORE50
using System.Runtime.InteropServices;
#endif

namespace Dotnet.Microservice
{
    /// <summary>
    ///  Object representing the application's current environment
    /// </summary>
    public class ApplicationEnvironment
    {
        public int ProcessId { get; private set; }
        public DateTime ProcessStartTime { get; private set; }
        public string CommandLine { get; private set; }
        public string Hostname { get; private set; }
        public string Os { get; private set; }
        public string OsVersion { get; private set; }
        public string Framework { get; private set; }
        public Dictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();
        public Dictionary<string, Dictionary<string, string>> ApplicationConfiguration { get; } = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Gather details about the environment the application is running in
        /// <param name="includeEnvVars">Indicate whether to include environment varialbles or not.Default is false</param>
        /// <returns>Returns an instance of ApplicationEnvironment</returns>
        /// </summary>
        public static ApplicationEnvironment GetApplicationEnvironment(bool includeEnvVars = false)
        {
            ApplicationEnvironment env = new ApplicationEnvironment();

            env.ProcessId = Process.GetCurrentProcess().Id;
            env.ProcessStartTime = Process.GetCurrentProcess().StartTime;
            env.Hostname = Dns.GetHostName();

#if NETCOREAPP1_0
            // Allow OS detection on .NET Core
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                env.Os = "Linux";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                env.Os = "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                env.Os = "Mac OSX";
            }
            else
            {
                env.Os = "Unknown";
            }
#else
            OperatingSystem os = Environment.OSVersion;
            PlatformID platform = os.Platform;

            switch (platform)
            {
                case PlatformID.Unix:
                    env.Os = "Linux/Unix";
                    break;
                case PlatformID.Win32NT:
                    env.Os = "Windows";
                    break;
                default:
                    env.Os = "Unknown";
                    break;
            }

            env.OsVersion = Environment.OSVersion.Version.ToString();
#endif

            // Framework detection
            #if DNXCORE50
                // Use compiler target detection for CoreCLR
                env.Framework = "CoreCLR";
            #else
                env.Framework = Type.GetType("Mono.Runtime") != null ? "Mono" : ".NET Framework";
#endif

            if (includeEnvVars)
            {
                // Loop over environment variables to escape special characters
                IDictionary envVars = Environment.GetEnvironmentVariables();

                foreach (var envVarKey in envVars.Keys)
                {
                    string envVarValue = envVars[envVarKey].ToString();
                    envVarValue = envVarValue.Replace("\\", "\\\\");
                    envVarValue = envVarValue.Replace('"', '\"');
                    env.EnvironmentVariables.Add(envVarKey.ToString(), envVarValue);
                }
            }

            // Loop over configuration sources and get their values
            foreach (var source in AppConfig.Sources.Keys)
            {
                env.ApplicationConfiguration.Add(source, AppConfig.GetAllValues(source));
            }

#if !NETCOREAPP1_0
            env.CommandLine = Environment.CommandLine.Replace("\\", "\\\\");
#endif
            return env;
        }

    }
}
