using System;
using System.Text;
using System.Runtime.InteropServices; // for DllImport, Marshal
using System.ComponentModel; // for Win32Exception

namespace RecursiveGeek.DotNetLib.Security
{
    /// <summary>
    /// Summary description for Crypt32.
    /// </summary>
    public class Crypt32
    {
        // Wrapper for DPAPI CryptProtectData function.
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern
            bool CryptProtectData(ref DataBlobStruct pPlainText,
            string szDescription,
            ref DataBlobStruct pEntropy,
            IntPtr pReserved,
            ref CryptProtectPromptStruct pPrompt,
            int dwFlags,
            ref DataBlobStruct pCipherText);

        // Wrapper for DPAPI CryptUnprotectData function.
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern
            bool CryptUnprotectData(ref DataBlobStruct pCipherText,
            ref string pszDescription,
            ref DataBlobStruct pEntropy,
            IntPtr pReserved,
            ref CryptProtectPromptStruct pPrompt,
            int dwFlags,
            ref DataBlobStruct pPlainText);

        // BLOB structure used to pass data to DPAPI functions.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DataBlobStruct
        {
            public int cbData;
            public IntPtr pbData;
        }

        // Prompt structure to be used for required parameters.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct CryptProtectPromptStruct
        {
            public int cbSize;
            public int dwPromptFlags;
            public IntPtr hwndApp;
            public string szPrompt;
        }

        // Wrapper for the NULL handle or pointer.
        static private readonly IntPtr NullPtr = ((IntPtr)((int)(0)));

        // DPAPI key initialization flags.
        private const int CryptProtectUiForbiddenConst = 0x1;
        private const int CryptProtectLocalMachineConst = 0x4;

        /// <summary>Initializes empty prompt structure.</summary>
        /// <param name="ps">Prompt parameter (which we do not actually need).</param>
        private static void InitPrompt(ref CryptProtectPromptStruct ps)
        {
            ps.cbSize = Marshal.SizeOf(typeof(CryptProtectPromptStruct));
            ps.dwPromptFlags = 0;
            ps.hwndApp = NullPtr;
            ps.szPrompt = null;
        }

        /// <summary>
        /// Initializes a BLOB structure from a byte array.
        /// </summary>
        /// <param name="data">Original data in a byte array format.</param>
        /// <param name="blob">Returned blob structure.</param>
        private static void InitBlob(byte[] data, ref DataBlobStruct blob)
        {
            blob.pbData = Marshal.AllocHGlobal(data.Length); // Allocate memory for the BLOB data.
            if (blob.pbData == IntPtr.Zero) throw new Exception("Unable to allocate data buffer for BLOB structure."); // Make sure that memory allocation was successful.
            blob.cbData = data.Length; // Specify number of bytes in the BLOB.
            Marshal.Copy(data, 0, blob.pbData, data.Length); // Copy data from original source to the BLOB structure.
        }

        // Flag indicating the type of key. DPAPI terminology refers to
        // key types as user store or machine store.
        public enum KeyType { UserKey = 1, MachineKey };

        // It is reasonable to set default key type to user key.
        private const KeyType DefaultKeyType = KeyType.UserKey;

        /// <summary>
        /// Calls DPAPI CryptProtectData function to encrypt a plaintext
        /// string value with a user-specific key. This function does not
        /// specify data description and additional entropy.
        /// </summary>
        /// <param name="plainText">Plaintext data to be encrypted.</param>
        /// <returns>Encrypted value in a base64-encoded format.</returns>
        public static string Encrypt(string plainText)
        {
            return Encrypt(DefaultKeyType, plainText, string.Empty, string.Empty);
        }

