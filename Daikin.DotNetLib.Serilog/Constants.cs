namespace Daikin.DotNetLib.Serilog
{
    public static class Constants
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

        public const int MaxLengthApplication = 50;
        public const int MaxLengthVersion = 10;
        public const int MaxLengthRemoteIp = 40;
        public const int MaxLengthClientId = 40;
        public const int MaxLengthEnvironment = 10;
        public const int MaxLengthUser = 50;
        public const int MaxLengthSource = 50;
        public const int MaxLengthSession = 50;
        public const int MaxLengthRequestId = 25;
        public const int MaxLengthUserAgent = 200;
        public const int MaxLengthData = 255;

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

        public const string EventMessageInternal = "Internal";
        public const string EventMessageStartup = "Startup";
        public const string EventMessageShutdown = "Shutdown";
        public const string EventMessageConfigure = "Configure";
        public const string EventMessageAuthenticate = "Authenticate";
        public const string EventMessageAuthorize = "Authorize";
        #endregion
    }
}
