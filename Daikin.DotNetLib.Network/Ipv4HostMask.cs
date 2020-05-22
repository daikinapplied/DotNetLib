using System;
using System.Linq;

namespace Daikin.DotNetLib.Network
{
    public class Ipv4HostMask
    {
        #region Constants
        public const int MaxSegments = 4;
        public const char Separator = '.';
        #endregion

        #region Fields
        private Ipv4Segment[] _ipv4Segments;
        #endregion

        #region Properties
        public string Value
        {
            set
            {
                var segments = value.Split(Separator);
                if (segments.Length != MaxSegments) return; // invalid structure passed
                for (var index = 0; index < MaxSegments; index++)
                {
                    _ipv4Segments[index].Value = Convert.ToByte(segments[index]);
                }
            }

            get
            {
                var host = _ipv4Segments[0].Value.ToString();
                for (var index = 1; index < MaxSegments; index++)
                {
                    host += Separator.ToString() + _ipv4Segments[index].Value;
                }
                return host;
            }
        }
        #endregion

        #region Constructors
        public Ipv4HostMask() // Create empty constructor for Razor (e.g., ASP.NET Core 3.1)
        {
            Init();
        }

        public Ipv4HostMask(bool maxValue = false)
        {
            Init(maxValue);
        }
        #endregion

        #region Methods
        private void Init(bool maxValue = false)
        {
            _ipv4Segments = new Ipv4Segment[MaxSegments];
            for (var index = 0; index < MaxSegments; index++)
            {
                _ipv4Segments[index] = new Ipv4Segment();
            }
            AssignFill((byte)(maxValue ? 255 : 0));
        }

        private void AssignFill(byte value)
        {
            for (var index = 0; index < MaxSegments; index++)
            {
                _ipv4Segments[index].Value = value;
            }
        }

        public long ToNumber()
        {
            var address = (long) 0;
            for (var index = 0; index < MaxSegments; index++)
            {
                address += (Convert.ToInt64(_ipv4Segments[index].Value) << (MaxSegments - index - 1) * 8);
            }
            return address;
        }

        public void FromNumber(long address)
        {
            var mask = MaskValueFromCidr(8); // 8 bit mask
            for (var index = 0; index < MaxSegments; index++)
            {
                var segment = (address & mask) >> ((MaxSegments - 1 - index) * 8);
                _ipv4Segments[index].Value = Convert.ToByte(segment);
                mask >>= 8;
            }
        }

        public void FromCidr(int cidr)
        {
            FromNumber(MaskValueFromCidr(cidr));
        }

        public int ToCidr()
        {
            var bytes = new byte[MaxSegments];
            for (var index = 0; index < MaxSegments; index++)
            {
                bytes[index] = _ipv4Segments[index].Value;
            }
            return Convert.ToString(BitConverter.ToInt32(bytes, 0), 2).ToCharArray().Count(x => x == '1');
        }

        public byte GetSegment(int index)
        {
            if (index < 0) index = 0;
            if (index > MaxSegments - 1) index = MaxSegments - 1;
            return _ipv4Segments[index].Value;
        }

        public void SetSegment(int index, byte value)
        {
            if (index < 0) index = 0;
            if (index > MaxSegments - 1) index = MaxSegments - 1;
            _ipv4Segments[index].Value = value;
        }

        public void FromHostBitLength(int hostPartLength)
        {
            if (hostPartLength > 31) hostPartLength = 31;
            var networkPartLength = 32 - hostPartLength;
            for (var index = 0; index < MaxSegments; index++)
            {
                if ((index + 1) * 8 <= networkPartLength)
                {
                    _ipv4Segments[index].Value = 255;
                }
                else if (index * 8 > networkPartLength)
                {
                    _ipv4Segments[index].Value = 0;
                }
                else
                {
                    var divider = networkPartLength - index * 8;
                    var binaryDigit = string.Empty.PadLeft(divider, '1').PadRight(8, '0');
                    _ipv4Segments[index].Value = Convert.ToByte(binaryDigit, 2);
                }
            }
        }

        public void FromNetworkBitLength(int networkPartLength)
        {
            if (networkPartLength < 1) networkPartLength = 1;
            var hostPartLength = 32 - networkPartLength;
            FromHostBitLength(hostPartLength);
        }

        public void FromNumberHosts(uint numberOfHosts)
        {
            FromHostBitLength(Convert.ToString(numberOfHosts, 2).Length);
        }

        public uint ToNumberHosts()
        {
            var bits = Bits();
            var numberHosts = (uint)0;
            for (var index = 0; index < bits.Length; index++)
            {
                if (bits[bits.Length - index - 1] != '0') break;
                numberHosts += Convert.ToUInt32(Math.Pow(2, index));
            }
            return numberHosts;
        }

        public void FromNumberNetworks(uint numberOfNetworks)
        {
            FromNetworkBitLength(Convert.ToString(numberOfNetworks, 2).Length);
        }

        public uint ToNumberNetworks()
        {
            var bits = Bits();
            
            var numberNetworks = (uint)0;
            for (var index = 0; index < bits.Length; index++)
            {
                if (bits[index] != '1') break;
                numberNetworks += Convert.ToUInt32(Math.Pow(2, index));
            }
            return numberNetworks;
        }


        public void ToggleInverseMask()
        {
            for (var index = 0; index < MaxSegments; index++)
            {
                _ipv4Segments[index].Value = (byte)(255 - _ipv4Segments[index].Value);
            }
        }

        public void SetInverseMask(string subnetMask)
        {
            Value = subnetMask;
            ToggleInverseMask();
        }

        public string GetInverseMask()
        {
            var inverseMask = string.Empty;
            for (var index = 0; index < MaxSegments; index++)
            {
                if (inverseMask.Length > 0) inverseMask += ".";
                inverseMask += (255 - _ipv4Segments[index].Value).ToString();
            }
            return inverseMask;
        }


        private string Bits()
        {
            var bits = string.Empty;
            for (var index = 0; index < MaxSegments; index++)
            {
                bits += Convert.ToString(_ipv4Segments[index].Value, 2).PadLeft(8, '0');
            }
            return bits;
        }

        //private string InverseBits()
        //{
        //    var bits = Bits();
        //    var inverseBits = string.Empty;
        //    foreach (var bit in bits)
        //    {
        //        inverseBits += (bit == '1' ? '0' : '1');
        //    }
        //    return inverseBits;
        //}
        #endregion

        #region Functions
        private static uint MaskValueFromCidr(int cidr)
        {

            return Convert.ToUInt32(Math.Pow(2, 8 * MaxSegments - cidr) - 1) ^ 4294967295;
        }
        #endregion
    }
}
