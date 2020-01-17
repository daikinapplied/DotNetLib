using System;
using System.Runtime.InteropServices;

// Remove inconsistent naming since simulating C++ coding standards in C#
// ReSharper disable InconsistentNaming

namespace RecursiveGeek.DotNetLib.Serial.Com
{
    // Interface Hooks to the C++ WIN32SerialLib library

    public class CommsSerial : IDisposable
    {
        //Result
        public enum SerialErrors
        {
            SERIAL_OK = 0,
            ERR_SERIAL_OPEN,
            ERR_SERIAL_GET_CONFIG,
            ERR_SERIAL_SET_CONFIG,
            ERR_SERIAL_SET_TIMEOUT,
            ERR_SERIAL_READ,
            ERR_SERIAL_WRITE,
            ERR_SERIAL_IO_PENDING,
            ERR_SERIAL_IO_MODE,
            ERR_SERIAL_INITIALIZE
        };

        //Hook error codes
        public enum SerialHookErrors
        {
            HOOK_OK = 0,
            ERR_HOOK_ARGUMENT,
            ERR_HOOK_CALLBACK_ALREADY_SET,
            ERR_HOOK_TYPE_NO_HANDLER,
            ERR_HOOK_RESOURCE_IN_USE,
            ERR_HOOK_INSTALL,
            ERR_HOOK_UNINSTALL,
            ERR_HOOK_ALREADY_INSTALLED,
            ERR_HOOK_NOT_INSTALLED,
        };

        //Read/Write operation mode
        public enum OperationMode
        {
            SERIAL_MODE_SYNC = 0,
            SERIAL_MODE_ASYNC
        };

        //Data size
        public enum DataSize
        {
            SEVEN_BITS = 7,
            EIGHT_BITS = 8
        };

        //Parity bit   
        public enum ParityBit
        {
            NOPARITY = 0,
            ODDPARITY = 1,
            EVENPARITY = 2,
            MARKPARITY = 3,
            SPACEPARITY = 4
        };

        //Stop bit       
        public enum StopBit
        {
            ONESTOPBIT = 0,
            ONE5STOPBITS = 1,
            TWOSTOPBITS = 2
        };

        //
        // Baud rates at which the communication device operates
        //
        public enum BaudRate
        {
            CBR_110 = 110,
            CBR_300 = 300,
            CBR_600 = 600,
            CBR_1200 = 1200,
            CBR_2400 = 2400,
            CBR_4800 = 4800,
            CBR_9600 = 9600,
            CBR_14400 = 14400,
            CBR_19200 = 19200,
            CBR_38400 = 38400,
            CBR_56000 = 56000,
            CBR_57600 = 57600,
            CBR_115200 = 115200,
            CBR_128000 = 128000,
            CBR_256000 = 256000
        };

        //
        // Variables and delegates
        //
        private bool isDisposed;
        private bool hookInstalled;
        private ushort serialPort;
        private delegate void serialHookHandlerT(/*[MarshalAs(UnmanagedType.LPArray)]*/ UIntPtr data, uint length);
        public delegate void serialCallbackPublisherT(object obj, ComEventArgs EventArgs);
        public event serialCallbackPublisherT SerialTransmitEvent;
        public event serialCallbackPublisherT SerialReceptionEvent;

        #region Constructor and Finalizer

        public CommsSerial(bool setReadHookBack = false, bool setWriteHookBack = false)
        {
            SerialHookErrors hookError;

            //
            isDisposed = false;

            //
            hookInstalled = false;

            //Default port
            serialPort = 0;

            if (setReadHookBack)
            {
                //Anonymous internal Rx callback
                serialHookHandlerT serialRxHookCallback = delegate (UIntPtr data, uint length)
                {
                    var eventArgument = new ComEventArgs();

                    byte[] _data = new byte[length];

                    GetSerialData(_data, length);

                    //Prepare event message
                    eventArgument.Length = length;
                    eventArgument.Message = _data;

                    OnSerialReceptionEvent(this, eventArgument);
                };

                //Set Rx hook callback
                hookError = (SerialHookErrors)SetUserHookCallback(serialRxHookCallback, HookTypes.HookRx);
                if (SerialHookErrors.HOOK_OK != hookError)
                {
                    GenerateSerialHookException(hookError);
                }
            }

            if (setWriteHookBack)
            {
                //Anonymous internal Tx callback
                serialHookHandlerT serialTxHookCallback = delegate (UIntPtr data, uint length)
                {
                    //Prepare event message
                    var eventArgument = new ComEventArgs { Length = length, Message = null };
                    OnSerialTransmitEvent(this, eventArgument);
                };

                //Set Tx hook callback
                hookError = (SerialHookErrors)SetUserHookCallback(serialTxHookCallback, HookTypes.HookTx);
                if (SerialHookErrors.HOOK_OK != hookError)
                {
                    GenerateSerialHookException(hookError);
                }
            }
        }

