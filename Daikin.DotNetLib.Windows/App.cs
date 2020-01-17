using System;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Daikin.DotNetLib.Windows
{
    public static class App
    {
        #region Properties
        public static string OperatingSystem
        {
            get
            {
                var isServer = IsServerVersion();
                var osInfo = Environment.OSVersion;
                string osName;

                // Determine the OS Name and Version being used
                switch (osInfo.Platform)
                {
                    case PlatformID.Win32Windows:
                        switch (osInfo.Version.Minor)
                        {
                            case 0:
                                osName = "Windows 95";
                                break;
                            case 10:
                                osName = osInfo.Version.Revision.ToString() == "2222A" ? "Windows 98 Second Edition" : "Windows 98";
                                break;
                            case 90:
                                osName = "Windows ME";
                                break;
                            default:
                                osName = "Generic Windows (non-NT)";
                                break;
                        }
                        break;
                    case PlatformID.Win32NT:
                        switch (osInfo.Version.Major)
                        {
                            case 3:
                                osName = "Windows NT 3.51";
                                break;
                            case 4:
                                osName = "Windows NT 4.0";
                                break;
                            case 5:
                                osName = osInfo.Version.Minor == 0 ? "Windows 2000" : "Windows XP";
                                break;
                            case 6:
                                switch (osInfo.Version.Minor)
                                {
                                    case 0:
                                        osName = isServer ? "Windows Server 2008" : "Windows Vista";
                                        break;
                                    case 1:
                                        osName = isServer ? "Windows Server 2008R2" : "Windows 7";
                                        break;
                                    case 2:
                                        osName = isServer ? "Windows Server 2012" : "Windows 8";
                                        break;
                                    case 3:
                                        osName = isServer ? "Windows Server 2012R2" : "Windows 8.1";
                                        break;
                                    case 4:
                                        osName = isServer ? "Windows Server 2016" : "Windows 10";
                                        break;
                                    case 5:
                                        osName = isServer ? "Windows Server 2019" : "Windows 10";
                                        break;
                                    default:
                                        osName = "Post Windows 10 / Server 2019";
                                        break;
                                }
                                break;
                            default:
                                osName = "NT Family";
                                break;
                        }
                        break;
                    case PlatformID.Win32S:
                        osName = "Windows 3.x"; // should never reach
                        break;
                    case PlatformID.WinCE:
                        osName = "Windows CE";
                        break;
                    case PlatformID.Unix:
                        osName = "Unix Variant (details unavailable)";
                        break;
                    case PlatformID.Xbox:
                        osName = "Xbox";
                        break;
                    case PlatformID.MacOSX:
                        osName = "MacOS";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return osName + " (" + Environment.OSVersion + ")";
            }
        }

        public static string NetFrameworkVersion => Environment.Version.ToString();
        public static string NetworkUser => Environment.UserDomainName + "\\" + Environment.UserName;
        public static string MemoryWorking => Environment.WorkingSet.ToString();
        public static string ComputerName => "\\\\" + System.Windows.Forms.SystemInformation.ComputerName;
        public static string CopyrightNotice => ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), typeof(AssemblyCopyrightAttribute))).Copyright;
        public static string Company => ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), typeof(AssemblyCompanyAttribute))).Company;
        public static string ApplicationTitle => ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), typeof(AssemblyTitleAttribute))).Title;

        public static string ApplicationVersion
        {
            get
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null) throw new InvalidOperationException();
                var appVer = entryAssembly.GetName().Version;
                return appVer.Major + "." + appVer.Minor + " (Build " + appVer.Build + ")";
            }
        }

        public static string ProductName => ApplicationTitle;
        public static string Description => ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException(), typeof(AssemblyDescriptionAttribute))).Description;

        //public static bool Is64Bit => System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 8;
        public static bool Is64Bit => Environment.Is64BitOperatingSystem;

        public static string NetPlatformCpu => Is64Bit ? "x64" : "x86";
        #endregion

        #region Functions
        /// <summary>
        /// Launch the Microsoft System Information application
        /// </summary>
        public static void LaunchSysInfo()
        {
            const string regkeysysinfoloc = "SOFTWARE\\Microsoft\\Shared Tools Location";
            const string regvalsysinfoloc = "MSINFO";
            const string regkeysysinfo = "SOFTWARE\\Microsoft\\Shared Tools\\MSINFO";
            const string regvalsysinfo = "PATH";
            const string msinfoexe = "\\msinfo32.exe";

            try
            {
                string regPath;

                if (Registry.HklmGetValue(regkeysysinfo, regvalsysinfo, out var sysInfoPath))
                {
                    regPath = "Reg Path: HKLM\\" + regkeysysinfo + "\\" + regvalsysinfo;
                }
                else if (Registry.HklmGetValue(regkeysysinfoloc, regvalsysinfoloc, out sysInfoPath))
                {
                    sysInfoPath += msinfoexe;
                    regPath = "Reg Path: HKLM\\" + regkeysysinfoloc + "\\" + regvalsysinfoloc;
                }
                else
                {
                    regPath = "MSInfo Registry Entries not found";
                }
                if (sysInfoPath.Length > 0)
                {
                    if (System.IO.File.Exists(sysInfoPath))
                    {
                        System.Diagnostics.Process.Start(sysInfoPath);
                        return;
                    }
                    else
                        regPath = "MSInfo Path not found";
                }
                System.Windows.Forms.MessageBox.Show("System Information is unavailable at this time (" + regPath + ").", "System Information Error", System.Windows.Forms.MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("System Information is unavailable at this time (Error reading registry): " + ex.Message, "System Information Error", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        [DllImport("kernel32.Dll")]
        public static extern short GetVersionEx(ref OSVERSIONINFO o);
        public static string GetServicePack()
        {
            var os = new OSVERSIONINFO
            {
                dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFO))
            };
            GetVersionEx(ref os);
            return os.szCSDVersion == "" ? "No Service Pack Installed" : os.szCSDVersion;
        }

        public static bool IsServerVersion()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (var managementObject in searcher.Get())
                {
                    // ProductType will be one of: 
                    // 1: Workstation 
                    // 2: Domain Controller 
                    // 3: Server 
                    var productType = (uint)managementObject.GetPropertyValue("ProductType");
                    return productType != 1;
                }
            }
            return false;
        }
        #endregion

        #region Structures
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct OSVERSIONINFO
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
        }
        #endregion
    }
}
