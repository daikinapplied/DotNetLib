using System;
using System.Runtime.InteropServices;

// Remove inconsistent naming since simulating C++ coding standards in C#
// ReSharper disable InconsistentNaming

namespace RecursiveGeek.DotNetLib.Serial.Usb
{
    public class Debugging
    {
        #region Constants
        public const short FORMAT_MESSAGE_FROM_SYSTEM = 0X1000;
        #endregion

        #region Methods
        ///  <summary>
        ///  Get text that describes the result of an API call.
        ///  </summary>
        ///  <param name="functionName"> the name of the API function. </param>
        ///  <returns>The text.</returns>
        public string ResultOfAPICall(string functionName)
        {
            var resultCode = Marshal.GetLastWin32Error(); // Returns the result code for the last API call.
            var resultString = new string(Convert.ToChar(0), 129); // Get the result message that corresponds to the code.

            long temp = 0;
            var bytes = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, ref temp, resultCode, 0, resultString, 128, 0);

            // Subtract two characters from the message to strip the CR and LF.
            if (bytes > 2)
            {
                resultString = resultString.Remove(bytes - 2, 2);
            }

            // Create the String to return.
            resultString = "\r\n" + functionName + "\r\n" + "Result = " + resultString + "\r\n";
            return resultString;
        }
        #endregion

        #region Functions
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int FormatMessage(int dwFlags, ref long lpSource, int dwMessageId, int dwLanguageZId, string lpBuffer, int nSize, int Arguments);
        #endregion
    }
}
