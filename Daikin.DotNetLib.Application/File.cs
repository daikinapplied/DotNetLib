using System.IO;

namespace Daikin.DotNetLib.Application
{
    public static class File
    {
        #region Functions
        public static string FixPathWindows(string path)
        {
            if (path.Length > 0 && !path.EndsWith(@"\"))
            {
                path += @"\";
            }
            return path;
        }

        public static string GetPathWindows(string pathAndFile)
        {
            var path = FixPathWindows(Path.GetDirectoryName(pathAndFile));
            return path;
        }
        #endregion
    }
}
