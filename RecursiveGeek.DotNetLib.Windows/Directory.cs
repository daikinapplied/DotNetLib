using System;

namespace RecursiveGeek.DotNetLib.Windows
{
    public class Directory
    {
        #region Functions
        /// <summary>
        /// Make sure the pathname is properly formed (and terminated properly)
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Adjusted path</returns>
        public static string CheckPath(string path)
        {
            return CheckPath(path, "\\");
        }

        /// <summary>
        /// Make sure the pathname is properly formed (and terminated properly)
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="endChar">Character to terminate</param>
        /// <returns>Adjusted path</returns>
        public static string CheckPath(string path, char endChar)
        {
            return CheckPath(path, endChar.ToString());
        }

        /// <summary>
        /// Make sure the pathname is properly formed (and terminated properly)
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <param name="endStr">String to terminate</param>
        /// <returns>Adjusted path</returns>
        public static string CheckPath(string path, string endStr)
        {
            var strPath = path;

            if (strPath.Length > 2 && !strPath.EndsWith(endStr))
            {
                strPath += endStr;
            }
            return strPath;
        }

        /// <summary>
        /// Expands a path, including those that contain environmental variables
        /// </summary>
        /// <param name="path">Path with environmental variables</param>
        /// <returns>Path expanded</returns>
        /// <remarks>
        /// If the environmental variable is not found, then it is not expanded.
        /// </remarks>
        public static string ExpandPath(string path)
        {
            var searchPath = path.ToLower(); // The search path is used to find and replace.
            var expandedPath = path;         // The expanded path is used to replace only (and maintain case)

            foreach (System.Collections.DictionaryEntry envVar in Environment.GetEnvironmentVariables())
            {
                int indexPos; // Position of an instance of a environmental variable
                do // get all instances of a given environmental variable
                {
                    indexPos = searchPath.IndexOf("%" + envVar.Key.ToString().ToLower() + "%", StringComparison.Ordinal);
                    var indexLen = envVar.Key.ToString().Length + 2; // Length of the instance of an environmental variable
                    if (indexPos < 0) continue;
                    searchPath = searchPath.Substring(0, indexPos) + envVar.Value + searchPath.Substring(indexPos + indexLen);
                    expandedPath = expandedPath.Substring(0, indexPos) + envVar.Value + expandedPath.Substring(indexPos + indexLen);

                } while (indexPos >= 0);
            }
            return expandedPath;
        }
        #endregion
    }
}