        /// <summary>
        /// Calls DPAPI CryptProtectData function to encrypt a plaintext
        /// string value. This function does not specify data description
        /// and additional entropy.
        /// </summary>
        /// <param name="keyType">
        /// Defines type of encryption key to use. When user key is
        /// specified, any application running under the same user account
        /// as the one making this call, will be able to decrypt data.
        /// Machine key will allow any application running on the same
        /// computer where data were encrypted to perform decryption.
        /// Note: If optional entropy is specifed, it will be required
        /// for decryption.
        /// </param>
        /// <param name="plainText">Plaintext data to be encrypted.</param>
        /// <returns>Encrypted value in a base64-encoded format.</returns>
        public static string Encrypt(KeyType keyType, string plainText)
        {
            return Encrypt(keyType, plainText, string.Empty, string.Empty);
        }

        /// <summary>
        /// Calls DPAPI CryptProtectData function to encrypt a plaintext
        /// string value. This function does not specify data description.
        /// </summary>
        /// <param name="keyType">
        /// Defines type of encryption key to use. When user key is
        /// specified, any application running under the same user account
        /// as the one making this call, will be able to decrypt data.
        /// Machine key will allow any application running on the same
        /// computer where data were encrypted to perform decryption.
        /// Note: If optional entropy is specifed, it will be required
        /// for decryption.
        /// </param>
        /// <param name="plainText">Plaintext data to be encrypted.</param>
        /// <param name="entropy">Optional entropy which - if specified - will be required to perform decryption.</param>
        /// <returns>Encrypted value in a base64-encoded format.</returns>
        public static string Encrypt(KeyType keyType, string plainText, string entropy)
        {
            return Encrypt(keyType, plainText, entropy, string.Empty);
        }

        /// <summary>
        /// Calls DPAPI CryptProtectData function to encrypt a plaintext
        /// string value.
        /// </summary>
        /// <param name="keyType">
        /// Defines type of encryption key to use. When user key is
        /// specified, any application running under the same user account
        /// as the one making this call, will be able to decrypt data.
        /// Machine key will allow any application running on the same
        /// computer where data were encrypted to perform decryption.
        /// Note: If optional entropy is specifed, it will be required
        /// for decryption.
        /// </param>
        /// <param name="plainText">Plaintext data to be encrypted.</param>
        /// <param name="entropy">Optional entropy which - if specified - will be required to perform decryption.</param>
        /// <param name="description">
        /// Optional description of data to be encrypted. If this value is
        /// specified, it will be stored along with encrypted data and
        /// returned as a separate value during decryption.
        /// </param>
        /// <returns>Encrypted value in a base64-encoded format.</returns>
        public static string Encrypt(KeyType keyType, string plainText, string entropy, string description)
        {
            // Make sure that parameters are valid.
            if (plainText == null) plainText = string.Empty;
            if (entropy == null) entropy = string.Empty;

            // Call encryption routine and convert returned bytes into a base64-encoded value.
            return Convert.ToBase64String(Encrypt(keyType, Encoding.UTF8.GetBytes(plainText), Encoding.UTF8.GetBytes(entropy), description));
        }