        ~CommsSerial()
        {
            //
            // Finalizer is called.
            //
            Dispose(false);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                Dispose(true);
            }
        }

        public void Dispose(bool dispose)
        {
            if (dispose)
            {
                //
                // Dispose managed code
                //
                GC.SuppressFinalize(this);
            }

            //
            // Cleanup unmanaged code
            //

            //Don't leave the hook uninstalled
            if (hookInstalled)
            {
                UninstallHooks();
            }

            //Destroy events
            SerialDeInitialize();

            //Close COM resource
            SerialClose();

            isDisposed = true;
        }

        #endregion //Constructor and Finalizer


        #region Methods

        public static ushort ParamConvertCom(string comPort)
        {
            ushort retVal = 1;

            switch (comPort.ToUpper())
            {
                case "COM1": retVal = 1; break;
                case "COM2": retVal = 2; break;
                case "COM3": retVal = 3; break;
                case "COM4": retVal = 4; break;
                case "COM5": retVal = 5; break;
                case "COM6": retVal = 6; break;
                case "COM7": retVal = 7; break;
                case "COM8": retVal = 8; break;
                case "COM9": retVal = 9; break;
            }

            return retVal;
        }


        public static DataSize ParamConvertDataSize(string dataSize)
        {
            return dataSize == "7" ? DataSize.SEVEN_BITS : DataSize.EIGHT_BITS;
        }


        public static ParityBit ParamConvertParityBit(string parityBit)
        {
            var retVal = ParityBit.NOPARITY;

            switch (parityBit.ToUpper())
            {
                case "N": retVal = ParityBit.NOPARITY; break;
                case "NONE": retVal = ParityBit.NOPARITY; break;
                case "E": retVal = ParityBit.EVENPARITY; break;
                case "EVEN": retVal = ParityBit.EVENPARITY; break;
                case "O": retVal = ParityBit.ODDPARITY; break;
                case "ODD": retVal = ParityBit.ODDPARITY; break;
                case " ": retVal = ParityBit.SPACEPARITY; break;
                case "SPACE": retVal = ParityBit.SPACEPARITY; break;
                case "M": retVal = ParityBit.MARKPARITY; break;
                case "MARK": retVal = ParityBit.MARKPARITY; break;
            }

            return retVal;
        }


        public static StopBit ParamConvertStopBit(string stopBit)
        {
            return (stopBit == "2" ? StopBit.TWOSTOPBITS : StopBit.ONESTOPBIT);
        }


        public static BaudRate ParamConvertBaud(string baud)
        {
            var retVal = BaudRate.CBR_19200;

            switch (baud)
            {
                case "110": retVal = BaudRate.CBR_110; break;
                case "300": retVal = BaudRate.CBR_300; break;
                case "600": retVal = BaudRate.CBR_600; break;
                case "1200": retVal = BaudRate.CBR_1200; break;
                case "2400": retVal = BaudRate.CBR_2400; break;
                case "4800": retVal = BaudRate.CBR_4800; break;
                case "9600": retVal = BaudRate.CBR_9600; break;
                case "14400": retVal = BaudRate.CBR_14400; break;
                case "14,400": retVal = BaudRate.CBR_14400; break;
                case "19200": retVal = BaudRate.CBR_19200; break;
                case "19,200": retVal = BaudRate.CBR_19200; break;
                case "38400": retVal = BaudRate.CBR_38400; break;
                case "38,400": retVal = BaudRate.CBR_38400; break;
                case "56000": retVal = BaudRate.CBR_56000; break;
                case "56,000": retVal = BaudRate.CBR_56000; break;
                case "57600": retVal = BaudRate.CBR_57600; break;
                case "57,600": retVal = BaudRate.CBR_57600; break;
                case "115200": retVal = BaudRate.CBR_115200; break;
                case "115,200": retVal = BaudRate.CBR_115200; break;
                case "128000": retVal = BaudRate.CBR_128000; break;
                case "128,000": retVal = BaudRate.CBR_128000; break;
                case "256000": retVal = BaudRate.CBR_256000; break;
                case "256,000": retVal = BaudRate.CBR_256000; break;
            }

            return retVal;
        }


