﻿using System;
using Serilog;

namespace Daikin.DotNetLib.Serilog
{
    public static class Logger
    {
        #region Constants

        public const string SourceField = "Source";
        public const string EventIdField = "EventId";
        public const string DataField = "Data";
        public const string UserField = "User";
        public const string UserAgentField = "UserAgent";
        public const string ClientField = "ClientId";
        public const string RemoteIpField = "RemoteIp";
        public const string SessionField = "Session";
        public const string RequestField = "RequestId";

        public const int EventIdInternal = 27000;
        public const int EventIdStartup = 27001;
        public const int EventIdShutdown = 27002;
        public const int EventIdConfigure = 27003;
        public const int EventIdAuthenticateStart = 27004;
        public const int EventIdAuthenticateSuccess = 27005;
        public const int EventIdAuthenticateFailure = 27006;
        public const int EventIdAuthorizeStart = 27007;
        public const int EventIdAuthorizeSuccess = 27008;
        public const int EventIdAuthorizeFailure = 27009;
        public const int EventIdTimerStart = 27010;
        public const int EventIdTimerEnd = 27011;
        public const int EventIdInformation = 27027;
        public const int EventIdDebug = 27096;
        public const int EventIdDebugStart = 27097;
        public const int EventIdDebugEnd = 27098;
        public const int EventIdException = 27099;

        public const string EventMessageInternal = "Internal";
        public const string EventMessageStartup = "Startup";
        public const string EventMessageShutdown = "Shutdown";
        public const string EventMessageConfigure = "Configure";
        public const string EventMessageAuthenticate = "Authenticate";
        public const string EventMessageAuthenticateStart = "Authentication Start";
        public const string EventMessageAuthenticateSuccess = "Authentication Success";
        public const string EventMessageAuthenticateFailure = "Authentication Failure";
        public const string EventMessageAuthorize = "Authorize";
        public const string EventMessageAuthorizeStart = "Authorization Start";
        public const string EventMessageAuthorizeSuccess = "Authorization Success";
        public const string EventMessageAuthorizeFailure = "Authorize Failure";
        public const string EventMessageTimerStart = "Time Started";
        public const string EventMessageTimerEnd = "Time Ended";
        public const string EventMessageInformation = "Information";
        public const string EventMessageDebug = "Debug";
        public const string EventMessageDebugStart = "Debug Start";
        public const string EventMessageDebugEnd = "Debug End";
        public const string EventMessageException = "Exception";
        #endregion

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
                .ForContext(SourceField, source.Truncate(LogColumns.MaxLengthSource))
                .ForContext(EventIdField, eventId)
                .ForContext(DataField, data);

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
