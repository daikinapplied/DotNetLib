using System.Net;
using System.Net.NetworkInformation;

namespace RecursiveGeek.DotNetLib.Network
{
    public static class Host
    {
        #region Functions
        public static string LocalFqDn()
        {
            var domainName = "." + IPGlobalProperties.GetIPGlobalProperties().DomainName;
            var hostName = Dns.GetHostName();
            if (!hostName.EndsWith(domainName)) { hostName += domainName; } // Verify the hostname does not already incldue the domain name
            return hostName;
        }
        #endregion
    }
}
