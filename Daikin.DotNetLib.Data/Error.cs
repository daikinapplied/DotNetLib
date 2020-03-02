using System;

namespace Daikin.DotNetLib.Data
{
    public enum ErrorType
    {
        Error,
        Warning,
        Informational
    }

    public static class Error
    {
        public static string BuildString(string code, string message, ErrorType errorType = ErrorType.Error, int? errorNumber = null, DateTime? when = null)
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

            var errorId = (errorNumber == null ? BuildDate(when ?? DateTime.Now) : "#" + errorNumber);
            return $"{errorTypeMsg}: {message} ({code} {errorId})";
        }

        public static string BuildDate(DateTime when)
        {
            return $"{when.Year.ToString().Substring(2)}{PadNumber(when.Month)}{PadNumber(when.Day)}{PadNumber(when.Hour)}{PadNumber(when.Minute)}";
        }

        private static string PadNumber(int number)
        {
            return number.ToString().PadLeft(2, '0');
        }
    }
}