        /// <summary>
        /// Calls DPAPI CryptProtectData function to encrypt an array of
        /// plaintext bytes.
        /// </summary>
        /// <param name="keyType">
        /// Defines type of encryption key to use. When user key is
        /// specified, any application running under the same user account
        /// as the one making this call, will be able to decrypt data.
        /// Machine key will allow any application running on the same
        /// computer where data were encrypted to perform decryption.
        /// Note: If optional entropy is specifed, it will be required
        /// for decryption.
        /// </param>
        /// <param name="plainTextBytes">Plaintext data to be encrypted.</param>
        /// <param name="entropyBytes">Optional entropy which - if specified - will be required to perform decryption.</param>
        /// <param name="description">
        /// Optional description of data to be encrypted. If this value is
        /// specified, it will be stored along with encrypted data and
        /// returned as a separate value during decryption.
        /// </param>
        /// <returns>Encrypted value.</returns>
        public static byte[] Encrypt(KeyType keyType, byte[] plainTextBytes, byte[] entropyBytes, string description)
        {
            // Make sure that parameters are valid.
            if (plainTextBytes == null) plainTextBytes = new byte[0];
            if (entropyBytes == null) entropyBytes = new byte[0];
            if (description == null) description = string.Empty;

            // Create BLOBs to hold data.
            var plainTextBlob = new DataBlobStruct();
            var cipherTextBlob = new DataBlobStruct();
            var entropyBlob = new DataBlobStruct();

            // We only need prompt structure because it is a required
            // parameter.
            var prompt = new CryptProtectPromptStruct();
            InitPrompt(ref prompt);

            try
            {
                // Convert plaintext bytes into a BLOB structure.
                try
                {
                    InitBlob(plainTextBytes, ref plainTextBlob);
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot initialize plaintext BLOB.", ex);
                }

                // Convert entropy bytes into a BLOB structure.
                try
                {
                    InitBlob(entropyBytes, ref entropyBlob);
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot initialize entropy BLOB.", ex);
                }

                // Disable any types of UI.
                var flags = CryptProtectUiForbiddenConst;

                // When using machine-specific key, set up machine flag.
                if (keyType == KeyType.MachineKey)
                {
                    flags |= CryptProtectLocalMachineConst;
                }

                var success = CryptProtectData(ref plainTextBlob, description, ref entropyBlob, IntPtr.Zero, ref prompt, flags, ref cipherTextBlob); // Call DPAPI to encrypt data.
                if (!success)
                {
                    var errCode = Marshal.GetLastWin32Error(); // If operation failed, retrieve last Win32 error.
                    throw new Exception("CryptProtectData failed.", new Win32Exception(errCode)); // Win32Exception will contain error message corresponding to the Windows error code.
                }

                var cipherTextBytes = new byte[cipherTextBlob.cbData]; // Allocate memory to hold ciphertext.
                Marshal.Copy(cipherTextBlob.pbData, cipherTextBytes, 0, cipherTextBlob.cbData); // Copy ciphertext from the BLOB to a byte array.
                return cipherTextBytes;
            }
            catch (Exception ex)
            {
                throw new Exception("DPAPI was unable to encrypt data.", ex);
            }
            // Free all memory allocated for BLOBs.
            finally
            {
                if (plainTextBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(plainTextBlob.pbData);
                if (cipherTextBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(cipherTextBlob.pbData);
                if (entropyBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }

        /// <summary>
        /// Calls DPAPI CryptUnprotectData to decrypt ciphertext bytes.
        /// This function does not use additional entropy and does not
        /// return data description.
        /// </summary>
        /// <param name="cipherText">
        /// Encrypted data formatted as a base64-encoded string.
        /// </param>
        /// <returns>Decrypted data returned as a UTF-8 string.</returns>
        /// <remarks>
        /// When decrypting data, it is not necessary to specify which
        /// type of encryption key to use: user-specific or
        /// machine-specific; DPAPI will figure it out by looking at
        /// the signature of encrypted data.
        /// </remarks>
        public static string Decrypt(string cipherText)
        {
            string description;
            return Decrypt(cipherText, string.Empty, out description);
        }

        /// <summary>Calls DPAPI CryptUnprotectData to decrypt ciphertext bytes. This function does not use additional entropy.</summary>
        /// <param name="cipherText">Encrypted data formatted as a base64-encoded string.</param>
        /// <param name="description"> Returned description of data specified during encryption.</param>
        /// <returns>Decrypted data returned as a UTF-8 string.</returns>
        /// <remarks>
        /// When decrypting data, it is not necessary to specify which
        /// type of encryption key to use: user-specific or
        /// machine-specific; DPAPI will figure it out by looking at
        /// the signature of encrypted data.
        /// </remarks>
        public static string Decrypt(string cipherText, out string description)
        {
            return Decrypt(cipherText, string.Empty, out description);
        }

        /// <summary>
        /// Calls DPAPI CryptUnprotectData to decrypt ciphertext bytes.
        /// </summary>
        /// <param name="cipherText">Encrypted data formatted as a base64-encoded string.</param>
        /// <param name="entropy">Optional entropy, which is required if it was specified during encryption.</param>
        /// <param name="description">Returned description of data specified during encryption.</param>
        /// <returns>Decrypted data returned as a UTF-8 string.</returns>
        /// <remarks>
        /// When decrypting data, it is not necessary to specify which
        /// type of encryption key to use: user-specific or
        /// machine-specific; DPAPI will figure it out by looking at
        /// the signature of encrypted data.
        /// </remarks>
        public static string Decrypt(string cipherText, string entropy, out string description)
        {
            if (entropy == null) entropy = string.Empty; // Make sure that parameters are valid.
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(cipherText), Encoding.UTF8.GetBytes(entropy), out description));
        }

        /// <summary>Calls DPAPI CryptUnprotectData to decrypt ciphertext bytes.</summary>
        /// <param name="cipherTextBytes">Encrypted data.</param>
        /// <param name="entropyBytes">Optional entropy, which is required if it was specified during encryption.</param>
        /// <param name="description">Returned description of data specified during encryption.</param>
        /// <returns>Decrypted data bytes.</returns>
        /// <remarks>
        /// When decrypting data, it is not necessary to specify which
        /// type of encryption key to use: user-specific or
        /// machine-specific; DPAPI will figure it out by looking at
        /// the signature of encrypted data.
        /// </remarks>
        public static byte[] Decrypt(byte[] cipherTextBytes, byte[] entropyBytes, out string description)
        {
            var plainTextBlob = new DataBlobStruct(); // Create BLOBs to hold data.
            var cipherTextBlob = new DataBlobStruct();
            var entropyBlob = new DataBlobStruct();

            var prompt = new CryptProtectPromptStruct(); // We only need prompt structure because it is a required parameter.
            InitPrompt(ref prompt);
            description = string.Empty;

            try
            {
                try
                {
                    InitBlob(cipherTextBytes, ref cipherTextBlob); // Convert ciphertext bytes into a BLOB structure.
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot initialize ciphertext BLOB.", ex);
                }

                try
                {
                    InitBlob(entropyBytes, ref entropyBlob); // Convert entropy bytes into a BLOB structure.
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot initialize entropy BLOB.", ex);
                }

                // Disable any types of UI. CryptUnprotectData does not
                // mention CRYPTPROTECT_LOCAL_MACHINE flag in the list of
                // supported flags so we will not set it up.
                const int flags = CryptProtectUiForbiddenConst;
                var success = CryptUnprotectData(ref cipherTextBlob, ref description, ref entropyBlob, IntPtr.Zero, ref prompt, flags, ref plainTextBlob); // Call DPAPI to decrypt data.
                if (!success)
                {
                    var errCode = Marshal.GetLastWin32Error(); // If operation failed, retrieve last Win32 error.
                    throw new Exception("CryptUnprotectData failed.", new Win32Exception(errCode)); // Win32Exception will contain error message corresponding to the Windows error code.
                }

                var plainTextBytes = new byte[plainTextBlob.cbData]; // Allocate memory to hold plaintext.
                Marshal.Copy(plainTextBlob.pbData, plainTextBytes, 0, plainTextBlob.cbData); // Copy ciphertext from the BLOB to a byte array.
                return plainTextBytes;
            }
            catch (Exception ex)
            {
                throw new Exception("DPAPI was unable to decrypt data.", ex);
            }
            // Free all memory allocated for BLOBs.
            finally
            {
                if (plainTextBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(plainTextBlob.pbData);
                if (cipherTextBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(cipherTextBlob.pbData);
                if (entropyBlob.pbData != IntPtr.Zero) Marshal.FreeHGlobal(entropyBlob.pbData);
            }
        }
    }

}