        private void OnSerialReceptionEvent(object obj, ComEventArgs args)
        {
            if (SerialReceptionEvent != null)
            {
                SerialReceptionEvent(obj, args);
            }
        }

        private void OnSerialTransmitEvent(object obj, ComEventArgs args)
        {
            if (SerialTransmitEvent != null)
            {
                SerialTransmitEvent(obj, args);
            }
        }

        public void InstallHooks()
        {
            var hookError = (SerialHookErrors)InstallSerialHook();
            if (SerialHookErrors.HOOK_OK != hookError)
            {
                GenerateSerialHookException(hookError);
            }

            hookInstalled = true;
        }

        public void UninstallHooks()
        {
            var hookError = (SerialHookErrors)UnInstallSerialHook();
            if (SerialHookErrors.HOOK_OK != hookError)
            {
                GenerateSerialHookException(hookError);
            }

            hookInstalled = false;
        }

        public void Open(ushort comPort, DataSize dataSize, ParityBit parityBit, StopBit stopBit, BaudRate baudRate)
        {
            serialPort = comPort;

            //Initialize events    
            var errCode = (SerialErrors)SerialInitialize(OperationMode.SERIAL_MODE_ASYNC);
            if (SerialErrors.SERIAL_OK == errCode)
            {
                errCode = (SerialErrors)SerialOpen((sbyte[])(Array)Conversion.ToByteArray("COM" + comPort),
                                                   (int)dataSize,
                                                   (short)parityBit,
                                                   (short)stopBit,
                                                   (short)dataSize);

                if (SerialErrors.SERIAL_OK == errCode)
                    return;
            }

            GenerateSerialException(errCode);
        }

        public void Read(out byte[] lpcDest, uint szDest, out uint pszRcv)
        {
            var errCode = (SerialErrors)SerialRead(out lpcDest, szDest, out pszRcv);
            if (SerialErrors.SERIAL_OK != errCode)
            {
                GenerateSerialException(errCode);
            }
        }

        public void Read(out byte[] lpcDest, uint szDest)
        {
            var errCode = (SerialErrors)SerialAsyncRead(out lpcDest, szDest);
            if (SerialErrors.SERIAL_OK != errCode)
            {
                GenerateSerialException(errCode);
            }
        }

        public void Write(byte[] lpcData, uint szData, out uint szWritten)
        {
            var errCode = (SerialErrors)SerialWrite(lpcData, szData, out szWritten);
            if (SerialErrors.SERIAL_OK != errCode && SerialErrors.ERR_SERIAL_IO_PENDING != errCode)
            {
                GenerateSerialException(errCode);
            }
        }

        public void Write(byte[] lpcData, uint szData)
        {
            var errCode = (SerialErrors)SerialAsyncWrite(lpcData, szData);
            if (SerialErrors.SERIAL_OK != errCode && SerialErrors.ERR_SERIAL_IO_PENDING != errCode)
            {
                GenerateSerialException(errCode);
            }
        }

        public void GetReadResult(out uint pszRead)
        {
            var errCode = (SerialErrors)SerialGetReadResult(out pszRead);
            if (SerialErrors.SERIAL_OK != errCode)
            {
                GenerateSerialException(errCode);
            }
        }

        public void GetWriteResult(out uint pszWritten)
        {
            var errCode = (SerialErrors)SerialGetWriteResult(out pszWritten);
            if (SerialErrors.SERIAL_OK != errCode)
            {
                GenerateSerialException(errCode);
            }
        }

