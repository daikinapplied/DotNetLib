using System.Net;
using System.Net.Http;
using Daikin.DotNetLib.Core.Tests.Models.JsonPlaceHolder;
using Daikin.DotNetLib.Network;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class NetworkTests
    {
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Simple Checks
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [Fact]
        public void ServerPathAndQuery1()
        {
            const string fullUrl = "https://www.daikinapplied.com/api/v1?key=value&key2=value2";
            var (url, path, query) = Url.SplitUrl(fullUrl);
            Assert.Equal("https://www.daikinapplied.com", url);
            Assert.Equal("/api/v1", path);
            Assert.Equal("key=value&key2=value2", query);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // WebAPI Testing
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [Fact]
        public void ApiGetDataReturned()
        {
            var (posts, response) = WebApi.Call<Posts>("https://jsonplaceholder.typicode.com/posts", HttpMethod.Get);
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(posts.Count > 0);
            Assert.Equal("sunt aut facere repellat provident occaecati excepturi optio reprehenderit", posts[0].Title);
        }

        [Fact]
        public void ApiPutDataReturned()
        {
            // The WebApi doesn't have a Put method to return data
            var (comments, response) = WebApi.Call<Comments>("https://jsonplaceholder.typicode.com/posts/1/comments", HttpMethod.Put);
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("Not Found", response.ReasonPhrase);
            Assert.Null(comments);
        }

        [Fact]
        public void ApiPostData()
        {
            var postRequest = new Post {UserId = 1, Title = "SSO Testing", Body = "Checking out the pushing of data"};

            var (postResponse, httpResponse) = WebApi.Call<Post, Post>("https://jsonplaceholder.typicode.com/posts", postRequest, HttpMethod.Post);
            Assert.True(httpResponse.IsSuccessStatusCode);
            Assert.NotNull(postResponse);
            Assert.Equal("SSO Testing", postResponse.Title);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Ipv4HostMask
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [Fact]
        public void NetworkBitLength()
        {
            var ipAddress = new Ipv4HostMask { Value = "209.148.63.10" };
            ipAddress.FromNetworkBitLength(0);
            Assert.Equal("128.0.0.0", ipAddress.Value);
        }

        [Fact]
        public void ConvertToNetworkNumber()
        {
            var ipAddress = new Ipv4HostMask { Value = "64.233.187.99" };
            Assert.Equal(1089059683, ipAddress.ToNumber());
        }

        [Fact]
        public void ConvertFromNetworkNumber()
        {
            var ipAddress = new Ipv4HostMask();
            ipAddress.FromNumber(1089059683);
            Assert.Equal("64.233.187.99", ipAddress.Value);
        }

        [Fact]
        public void IpSegments()
        {
            var ipAddress = new Ipv4HostMask {Value = "192.168.2.6"};
            Assert.Equal(192, ipAddress.GetSegment(-1));
            Assert.Equal(6, ipAddress.GetSegment(5));

            ipAddress.SetSegment(-1, 10);
            ipAddress.SetSegment(50, 110);
            Assert.Equal("10.168.2.110", ipAddress.Value);
        }


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Ipv4Address
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        [Fact]
        public void InvalidAddress()
        {
            var address = new Ipv4Address("192.168.1", "255.255.0");
            Assert.Equal("0.0.0.0", address.IpAddress);
            Assert.Equal("255.255.255.255", address.SubnetMask);
        }

        [Fact]
        public void NetworkClassC()
        {
            var address = new Ipv4Address("192.168.1.2", "255.255.255.0");
            Assert.Equal("192.168.1.2", address.IpAddress);
            Assert.Equal("255.255.255.0", address.SubnetMask);
            Assert.Equal("192.168.1.0", address.NetworkAddress);
            Assert.Equal("192.168.1.255",address.BroadcastAddress);
            Assert.Equal("192.168.1.1", address.FirstNetworkAddress);
            Assert.Equal("192.168.1.254", address.LastNetworkAddress);
            Assert.Equal(24, address.Cidr);
            Assert.Equal((uint)255, address.Hosts);
            Assert.Equal((uint)16777215, address.Networks);
            Assert.Equal("0.0.0.255", address.WildcardMask);
        }

        [Fact]
        public void NetworkClassB()
        {
            var address = new Ipv4Address("10.27.0.10", "255.255.0.0");
            Assert.Equal("10.27.0.10", address.IpAddress);
            Assert.Equal("255.255.0.0", address.SubnetMask);
            Assert.Equal("10.27.0.0", address.NetworkAddress);
            Assert.Equal("10.27.255.255", address.BroadcastAddress);
            Assert.Equal("10.27.0.1", address.FirstNetworkAddress);
            Assert.Equal("10.27.255.254", address.LastNetworkAddress);
            Assert.Equal(16, address.Cidr);
            Assert.Equal((uint)65535, address.Hosts);
            Assert.Equal((uint)65535, address.Networks);
            Assert.Equal("0.0.255.255", address.WildcardMask);
        }

        [Fact]
        public void NetworkClass22()
        {
            var address = new Ipv4Address {IpAddress = "172.137.250.11", SubnetMask = "255.255.252.0"};
            Assert.Equal("172.137.250.11", address.IpAddress);
            Assert.Equal("255.255.252.0", address.SubnetMask);
            Assert.Equal("172.137.248.0", address.NetworkAddress);
            Assert.Equal("172.137.251.255", address.BroadcastAddress);
            Assert.Equal("172.137.248.1", address.FirstNetworkAddress);
            Assert.Equal("172.137.251.254", address.LastNetworkAddress);
            Assert.Equal(22, address.Cidr);
            Assert.Equal((uint)1023, address.Hosts);
            Assert.Equal((uint)4194303, address.Networks);
            Assert.Equal("0.0.3.255", address.WildcardMask);
        }

        [Fact]
        public void NetworkSubnet()
        {
            var address1 = new Ipv4Address("10.11.12.13", "255.255.255.0") {Cidr = 8};
            var address2 = new Ipv4Address("10.12.13.14", "255.255.255.0");
            Assert.False(Ipv4Address.IsSameSubnet(address1, address2));
            Assert.Equal("255.0.0.0", address1.SubnetMask);
            address2.Cidr = 8;
            Assert.True(Ipv4Address.IsSameSubnet(address1, address2));
        }

        [Fact]
        public void WildcardMask()
        {
            var address = new Ipv4Address("10.11.12.13", "255.255.255.0") {WildcardMask = "0.0.0.255"};
            Assert.Equal("255.255.255.0", address.SubnetMask);
        }

        [Fact]
        public void NumberHostsNetworks()
        {
            var address = new Ipv4Address("10.11.12.13", "255.255.0.0");
            Assert.Equal((uint)65535, address.Hosts);
            Assert.Equal((uint)65535, address.Networks);
            
            address.Hosts = 255;
            Assert.Equal("255.255.255.0", address.SubnetMask);

            address.Networks = 255;
            Assert.Equal("255.0.0.0", address.SubnetMask);

            address.Hosts = uint.MaxValue;
            Assert.Equal("128.0.0.0", address.SubnetMask);

            address.Hosts = 0;
            Assert.Equal("255.255.255.254", address.SubnetMask);

            address.Networks = uint.MaxValue;
            Assert.Equal("255.255.255.255", address.SubnetMask);

            address.Networks = 0;
            Assert.Equal("128.0.0.0", address.SubnetMask);
        }
    }
}
