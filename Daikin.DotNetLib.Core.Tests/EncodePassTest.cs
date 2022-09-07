using Daikin.DotNetLib.Security;
using System;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class EncodePassTest
    {
        [Theory]
        [InlineData(@"ThisIs_A_Test_InputData&%", "TestKey__", "6vHm0YpmqBfBv68rv7nXAQZK4OIdOohHpHTuOgCZiX0=")]
        public void Encryption3DESTest(string data, string testKey, string expected)
        {
            string encryptedData = EncodePass.Encryption3DES(data, testKey);
            Assert.Equal(expected, encryptedData);
        }

        [Theory]
        [InlineData("6vHm0YpmqBfBv68rv7nXAQZK4OIdOohHpHTuOgCZiX0=", "TestKey__", @"ThisIs_A_Test_InputData&%")]
        public void Decryption3DESTest(string data, string testKey, string expected)
        {
            string encryptedData = EncodePass.Decryption3DES(data, testKey);
            Assert.Equal(expected, encryptedData);
        }


        [Theory]
        [InlineData(@"ThisIs_A_Test_InputData&%", "DaikinTest")]
        public void EncryptUsingCertificateTest(string data, string certPass)
        {
            string pathEn = Environment.CurrentDirectory + @"\TestCertificate\mycertpublickey.pem";
            string pathDe = Environment.CurrentDirectory + @"\TestCertificate\mycertprivatekey.pfx";
            string encryptedData = EncodePass.EncryptUsingCertificate(data, pathEn);
            string decryptedData = EncodePass.DecryptUsingCertificate(encryptedData, pathDe, certPass);
            Assert.Equal(data, decryptedData);
        }
    }
}
