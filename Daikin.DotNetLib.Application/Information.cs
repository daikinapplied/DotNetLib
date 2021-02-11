using System;
using System.Reflection;

namespace Daikin.DotNetLib.Application
{
    public class Information
    {
        #region Functions

        /// <summary>
        /// Get the Assembly based on a class
        /// </summary>
        /// <param name="type">Example: typeof(Classname)</param>
        /// <returns>Assembly</returns>
        public static Assembly GetAssembly(Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static Assembly GetAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) assembly = Assembly.GetCallingAssembly();
            return assembly;
        }

        public static string GetVersion(Assembly assembly = null)
        {
            if (assembly == null) assembly = GetAssembly();
            var version = assembly.GetName().Version;
            //version.Major + "." + version.Minor + "." + version.Build; // Omitting the last segment: version.Revision
            return version.ToString();
        }

        public static string GetAssemblyVersionMessage(Assembly assembly = null)
        {
            return "Version " + GetVersion(assembly);
        }

        public static string GetName(Assembly assembly = null)
        {
            if (assembly == null) assembly = GetAssembly();
            return assembly.GetName().Name;
        }
        #endregion
    }
}
