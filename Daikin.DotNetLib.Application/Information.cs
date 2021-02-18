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
            Assembly assembly;
            try
            {
                assembly = Assembly.GetEntryAssembly();
            }
            catch
            {
                assembly = null;
            }

            try
            {
                if (assembly == null) assembly = Assembly.GetCallingAssembly();
            }
            catch
            {
                assembly = null;
            }
            return assembly;
        }

        public static string GetVersion(Assembly assembly = null)
        {
            try
            {
                if (assembly == null) assembly = GetAssembly();
                var version = assembly.GetName().Version;
                //version.Major + "." + version.Minor + "." + version.Build; // Omitting the last segment: version.Revision
                return version.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string GetVersionMessage(Assembly assembly = null)
        {
            return "Version " + GetVersion(assembly);
        }

        public static string GetName(Assembly assembly = null)
        {
            try
            {
                if (assembly == null) assembly = GetAssembly();
                return assembly.GetName().Name;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
