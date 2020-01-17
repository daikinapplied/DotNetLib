using System;
using System.Collections.Generic;

namespace Daikin.DotNetLib.Data
{
    public class DateTimeRange
    {
        #region Enumerators
        private enum DateRangeType
        {
            Month,
            Day,
            Hour,
            Minute
        }
        #endregion

        #region Properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        #endregion

        #region Constructors
        public DateTimeRange()
        {
            StartDate = DateTime.Now;
            EndDate = StartDate;
        }

        public DateTimeRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        #endregion

        #region Methods
        public double TotalDays() { return (EndDate - StartDate).TotalDays; }
        public double TotalHours() { return (EndDate - StartDate).TotalHours; }
        public double TotalMinutes() { return (EndDate - StartDate).TotalMinutes; }
        public double TotalSeconds() { return (EndDate - StartDate).TotalSeconds; }
        public double TotalMilliseconds() { return (EndDate - StartDate).TotalMilliseconds; }
        public int Months() { return (EndDate.Year - StartDate.Year) * 12 + (EndDate.Month - StartDate.Month) - (EndDate.Day < StartDate.Day ? 1 : 0); }
        public int Days() { return (EndDate - StartDate).Days; }
        public int Hours() { return (EndDate - StartDate).Hours; }
        public int Minutes() { return (EndDate - StartDate).Minutes; }
        public int Seconds() { return (EndDate - StartDate).Seconds; }
        public int Milliseconds() { return (EndDate - StartDate).Milliseconds; }
        public long Microseconds() { return (EndDate.Ticks - StartDate.Ticks) / Date.TicksPerMicrosecond; }
        public long Nanoseconds() { return (EndDate.Ticks - StartDate.Ticks) / Date.NanosecondsPerTick; }
        #endregion

        #region Properties
        public List<DateTime> GetMonthly()
        {
            return GetRange(DateRangeType.Month);
        }

        public List<DateTime> GetDaily()
        {
            return GetRange(DateRangeType.Day);
        }

        public List<DateTime> GetHourly()
        {
            return GetRange(DateRangeType.Hour);
        }

        public List<DateTime> GetMinutes()
        {
            return GetRange(DateRangeType.Minute);
        }

        private List<DateTime> GetRange(DateRangeType dateRange)
        {
            var indexDate = StartDate;
            var dates = new List<DateTime>();
            while (indexDate <= EndDate)
            {
                dates.Add(indexDate);
                switch (dateRange)
                {
                    case DateRangeType.Month:
                        indexDate.AddMonths(1);
                        break;
                    case DateRangeType.Day:
                        indexDate.AddDays(1);
                        break;
                    case DateRangeType.Hour:
                        indexDate.AddHours(1);
                        break;
                    case DateRangeType.Minute:
                        indexDate.AddMinutes(1);
                        break;
                }
            }
            return dates;
        }
        #endregion
    }
}
