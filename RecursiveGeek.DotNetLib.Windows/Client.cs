using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Net.NetworkInformation;

namespace RecursiveGeek.DotNetLib.Windows
{
    public static class Client
    {
        public static string GetLocalFqdn()
        {
            var domainName = "." + IPGlobalProperties.GetIPGlobalProperties().DomainName;
            var hostName = Dns.GetHostName();
            if (!hostName.EndsWith(domainName)) { hostName += domainName; } // Verify the hostname does not already incldue the domain name
            return hostName;
        }

        public static string GetFriendlyName()
        {
            //Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            //var principal = (WindowsPrincipal)Thread.CurrentPrincipal; // var principal = (WindowsPrincipal)User; // ASP.NET
            //using (var pc = new PrincipalContext(ContextType.Domain))
            //{
            //    var up = UserPrincipal.FindByIdentity(pc, principal.Identity.Name);
            //    return up.DisplayName;
            //    // or return up.GivenName + " " + up.Surname;
            //}

            var userPrincipal = UserPrincipal.Current;
            return userPrincipal.GivenName; //userPrincipal.DisplayName;
        }
    }
}
