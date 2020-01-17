using System;
using System.Collections.Generic;

namespace Daikin.DotNetLib.Data
{
    public static class Date
    {
        #region Constants
        public const int TicksPerMicrosecond = 10;
        public const int NanosecondsPerTick = 100;
        #endregion

        #region Functions
        public static int GetMicroseconds(DateTime dateTime)
        {
            return (int)Math.Floor(dateTime.Ticks % TimeSpan.TicksPerMillisecond / (double)TicksPerMicrosecond);
        }

        public static DateTime AddMicroseconds(DateTime dateTime, int microseconds)
        {
            return dateTime.AddTicks(microseconds * TicksPerMicrosecond);
        }
        public static DateTime AddNanoseconds(DateTime dateTime, int nanoseconds)
        {
            return dateTime.AddTicks((int)Math.Floor(nanoseconds / (double)NanosecondsPerTick));
        }

        public static int GetNanoseconds(DateTime dateTime)
        {
            return (int)(dateTime.Ticks % TimeSpan.TicksPerMillisecond % TicksPerMicrosecond) * NanosecondsPerTick;
        }

        public static DateTime BuildDateTime(int year, int month, int day, int hour, int minute, int seconds, int milliseconds, int microseconds, int nanoseconds)
        {
            var dateTime = new DateTime(year, month, day, hour, minute, seconds, milliseconds);
            AddMicroseconds(dateTime, microseconds);
            AddNanoseconds(dateTime, nanoseconds);
            return dateTime;
        }

        public static DateTime GetFirstDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            return GetFirstDayOfMonth(dateTime).AddMonths(1).AddTicks(-1);
        }
        public static DateTime GetFirstTimeOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }

        public static DateTime GetLastTimeOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999).AddTicks(9999);
        }

        public static int GetDaysInMonth(DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }

        public static int DaysInYear(int year)
        {
            var days = 0;
            for (var month = 1; month <= 12; month++)
            {
                days += DateTime.DaysInMonth(year, month);
            }
            return days;
        }

        public static int DaysInYear(DateTime dateTime)
        {
            return DaysInYear(dateTime.Year);
        }

        public static List<DateTimeRange> GetMonthlyRange(DateTime startDate, DateTime endDate, bool wholeMonths = true)
        {
            var dateTimeRange = new List<DateTimeRange>();
            if (endDate < startDate) // Swap?
            {
                var tempDate = startDate;
                startDate = endDate;
                endDate = tempDate;
            }
            var startRange = wholeMonths ? GetFirstDayOfMonth(startDate) : startDate; // Start at the beginning
            var endingDate = wholeMonths ? GetLastDayOfMonth(endDate) : GetLastTimeOfDay(endDate); // Final date
            while (startRange < endingDate)
            {
                var endRange = GetLastDayOfMonth(startRange);
                dateTimeRange.Add(new DateTimeRange(startRange, !wholeMonths && startRange.Year == endingDate.Year && startRange.Month == endingDate.Month ? endingDate : endRange));
                startRange = GetFirstDayOfMonth(endRange.AddDays(1));
            }
            return dateTimeRange;
        }
        #endregion
    }
}
