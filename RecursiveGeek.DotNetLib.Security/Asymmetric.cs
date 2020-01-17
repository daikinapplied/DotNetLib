using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace RecursiveGeek.DotNetLib.Security
{
    public class Asymmetric
    {
        /// <summary>
        /// Asymmetrical Encryption of a String
        /// </summary>
        /// <param name="inputString">String to encrypt</param>
        /// <param name="dwKeySize">Key Size (larger is more secure but slower)</param>
        /// <param name="xmlRsaString">XML-based Encryption Key (public only needed) to Encrypt String</param>
        /// <returns>Encrypted String</returns>
        /// <remarks>Adopted from http://www.codeproject.com/Articles/10877/Public-Key-RSA-Encryption-in-C-NET</remarks>
        public static string Encrypt(string inputString, int dwKeySize, string xmlRsaString)
        {
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlRsaString);
            if (dwKeySize != rsaCryptoServiceProvider.KeySize)
            {
                throw new Exception("Key size passed doesn't match the encryption key passed");
            }

            var keySize = dwKeySize / 8;
            var bytes = Encoding.UTF32.GetBytes(inputString);
            // The hash function in use by the .NET RSACryptoServiceProvider here is SHA1
            // int maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            var maxLength = keySize - 42;
            var dataLength = bytes.Length;
            var iterations = dataLength / maxLength;
            var stringBuilder = new StringBuilder();
            for (var i = 0; i <= iterations; i++)
            {
                var tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                var encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes. It does this after encryption and before 
                // decryption. If you do not require compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors, comment out the 
                // next line and the corresponding one in the DecryptString function.
                Array.Reverse(encryptedBytes);
                // Why convert to base 64? Because it is the largest power-of-two base printable using only ASCII characters
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Asymmetrical Decryption of a String
        /// </summary>
        /// <param name="inputString">String to Decrypt</param>
        /// <param name="dwKeySize">Key Size (larger is more secure but slower)</param>
        /// <param name="xmlRsaString">XML-based Encryption Key (public only needed) to Encrypt String</param>
        /// <returns>Decrypted String</returns>
        /// <remarks>Adopted from http://www.codeproject.com/Articles/10877/Public-Key-RSA-Encryption-in-C-NET</remarks>
        public static string Decrypt(string inputString, int dwKeySize, string xmlRsaString)
        {
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlRsaString);
            if (dwKeySize != rsaCryptoServiceProvider.KeySize)
            {
                throw new Exception("Key size passed doesn't match the encryption key passed");
            }

            var base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            var iterations = inputString.Length / base64BlockSize;
            var arrayList = new ArrayList();
            for (var i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors, comment out the 
                // next line and the corresponding one in the EncryptString function.
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            var byteType = Type.GetType("System.Byte");
            if (byteType == null) return string.Empty;
            return !(arrayList.ToArray(byteType) is byte[] decryptedArray) ? string.Empty : Encoding.UTF32.GetString(decryptedArray);
        }

        /// <summary>
        /// Create a Public and Private XML-based Key for RSA asymmetrical encryption/decryption
        /// </summary>
        /// <param name="keySize">Size of the Key (larger is more secure but slower)</param>
        /// <param name="xmlRsaPublicKeyOnly">XML data with only the public key (hand this out)</param>
        /// <param name="xmlRsaPublicAndPrivateKey">XML data with the public and private key (keep this secret)</param>
        public static void CreateRsaKey(int keySize, out string xmlRsaPublicKeyOnly, out string xmlRsaPublicAndPrivateKey)
        {
            var rsa = new RSACryptoServiceProvider(keySize);
            xmlRsaPublicKeyOnly = rsa.ToXmlString(false);
            xmlRsaPublicAndPrivateKey = rsa.ToXmlString(true);
        }
    }
}
