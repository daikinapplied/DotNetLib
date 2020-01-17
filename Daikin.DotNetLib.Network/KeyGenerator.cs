using System;
using System.Text;

namespace Daikin.DotNetLib.Network
{
    public static class KeyGenerator
    {
        #region Functions
        // Possible Confusions (hence removed): 
        //   Uppercase Letter O and number 0
        //   Lowercase Letter l (L) and number 1 and pipe (|) and uppercase letter I (i)
        public static string BuildPassword(int maxSize)
        {
            return BuildUniqueKey(maxSize, "abcdefghijkmnopqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ23456789~!@#$%^&*()-+={}[]?,.:<>");
        }

        public static string BuildId(int maxSize)
        {
            return BuildUniqueKey(maxSize, "abcdefghijklmnopqrstuvwxyz123456789~!@#$%^*()-+={}[]?,.:_");
        }

        public static string BuildFilename(int maxSize)
        {
            string key;
            do
            {
                key = BuildUniqueKey(maxSize, "abcdefghijklmnopqrstuvwxyz123456789!@#$^()-=[],");
            } while (key.Contains("_vti_") || key.StartsWith("_") || key.Contains(".."));
            return key;
        }

        public static string BuildUniqueKey(int maxSize, string validCharacters)
        {
            var random = new Random(Guid.NewGuid().GetHashCode() + Environment.TickCount);
            var generatedKey = new StringBuilder();
            for (var characterLength = 0; characterLength < maxSize; characterLength++)
            {
                generatedKey.Append(validCharacters[random.Next(validCharacters.Length - 1)]);
            }
            return generatedKey.ToString();
        }
        #endregion
    }
}
