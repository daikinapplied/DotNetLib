using System;

namespace Daikin.DotNetLib.Data
{
    public enum ErrorType
    {
        Error,
        Warning,
        Informational
    }

    public enum TrackingType
    {
        DateTime,
        Counter,
        Guid
    }

    public static class Error
    {
        public static string BuildString(string code, string message, ErrorType errorType = ErrorType.Error, TrackingType trackingType = TrackingType.DateTime, object data = null)
        {
            string errorTypeMsg;
            switch (errorType)
            {
                case ErrorType.Error:
                    errorTypeMsg = "Error Encountered";
                    break;
                case ErrorType.Warning:
                    errorTypeMsg = "Warning Notification";
                    break;
                case ErrorType.Informational:
                    errorTypeMsg = "Informational Notification";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorType), errorType, null);
            }

            string errorId;
            switch (trackingType)
            {
                case TrackingType.DateTime:
                    errorId = BuildDateId(data == null ? DateTime.Now : Convert.ToDateTime(data));
                    break;
                case TrackingType.Counter:
                    if (data == null) throw new Exception("Counter usage desired but not specified");
                    errorId = "#" + Convert.ToInt64(data);
                    break;
                case TrackingType.Guid:
                    errorId = "G" + (data == null ? new Guid().ToString() : Convert.ToString(data));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null);
            }
            return $"{errorTypeMsg}: {message} ({code} {errorId})";
        }

        public static string BuildDateId(DateTime when)
        {
            // Convert hex code based on the current date and time
            return Convert.ToString(Convert.ToInt64($"{when.Year.ToString().Substring(2)}{PadNumber(when.Month)}{PadNumber(when.Day)}{PadNumber(when.Hour)}{PadNumber(when.Minute)}{PadNumber(when.Second)}"), 16);
        }

        private static string PadNumber(int number)
        {
            return number.ToString().PadLeft(2, '0');
        }
    }
}
