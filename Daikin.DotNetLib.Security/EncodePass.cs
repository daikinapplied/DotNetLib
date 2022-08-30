using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Daikin.DotNetLib.Security
{
    public class EncodePass
    {
        /// <summary>
        /// Crypto strings using 3DES (192 bits)
        /// </summary>
        public static string Encrypt(string data, string encryptKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Mode = CipherMode.ECB;
            DES.Key = GetKey(encryptKey);

            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            Byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(data);

            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// Descrypto strings using 3 DES (192 bits)
        /// </summary>
        public static string Decrypt(string data, string encryptKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            DES.Mode = CipherMode.ECB;
            DES.Key = GetKey(encryptKey);

            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESEncrypt = DES.CreateDecryptor();
            Byte[] Buffer = Convert.FromBase64String(data.Replace(" ", "+"));

            return Encoding.UTF8.GetString(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        /// <summary>
        /// Convert key in bype[] type
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static byte[] GetKey(string password)
        {
            string pwd = null;

            if (Encoding.UTF8.GetByteCount(password) < 24)
            {
                pwd = password.PadRight(24, ' ');
            }
            else
            {
                pwd = password.Substring(0, 24);
            }
            return Encoding.UTF8.GetBytes(pwd);
        }

    }
}
