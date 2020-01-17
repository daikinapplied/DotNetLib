using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global

namespace RecursiveGeek.DotNetLib.Network
{
    /// <summary>
    /// Based on RFC 2030 for NTP Client
    /// </summary>
    /// <remarks>
    ///  Structure of the standard NTP header (as described in RFC 2030)
    ///                       1                   2                   3
    ///   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |LI | VN  |Mode |    Stratum    |     Poll      |   Precision   |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                          Root Delay                           |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                       Root Dispersion                         |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                     Reference Identifier                      |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                   Reference Timestamp (64)                    |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                   Originate Timestamp (64)                    |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                    Receive Timestamp (64)                     |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                    Transmit Timestamp (64)                    |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                 Key Identifier (optional) (32)                |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                                                               |
    ///  |                 Message Digest (optional) (128)               |
    ///  |                                                               |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///
    /// -----------------------------------------------------------------------------
    ///
    /// NTP Timestamp Format (as described in RFC 2030)
    ///                         1                   2                   3
    ///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                           Seconds                             |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                  Seconds Fraction (0-padded)                  |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// </remarks>
    public class Time
    {
        #region Enumerators
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Leap indicator field values
        //[CLSCompliantAttribute(false)]
        public enum LeapIndicatorType
        {
            NoWarning,              // 0 - No warning
            LastMinute61,           // 1 - Last minute has 61 seconds
            LastMinute59,           // 2 - Last minute has 59 seconds
            Alarm                   // 3 - Alarm condition (clock not synchronized)
        }

        // Mode field values
        //[CLSCompliantAttribute(false)]
        public enum ModeType
        {
            SymmetricActive,        // 1 - Symmetric active
            SymmetricPassive,       // 2 - Symmetric pasive
            Client,                 // 3 - Client
            Server,                 // 4 - Server
            Broadcast,              // 5 - Broadcast
            Unknown                 // 0, 6, 7 - Reserved
        }

        // Stratum field values
        //[CLSCompliantAttribute(false)]
        public enum StratumType
        {
            Unspecified,                // 0 - unspecified or unavailable
            PrimaryReference,           // 1 - primary reference (e.g. radio-clock)
            SecondaryReference,         // 2-15 - secondary reference (via NTP or SNTP)
            Reserved                    // 16-255 - reserved
        }
        #endregion

