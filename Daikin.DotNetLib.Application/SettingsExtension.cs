using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Daikin.DotNetLib.Application
{
    public static class SettingsExtension
    {
        public static IConfigurationBuilder AddSettingsConfiguration(this IConfigurationBuilder builder)
        {
            if (builder == null) { throw new ArgumentNullException("builder"); }

            return builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Settings.ApplicationEnvironment()}.json", optional: true, reloadOnChange: false)
                .AddUserSecrets(Assembly.GetExecutingAssembly()) // with .NET 6, add (true, true)
                .AddEnvironmentVariables()
                .AddAzureKeyVault(Settings.AzureSecretClient(), new KeyVaultSecretManager());

            // SetBasePath() requires:
            //   Microsoft.Extensions.Configuration.Abstractions
            // AddJsonFile() requires:
            //   Microsoft.Extensions.Configuration.FileExtensions
            //   Microsoft.Extensions.Configuration.Json
            // AddEnvironmentVariables() requires:
            //   Microsoft.Extensions.Configuration.EnvironmentVariables and possibly
            //   Microsoft.Extensions.Configuration.UserSecrets

            // Pass in: Microsoft.Azure.WebJobs.ExecutionContext context
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(context.FunctionAppDirectory)
            //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // Local file settings
            //    .AddEnvironmentVariables() // Application Settings in Azure
            //    .AddUserSecrets(typeof(Config).Assembly)
            //    .Build();
            //return config;
        }
    }
}
