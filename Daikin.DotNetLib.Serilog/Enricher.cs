using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace Daikin.DotNetLib.Serilog
{
    public static class Enricher
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
        public static void PostLog(string source, int eventId, string message, string data = null, LogType logType = LogType.Informational)
        {
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

        public static void AddMiddleware(IApplicationBuilder app)
        {
            // Add Serilog Middleware for information injection
            //app.UseMiddleware<SerilogMiddleware>();
            app.Use(async (context, next) =>
            {
                var properties = new List<ILogEventEnricher>
                {
                    PropertyClientId(context),
                    PropertyRemoteIp(context),
                    PropertyUserAgent(context),
                    PropertySession(context),
                    PropertyUser(context),
                    PropertyRequestId(context)
                };

                using (LogContext.Push(properties.ToArray()))
                {
                    await next.Invoke();
                }
            });
        }

        public static PropertyEnricher PropertyClientId(HttpContext context)
        {
            const string property = "ClientId";
            try
            {
                var queryVars = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
                var clientId = queryVars.Get("client_id");
                if (!string.IsNullOrEmpty(clientId)) return new PropertyEnricher(property, clientId.Truncate(Constants.MaxLengthClientId));
                var returnUrl = queryVars.Get("returnurl");
                if (string.IsNullOrEmpty(returnUrl)) return new PropertyEnricher(property, null);
                var queryReturnUrlVars = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(returnUrl));
                clientId = queryReturnUrlVars.Get("client_id");
                return new PropertyEnricher(property, clientId.Truncate(Constants.MaxLengthClientId));
            }
            catch (Exception)
            {
                return new PropertyEnricher(property, null);
            }
        }

        public static PropertyEnricher PropertyRemoteIp(HttpContext context)
        {
            const string property = "RemoteIp";
            try
            {
                return context.Connection.RemoteIpAddress != null
                    ? new PropertyEnricher(property, context.Connection.RemoteIpAddress.ToString().Truncate(Constants.MaxLengthRemoteIp))
                    : new PropertyEnricher(property, null);
            }
            catch (Exception)
            {
                return new PropertyEnricher(property, null);
            }
        }

        public static PropertyEnricher PropertyUserAgent(HttpContext context)
        {
            const string property = "UserAgent";
            try
            {
                return new PropertyEnricher(property, context.Request.Headers["User-Agent"].FirstOrDefault().Truncate(Constants.MaxLengthUserAgent));
            }
            catch (Exception)
            {
                return new PropertyEnricher(property, null);
            }
        }

        public static PropertyEnricher PropertySession(HttpContext context)
        {
            const string property = "Session";
            try
            {
                var session = context.Request.Cookies["idsrv.session"];
                if (string.IsNullOrEmpty(session)) session = context.Request.Headers["Session"].FirstOrDefault();
                return new PropertyEnricher(property, session.Truncate(Constants.MaxLengthSession));
            }
            catch (Exception)
            {
                return new PropertyEnricher(property, null);
            }
        }

        public static PropertyEnricher PropertyUser(HttpContext context)
        {
            const string property = "User";
            try
            {
                // context.User.GetSubjectId()
                return context.User.Identity != null
                    ? new PropertyEnricher(property, context.User.Identity.Name.Truncate(Constants.MaxLengthUser))
                    : new PropertyEnricher(property, null);
            }
            catch (Exception)
            {
                return new PropertyEnricher(property, null);
            }
        }

        public static PropertyEnricher PropertyRequestId(HttpContext context)
        {
            const string property = "RequestId";
            try
            {
                return new PropertyEnricher(property, context.TraceIdentifier.Truncate(Constants.MaxLengthRequestId));
            }
            catch (Exception)
            {
                return new PropertyEnricher(property, null);
            }
        }

        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
        }
        #endregion
    }
}
