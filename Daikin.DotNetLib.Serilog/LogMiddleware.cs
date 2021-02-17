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
            LogContext.PushProperty(Constants.UserField, PropertyUser(context));
            LogContext.PushProperty(Constants.ClientField, PropertyClient(context));
            LogContext.PushProperty(Constants.RemoteIpField, PropertyRemoteIp(context));
            LogContext.PushProperty(Constants.UserAgentField, PropertyUserAgent(context));
            LogContext.PushProperty(Constants.SessionField, PropertySession(context));
            LogContext.PushProperty(Constants.RequestField, PropertyRequest(context));
            return _next(context);
        }
        #endregion

        #region Functions
        private static string PropertyUser(HttpContext context)
        {
            try
            {
                // context.User.GetSubjectId()
                return context.User.Identity?.Name.Truncate(Constants.MaxLengthUser);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(Constants.MaxLengthUser);
            }
        }

        private static string PropertyClient(HttpContext context)
        {
            try
            {
                var queryVars = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
                var clientId = queryVars.Get("client_id");
                if (!string.IsNullOrEmpty(clientId)) return clientId.Truncate(Constants.MaxLengthClientId);
                var returnUrl = queryVars.Get("returnurl");
                if (string.IsNullOrEmpty(returnUrl)) return null;
                var queryReturnUrlVars = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(returnUrl));
                clientId = queryReturnUrlVars.Get("client_id");
                return clientId.Truncate(Constants.MaxLengthClientId);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(Constants.MaxLengthClientId);
            }
        }

        public static string PropertyRemoteIp(HttpContext context)
        {
            try
            {
                return context.Connection.RemoteIpAddress?.ToString().Truncate(Constants.MaxLengthRemoteIp);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(Constants.MaxLengthRemoteIp);
            }
        }

        private static string PropertyUserAgent(HttpContext context)
        {
            try
            {
                return context.Request.Headers["User-Agent"].FirstOrDefault().Truncate(Constants.MaxLengthUserAgent);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(Constants.MaxLengthUserAgent);
            }
        }

        private static string PropertySession(HttpContext context)
        {
            try
            {
                var session = context.Request.Cookies["idsrv.session"];
                if (string.IsNullOrEmpty(session)) session = context.Request.Headers["Session"].FirstOrDefault();
                return session.Truncate(Constants.MaxLengthSession);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(Constants.MaxLengthSession);
            }
        }

        private static string PropertyRequest(HttpContext context)
        {
            try
            {
                return context.TraceIdentifier.Truncate(Constants.MaxLengthRequestId);
            }
            catch (Exception ex)
            {
                return ex.Message.Truncate(Constants.MaxLengthRequestId);
            }
        }
        #endregion

    }
}