        public void Close()
        {
            if (!isDisposed)
            {
                Dispose(true);
            }
        }

        private void GenerateSerialException(SerialErrors errorCode)
        {
            string exDescription;

            if (SerialErrors.SERIAL_OK == errorCode)
                return;

            switch (errorCode)
            {
                case SerialErrors.ERR_SERIAL_INITIALIZE:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_INITIALIZE;
                    exDescription += "\nDescription: ";
                    exDescription += "Fatal error occured while initializing the serial component.";
                    break;

                case SerialErrors.ERR_SERIAL_OPEN:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_OPEN;
                    exDescription += "\nDescription: ";
                    exDescription += "Cannot open serial COM" + serialPort + ". ";
                    exDescription += "Resource is either in use or not available.";
                    break;

                case SerialErrors.ERR_SERIAL_GET_CONFIG:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_GET_CONFIG;
                    exDescription += "\nDescription: ";
                    exDescription += "Failed retrieving default serial settings.";
                    break;

                case SerialErrors.ERR_SERIAL_SET_TIMEOUT:
                case SerialErrors.ERR_SERIAL_SET_CONFIG:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_SET_CONFIG;
                    exDescription += "/" + SerialErrors.ERR_SERIAL_SET_TIMEOUT.ToString();
                    exDescription += "\nDescription: ";
                    exDescription += "Failed to configure the serial component.";
                    break;

                case SerialErrors.ERR_SERIAL_WRITE:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_WRITE;
                    exDescription += "\nDescription: ";
                    exDescription += "An error occured while attempting to write the data.";
                    break;

                case SerialErrors.ERR_SERIAL_READ:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_READ;
                    exDescription += "\nDescription: ";
                    exDescription += "An error occured while attempting to read data.";
                    break;

                case SerialErrors.ERR_SERIAL_IO_PENDING:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_IO_PENDING;
                    exDescription += "\nDescription: ";
                    exDescription += "I/O process has not finished yet.";
                    break;

                case SerialErrors.ERR_SERIAL_IO_MODE:
                    exDescription = "\nException  : " + SerialErrors.ERR_SERIAL_IO_MODE;
                    exDescription += "\nDescription: ";
                    exDescription += "Invalid I/O mode for this operation.";
                    break;
                default:
                    exDescription = "\nException  : UNKNOWN (" + errorCode.ToString() + ").";
                    exDescription += "\nDescription: ";
                    exDescription += "An unexpected exception occured.";
                    break;
            }

            throw new SerialManagedException(exDescription);
        }

        private void GenerateSerialHookException(SerialHookErrors hookError)
        {
            string exDescription;

            if (SerialHookErrors.HOOK_OK == hookError)
                return;

            switch (hookError)
            {
                case SerialHookErrors.ERR_HOOK_ARGUMENT:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_ARGUMENT;
                    exDescription += "\nDescription: ";
                    exDescription += "Invalid parameter is passed to serial hook component. ";
                    exDescription += "Ensure that an appropriate delegate is passed as argument.";

                    throw new ArgumentException(exDescription);

                case SerialHookErrors.ERR_HOOK_CALLBACK_ALREADY_SET:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_CALLBACK_ALREADY_SET;
                    exDescription += "\nDescription: ";
                    exDescription += "A callback function has previously been set. ";
                    exDescription += "Callbacks should be set once only.";
                    break;

                case SerialHookErrors.ERR_HOOK_ALREADY_INSTALLED:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_ALREADY_INSTALLED;
                    exDescription += "\nDescription: ";
                    exDescription += "Hooks have previously been installed. ";
                    exDescription += "Uninstall the active hooks before calling this service.";
                    break;

                case SerialHookErrors.ERR_HOOK_INSTALL:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_INSTALL;
                    exDescription += "\nDescription: ";
                    exDescription += "An error occured while attempting to install serial hooks.";
                    break;

                case SerialHookErrors.ERR_HOOK_NOT_INSTALLED:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_INSTALL;
                    exDescription += "\nDescription: ";
                    exDescription += "No serial hook is currently installed. ";
                    exDescription += "Call the serial hook Install service first.";
                    break;

                case SerialHookErrors.ERR_HOOK_RESOURCE_IN_USE:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_RESOURCE_IN_USE;
                    exDescription += "\nDescription: ";
                    exDescription += "One or more resource required by the serial hook component ";
                    exDescription += "is currently not available.";
                    break;

                case SerialHookErrors.ERR_HOOK_TYPE_NO_HANDLER:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_TYPE_NO_HANDLER;
                    exDescription += "\nDescription: ";
                    exDescription += "The serial hook type requested has no handler implemented.";

                    throw new ArgumentException(exDescription);

                case SerialHookErrors.ERR_HOOK_UNINSTALL:
                    exDescription = "\nException  : " + SerialHookErrors.ERR_HOOK_UNINSTALL;
                    exDescription += "\nDescription: ";
                    exDescription += "Failed to uninstall serial hooks.";
                    break;

                default:
                    exDescription = "\nException  : UNKNOWN (" + hookError + ").";
                    exDescription += "\nDescription: ";
                    exDescription += "An unexpected exception occured.";
                    break;
            }

            throw new SerialManagedException(exDescription);
        }

