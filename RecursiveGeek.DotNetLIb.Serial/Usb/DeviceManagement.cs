using System;
using System.Runtime.InteropServices;

// Remove inconsistent naming since simulating C++ coding standards in C#
// ReSharper disable InconsistentNaming

namespace RecursiveGeek.DotNetLib.Serial.Usb
{
    public class DeviceManagement
    {
        // API declarations relating to device management (SetupDixxx and RegisterDeviceNotification functions).   

        #region Constants
        // from dbt.h
        public const int DBT_DEVICEARRIVAL = 0X8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0X8004;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DBT_DEVTYP_HANDLE = 6;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const int WM_DEVICECHANGE = 0X219;

        // from setupapi.h
        public const int DIGCF_PRESENT = 2;
        public const int DIGCF_DEVICEINTERFACE = 0X10;
        #endregion

        #region Methods
        ///  <summary>
        ///  Compares two device path names. Used to find out if the device name 
        ///  of a recently attached or removed device matches the name of a 
        ///  device the application is communicating with.
        ///  </summary>
        ///  
        ///  <param name="m"> a WM_DEVICECHANGE message. A call to RegisterDeviceNotification
        ///  causes WM_DEVICECHANGE messages to be passed to an OnDeviceChange routine.. </param>
        ///  <param name="mydevicePathName"> a device pathname returned by 
        ///  SetupDiGetDeviceInterfaceDetail in an SP_DEVICE_INTERFACE_DETAIL_DATA structure. </param>
        ///  
        ///  <returns>
        ///  True if the names match, False if not.
        ///  </returns>
        ///  
        public bool DeviceNameMatch(System.Windows.Forms.Message m, string mydevicePathName)
        {
            var devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE_1();
            var devBroadcastHeader = new DEV_BROADCAST_HDR();

            // The LParam parameter of Message is a pointer to a DEV_BROADCAST_HDR structure.

            Marshal.PtrToStructure(m.LParam, devBroadcastHeader);

            if ((devBroadcastHeader.dbch_devicetype != DBT_DEVTYP_DEVICEINTERFACE)) return false;

            // The dbch_devicetype parameter indicates that the event applies to a device interface.
            // So the structure in LParam is actually a DEV_BROADCAST_INTERFACE structure, 
            // which begins with a DEV_BROADCAST_HDR.

            // Obtain the number of characters in dbch_name by subtracting the 32 bytes
            // in the strucutre that are not part of dbch_name and dividing by 2 because there are 
            // 2 bytes per character.

            var stringSize = Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);

            // The dbcc_name parameter of devBroadcastDeviceInterface contains the device name. 
            // Trim dbcc_name to match the size of the string.         

            devBroadcastDeviceInterface.dbcc_name = new char[stringSize + 1];

            // Marshal data from the unmanaged block pointed to by m.LParam 
            // to the managed object devBroadcastDeviceInterface.

            Marshal.PtrToStructure(m.LParam, devBroadcastDeviceInterface);

            // Store the device name in a string.

            var DeviceNameString = new string(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);

            // Compare the name of the newly attached device with the name of the device 
            // the application is accessing (mydevicePathName).
            // Set ignorecase True.

            if ((String.Compare(DeviceNameString, mydevicePathName, StringComparison.OrdinalIgnoreCase) == 0))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        ///  <summary>
        ///  Use SetupDi API functions to retrieve the device path name of an
        ///  attached device that belongs to a device interface class.
        ///  </summary>
        ///  
        ///  <param name="myGuid"> an interface class GUID. </param>
        ///  <param name="devicePathName"> a pointer to the device path name 
        ///  of an attached device. </param>
        ///  
        ///  <returns>
        ///   True if a device is found, False if not. 
        ///  </returns>

