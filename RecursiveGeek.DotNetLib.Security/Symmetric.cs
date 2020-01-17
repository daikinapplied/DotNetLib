using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RecursiveGeek.DotNetLib.Security
{
    public class Symmetric
    {
        #region Functions
        /// <summary>
        /// Symmetrical Encryption
        /// </summary>
        /// <param name="data">Data to Encrypt</param>
        /// <param name="encryptionKey">Encryption Key to use to Encrypt</param>
        /// <returns>Encrypted Data</returns>
        public static string Encrypt(string data, string encryptionKey)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty; // If no data to encrypt, then return no data

            var encryptKey = encryptionKey;
            var rijndaelCipher = new RijndaelManaged();

            // Convert the text to encrypt into a byte array
            var plainText = Encoding.Unicode.GetBytes(data);

            // Using Salt to make it harder to guess the key using a dictionary attack
            var salt = Encoding.ASCII.GetBytes(encryptKey.Length.ToString(CultureInfo.InvariantCulture));

            try
            {
                var secretKey = new Rfc2898DeriveBytes(encryptKey, salt); // The Secret key will be generated from the specified password and Salt

                // Create an encryptor from the SecretKey bytes (32 bytes for the secret key
                // (the default for the Rijndael key length is 256 bit = 32 bytes) and then
                // 16 bytes for the IV (initialization vector) (the default for the Rijndael IV
                // length is 128 bit = 16 bytes)
                var encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

                var memoryStream = new MemoryStream(); // Create a MemoryStream that is going to hold the encrypted bytes
                var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write); // Create a CryptoStream to process the data.  The write method puts data into the stream and the output goes to MemoryStream
                cryptoStream.Write(plainText, 0, plainText.Length); // Start the encryption process
                cryptoStream.FlushFinalBlock(); // Finish Encrypting
                var cipherBytes = memoryStream.ToArray(); // Convert the encrypted data from the memoryStream into a byte array

                memoryStream.Close();
                cryptoStream.Close();

                var encryptedData = Convert.ToBase64String(cipherBytes); // Convert the encrypted byte data into a base64 encoded string
                return encryptedData;
            }
            catch (CryptographicException ce)
            {
                throw new CryptographicException("SymmetrciEncrypt method had a problem", ce.InnerException);
            }
        }

        /// <summary>
        /// Symmetrical Decryption
        /// </summary>
        /// <param name="data">Encrypted Data to Decrypt</param>
        /// <param name="encryptionKey">Encryption Key</param>
        /// <returns>Decrypted data</returns>
        public static string Decrypt(string data, string encryptionKey)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty; // If no data to decrypt, then return no data

            var encryptKey = encryptionKey;
            var rijndaelCipher = new RijndaelManaged();

            try
            {
                var encryptedData = Convert.FromBase64String(data);
                var salt = Encoding.ASCII.GetBytes(encryptKey.Length.ToString(CultureInfo.InvariantCulture));
                var secretKey = new Rfc2898DeriveBytes(encryptKey, salt);

                var decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)); // Create a decryptor from the existing SecretKey bytes.
                var memoryStream = new MemoryStream(encryptedData);
                var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read); // Create a CryptoStream. (always use Read mode for decryption).

                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold EncryptedData;
                // DecryptedData is never longer than EncryptedData.
                var plainText = new byte[encryptedData.Length];
                var decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length); // Start decrypting.

                memoryStream.Close();
                cryptoStream.Close();

                // Convert decrypted data into a string.
                var decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);

                // Return decrypted string.  
                return decryptedData;
            }
            catch (CryptographicException ce)
            {
                throw new CryptographicException("Decrypt Error Encountered (Verify Encryption Key)", ce.InnerException);

            }
            catch (FormatException ce)
            {
                throw new FormatException("Decrypt Error Encountered (Data not recognized as encrypted data.  Verify data is encrypted and associated encryption key is correct", ce.InnerException);
            }
        }
        #endregion
    }
}
