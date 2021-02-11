using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.MSSqlServer;

namespace Daikin.DotNetLib.Serilog
{
    public static class MsSql
    {
        // Call with typeof(Startup) or typeof(Program)
        public static LoggerConfiguration AddLogger(Type caller, IConfiguration configuration = null, string sqlConnectionStr = null, string tableName = "!_Serilog", IHostingEnvironment env = null)
        {
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

            var loggerConfiguration = new LoggerConfiguration()
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
                .Enrich.WithProperty("Environment", environment);

            if (configuration != null) loggerConfiguration.ReadFrom.Configuration(configuration); // get additional settings from appsettings.json

            return loggerConfiguration;
        }

    }
}
