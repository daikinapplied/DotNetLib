using System;

namespace RecursiveGeek.DotNetLib.DotNetNuke
{
    public static class Date
    {
        public static DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId = "Central Standard Time")
        {
            try
            {
                var timeUtc = dateTime.ToUniversalTime();
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                var convertedTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZone);
                return convertedTime;
            }
            catch (TimeZoneNotFoundException)
            {
                throw new Exception($"The registry does not define the {timeZoneId} zone.");
            }
            catch (InvalidTimeZoneException)
            {
                throw new Exception($"Registry data on the {timeZoneId} zone has been corrupted.");
            }
        }

        public static string MinutesToText(int minutes)
        {
            string result;
            var hrs = minutes / 60;
            var min = minutes - hrs * 60;
            if (hrs == 1 && min == 0)
            {
                result = hrs + " hour";
            }
            else if (hrs > 1 && min == 0)
            {
                result = hrs + " hours";
            }
            else if (hrs == 0 && min == 1)
            {
                result = min + " minute";
            }
            else if (hrs == 0 && min > 1)
            {
                result = min + " minutes";
            }
            else if (hrs == 0 && min == 0)
            {
                result = string.Empty;
            }
            else
            {
                result = hrs + "h " + min + "m";
            }
            return result;
        }
    }
}
