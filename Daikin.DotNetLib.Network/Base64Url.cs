using System;

namespace Daikin.DotNetLib.Network
{
    public static class Base64Url
    {
        #region Functions
        /// <summary>
        /// Encode bytes to base64 that is URL-safe
        /// </summary>
        /// <param name="bytesToEncode">Any array of bytes</param>
        /// <returns>Base64 string that is URL safe</returns>
        public static string Encode(byte[] bytesToEncode)
        {
            var encodeString = Convert.ToBase64String(bytesToEncode); // Standard base64 encoder

            encodeString = encodeString.Split('=')[0]; // Remove any trailing '='s
            encodeString = encodeString.Replace('+', '-'); // 62nd char of encoding
            encodeString = encodeString.Replace('/', '_'); // 63rd char of encoding

            return encodeString;
        }

        /// <summary>
        /// Convert string from base64 URL-safe to an array of bytes
        /// </summary>
        /// <param name="stringToDecode">String to decode</param>
        /// <returns>Decoded string after reversing the URL-safety and converting from base64</returns>
        public static byte[] Decode(string stringToDecode)
        {
            var encodedString = stringToDecode;
            encodedString = encodedString.Replace('-', '+'); // 62nd char of encoding
            encodedString = encodedString.Replace('_', '/'); // 63rd char of encoding

            switch (encodedString.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: encodedString += "=="; break; // Two pad chars
                case 3: encodedString += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }

            return Convert.FromBase64String(encodedString); // Standard base64 decoder
        }
        #endregion
    }
}
