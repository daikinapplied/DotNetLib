using System;
using System.Linq;

namespace RecursiveGeek.DotNetLib.Data
{
    public static class String
    {
        #region Functions
        public static string Left(string s, int size)
        {
            if (s == null) return null;
            return s.Length <= size ? s : s.Substring(0, size);
        }

        public static string Right(string s, int size)
        {
            if (s == null) return null;
            return s.Length <= size ? s : s.Substring(s.Length - size, size);
        }

        public static string MassiveReplace(string oldString, string[,] replacementMatches)
        {
            var newString = oldString;
            for (var index = 0; index < replacementMatches.Length / 2; index++)
            {
                newString = newString.Replace(replacementMatches[index, 0], replacementMatches[index, 1]);
            }
            return newString;
        }

        public static string GetIniValue(string s, string key, char keyValueDelimiter = ';', char keyValueSeparator = '=', bool caseInsentive = true) // Look for value based on key in a string of format: key1=value1;key2=value2;key3=value3 (etc)
        {
            if (caseInsentive) key = key.ToLower();
            var keyValuePairs = s.Split(keyValueDelimiter);
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var keyValuePair in keyValuePairs)
            {
                var keyValue = keyValuePair.Split(keyValueSeparator);
                if (caseInsentive) keyValue[0] = keyValue[0].ToLower();
                if (keyValue[0] == key) return keyValue[1];
            }
            return null;
        }

        public static int ConvertToInt32(string s, int defaultValue = 0)
        {
            try
            {
                return Convert.ToInt32(s);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static bool CharInString(string mainStr, string charsToCheck)
        {
            return charsToCheck.Any(c => mainStr.Contains(c.ToString()));
        }

        public static byte[] ConvertToBytes(string data)
        {
            var encoding = new System.Text.ASCIIEncoding();
            var rawData = encoding.GetBytes(data);

            return rawData;
        }

        public static string ConvertFromBytes(byte[] data)
        {
            var encoding = new System.Text.ASCIIEncoding();
            var strData = encoding.GetString(data);

            return strData;
        }

        public static string Split(string s, char delimiter, int itemNumber)
        {
            var arrayString = s.Split(delimiter);
            if (itemNumber >= 0 && itemNumber < arrayString.Length)
            {
                return arrayString[itemNumber];
            }
            return null;
        }

        public static string DelimiterBuild(string[] values, string delimiter)
        {
            var fullValue = string.Empty;
            for (var index = 0; index < values.Length; index++)
            {
                var elementValue = values[index].Replace(delimiter, string.Empty);
                if (index > 0) { fullValue += delimiter; }
                fullValue += elementValue;
            }

            return fullValue;
        }
        #endregion
    }
}
