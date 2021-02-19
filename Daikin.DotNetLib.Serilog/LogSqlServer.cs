using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Exceptions;
using Serilog.Sinks.MSSqlServer;

namespace Daikin.DotNetLib.Serilog
{
    public static class LogSqlServer
    {
        /// <summary>
        /// Adds WriteTo.DetailedMsSql() MSSQL Server to logger configuration with pre-defined custom columns
        /// </summary>
        /// <param name="loggerSinkConfiguration">The logger configuration</param>
        /// <param name="caller">typeof(Startup) or typeof(Program) will likely work</param>
        /// <param name="sqlConnectionStr">SQL Connection string for Serilog</param>
        /// <param name="additionalColumns">Additional Columns to include (extending base set is always included)</param>
        /// <param name="configuration">(Optional) Read appsettings configuration</param>
        /// <param name="env">(Optional) Environment information - if not available, attempt to get Environmental Variable ASPNETCORE_ENVIRONMENT</param>
        /// <param name="sinkOptions">Supples additional settings for the sink</param>
        /// <param name="tableName">Name of the SQL table</param>
        /// <returns></returns>
        public static LoggerConfiguration DetailedMsSql(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            Type caller,
            string sqlConnectionStr,
            List<SqlColumn> additionalColumns = null,
            //IConfiguration configuration = null, 
            IHostingEnvironment env = null,
            MSSqlServerSinkOptions sinkOptions = null,
            string tableName = "!_Serilog")
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            if (caller == null) throw new ArgumentNullException(nameof(caller));
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(nameof(tableName));
            if (string.IsNullOrEmpty(sqlConnectionStr)) throw new ArgumentNullException(nameof(sqlConnectionStr));

            var appName = caller.GetTypeInfo().Assembly.GetName();
            var applicationName = appName.Name.Truncate(LogColumns.MaxLengthApplication);
            var applicationVersion = appName.Version?.ToString().Truncate(LogColumns.MaxLengthVersion);
            var environment = env != null
                ? env.EnvironmentName.Truncate(LogColumns.MaxLengthEnvironment)
                : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Truncate(LogColumns.MaxLengthEnvironment);
            
            if (additionalColumns == null) additionalColumns = LogColumns.Build(); // get base
            var columnOptions = new ColumnOptions {AdditionalColumns = additionalColumns};
            columnOptions.Store.Add(StandardColumn.LogEvent); // want JSON data
            columnOptions.Store.Remove(StandardColumn.Properties); // don't need XML data

            if (sinkOptions == null) sinkOptions = new MSSqlServerSinkOptions { AutoCreateSqlTable = true, TableName = tableName };

            var loggerConfiguration = loggerSinkConfiguration.MSSqlServer(
                connectionString: sqlConnectionStr,
                //appConfiguration: configuration,
                columnOptions: columnOptions,
                sinkOptions: sinkOptions);

            return loggerConfiguration
                .Enrich.FromLogContext() // Required to support custom logging
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Version", applicationVersion)
                .Enrich.WithProperty("Environment", environment);
        }
    }
}
