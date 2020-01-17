using System;
using System.Globalization;

namespace Daikin.DotNetLib.Data
{
    [Serializable]
    public class Percent
    {
        #region Fields
        private double _percent;
        #endregion

        #region Properties

        public string Value
        {
            get => _percent.ToString(CultureInfo.InvariantCulture);
            set => _percent = Convert.ToDouble(value);
        }
        #endregion

        #region Constructors
        public Percent()
        {
            _percent = 0.0;
        }

        public Percent(Percent percent)
        {
            Set(percent._percent);
        }

        public Percent(double percent)
        {
            Set(percent);
        }

        public Percent(int percent)
        {
            Set(percent);
        }

        public Percent(string percent)
        {
            Set(percent);
        }
        #endregion

        #region Operators
        public static implicit operator Percent(double percent)
        {
            return new Percent(percent);
        }

        public static implicit operator Percent(int percent)
        {
            return new Percent(percent);
        }

        public static implicit operator Percent(string percent)
        {
            return new Percent(percent);
        }

        public static Percent operator +(Percent first, Percent second)
        {
            return new Percent(first._percent + second._percent);
        }

        public static Percent operator -(Percent first, Percent second)
        {
            return new Percent(first._percent - second._percent);
        }
        #endregion

        #region Methods
        //private void Set(Percent percent)
        //{
        //    Set(percent._percent);
        //}

        private void Set(double percent)
        {
            _percent = percent;
        }

        private void Set(int percent)
        {
            _percent = percent / 100.0;
        }

        private void Set(string percent)
        {
            try
            {
                var percentInt = Convert.ToInt32(percent);
                Set(percentInt);
            }
            catch
            {
                _percent = 0.0;
            }
        }

        public override string ToString()
        {
            return (Math.Round(_percent * 100, 0)).ToString(CultureInfo.InvariantCulture);
        }

        //public int ToInt()
        //{
        //    return Convert.ToInt32(Math.Round(this._percent * 100, 0));
        //}

        public double ToDouble()
        {
            return _percent;
        }
        #endregion
    }
}
