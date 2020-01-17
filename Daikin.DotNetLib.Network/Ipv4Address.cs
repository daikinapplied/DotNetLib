using System;

namespace Daikin.DotNetLib.Network
{
    public class Ipv4Address
    {
        #region Fields
        private readonly Ipv4HostMask _ipAddress = new Ipv4HostMask();
        private readonly Ipv4HostMask _subnetMask = new Ipv4HostMask(true);
        #endregion

        #region Properties
        public string IpAddress
        {
            get => _ipAddress.Value;
            set => _ipAddress.Value = value;
        }

        public string SubnetMask
        {
            get => _subnetMask.Value;
            set => _subnetMask.Value = value;
        }

        public string WildcardMask
        {
            get => _subnetMask.GetInverseMask();
            set => _subnetMask.SetInverseMask(value);
        }

        public int Cidr
        {
            get => _subnetMask.ToCidr();
            set => _subnetMask.FromCidr(value);
        }

        public uint Hosts
        {
            get => _subnetMask.ToNumberHosts();
            set => _subnetMask.FromNumberHosts(value);
        }

        public uint Networks
        {
            get => _subnetMask.ToNumberNetworks();
            set => _subnetMask.FromNumberNetworks(value);
        }

        public string BroadcastAddress => BuildBroadcastAddress().IpAddress;

        public string NetworkAddress => BuildNetworkAddress().IpAddress;

        public string FirstNetworkAddress
        {
            get
            {
                var firstNetworkAddress = BuildNetworkAddress();
                const int adjustSegment = Ipv4HostMask.MaxSegments - 1;
                firstNetworkAddress._ipAddress.SetSegment(adjustSegment,(byte)(Convert.ToInt16(firstNetworkAddress._ipAddress.GetSegment(adjustSegment)) + 1));
                return firstNetworkAddress.IpAddress;
            }
        }

        public string LastNetworkAddress
        {
            get
            {
                var lastNetworkAddress = BuildBroadcastAddress();
                const int adjustSegment = Ipv4HostMask.MaxSegments - 1;
                lastNetworkAddress._ipAddress.SetSegment(adjustSegment, (byte)(Convert.ToInt16(lastNetworkAddress._ipAddress.GetSegment(adjustSegment)) - 1));
                return lastNetworkAddress.IpAddress;
            }
        }
        #endregion

        #region Constructors
        public Ipv4Address(string ipAddress = "", string subnetMask = "")
        {
            if (ipAddress.Length > 0) _ipAddress.Value = ipAddress;
            if (subnetMask.Length > 0) _subnetMask.Value = subnetMask;
        }
        #endregion

        #region Methods
        private Ipv4Address BuildNetworkAddress()
        {
            var networkAddress = new Ipv4Address {SubnetMask = SubnetMask};
            for (var index = 0; index < Ipv4HostMask.MaxSegments; index++)
            {
                networkAddress._ipAddress.SetSegment(index, (byte)(_ipAddress.GetSegment(index) & _subnetMask.GetSegment(index)));
            }
            return networkAddress;
        }

        private Ipv4Address BuildBroadcastAddress()
        {
            var broadcastAddress = new Ipv4Address {SubnetMask = SubnetMask};
            for (var index = 0; index < Ipv4HostMask.MaxSegments; index++)
            {
                broadcastAddress._ipAddress.SetSegment(index, (byte)(_ipAddress.GetSegment(index) | (_subnetMask.GetSegment(index) ^ 255)));
            }
            return broadcastAddress;
        }
        #endregion

        #region Functions
        public static bool IsSameSubnet(Ipv4Address address1, Ipv4Address address2)
        {
            return address1.BuildNetworkAddress()._ipAddress.Value.Equals(address2.BuildNetworkAddress()._ipAddress.Value);
        }
        #endregion
    }
}
