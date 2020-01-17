using System;
using System.Xml.Serialization;

namespace RecursiveGeek.DotNetLib.Data
{
    public class Bit
    {
        #region Enumerators
        /// <summary>
        /// Bit position to flag an environment
        /// </summary>
        public enum Environment
        {
            Unknown = 0,
            Production = 1,   // PRD (Integer 1)
            Uat = 2,          // UAT (Integer 2) - this can be used as Test (depending on solution, typical)
            Stage = 3,        // STG (Integer 4) - this can be used as Test (depending on solution, not typical)
            Build = 4,        // BLD (Integer 8)
            Development = 5,  // DEV (Integer 16)
            Demo = 6          // DEM (Integer 32)
        }
        #endregion

        #region Fields
        private bool _bit;
        #endregion

        #region Properties
        [XmlText]
        public string Value
        {
            set
            {
                var s = value.ToLower();
                _bit = (s == "1" || s == "true");
            }

            get => _bit ? "1" : "0";
        }
        #endregion

        #region Constructors
        public Bit()
        {
            _bit = false;
        }

        public Bit(bool bit)
        {
            _bit = bit;
        }

        public Bit(int number)
        {
            _bit = (number != 0);
        }
        #endregion

        #region Operators
        public static implicit operator string(Bit bit)
        {
            return bit.ToString();
        }

        public static implicit operator Bit(bool bit)
        {
            return new Bit(bit);
        }

        public static implicit operator bool(Bit bit)
        {
            return bit._bit;
        }

        public static implicit operator Bit(int number)
        {
            return new Bit(number != 0);
        }

        public static Bit operator |(Bit bit1, Bit bit2)
        {
            return new Bit(bit1._bit | bit2._bit);
        }

        public static Bit operator &(Bit bit1, Bit bit2)
        {
            return new Bit(bit1._bit & bit2._bit);
        }

        public static Bit operator ^(Bit bit1, Bit bit2)
        {
            return new Bit(bit1._bit ^ bit2._bit);
        }

        public static Bit operator !(Bit bit)
        {
            return new Bit(!bit._bit);
        }

        public static bool operator ==(Bit bit1, Bit bit2)
        {
            if (bit1 == null || bit2 == null) return false;
            return bit1._bit == bit2._bit;
        }

        public static bool operator !=(Bit bit1, Bit bit2)
        {
            if (bit1 == null || bit2 == null) return false;
            return bit1._bit != bit2._bit;
        }

        public bool Equals(Bit bit)
        {
            return _bit == bit._bit;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Bit)obj);
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return _bit ? "1" : "0";
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _bit.GetHashCode();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Check to see if a bit is set within a byte
        /// </summary>
        /// <param name="b">Byte to test for a bit</param>
        /// <param name="pos">Bit position (1=based)</param>
        /// <returns>Whether bit is set</returns>
        public static bool IsBitSet(byte b, int pos)
        {
            if (pos < 1) return false;
            return (b & (1 << (pos - 1))) != 0;
        }

        /// <summary>
        /// Check to see if a bit is set within a number
        /// </summary>
        /// <param name="i">Integer number to test for a bit</param>
        /// <param name="env">Bit position (1=based) using enumerator</param>
        /// <returns>Whether bit is set</returns>
        public static bool IsBitSet(int i, Environment env)
        {
            return IsBitSet(Convert.ToByte(i), (int)env);
        }
        #endregion
    }
}
