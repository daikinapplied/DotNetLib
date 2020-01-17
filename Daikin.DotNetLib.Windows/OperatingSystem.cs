using Microsoft.VisualBasic.Devices;

namespace Daikin.DotNetLib.Windows
{
    public static class OperatingSystem
    {
        #region Functions

        public static string VersionName
        {
            get
            {
                var versionId = new ComputerInfo().OSVersion; // 6.1.7601.65536
                var versionName = new ComputerInfo().OSFullName; // Microsoft Windows 7 Ultimate
                var versionPlatform = new ComputerInfo().OSPlatform; // WinNT
                return versionName + " (" + versionPlatform + " " + versionId + ")";
            }
        }
        #endregion
    }
}
