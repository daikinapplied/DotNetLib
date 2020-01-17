using System.Reflection;

namespace RecursiveGeek.DotNetLib.Application
{
    public class Version
    {
        #region Functions
        public static string GetAssemblyVersion(Assembly assembly = null)
        {
            if (assembly == null) assembly = Assembly.GetEntryAssembly();
            if (assembly == null) assembly = Assembly.GetCallingAssembly();
            if (assembly == null) assembly = Assembly.GetExecutingAssembly();
            if (assembly == null) return "0.0.0";
            var version = assembly.GetName().Version;
            //var displayVersion = version.Major + "." + version.Minor + "." + version.Build; // Omitting the last segment: version.Revision
            var displayVersion = version.ToString();
            return displayVersion;
        }

        public static string GetAssemblyVersionMessage(Assembly assembly = null)
        {
            return "Version " + GetAssemblyVersion(assembly);
        }
        #endregion
    }
}
