using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace RecursiveGeek.DotNetLib.Security
{
    public class Machine
    {
        #region Functions
        public static byte[] Encrypt(string keyPassword, string machineKey, string passphase)
        {
            var saltKey = Encoding.UTF8.GetBytes(machineKey);
            var clearBytes = Encoding.Unicode.GetBytes(passphase);
            using (var encryptor = Aes.Create())
            {
                if (encryptor == null) return null;
                var pdb = new Rfc2898DeriveBytes(keyPassword, saltKey);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    return ms.ToArray();
                }
            }
        }

        public static SecureString GetSecureString(string keyPassword, string passPhrase)
        {
            var passWord = new SecureString();
            var secondKey = Environment.MachineName;
            if (Environment.GetEnvironmentVariable("PassPhrase") != null)
            {
                passPhrase = Environment.GetEnvironmentVariable("PassPhrase");
                secondKey = Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%");
            }
            var pass = Decrypt(keyPassword, passPhrase, secondKey);
            foreach (var c in pass) passWord.AppendChar(c);
            return passWord;
        }

        public static SecureString GetSecureString(string keyPassword, string passPhrase, string machineName)
        {
            var passWord = new SecureString();
            var pass = Decrypt(keyPassword, passPhrase, machineName);
            foreach (var c in pass) passWord.AppendChar(c);

            return passWord;
        }

        internal static string Decrypt(string keyPassword, string passPhrase, string secondKey)
        {
            var cipherBytes = Convert.FromBase64String(passPhrase);
            var saltKey = Encoding.UTF8.GetBytes(secondKey);
            using (var encryptor = Aes.Create())
            {
                if (encryptor == null) return null;
                var pdb = new Rfc2898DeriveBytes(keyPassword, saltKey);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }
        #endregion
    }
}
