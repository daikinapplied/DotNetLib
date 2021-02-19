using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
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
        /// <param name="connectionString">SQL Connection string for Serilog</param>
        /// <param name="configuration">(Optional) Read appsettings configuration</param>
        /// <param name="environment">(Optional) Environment information - if not available, attempt to get Environmental Variable ASPNETCORE_ENVIRONMENT</param>
        /// <param name="sinkOptions">(Optional) Supples additional settings for the sink - if not specified, default settings used</param>
        /// <param name="restrictedToMinimumLevel">(Optional) Minimum Logging Level</param>
        /// <param name="columnOptions">(Optional) Column settings and definition - if not specified, default settings used</param>
        /// <param name="tableName"(Optional) >Name of the SQL table - if not specified, default setting used</param>
        /// <returns></returns>
        public static LoggerConfiguration DetailedMsSql(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            Type caller,
            string connectionString,
            IConfiguration configuration = null, 
            IHostingEnvironment environment = null,
            MSSqlServerSinkOptions sinkOptions = null,
            ColumnOptions columnOptions = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Information,
            string tableName = "!_Serilog")
        {
            var assemblyName = caller.GetTypeInfo().Assembly.GetName();
            var applicationName = assemblyName.Name.Truncate(LogColumns.MaxLengthApplication);
            var applicationVersion = assemblyName.Version?.ToString().Truncate(LogColumns.MaxLengthVersion);
            var environmentName = environment != null
                ? environment.EnvironmentName.Truncate(LogColumns.MaxLengthEnvironment)
                : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Truncate(LogColumns.MaxLengthEnvironment);
            
            if (sinkOptions == null) sinkOptions = new MSSqlServerSinkOptions { AutoCreateSqlTable = true, TableName = tableName };
            if (columnOptions == null) columnOptions = LogColumns.DefaultColumnOptions();

            return loggerSinkConfiguration.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: sinkOptions,
                    appConfiguration: configuration,
                    columnOptions: columnOptions,
                    restrictedToMinimumLevel: restrictedToMinimumLevel)
                .Enrich.FromLogContext() // Required to support custom logging
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("Version", applicationVersion)
                .Enrich.WithProperty("Environment", environmentName);
        }
    }
}
