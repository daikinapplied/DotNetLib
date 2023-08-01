using System.Net;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class IpTests
    {
        // Match appsettings.json
        // (NOTE: Backslash Escaped only for C# and json)
        private const string Filter1 = "127\\.0\\.0\\.1 || ::1 || 10(?:\\.(25[0-5]|24[4-9][0-9]|[1-2][0-9][0-9]|[1-9][0-9]|[0-9])){3} || 75\\.72\\.51\\.204 || bad.tonka.centralus.cloudapp.azure.com || site24x7.enduserexp.com";
        private const string Filter2 = @"11(?:\.(25[0-5]|24[4-9][0-9]|[1-2][0-9][0-9]|[1-9][0-9]|[0-9])){3}";
        private const string Filter3 = @"127\.0\.0\.1";
        private const string Filter4 = "10\\.147\\.(196|212|149)\\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[0-9][0-9]|[0-9])";

        [Fact]
        public void VerifyHostname()
        {
            Assert.True(Network.Ip.IsHostName("daikinapplied.com"));
            Assert.True(Network.Ip.IsHostName("www.r32reasons.dev.daikinapplied.com"));
            Assert.False(Network.Ip.IsHostName("127.0.0.1"));
            Assert.False(Network.Ip.IsHostName("::1"));
            Assert.False(Network.Ip.IsHostName("10.{0,255}.{0,255}.{0,255}"));
        }

        [Fact]
        public void VerifyRemoteAccessByIpAddress()
        {
            // this may only work when running live
            //Assert.That(Portable.Ip.HasRemoteAccess(filter1, new IPAddress(new byte[] { 104, 36, 18, 45 })), Is.True); // https://www.site24x7.com/multi-location-web-site-monitoring.html

            Assert.True(Network.Ip.HasRemoteAccess("localhost", new IPAddress(new byte[] {127, 0, 0, 1})));
            Assert.True(Network.Ip.HasRemoteAccess("www.r32reasons.dev.daikinapplied.com", new IPAddress(new byte[] {40, 70, 147, 2})));
            Assert.True(Network.Ip.HasRemoteAccess(Filter3, new IPAddress(new byte[] {127, 0, 0, 1})));
            Assert.True(Network.Ip.HasRemoteAccess(Filter2, new IPAddress(new byte[] {11, 22, 23, 24})));
            Assert.False(Network.Ip.HasRemoteAccess(Filter2, new IPAddress(new byte[] {111, 0, 0, 1})));
            Assert.False(Network.Ip.HasRemoteAccess(Filter1, new IPAddress(new byte[] {107, 77, 208, 232})));
            Assert.True(Network.Ip.HasRemoteAccess(Filter4, new IPAddress(new byte[] {10, 147, 196, 10})));
            Assert.False(Network.Ip.HasRemoteAccess(Filter4, new IPAddress(new byte[] {10, 147, 192, 10})));
            Assert.False(Network.Ip.HasRemoteAccess(Filter4, new IPAddress(new byte[] {75, 24, 110, 170})));
        }

        [Fact]
        public void VerifyRemoteAccessByString()
        {
            Assert.True(Network.Ip.HasRemoteAccess("localhost", "127.0.0.1"));
            Assert.True(Network.Ip.HasRemoteAccess("www.r32reasons.dev.daikinapplied.com", "40.70.147.2"));
            Assert.True(Network.Ip.HasRemoteAccess(Filter3, "127.0.0.1"));
            Assert.True(Network.Ip.HasRemoteAccess(Filter2, "11.22.23.24"));
            Assert.False(Network.Ip.HasRemoteAccess(Filter2, "111.0.0.1"));
            Assert.False(Network.Ip.HasRemoteAccess(Filter1, "107.77.208.232"));
            Assert.True(Network.Ip.HasRemoteAccess(Filter4, "10.147.196.10"));
            Assert.False(Network.Ip.HasRemoteAccess(Filter4, "10.147.192.10"));
            Assert.False(Network.Ip.HasRemoteAccess(Filter4, "75.24.110.170"));
        }
    }
}
