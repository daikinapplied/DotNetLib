using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Daikin.DotNetLib.Network
{
    public static class Certificate
    {
        #region Functions
        /// <summary>
        /// Cleanse Thumbprint to make sure now hidden characters (from a copy paste, for example, via Certificate MMC)
        /// </summary>
        /// <param name="thumbprint">Thumbprint</param>
        /// <returns>Cleaned up thumbprint</returns>
        public static string CleanseThumbprint(string thumbprint)
        {
            thumbprint = Regex.Replace(thumbprint, @"[^\da-zA-z]", string.Empty).ToUpper(); // done to remove hidden characters that tend to sneak in when copying Thumbprint string
            return thumbprint;
        }

        /// <summary>
        /// Get Certificate from Thumbprint
        /// </summary>
        /// <param name="thumbprint">SSL Certificate Thumbprint</param>
        /// <returns>SSL Certificate</returns>
        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            thumbprint = CleanseThumbprint(thumbprint);
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine); // Personal Store on the Local Computer
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var certCollection = store.Certificates;
            var certs = certCollection.Find(X509FindType.FindByThumbprint, thumbprint, false);
            if (certs.Count < 1)
            {
                return null;
            }
            var cert = certs[0];
            return cert;
        }

        /// <summary>
        /// Certificate Days Remaining
        /// </summary>
        /// <param name="certificate">Certificate to review</param>
        /// <returns>Number of days remaining until expiration or before it is effective (if positive), Rrror message (if not empty)</returns>
        public static (int, string) DaysRemaining(X509Certificate2 certificate)
        {

            var effectiveDays = (DateTime.Now - Convert.ToDateTime(certificate.GetEffectiveDateString())).Days;
            if (effectiveDays < 0) return (effectiveDays, "Certificate Not Yet Effective");
            var expirationDays = (Convert.ToDateTime(certificate.GetExpirationDateString()) - DateTime.Now).Days;
            return expirationDays < 0 ? (expirationDays, "Certificate Expired") : (expirationDays, string.Empty);
        }
        #endregion
    }
}
