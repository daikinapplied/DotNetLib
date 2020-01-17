using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RecursiveGeek.DotNetLib.Network
{
    public static class Json
    {
        #region Functions
        /// <summary>
        /// Convert an object to a JSON string
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>JSON String</returns>
        public static string ObjectToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Convert an object to a JSON string
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <param name="formatting">Json Formatting Options</param>
        /// <returns>JSON String</returns>
        public static string ObjectToString(object obj, Formatting formatting)
        {
            return JsonConvert.SerializeObject(obj, formatting);
        }

        /// <summary>
        /// Convert a List of Strings to a JSON string
        /// </summary>
        /// <param name="list">Generic Generic List of string</param>
        /// <returns>JSON String</returns>
        public static string ListToString(List<string> list)
        {
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// Convert a JSON string into an object
        /// </summary>
        /// <typeparam name="T">Object Type to convert from JSON</typeparam>
        /// <param name="jsonString">JSON string</param>
        /// <returns>Object</returns>
        public static T ObjectFromString<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Save an class object to a JSON file
        /// </summary>
        /// <param name="obj">Object to convert and save</param>
        /// <param name="fileAndPath">Path and filename of file</param>
        /// <param name="formatting">Type of formatting</param>
        public static void ObjectToFile(object obj, string fileAndPath, Formatting formatting = Formatting.Indented)
        {
            File.WriteAllText(fileAndPath, ObjectToString(obj, formatting));
        }

        /// <summary>
        /// Read a JSON file to a class object
        /// </summary>
        /// <typeparam name="T">Type of object to use in converting</typeparam>
        /// <param name="fileAndPath">Path and filename of file</param>
        /// <returns>Object based on the JSON file</returns>
        public static T ObjectFromFile<T>(string fileAndPath)
        {
            var jsonString = File.ReadAllText(fileAndPath);
            return ObjectFromString<T>(jsonString);
        }

        /// <summary>
        /// Convert a string to a JObject
        /// </summary>
        /// <param name="jsonString">JSON String to convert</param>
        /// <returns>JObject</returns>
        public static JObject Parse(string jsonString)
        {
            return JObject.Parse(jsonString);
        }
        #endregion
    }
}
