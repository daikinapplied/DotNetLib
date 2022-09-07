using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Daikin.DotNetLib.Security
{
    public class EncodePass
    {

        #region TripleDES_Certificate
        /// <summary>
        /// Encrypt data by 3DES method
        /// </summary>
        /// <param name="data">the data to be encrypted</param>
        /// <param name="encryptKey">the key used to encrypt data</param>
        /// <returns>the encrypted data</returns>
        public static string Encryption3DES(string data, string encryptKey)
        {
            try
            {
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

                DES.Mode = CipherMode.ECB;
                DES.Key = GetKey(encryptKey);

                DES.Padding = PaddingMode.PKCS7;
                ICryptoTransform DESEncrypt = DES.CreateEncryptor();
                Byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(data);

                return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Decrypt data by 3DES method
        /// </summary>
        /// <param name="data">the data to be decrypted</param>
        /// <param name="encryptKey">the key to be used to encrypt data</param>
        /// <returns>the decrypted data</returns>
        public static string Decryption3DES(string data, string encryptKey)
        {
            try
            {
                TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

                DES.Mode = CipherMode.ECB;
                DES.Key = GetKey(encryptKey);

                DES.Padding = PaddingMode.PKCS7;
                ICryptoTransform DESEncrypt = DES.CreateDecryptor();
                Byte[] Buffer = Convert.FromBase64String(data.Replace(" ", "+"));

                return Encoding.UTF8.GetString(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Convert key in bype[] type
        /// </summary>
        /// <param name="password"></param>
        /// <returns>a key with 24 characters</returns>
        private static byte[] GetKey(string password)
        {
            string pwd = "";

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
        #endregion TripleDES_Certificate

        #region RSA_Certificate
        /// <summary>
        /// Encrypt the input data
        /// To use this method, use openssl to generate X509 Certificate with RSA public key first.
        /// </summary>
        /// <param name="data">the data to be encrypted</param>
        /// <param name="certPath">the path to the certificate file containing RSA public key</param>
        /// <returns>the encrypted data</returns>
        public static string EncryptUsingCertificate(string data, string certPath)
        {
            try
            {
                byte[] byteData = Encoding.UTF8.GetBytes(data);
                var collection = new X509Certificate2Collection();
                collection.Import(certPath);
                var certificate = collection[0];
                var output = "";
                using (RSA csp = (RSA)certificate.PublicKey.Key)
                {
                    byte[] bytesEncrypted = csp.Encrypt(byteData, RSAEncryptionPadding.OaepSHA1);
                    output = Convert.ToBase64String(bytesEncrypted);
                }
                return output;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        /// <summary>
        /// Decrypt the data encrypted by RSA public key
        /// To use this method, use openssl to generate X509 Certificate with RSA private key first.
        /// </summary>
        /// <param name="data">the data to be decrypted</param>
        /// <param name="certPath">the path to the certificate file containing RSA private key</param>
        /// <param name="certPass">the password to access the certificate file</param>
        /// <returns>the decrypted data</returns>
        public static string DecryptUsingCertificate(string data, string certPath, string certPass)
        {
            try
            {
                string returnValue = "";
                byte[] byteData = Convert.FromBase64String(data); 
                var collection = new X509Certificate2Collection();
                collection.Import(File.ReadAllBytes(certPath), certPass, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                X509Certificate2 certificate = new X509Certificate2();
                certificate = collection[0];
                // Keep the following code please for the future
                //foreach (var cert in collection)
                //{
                //    if (cert.FriendlyName.Contains("my certificate"))
                //    {
                //        certificate = cert;
                //    }
                //}
                if (certificate.HasPrivateKey)
                {
                    RSA csp = (RSA)certificate.PrivateKey;
                    var privateKey = certificate.PrivateKey as RSACryptoServiceProvider;
                    returnValue = Encoding.UTF8.GetString(csp.Decrypt(byteData, RSAEncryptionPadding.OaepSHA1));
                }
                return returnValue;
            }
            catch (Exception ex) 
            {
                return "";
            }
        }
        #endregion Encryption_Decryption_Certificate
    }
}