        #endregion //Methods


        #region WIN32SerialLib.dll imports

        /*
        * WIN32SERIALLIB_API INT SetUserHookCallback(UserHookProc userProc, SerialHookTypes hookType);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", EntryPoint = "SetUserHookCallback", SetLastError = false,
                 CharSet = CharSet.Auto, ExactSpelling = true,
                 CallingConvention = CallingConvention.Cdecl)]
        private static extern SerialErrors SetUserHookCallback(serialHookHandlerT hookCallback, HookTypes hookType);

        /*
        * WIN32SERIALLIB_API   BYTE     SerialGetIOMode(void);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        // ReSharper disable once UnusedMember.Local
        private static extern OperationMode SerialGetIOMode();

        /*
        * WIN32SERIALLIB_API INT InstallSerialHook(void);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short InstallSerialHook();

        /*
        * WIN32SERIALLIB_API INT UnInstallSerialHook(void);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short UnInstallSerialHook();

        /*
        * WIN32SERIALLIB_API void GetSerialData(BYTE* data, UINT length);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetSerialData(/*[MarshalAs(UnmanagedType.LPArray)]*/ byte[] data, uint length);

        /*
        * WIN32SERIALLIB_API	INT	   SerialInitialize(BYTE mode);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialInitialize(OperationMode opMode);

        /*
        * WIN32SERIALLIB_API	void	   SerialDeInitialize(void);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SerialDeInitialize();

        /*
        * WIN32SERIALLIB_API	INT		SerialOpen(LPCSTR lpcPort,LONG baudrate,INT parity,INT stopbit,INT datasize);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialOpen(sbyte[] lpcPort, int baudrate, short parity, short stopbit, short datasize);

        /*
        * WIN32SERIALLIB_API	INT		SerialRead(BYTE* lpbDest,DWORD szDest, DWORD* pszRcv);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialRead(out byte[] lpbDest, uint szDest, out uint pszRcv);

        /*
        * WIN32SERIALLIB_API   INT      SerialAsyncRead(BYTE* lpbDest,DWORD szDest);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialAsyncRead(out byte[] lpbDest, uint szDest);

        /*
        * WIN32SERIALLIB_API	INT		SerialGetReadResult(DWORD* pszRead);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialGetReadResult(out uint pszRead);

        /*
        * WIN32SERIALLIB_API	INT		SerialWrite(BYTE* lpcData, DWORD szData, DWORD* szWritten);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialWrite(byte[] lpcData, uint szData, out uint szWritten);

        /*
        * WIN32SERIALLIB_API   INT      SerialAsyncWrite(BYTE* lpbData, DWORD szData);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialAsyncWrite(byte[] lpbData, uint szData);

        /*
        *  WIN32SERIALLIB_API	INT		SerialGetWriteResult(DWORD* pszWritten);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern short SerialGetWriteResult(out uint pszWrite);

        /*        
        * WIN32SERIALLIB_API	void	   SerialClose(void);
        */
        [DllImport("Aeriden.Lib.Serial.WIN32SerialLib.v40.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SerialClose();

        #endregion
    }
}
