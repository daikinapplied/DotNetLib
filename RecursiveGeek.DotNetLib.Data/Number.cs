namespace RecursiveGeek.DotNetLib.Data
{
    public static class Number
    {
        #region Enumerators
        public enum GetDigitsType
        {
            FirstDigits,
            AllDigits
        }

        public enum Base
        {
            Hexidecimal = 16,
            Decimal = 10,
            Octal = 8,
            Binary = 2
        }
        #endregion

        #region Functions
        /// <summary>
        /// Get Digits from a string a characters
        /// </summary>
        /// <param name="originalValue">Original Value to Pull Digit</param>
        /// <param name="getDigitsStyle">What digits to retrieve</param>
        /// <returns>Digits only</returns>
        public static string GetDigits(string originalValue, GetDigitsType getDigitsStyle)
        {
            var newValue = string.Empty;

            for (var index = 0; index < originalValue.Length; index++)
            {
                if (char.IsDigit(originalValue, index))
                {
                    newValue += originalValue[index];
                }
                else
                {
                    if (getDigitsStyle == GetDigitsType.FirstDigits)
                    {
                        break; // all done trying to extract
                    }
                }
            }

            return newValue;
        }

        /// <summary>
        /// Convert Base 10 (Decimal) number to another numeric base
        /// </summary>
        /// <param name="decimalValue">Base 10 value</param>
        /// <param name="baseStyle">Which base to convert to</param>
        /// <returns>Converted number</returns>
        public static string DecimalToBase(int decimalValue, Base baseStyle)
        {
            return DecimalToBase((long)decimalValue, baseStyle);
        }

        /// <summary>
        /// Convert Base 10 (Decimal) number to another numeric base
        /// </summary>
        /// <param name="decimalValue">Base 10 value</param>
        /// <param name="baseStyle">Which base to convert to</param>
        /// <param name="width">Width (prepend with 0's) of output</param>
        /// <returns>Converted number</returns>
        public static string DecimalToBase(int decimalValue, Base baseStyle, int width)
        {
            return DecimalToBase((long)decimalValue, baseStyle, width);
        }

        /// <summary>
        /// Convert Base 10 (Decimal) number to another numeric base
        /// </summary>
        /// <param name="decimalValue">Base 10 value</param>
        /// <param name="baseStyle">Which base to convert to</param>
        /// <returns>Converted number</returns>
        public static string DecimalToBase(long decimalValue, Base baseStyle)
        {
            var cval = "";
            var nval = decimalValue;
            char[] aval = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            while (nval > 0)
            {
                var remainder = nval % (int)baseStyle;
                nval /= (int)baseStyle;
                cval = aval[remainder] + cval;
            }
            return cval;
        }

        /// <summary>
        /// Convert to Base 10 (Decimal) from another base
        /// </summary>
        /// <param name="baseValue">Value to convert from</param>
        /// <param name="fromBase">Base to convert from</param>
        /// <returns>Base 10 (Decimal) value</returns>
        public static int BaseToDecimal(string baseValue, Base fromBase)
        {
            var nval = 0;
            var cvalue = baseValue.ToUpper();
            for (var index = 0; index < cvalue.Length; index++)
            {
                int nchar;
                var cchar = cvalue[index];
                if (cchar >= 'A' && cchar <= 'Z')
                {
                    nchar = cchar - 'A' + 10;
                }
                else
                {
                    nchar = cchar - '0';
                }

                if (index == cvalue.Length - 1) // If the last digit, then just add it since frombase^0 = 1
                {
                    nval += nchar;
                }
                else
                {
                    nval = (nval + nchar) * (int)fromBase; // Using Nested Multiplication for efficiency
                }
            }

            return nval;
        }

        /// <summary>
        /// Convert to Base 10 (Decimal) from another base
        /// </summary>
        /// <param name="decimalBase">Value to convert from</param>
        /// <param name="baseStyle">Base to convert from</param>
        /// <param name="width">Width to prepend with 0's</param>
        /// <returns>Base 10 (Decimal) value</returns>
        public static string DecimalToBase(long decimalBase, Base baseStyle, int width)
        {
            var cval = DecimalToBase(decimalBase, baseStyle);
            while (cval.Length < width)
                cval = "0" + cval;
            return cval;
        }
        #endregion
    }
}
