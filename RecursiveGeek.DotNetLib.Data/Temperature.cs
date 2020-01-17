namespace RecursiveGeek.DotNetLib.Data
{
    public class Temperature
    {
        #region Constants
        private const double KelvinConstant = 273.15;
        #endregion

        #region Enumerators
        public enum Units
        {
            Celcius,
            Fahrenheit,
            Kelvin
        }
        #endregion

        #region Fields
        private Units _units;
        private double _temperature;
        #endregion

        #region Constructors
        public Temperature(double temperature, Units units)
        {
            _temperature = temperature;
            _units = units;
        }
        #endregion

        #region Methods
        public Temperature Convert(Units units)
        {
            double temperature = 0.0;
            if (_units == units)
            {
                temperature = _temperature; // no conversion
            }
            if (_units == Units.Celcius && units == Units.Fahrenheit)
            {
                temperature = _temperature * 9.0 / 5.0 + 32.0;
            }
            if (_units == Units.Celcius && units == Units.Kelvin)
            {
                temperature = _temperature + KelvinConstant;
            }
            if (_units == Units.Fahrenheit && units == Units.Celcius)
            {
                temperature = (_temperature - 32.0) * 5.0 / 9.0;
            }
            if (_units == Units.Fahrenheit && units == Units.Kelvin)
            {
                temperature = (_temperature - 32.0) * 5.0 / 9.0 + KelvinConstant;
            }
            if (_units == Units.Kelvin && units == Units.Celcius)
            {
                temperature = _temperature - KelvinConstant;
            }
            if (_units == Units.Kelvin && units == Units.Fahrenheit)
            {
                temperature = (_temperature - KelvinConstant) * 9.0 / 5.0 + 32.0;
            }
            return new Temperature(temperature, units);
        }

        public void Calculate(Units units)
        {
            _temperature = Convert(units)._temperature;
            _units = units;
        }

        public void Set(float temperature, Units units)
        {
            _temperature = temperature;
            _units = units;
        }
        #endregion

    }
}
