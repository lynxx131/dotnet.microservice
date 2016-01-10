/*
    This code is taken from https://github.com/jsakamoto/ipaddressrange/blob/master/IPAddressRange/IPAddressRange.cs with some modifications to work under .NET Core
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Dotnet.Microservice.Utils
{
    /// <summary>
    /// Represents a specified range of IP addresses
    /// </summary>
    public class IpAddressRange : IEnumerable<IPAddress>
    {
        // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::/10"
        private static readonly Regex M1Regex = new Regex(@"^(?<adr>[\da-f\.:]+)/(?<maskLen>\d+)$", RegexOptions.IgnoreCase);

        // Pattern 2. Uni address: "127.0.0.1", ":;1"
        private static readonly Regex M2Regex = new Regex(@"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase);

        // Pattern 3. Begin end range: "169.258.0.0-169.258.0.255"
        private static readonly Regex M3Regex = new Regex(@"^(?<begin>[\da-f\.:]+)[\-–](?<end>[\da-f\.:]+)$", RegexOptions.IgnoreCase);

        // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
        private static readonly Regex M4Regex = new Regex(@"^(?<adr>[\da-f\.:]+)/(?<bitmask>[\da-f\.:]+)$", RegexOptions.IgnoreCase);


        public IPAddress Begin { get; }

        public IPAddress End { get; }

        /// <summary>
        /// Creates an empty range object, equivalent to "0.0.0.0/0".
        /// </summary>
        public IpAddressRange() : this(new IPAddress(0L)) { }

        /// <summary>
        /// Creates a new range with the same start/end address (range of one)
        /// </summary>
        /// <param name="singleAddress"></param>
        public IpAddressRange(IPAddress singleAddress)
        {
            Begin = End = singleAddress;
        }

        /// <summary>
        /// Create a new range from a begin and end address.
        /// Throws an exception if Begin comes after End, or the
        /// addresses are not in the same family.
        /// </summary>
        public IpAddressRange(IPAddress begin, IPAddress end)
        {
            Begin = begin;
            End = end;

            if (Begin.AddressFamily != End.AddressFamily) throw new ArgumentException("Elements must be of the same address family", "beginEnd");

            var beginBytes = Begin.GetAddressBytes();
            var endBytes = End.GetAddressBytes();
            if (!Bits.LE(endBytes, beginBytes)) throw new ArgumentException("Begin must be smaller than the End", "beginEnd");
        }

        /// <summary>
        /// Creates a range from a base address and mask bits.
        /// This can also be used with <see cref="SubnetMaskLength"/> to create a
        /// range based on a subnet mask.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="maskLength"></param>
        public IpAddressRange(IPAddress baseAddress, int maskLength)
        {
            var baseAdrBytes = baseAddress.GetAddressBytes();
            if (baseAdrBytes.Length * 8 < maskLength) throw new FormatException();
            var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, maskLength);
            baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);

            Begin = new IPAddress(baseAdrBytes);
            End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
        }

        [Obsolete("Use IPAddressRange.Parse static method instead.")]
        public IpAddressRange(string ipRangeString)
        {
            var parsed = Parse(ipRangeString);
            Begin = parsed.Begin;
            End = parsed.End;
        }

        public bool Contains(IPAddress ipaddress)
        {
            if (ipaddress.AddressFamily != Begin.AddressFamily) return false;
            var adrBytes = ipaddress.GetAddressBytes();
            return Bits.GE(this.Begin.GetAddressBytes(), adrBytes) && Bits.LE(End.GetAddressBytes(), adrBytes);
        }

        public bool Contains(IpAddressRange range)
        {
            if (this.Begin.AddressFamily != range.Begin.AddressFamily) return false;

            return
                Bits.GE(this.Begin.GetAddressBytes(), range.Begin.GetAddressBytes()) &&
                Bits.LE(this.End.GetAddressBytes(), range.End.GetAddressBytes());
        }

        public static IpAddressRange Parse(string ipRangeString)
        {
            if (ipRangeString == null) throw new ArgumentNullException(nameof(ipRangeString));

            // remove all spaces.
            ipRangeString = ipRangeString.Replace(" ", String.Empty);

            // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::/10"
            var m1 = M1Regex.Match(ipRangeString);
            if (m1.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m1.Groups["adr"].Value).GetAddressBytes();
                var maskLen = int.Parse(m1.Groups["maskLen"].Value);
                if (baseAdrBytes.Length * 8 < maskLen) throw new FormatException();
                var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, maskLen);
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                return new IpAddressRange(new IPAddress(baseAdrBytes), new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes))));
            }

            // Pattern 2. Uni address: "127.0.0.1", ":;1"
            var m2 = M2Regex.Match(ipRangeString);
            if (m2.Success)
            {
                return new IpAddressRange(IPAddress.Parse(ipRangeString));
            }

            // Pattern 3. Begin end range: "169.258.0.0-169.258.0.255"
            var m3 = M3Regex.Match(ipRangeString);
            if (m3.Success)
            {
                return new IpAddressRange(IPAddress.Parse(m3.Groups["begin"].Value), IPAddress.Parse(m3.Groups["end"].Value));
            }

            // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
            var m4 = M4Regex.Match(ipRangeString);
            if (m4.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m4.Groups["adr"].Value).GetAddressBytes();
                var maskBytes = IPAddress.Parse(m4.Groups["bitmask"].Value).GetAddressBytes();
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                return new IpAddressRange(new IPAddress(baseAdrBytes), new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes))));
            }

            throw new FormatException("Unknown IP range string.");
        }

        public static bool TryParse(string ipRangeString, out IpAddressRange ipRange)
        {
            try
            {
                ipRange = Parse(ipRangeString);
                return true;
            }
            catch (Exception)
            {
                ipRange = null;
                return false;
            }
        }

        /// <summary>
        /// Takes a subnetmask (eg, "255.255.254.0") and returns the CIDR bit length of that
        /// address. Throws an exception if the passed address is not valid as a subnet mask.
        /// </summary>
        /// <param name="subnetMask">The subnet mask to use</param>
        /// <returns></returns>
        public static int SubnetMaskLength(IPAddress subnetMask)
        {
            var length = Bits.GetBitMaskLength(subnetMask.GetAddressBytes());
            if (length == null) throw new ArgumentException("Not a valid subnet mask", "subnetMask");
            return length.Value;
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            var first = Begin.GetAddressBytes();
            var last = End.GetAddressBytes();
            for (var ip = first; Bits.GE(ip, last); ip = Bits.Increment(ip))
                yield return new IPAddress(ip);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the range in the format "begin-end", or 
        /// as a single address if End is the same as Begin.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Equals(Begin, End) ? Begin.ToString() : string.Format("{0}-{1}", Begin, End);
        }
    }
}