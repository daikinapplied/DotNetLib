using System;
using Serilog;

namespace Daikin.DotNetLib.Serilog
{
    public static class Logger
    {
        #region Enumerators
        public enum LogType
        {
            Informational,
            Warning,
            Error,
            Critical
        }
        #endregion

        #region Functions
        public static void PostEntry(string source, int eventId, string message, string data = null, LogType logType = LogType.Informational)
        {
            // Use Static Log
            var log = Log
                .ForContext(Constants.SourceField, source.Truncate(Constants.MaxLengthSource))
                .ForContext(Constants.EventIdField, eventId)
                .ForContext(Constants.DataField, data.Truncate(Constants.MaxLengthData));

            switch (logType)
            {
                case LogType.Informational:
                    log.Information(message);
                    break;
                case LogType.Warning:
                    log.Warning(message);
                    break;
                case LogType.Error:
                    log.Error(message);
                    break;
                case LogType.Critical:
                    log.Fatal(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
        #endregion
    }
}