        #region Structures
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // SYSTEMTIME structure used by SetSystemTime
        [StructLayoutAttribute(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        private struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }
        #endregion

        #region Fields
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private readonly string _timeServer;                  // The URL of the time server we're connecting to
        private byte[] _ntpData = new byte[NtpDataLength];   // NTP Data Structure (as described in RFC 2030)
        #endregion

        #region Constants
        private const byte NtpDataLength = 48;      // NTP Data Structure Length
        public DateTime ReceptionTimestamp;         // Reception Timestamp

        // Offset constants for timestamps in the data structure
        private const byte OffReferenceId = 12;
        private const byte OffReferenceTimestamp = 16;
        private const byte OffOriginateTimestamp = 24;
        private const byte OffReceiveTimestamp = 32;
        private const byte OffTransmitTimestamp = 40;
        #endregion

        #region Constructors
        public Time(string host)
        {
            _timeServer = host;
        }
        #endregion

        #region Properties
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Leap Indicator
        //[CLSCompliantAttribute(false)]
        public LeapIndicatorType LeapIndicator
        {
            get
            {
                // Isolate the two most significant bits
                var val = (byte)(_ntpData[0] >> 6);
                switch (val)
                {
                    case 0: return LeapIndicatorType.NoWarning;
                    case 1: return LeapIndicatorType.LastMinute61;
                    case 2: return LeapIndicatorType.LastMinute59;
                    case 3: goto default;
                    default: return LeapIndicatorType.Alarm;
                }
            }
        }

        public byte VersionNumber
        {
            get
            {
                // Isolate bits 3 - 5
                var val = (byte)((_ntpData[0] & 0x38) >> 3);
                return val;
            }
        }

        //[CLSCompliantAttribute(false)]
        public ModeType Mode
        {
            get
            {
                // Isolate bits 0 - 3
                var val = (byte)(_ntpData[0] & 0x7);
                switch (val)
                {
                    case 0: goto default;
                    case 1: return ModeType.SymmetricActive;
                    case 2: return ModeType.SymmetricPassive;
                    case 3: return ModeType.Client;
                    case 4: return ModeType.Server;
                    case 5: return ModeType.Broadcast;
                    case 6: goto default;
                    case 7: goto default;
                    default: return ModeType.Unknown;
                }
            }
        }

        //[CLSCompliantAttribute(false)]
        public StratumType Stratum
        {
            get
            {
                var val = _ntpData[1];

                if (val == 0)
                {
                    return StratumType.Unspecified;
                }
                else if (val == 1)
                {
                    return StratumType.PrimaryReference;
                }
                else if (val <= 15)
                {
                    return StratumType.SecondaryReference;
                }
                else
                {
                    return StratumType.Reserved;
                }
            }
        }

        //[CLSCompliantAttribute(false)]
        public uint PollInterval => (uint)Math.Round(Math.Pow(2, _ntpData[2]));

        public double Precision // (in milliseconds)
            =>
                (1000 * Math.Pow(2, _ntpData[3]));

        public double RootDelay // (in milliseconds)
        {
            get
            {
                var temp = 256 * (256 * (256 * _ntpData[4] + _ntpData[5]) + _ntpData[6]) + _ntpData[7];
                return 1000 * (((double)temp) / 0x10000);
            }
        }

        public double RootDispersion // (in milliseconds)
        {
            get
            {
                var temp = 256 * (256 * (256 * _ntpData[8] + _ntpData[9]) + _ntpData[10]) + _ntpData[11];
                return 1000 * (((double)temp) / 0x10000);
            }
        }

        public string ReferenceId
        {
            get
            {
                var val = "";
                switch (Stratum)
                {
                    case StratumType.Unspecified:
                        goto case StratumType.PrimaryReference;
                    case StratumType.PrimaryReference:
                        val += (char)_ntpData[OffReferenceId + 0];
                        val += (char)_ntpData[OffReferenceId + 1];
                        val += (char)_ntpData[OffReferenceId + 2];
                        val += (char)_ntpData[OffReferenceId + 3];
                        break;
                    case StratumType.SecondaryReference:
                        switch (VersionNumber)
                        {
                            case 3: // Version 3, Reference ID is an IPv4 address
                                var address = _ntpData[OffReferenceId + 0].ToString() + "." +
                                                                    _ntpData[OffReferenceId + 1].ToString() + "." +
                                                                    _ntpData[OffReferenceId + 2].ToString() + "." +
                                                                    _ntpData[OffReferenceId + 3].ToString();
                                try
                                {
                                    var host = Dns.GetHostEntry(address); //IPHostEntry Host = System.Net.Dns.GetHostByAddress(Address);
                                    val = host.HostName + " (" + address + ")";
                                }
                                catch (Exception)
                                {
                                    val = "N/A";
                                }
                                break;
                            case 4: // Version 4, Reference ID is the timestamp of last update
                                var time = ComputeDate(GetMilliSeconds(OffReferenceId));
                                // Take care of the time zone
                                var offspan = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                                val = (time + offspan).ToString(CultureInfo.InvariantCulture);
                                break;
                            default:
                                val = "N/A";
                                break;
                        }
                        break;
                }

                return val;
            }
        }

        public DateTime ReferenceTimestamp
        {
            get
            {
                var time = ComputeDate(GetMilliSeconds(OffReferenceTimestamp));
                // Take care of the time zone
                var offspan = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
        }

        public DateTime OriginateTimestamp => ComputeDate(GetMilliSeconds(OffOriginateTimestamp));

        public DateTime ReceiveTimestamp
        {
            get
            {
                var time = ComputeDate(GetMilliSeconds(OffReceiveTimestamp));
                // Take care of the time zone
                var offspan = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
        }

        public DateTime TransmitTimestamp
        {
            get
            {
                var time = ComputeDate(GetMilliSeconds(OffTransmitTimestamp));
                // Take care of the time zone
                var offspan = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
            set => SetDate(OffTransmitTimestamp, value);
        }

        public int RoundTripDelay // (in milliseconds)
        {
            get
            {
                var span = (ReceiveTimestamp - OriginateTimestamp) + (ReceptionTimestamp - TransmitTimestamp);
                return (int)span.TotalMilliseconds;
            }
        }

        public int LocalClockOffset // (in milliseconds)
        {
            get
            {
                var span = (ReceiveTimestamp - OriginateTimestamp) - (ReceptionTimestamp - TransmitTimestamp);
                return (int)(span.TotalMilliseconds / 2);
            }
        }
        #endregion

        #region Methods
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref SYSTEMTIME time);

        // Compute date, given the number of milliseconds since January 1, 1900
        private static DateTime ComputeDate(ulong milliseconds)
        {
            var span = TimeSpan.FromMilliseconds(milliseconds);
            var time = new DateTime(1900, 1, 1);
            time += span;
            return time;
        }

        // Compute the number of milliseconds, given the offset of a 8-byte array
        private ulong GetMilliSeconds(byte offset)
        {
            ulong intpart = 0, fractpart = 0;

            for (var i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + _ntpData[offset + i];
            }
            for (var i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + _ntpData[offset + i];
            }
            var milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;
            return milliseconds;
        }

        // Compute the 8-byte array, given the date
        private void SetDate(byte offset, DateTime date)
        {
            var startOfCentury = new DateTime(1900, 1, 1, 0, 0, 0);    // January 1, 1900 12:00 AM

            var milliseconds = (ulong)(date - startOfCentury).TotalMilliseconds;
            var intpart = milliseconds / 1000;
            var fractpart = milliseconds % 1000 * 0x100000000L / 1000;

            var temp = intpart;
            for (var i = 3; i >= 0; i--)
            {
                _ntpData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (var i = 7; i >= 4; i--)
            {
                _ntpData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }
        }

        // Initialize the NTPClient data
        private void Initialize()
        {
            // Set version number to 4 and Mode to 3 (client)
            _ntpData[0] = 0x1B;
            // Initialize all other fields with 0
            for (var i = 1; i < 48; i++)
            {
                _ntpData[i] = 0;
            }
            // Initialize the transmit timestamp
            TransmitTimestamp = DateTime.Now;
        }

        // Connect to the time server and update system time
        public void Connect(bool updateSystemTime)
        {
            try
            {
                // Resolve server address
                var hostadd = Dns.GetHostEntry(_timeServer); //IPHostEntry hostadd = System.Net.Dns.Resolve(TimeServer); // Obsoleted

                var ePhost = new IPEndPoint(hostadd.AddressList[0], 123);

                //Connect the time server
                var timeSocket = new UdpClient();
                timeSocket.Connect(ePhost);

                // Initialize data structure
                Initialize();
                timeSocket.Send(_ntpData, _ntpData.Length);
                _ntpData = timeSocket.Receive(ref ePhost);
                if (!IsResponseValid())
                {
                    throw new Exception("Invalid response from " + _timeServer);
                }
                ReceptionTimestamp = DateTime.Now;
            }
            catch (SocketException e)
            {
                throw new Exception(e.Message);
            }

            // Update system time
            if (updateSystemTime)
            {
                SetTime();
            }
        }

        // Check if the response from server is valid
        public bool IsResponseValid()
        {
            if (_ntpData.Length < NtpDataLength || Mode != ModeType.Server)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Converts the object to string
        public override string ToString()
        {
            var str = "Leap Indicator: ";
            switch (LeapIndicator)
            {
                case LeapIndicatorType.NoWarning:
                    str += "No warning";
                    break;
                case LeapIndicatorType.LastMinute61:
                    str += "Last minute has 61 seconds";
                    break;
                case LeapIndicatorType.LastMinute59:
                    str += "Last minute has 59 seconds";
                    break;
                case LeapIndicatorType.Alarm:
                    str += "Alarm Condition (clock not synchronized)";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            str += "\r\nVersion number: " + VersionNumber.ToString() + "\r\n";
            str += "Mode: ";
            switch (Mode)
            {
                case ModeType.Unknown:
                    str += "Unknown";
                    break;
                case ModeType.SymmetricActive:
                    str += "Symmetric Active";
                    break;
                case ModeType.SymmetricPassive:
                    str += "Symmetric Pasive";
                    break;
                case ModeType.Client:
                    str += "Client";
                    break;
                case ModeType.Server:
                    str += "Server";
                    break;
                case ModeType.Broadcast:
                    str += "Broadcast";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            str += "\r\nStratum: ";
            switch (Stratum)
            {
                case StratumType.Unspecified:
                case StratumType.Reserved:
                    str += "Unspecified";
                    break;
                case StratumType.PrimaryReference:
                    str += "Primary Reference";
                    break;
                case StratumType.SecondaryReference:
                    str += "Secondary Reference";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            str += "\r\nLocal time: " + TransmitTimestamp.ToString(CultureInfo.InvariantCulture);
            str += "\r\nPrecision: " + Precision.ToString(CultureInfo.InvariantCulture) + " ms";
            str += "\r\nPoll Interval: " + PollInterval + " s";
            str += "\r\nReference ID: " + ReferenceId;
            str += "\r\nRoot Dispersion: " + RootDispersion.ToString(CultureInfo.InvariantCulture) + " ms";
            str += "\r\nRound Trip Delay: " + RoundTripDelay + " ms";
            str += "\r\nLocal Clock Offset: " + LocalClockOffset + " ms";
            str += "\r\n";

            return str;
        }

        // Set system time according to transmit timestamp
        private void SetTime()
        {
            SYSTEMTIME st;

            var trts = TransmitTimestamp;
            st.year = (short)trts.Year;
            st.month = (short)trts.Month;
            st.dayOfWeek = (short)trts.DayOfWeek;
            st.day = (short)trts.Day;
            st.hour = (short)trts.Hour;
            st.minute = (short)trts.Minute;
            st.second = (short)trts.Second;
            st.milliseconds = (short)trts.Millisecond;

            SetLocalTime(ref st);
        }
        #endregion

    }
}
