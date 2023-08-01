using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Daikin.DotNetLib.Network
{
    public static class Ip
    {
        #region Functions
        public static bool IsV4Address(string ipTest)
        {
            const string validIpv4Regex = "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            var regEx = new Regex(validIpv4Regex);
            return regEx.IsMatch(ipTest);
        }

        public static bool IsV6Address(string ipTest)
        {
            const string validIpv6Regex = "(?:^|(?<=\\s))(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))(?=\\s|$)";
            var regEx = new Regex(validIpv6Regex);
            return regEx.IsMatch(ipTest);
        }

        public static bool IsHostName(string hostTest)
        {
            // const string validHostnameRegex = "^([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]{{0,61}}[a-zA-Z0-9])(\\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]{{0,61}}[a-zA-Z0-9]))*$";
            const string validHostnameRegex = "^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\\-]*[a-zA-Z0-9])\\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\\-]*[A-Za-z0-9])$";
            if (IsV4Address(hostTest) || IsV6Address(hostTest)) return false;
            var regEx = new Regex(validHostnameRegex);
            return regEx.IsMatch(hostTest);
        }

        public static bool HasRemoteAccess(string filterAccess, System.Net.IPAddress ipRemote)
        {
            var filterAccessEntries = filterAccess.Split(" || ");
            foreach (var filterAccessEntry in filterAccessEntries) // Hosts and IPs Allowed Access
            {
                // Determine if access check is IP address (which allows RegEx) or hostname
                if (IsHostName(filterAccessEntry))
                {
                    try
                    {
                        var ipHostEntry = System.Net.Dns.GetHostEntry(filterAccessEntry); // Convert hostname to IP address
                        if (ipHostEntry.AddressList.Contains(ipRemote)) return true; // Remote IP matches an IP Address associated with the hostname.  Access Granted.
                    }
                    catch (Exception)
                    {
                        // Do nothing - likely a bad hostname
                    }
                }
                else
                {
                    var regEx = new Regex($"^{filterAccessEntry}$");
                    if (regEx.IsMatch(ipRemote?.ToString() ?? string.Empty)) return true; // Remote IP matches the RegEx of an acceptable IP address.  Access Granted.
                }
            }
            return false; // no access
        }

        // Not needed with .NET Core
        public static string[] Split(this string data, string sep)
        {
            return data.Split(new[] { sep }, StringSplitOptions.None);
        }
        #endregion

    }
}