        public bool FindDeviceFromGuid(Guid myGuid, ref string[] devicePathName)
        {
            var bufferSize = 0;
            var detailDataBuffer = IntPtr.Zero;
            bool deviceFound;
            var deviceInfoSet = new IntPtr();
            var lastDevice = false;
            var MyDeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();

            try
            {
                // ***
                //  API function

                //  summary 
                //  Retrieves a device information set for a specified group of devices.
                //  SetupDiEnumDeviceInterfaces uses the device information set.

                //  parameters 
                //  Interface class GUID.
                //  Null to retrieve information for all device instances.
                //  Optional handle to a top-level window (unused here).
                //  Flags to limit the returned information to currently present devices 
                //  and devices that expose interfaces in the class specified by the GUID.

                //  Returns
                //  Handle to a device information set for the devices.
                // ***

                deviceInfoSet = SetupDiGetClassDevs(ref myGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

                deviceFound = false;
                var memberIndex = 0;

                // The cbSize element of the MyDeviceInterfaceData structure must be set to
                // the structure's size in bytes. 
                // The size is 28 bytes for 32-bit code and 32 bits for 64-bit code.

                MyDeviceInterfaceData.cbSize = Marshal.SizeOf(MyDeviceInterfaceData);

                do
                {
                    // Begin with 0 and increment through the device information set until
                    // no more devices are available.

                    // ***
                    //  API function

                    //  summary
                    //  Retrieves a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.
                    //  On return, MyDeviceInterfaceData contains the handle to a
                    //  SP_DEVICE_INTERFACE_DATA structure for a detected device.

                    //  parameters
                    //  DeviceInfoSet returned by SetupDiGetClassDevs.
                    //  Optional SP_DEVINFO_DATA structure that defines a device instance 
                    //  that is a member of a device information set.
                    //  Device interface GUID.
                    //  Index to specify a device in a device information set.
                    //  Pointer to a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.

                    //  Returns
                    //  True on success.
                    // ***

                    var success = SetupDiEnumDeviceInterfaces
                    (deviceInfoSet,
                        IntPtr.Zero,
                        ref myGuid,
                        memberIndex,
                        ref MyDeviceInterfaceData);

                    // Find out if a device information set was retrieved.

                    if (!success)
                    {
                        lastDevice = true;

                    }
                    else
                    {
                        // A device is present.

                        // ***
                        //  API function: 

                        //  summary:
                        //  Retrieves an SP_DEVICE_INTERFACE_DETAIL_DATA structure
                        //  containing information about a device.
                        //  To retrieve the information, call this function twice.
                        //  The first time returns the size of the structure.
                        //  The second time returns a pointer to the data.

                        //  parameters
                        //  DeviceInfoSet returned by SetupDiGetClassDevs
                        //  SP_DEVICE_INTERFACE_DATA structure returned by SetupDiEnumDeviceInterfaces
                        //  A returned pointer to an SP_DEVICE_INTERFACE_DETAIL_DATA 
                        //  Structure to receive information about the specified interface.
                        //  The size of the SP_DEVICE_INTERFACE_DETAIL_DATA structure.
                        //  Pointer to a variable that will receive the returned required size of the 
                        //  SP_DEVICE_INTERFACE_DETAIL_DATA structure.
                        //  Returned pointer to an SP_DEVINFO_DATA structure to receive information about the device.

                        //  Returns
                        //  True on success.
                        // ***                     
                        SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref MyDeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);

                        // Allocate memory for the SP_DEVICE_INTERFACE_DETAIL_DATA structure using the returned buffer size.
                        detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                        // Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.
                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        // Call SetupDiGetDeviceInterfaceDetail again.
                        // This time, pass a pointer to DetailDataBuffer
                        // and the returned required buffer size.
                        SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref MyDeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, IntPtr.Zero);

                        // Skip over cbsize (4 bytes) to get the address of the devicePathName.
                        var pDevicePathName = new IntPtr(detailDataBuffer.ToInt32() + 4);

                        // Get the string containing the devicePathName.
                        devicePathName[memberIndex] = Marshal.PtrToStringAuto(pDevicePathName);


                        deviceFound = true;
                    }
                    memberIndex += 1;
                }
                while (lastDevice != true);

                return deviceFound;
            }
            finally
            {
                if (detailDataBuffer != IntPtr.Zero)
                {
                    // Free the memory allocated previously by AllocHGlobal.

                    Marshal.FreeHGlobal(detailDataBuffer);
                }
                // ***
                //  API function

                //  summary
                //  Frees the memory reserved for the DeviceInfoSet returned by SetupDiGetClassDevs.

                //  parameters
                //  DeviceInfoSet returned by SetupDiGetClassDevs.

                //  returns
                //  True on success.
                // ***
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }


