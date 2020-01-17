using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RecursiveGeek.DotNetLib.Windows
{
    public class PrintRaw
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class Docinfoa
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] Docinfoa di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        // When the function is given a printer name and an unmanaged array of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, int dwCount, string szJobName)
        {
            var di = new Docinfoa();
            var bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = szJobName;
            di.pDataType = "RAW";

            // Open the printer.
            if (!OpenPrinter(szPrinterName.Normalize(), out var hPrinter, IntPtr.Zero)) return false;
            // Start a document.
            if (StartDocPrinter(hPrinter, 1, di))
            {
                // Start a page.
                if (StartPagePrinter(hPrinter))
                {
                    // Write your bytes.
                    bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out _);
                    EndPagePrinter(hPrinter);
                }
                EndDocPrinter(hPrinter);
            }
            ClosePrinter(hPrinter);
            // If you did not succeed, GetLastError may give more information
            // about why not.
            /*
            if (bSuccess == false)
            {
                Int32 dwError = 0;
                dwError = Marshal.GetLastWin32Error();
            }
            */
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName, string szJobName)
        {

            var fs = new FileStream(szFileName, FileMode.Open); // Open the file.
            var br = new BinaryReader(fs); // Create a BinaryReader on the file.

            var nLength = Convert.ToInt32(fs.Length);
            var bytes = br.ReadBytes(nLength);
            var pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength); // Copy the managed byte array into the unmanaged array.
            var bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength, szJobName);
            Marshal.FreeCoTaskMem(pUnmanagedBytes); // Free the unmanaged memory that you allocated earlier.
            br.Close();
            fs.Close();
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString, string szJobName)
        {
            var dwCount = szString.Length; // How many characters are in the string?
            var pBytes = Marshal.StringToCoTaskMemAnsi(szString); // Assume that the printer is expecting ANSI text, and then convert the string to ANSI text.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount, szJobName); // Send the converted ANSI string to the printer.
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

    }
}
