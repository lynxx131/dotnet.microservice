using Dotnet.Microservice.Utils;

namespace Dotnet.Microservice
{
    public class MicroserviceConfiguration
    {
        /// <summary>
        /// Specifies the IP address range that is allowed to access the actuator endpoints. Default allows all IPs
        /// </summary>
        public static IpAddressRange AllowedIpAddresses;
    }
}
