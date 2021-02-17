using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Exceptions;
using Serilog.Sinks.MSSqlServer;

namespace Daikin.DotNetLib.Serilog
{
    public static class MsSqlServer
    {
        /// <summary>
        /// Adds WriteTo.DetailedMsSql() MSSQL Server to logger configuration with pre-defined custom columns
        /// </summary>
        /// <param name="loggerSinkConfiguration">The logger configuration</param>
        /// <param name="caller">typeof(Startup) or typeof(Program) will likely work</param>
        /// <param name="configuration">(Optional) Read appsettings configuration</param>
        /// <param name="sqlConnectionStr">(Optional) SQL Connection string - if not available, attempt to get from ConnectionStrings:DefaultConnection in the configuration</param>
        /// <param name="env">(Optional) Environment information - if not available, attempt to get Environmental Variable ASPNETCORE_ENVIRONMENT</param>
        /// <param name="sinkOptions">Supples additional settings for the sink</param>
        /// <returns></returns>
        public static LoggerConfiguration DetailedMsSql(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            Type caller, 
            IConfiguration configuration = null, 
            string sqlConnectionStr = null, 
            IHostingEnvironment env = null,
            MSSqlServerSinkOptions sinkOptions = null)
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            if (caller == null) throw new ArgumentNullException(nameof(caller));

            var appName = caller.GetTypeInfo().Assembly.GetName();
            var applicationName = appName.Name.Truncate(Constants.MaxLengthApplication);
            var applicationVersion = appName.Version?.ToString().Truncate(Constants.MaxLengthVersion);
            var environment = env != null
                ? env.EnvironmentName.Truncate(Constants.MaxLengthEnvironment)
                : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Truncate(Constants.MaxLengthEnvironment);
            if (string.IsNullOrEmpty(sqlConnectionStr) && configuration != null) sqlConnectionStr = configuration["ConnectionStrings:DefaultConnection"];

            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new List<SqlColumn>
                    {
                        new SqlColumn { ColumnName = "EventId", DataType = SqlDbType.Int, AllowNull = true },
                        new SqlColumn { ColumnName = "Application", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthApplication, AllowNull = true },
                        new SqlColumn { ColumnName = "Version", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthVersion, AllowNull = true },
                        new SqlColumn { ColumnName = "RemoteIp", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthRemoteIp, AllowNull = true },
                        new SqlColumn { ColumnName = "ClientId", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthClientId, AllowNull = true },
                        new SqlColumn { ColumnName = "Environment", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthEnvironment, AllowNull = true },
                        new SqlColumn { ColumnName = "User", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthUser, AllowNull = true },
                        new SqlColumn { ColumnName = "Source", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthSource, AllowNull = true },
                        new SqlColumn { ColumnName = "Session", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthSession, AllowNull = true },
                        new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthUserAgent, AllowNull = true },
                        new SqlColumn { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthRequestId, AllowNull = true },
                        new SqlColumn { ColumnName = "Data", DataType = SqlDbType.NVarChar, DataLength = Constants.MaxLengthData, AllowNull = true }
                    }
            };
            columnOptions.Store.Add(StandardColumn.LogEvent); // want JSON data
            columnOptions.Store.Remove(StandardColumn.Properties); // don't need XML data

            if (sinkOptions == null)
            {
                sinkOptions = new MSSqlServerSinkOptions { AutoCreateSqlTable = true, TableName = "!_Serilog" };

            }

            var loggerConfiguration = loggerSinkConfiguration.MSSqlServer(
                connectionString: sqlConnectionStr,
                appConfiguration: configuration,
                columnOptions: columnOptions,
                sinkOptions: sinkOptions);

            return loggerConfiguration
                .Enrich.FromLogContext() // Required to support custom fields
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Version", applicationVersion)
                .Enrich.WithProperty("Environment", environment);
        }
    }
}
