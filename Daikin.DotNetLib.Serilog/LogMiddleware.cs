using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Daikin.DotNetLib.Serilog
{
    public class LogMiddleware
    {
        #region Fields
        private readonly RequestDelegate _next;
        #endregion

        #region Constructors
        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion

        #region Methods
        public Task Invoke(HttpContext context)
        {
            // Order Doesn't matter
            LogContext.PushProperty(Logger.ClientField, PropertyClient(context));
            LogContext.PushProperty(Logger.RemoteIpField, PropertyRemoteIp(context));
            LogContext.PushProperty(Logger.UserAgentField, PropertyUserAgent(context));
            LogContext.PushProperty(Logger.SessionField, PropertySession(context));
            LogContext.PushProperty(Logger.UserField, PropertyUser(context));
            LogContext.PushProperty(Logger.RequestField, PropertyRequest(context));
            return _next(context);
        }
        #endregion

        #region Functions
        private static string PropertyUser(HttpContext context)
        {
            try
            {
                // context.User.GetSubjectId()
                return context.User.Identity?.Name.Truncate(LogColumns.MaxLengthUser);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(LogColumns.MaxLengthUser);
            }
        }

        private static string PropertyClient(HttpContext context)
        {
            try
            {
                var queryVars = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
                var clientId = queryVars.Get("client_id");
                if (!string.IsNullOrEmpty(clientId)) return clientId.Truncate(LogColumns.MaxLengthClientId);
                var returnUrl = queryVars.Get("returnurl");
                if (string.IsNullOrEmpty(returnUrl)) return null;
                var queryReturnUrlVars = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(returnUrl));
                clientId = queryReturnUrlVars.Get("client_id");
                return clientId.Truncate(LogColumns.MaxLengthClientId);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(LogColumns.MaxLengthClientId);
            }
        }

        public static string PropertyRemoteIp(HttpContext context)
        {
            try
            {
                return context.Connection.RemoteIpAddress?.ToString().Truncate(LogColumns.MaxLengthRemoteIp);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(LogColumns.MaxLengthRemoteIp);
            }
        }

        private static string PropertyUserAgent(HttpContext context)
        {
            try
            {
                return context.Request.Headers["User-Agent"].FirstOrDefault().Truncate(LogColumns.MaxLengthUserAgent);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(LogColumns.MaxLengthUserAgent);
            }
        }

        private static string PropertySession(HttpContext context)
        {
            try
            {
                var session = context.Request.Cookies["idsrv.session"];
                if (string.IsNullOrEmpty(session)) session = context.Request.Headers["Session"].FirstOrDefault();
                return session.Truncate(LogColumns.MaxLengthSession);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(LogColumns.MaxLengthSession);
            }
        }

        private static string PropertyRequest(HttpContext context)
        {
            try
            {
                return context.TraceIdentifier.Truncate(LogColumns.MaxLengthRequestId);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(LogColumns.MaxLengthRequestId);
            }
        }
        #endregion

    }
}
