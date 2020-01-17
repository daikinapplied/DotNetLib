using System;

namespace RecursiveGeek.DotNetLib.Windows
{
    public class Registry
    {
        #region Methods
        /// <summary>
        /// Get HKLM Value
        /// </summary>
        /// <param name="keyName">Key Name</param>
        /// <param name="subKeyRef">Sub-Key Reference</param>
        /// <param name="keyVal">Key Value (by Reference)</param>
        /// <returns>Whether value found</returns>
        public static bool HklmGetValue(string keyName, string subKeyRef, out string keyVal)
        {
            var regHklm = Microsoft.Win32.Registry.LocalMachine;
            var regSubKey = regHklm.OpenSubKey(keyName);
            if (regSubKey == null) throw new Exception($"Unable to access Key '{keyName}'");
            keyVal = regSubKey.GetValue(subKeyRef, null).ToString();
            return keyVal.Length > 0;
        }
        #endregion
    }
}
