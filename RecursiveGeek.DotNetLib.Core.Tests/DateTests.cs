using System;
using System.Collections.Generic;
using RecursiveGeek.DotNetLib.Data;
using Xunit;

namespace RecursiveGeek.DotNetLib.Core.Tests
{
    public class DateTests
    {
        [Fact]
        public void FirstDayOfMonth()
        {
            var firstDate = Date.GetFirstDayOfMonth(new DateTime(2016, 12, 12));
            var expectDate = new DateTime(2016, 12, 1, 0, 0, 0);
            Assert.Equal(firstDate, expectDate);
        }

        [Fact]
        public void LastDayOfMonth()
        {
            var lastDate = Date.GetLastDayOfMonth(new DateTime(2016, 2, 16));
            var expectDate = new DateTime(2016, 2, 29, 23, 59, 59, 999);
            expectDate = Date.AddMicroseconds(expectDate, 999);
            expectDate = Date.AddNanoseconds(expectDate, 999);
            Assert.Equal(lastDate, expectDate);
        }

        [Fact]
        public void DateRangeWholeMonths()
        {
            var dateTimeRange = Date.GetMonthlyRange(new DateTime(2016, 2, 16), new DateTime(2017, 2, 16));
            var expectRange = new List<DateTimeRange>
            {
                new DateTimeRange(new DateTime(2016, 2, 1, 0, 0, 0), new DateTime(2016, 2, 29, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 3, 1, 0, 0, 0), new DateTime(2016, 3, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 4, 1, 0, 0, 0), new DateTime(2016, 4, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 5, 1, 0, 0, 0), new DateTime(2016, 5, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 6, 1, 0, 0, 0), new DateTime(2016, 6, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 7, 1, 0, 0, 0), new DateTime(2016, 7, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 8, 1, 0, 0, 0), new DateTime(2016, 8, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 9, 1, 0, 0, 0), new DateTime(2016, 9, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 10, 1, 0, 0, 0), new DateTime(2016, 10, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 11, 1, 0, 0, 0), new DateTime(2016, 11, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 12, 1, 0, 0, 0), new DateTime(2016, 12, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2017, 1, 1, 0, 0, 0), new DateTime(2017, 1, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2017, 2, 1, 0, 0, 0), new DateTime(2017, 2, 28, 23, 59, 59, 999).AddTicks(9999))
            };
            if (dateTimeRange.Count != expectRange.Count) { throw new Exception("Number of items mismatched"); }
            for (var index = 0; index < dateTimeRange.Count; index++)
            {
                if (dateTimeRange[index].StartDate != expectRange[index].StartDate) { throw new Exception("Start Date Mismatch...  Got: " + dateTimeRange[index].StartDate + " Expected: " + expectRange[index].StartDate); }
                if (dateTimeRange[index].EndDate != expectRange[index].EndDate) { throw new Exception("End Date Mismatch...  Got: " + dateTimeRange[index].EndDate + " Expected: " + expectRange[index].EndDate); }
            }
        }

        [Fact]
        public void DateRangePartialMonths()
        {
            var dateTimeRange = Date.GetMonthlyRange(new DateTime(2016, 2, 16), new DateTime(2017, 2, 16), false);
            var expectRange = new List<DateTimeRange>
            {
                new DateTimeRange(new DateTime(2016, 2, 16, 0, 0, 0), new DateTime(2016, 2, 29, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 3, 1, 0, 0, 0), new DateTime(2016, 3, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 4, 1, 0, 0, 0), new DateTime(2016, 4, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 5, 1, 0, 0, 0), new DateTime(2016, 5, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 6, 1, 0, 0, 0), new DateTime(2016, 6, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 7, 1, 0, 0, 0), new DateTime(2016, 7, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 8, 1, 0, 0, 0), new DateTime(2016, 8, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 9, 1, 0, 0, 0), new DateTime(2016, 9, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 10, 1, 0, 0, 0), new DateTime(2016, 10, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 11, 1, 0, 0, 0), new DateTime(2016, 11, 30, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2016, 12, 1, 0, 0, 0), new DateTime(2016, 12, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2017, 1, 1, 0, 0, 0), new DateTime(2017, 1, 31, 23, 59, 59, 999).AddTicks(9999)),
                new DateTimeRange(new DateTime(2017, 2, 1, 0, 0, 0), new DateTime(2017, 2, 16, 23, 59, 59, 999).AddTicks(9999))
            };
            if (dateTimeRange.Count != expectRange.Count) { throw new Exception("Number of items mismatched"); }
            for (var index = 0; index < dateTimeRange.Count; index++)
            {
                if (dateTimeRange[index].StartDate != expectRange[index].StartDate) { throw new Exception("Start Date Mismatch...  Got: " + dateTimeRange[index].StartDate + " Expected: " + expectRange[index].StartDate); }
                if (dateTimeRange[index].EndDate != expectRange[index].EndDate) { throw new Exception("End Date Mismatch...  Got: " + dateTimeRange[index].EndDate + " Expected: " + expectRange[index].EndDate); }
            }
        }
    }
}
