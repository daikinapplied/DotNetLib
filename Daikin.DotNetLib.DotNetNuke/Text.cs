using System;

namespace Daikin.DotNetLib.DotNetNuke
{
    public static class Text
    {
        public static string CheckPath(string path)
        {
            return (path.Length > 2 && !path.EndsWith(@"\") ? path + @"\" : path);
        }

        public static T ToEnum<T>(string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            //return (T) Enum.Parse(typeof(T), value, true);
            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }
    }
}