        ///  <summary>
        ///  Requests to receive a notification when a device is attached or removed.
        ///  </summary>
        ///  
        ///  <param name="devicePathName"> handle to a device. </param>
        ///  <param name="formHandle"> handle to the window that will receive device events. </param>
        ///  <param name="classGuid"> device interface GUID. </param>
        ///  <param name="deviceNotificationHandle"> returned device notification handle. </param>
        ///  
        ///  <returns>
        ///  True on success.
        ///  </returns>
        ///  
        public bool RegisterForDeviceNotifications(string devicePathName, IntPtr formHandle, Guid classGuid, ref IntPtr deviceNotificationHandle)
        {
            // A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.

            var devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
            var devBroadcastDeviceInterfaceBuffer = IntPtr.Zero;
            try
            {
                // Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.

                // Set the size.

                var size = Marshal.SizeOf(devBroadcastDeviceInterface);
                devBroadcastDeviceInterface.dbcc_size = size;

                // Request to receive notifications about a class of devices.

                devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;

                devBroadcastDeviceInterface.dbcc_reserved = 0;

                // Specify the interface class to receive notifications about.

                devBroadcastDeviceInterface.dbcc_classguid = classGuid;

                // Allocate memory for the buffer that holds the DEV_BROADCAST_DEVICEINTERFACE structure.

                devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);

                // Copy the DEV_BROADCAST_DEVICEINTERFACE structure to the buffer.
                // Set fDeleteOld True to prevent memory leaks.

                Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);

                // ***
                //  API function

                //  summary
                //  Request to receive notification messages when a device in an interface class
                //  is attached or removed.

                //  parameters 
                //  Handle to the window that will receive device events.
                //  Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of 
                //  device to send notifications for.
                //  DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.

                //  Returns
                //  Device notification handle or NULL on failure.
                // ***

                deviceNotificationHandle = RegisterDeviceNotification(formHandle, devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);

                // Marshal data from the unmanaged block devBroadcastDeviceInterfaceBuffer to
                // the managed object devBroadcastDeviceInterface

                Marshal.PtrToStructure(devBroadcastDeviceInterfaceBuffer, devBroadcastDeviceInterface);



                if ((deviceNotificationHandle.ToInt32() == IntPtr.Zero.ToInt32()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            finally
            {
                if (devBroadcastDeviceInterfaceBuffer != IntPtr.Zero)
                {
                    // Free the memory allocated previously by AllocHGlobal.

                    Marshal.FreeHGlobal(devBroadcastDeviceInterfaceBuffer);
                }
            }
        }

        ///  <summary>
        ///  Requests to stop receiving notification messages when a device in an
        ///  interface class is attached or removed.
        ///  </summary>
        ///  
        ///  <param name="deviceNotificationHandle"> handle returned previously by
        ///  RegisterDeviceNotification. </param>

        public void StopReceivingDeviceNotifications(IntPtr deviceNotificationHandle)
        {
            // ***
            //  API function

            //  summary
            //  Stop receiving notification messages.

            //  parameters
            //  Handle returned previously by RegisterDeviceNotification.  

            //  returns
            //  True on success.
            // ***

            //  Ignore failures.

            UnregisterDeviceNotification(deviceNotificationHandle);
        }
        #endregion

        #region Functions
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiCreateDeviceInfoList(ref Guid ClassGuid, int hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterDeviceNotification(IntPtr Handle);
        #endregion

        #region Classes
        // Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.
        // Use this one in the call to RegisterDeviceNotification() and in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:
        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid  dbcc_classguid;
            public short dbcc_name;
        }

        // Use this to read the dbcc_name string and classguid:

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DEV_BROADCAST_DEVICEINTERFACE_1
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
            public byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public char[] dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
        }
        #endregion

        #region Structures
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr Reserved;
        }

        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            public string DevicePath;
        }

        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid ClassGuid;
            public int DevInst;
            public int Reserved;
        }
        #endregion
    }
}
