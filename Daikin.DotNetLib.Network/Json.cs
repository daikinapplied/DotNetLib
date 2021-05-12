using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Daikin.DotNetLib.Network
{
    public static class Json
    {
        #region Functions
        /// <summary>
        /// Convert an object to a JSON string
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <param name="settings">Json settings options</param>
        /// <returns>JSON String</returns>
        public static string ObjectToString(object obj, JsonSerializerSettings settings = null)
        {
            if (settings == null) settings = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>
        /// Convert an object to a JSON string
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <param name="formatting">Json Formatting Options</param>
        /// <param name="settings">Json settings options</param>
        /// <returns>JSON String</returns>
        public static string ObjectToString(object obj, Formatting formatting, JsonSerializerSettings settings = null)
        {
            if (settings == null) settings = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        /// <summary>
        /// Convert a List of Strings to a JSON string
        /// </summary>
        /// <param name="list">Generic Generic List of string</param>
        /// <param name="settings">Json settings options</param>
        /// <returns>JSON String</returns>
        public static string ListToString(List<string> list, JsonSerializerSettings settings = null)
        {
            if (settings == null) settings = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(list, settings);
        }

        /// <summary>
        /// Convert a JSON string into an object
        /// </summary>
        /// <typeparam name="T">Object Type to convert from JSON</typeparam>
        /// <param name="jsonString">JSON string</param>
        /// <param name="settings">Json settings options</param>
        /// <returns>Object</returns>
        public static T ObjectFromString<T>(string jsonString, JsonSerializerSettings settings = null)
        {
            if (settings == null) settings = new JsonSerializerSettings();
            return JsonConvert.DeserializeObject<T>(jsonString, settings);
        }

        /// <summary>
        /// Save an class object to a JSON file
        /// </summary>
        /// <param name="obj">Object to convert and save</param>
        /// <param name="fileAndPath">Path and filename of file</param>
        /// <param name="formatting">Type of formatting</param>
        /// <param name="settings">Json settings options</param>
        public static void ObjectToFile(object obj, string fileAndPath, Formatting formatting = Formatting.Indented, JsonSerializerSettings settings = null)
        {
            File.WriteAllText(fileAndPath, ObjectToString(obj, formatting, settings));
        }

        /// <summary>
        /// Read a JSON file to a class object
        /// </summary>
        /// <typeparam name="T">Type of object to use in converting</typeparam>
        /// <param name="fileAndPath">Path and filename of file</param>
        /// <param name="settings">Json settings options</param>
        /// <returns>Object based on the JSON file</returns>
        public static T ObjectFromFile<T>(string fileAndPath, JsonSerializerSettings settings = null)
        {
            var jsonString = File.ReadAllText(fileAndPath);
            return ObjectFromString<T>(jsonString, settings);
        }

        /// <summary>
        /// Convert a string to a JObject
        /// </summary>
        /// <param name="jsonString">JSON String to convert</param>
        /// <param name="settings">Json settings options</param>
        /// <returns>JObject</returns>
        public static JObject Parse(string jsonString, JsonLoadSettings settings = null)
        {
            if (settings == null) settings = new JsonLoadSettings();
            return JObject.Parse(jsonString, settings);
        }
        #endregion
    }
}
