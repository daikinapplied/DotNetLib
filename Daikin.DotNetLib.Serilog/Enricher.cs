using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Exceptions;
using Serilog.Sinks.MSSqlServer;

namespace Daikin.DotNetLib.Serilog
{
    public static class Enricher
    {
        #region Constants
        public const string SourceField = "Source";
        public const string EventIdField = "EventId";
        public const string DataField = "Data";

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
        public static void PostLog(string source, int eventId, string message, string data = null, LogType logType = LogType.Informational)
        {
            var log = Log
                .ForContext(SourceField, source.Truncate(MaxLengthSource))
                .ForContext(EventIdField, eventId)
                .ForContext(DataField, data.Truncate(MaxLengthData));

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

        // Call with typeof(Startup) or typeof(Program)
        public static Logger AddSqlLogger(Type caller, IConfiguration configuration, string sqlConnectionStr = null, string tableName = "!_Serilog", IHostingEnvironment env = null)
        {
            var appName = caller.GetTypeInfo().Assembly.GetName();
            var applicationName = appName.Name.Truncate(MaxLengthApplication);
            var applicationVersion = appName.Version?.ToString().Truncate(MaxLengthVersion);
            var environment = env != null
                ? env.EnvironmentName.Truncate(MaxLengthEnvironment)
                : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Truncate(MaxLengthEnvironment);
            if (string.IsNullOrEmpty(sqlConnectionStr)) sqlConnectionStr = configuration["ConnectionStrings:DefaultConnection"];

            var columnOptions = new ColumnOptions
                {
                    AdditionalColumns = new List<SqlColumn>
                        {
                            new SqlColumn { ColumnName = "EventId", DataType = SqlDbType.Int, AllowNull = true },
                            new SqlColumn { ColumnName = "Application", DataType = SqlDbType.NVarChar, DataLength = MaxLengthApplication, AllowNull = true },
                            new SqlColumn { ColumnName = "Version", DataType = SqlDbType.NVarChar, DataLength = MaxLengthVersion, AllowNull = true },
                            new SqlColumn { ColumnName = "RemoteIp", DataType = SqlDbType.NVarChar, DataLength = MaxLengthRemoteIp, AllowNull = true },
                            new SqlColumn { ColumnName = "ClientId", DataType = SqlDbType.NVarChar, DataLength = MaxLengthClientId, AllowNull = true },
                            new SqlColumn { ColumnName = "Environment", DataType = SqlDbType.NVarChar, DataLength = MaxLengthEnvironment, AllowNull = true },
                            new SqlColumn { ColumnName = "User", DataType = SqlDbType.NVarChar, DataLength = MaxLengthUser, AllowNull = true },
                            new SqlColumn { ColumnName = "Source", DataType = SqlDbType.NVarChar, DataLength = MaxLengthSource, AllowNull = true },
                            new SqlColumn { ColumnName = "Session", DataType = SqlDbType.NVarChar, DataLength = MaxLengthSession, AllowNull = true },
                            new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = MaxLengthUserAgent, AllowNull = true },
                            new SqlColumn { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = MaxLengthRequestId, AllowNull = true },
                            new SqlColumn { ColumnName = "Data", DataType = SqlDbType.NVarChar, DataLength = MaxLengthData, AllowNull = true }
                        }
                };
            columnOptions.Store.Add(StandardColumn.LogEvent); // want JSON data
            columnOptions.Store.Remove(StandardColumn.Properties); // don't need XML data

            var log = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // get additional settings from appsettings.json
                .WriteTo.Console(outputTemplate: Theme.Template, theme: Theme.System)
                .WriteTo.MSSqlServer(
                        connectionString: sqlConnectionStr, 
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            AutoCreateSqlTable = true,
                            TableName = tableName,
                        }, 
                        columnOptions: columnOptions
                    )
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Version", applicationVersion)
                .Enrich.WithProperty("Environment", environment)
                .CreateLogger();

            return log;
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
                if (!string.IsNullOrEmpty(clientId)) return new PropertyEnricher(property, clientId.Truncate(MaxLengthClientId));
                var returnUrl = queryVars.Get("returnurl");
                if (string.IsNullOrEmpty(returnUrl)) return new PropertyEnricher(property, null);
                var queryReturnUrlVars = HttpUtility.ParseQueryString(HttpUtility.UrlDecode(returnUrl));
                clientId = queryReturnUrlVars.Get("client_id");
                return new PropertyEnricher(property, clientId.Truncate(MaxLengthClientId));
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
                    ? new PropertyEnricher(property, context.Connection.RemoteIpAddress.ToString().Truncate(MaxLengthRemoteIp))
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
                return new PropertyEnricher(property, context.Request.Headers["User-Agent"].FirstOrDefault().Truncate(MaxLengthUserAgent));
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
                return new PropertyEnricher(property, session.Truncate(MaxLengthSession));
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
                    ? new PropertyEnricher(property, context.User.Identity.Name.Truncate(MaxLengthUser))
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
                return new PropertyEnricher(property, context.TraceIdentifier.Truncate(MaxLengthRequestId));
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
