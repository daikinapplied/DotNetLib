namespace RecursiveGeek.DotNetLib.Network
{
    public class Ipv4Segment
    {
        #region Fields
        private byte _value;
        #endregion

        #region Properties
        public byte Value { 
            get => _value;
            set
            {
                var proposedValue = value;
                _value = proposedValue;
            }
        }
        #endregion
    }
}
