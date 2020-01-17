using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

// Remove inconsistent naming since simulating C++ coding standards in C#
// ReSharper disable InconsistentNaming

namespace RecursiveGeek.DotNetLib.Serial.Usb
{
    public class FileIO
    {
        #region Constants
        public const int FILE_FLAG_OVERLAPPED = 0X40000000;
        public const int FILE_SHARE_READ = 1;
        public const int FILE_SHARE_WRITE = 2;
        public const uint GENERIC_READ = 0X80000000;
        public const uint GENERIC_WRITE = 0X40000000;
        public const int INVALID_HANDLE_VALUE = -1;
        public const int OPEN_EXISTING = 3;
        public const int WAIT_TIMEOUT = 0X102;
        public const int WAIT_OBJECT_0 = 0;
        #endregion

        #region Functions
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CancelIo(SafeFileHandle hFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateEvent(IntPtr SecurityAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetOverlappedResult(SafeFileHandle hFile, IntPtr lpOverlapped, ref int lpNumberOfBytesTransferred, bool bWait);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);
        #endregion

        #region Classes
        [StructLayout(LayoutKind.Sequential)]
        public class SECURITY_ATTRIBUTES
        {
            public int nLength;
            public int lpSecurityDescriptor;
            public int bInheritHandle;
        }
        #endregion
    }
}
