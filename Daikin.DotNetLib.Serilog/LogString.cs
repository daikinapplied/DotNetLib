using System;

namespace Daikin.DotNetLib.Serilog
{
    public static class LogString
    {
        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
