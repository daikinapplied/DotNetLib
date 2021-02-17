using System;

namespace Daikin.DotNetLib.Serilog
{
    public static class String
    {
        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
        }
    }
}
