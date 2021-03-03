using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Daikin.DotNetLib.Application
{
    public static class Appsettings
    {
        /// <summary>
        /// Access the appsettings.[{environment}].json configuration settings and optionally the secrets.json
        /// </summary>
        /// <param name="location">Location for appsettings.json file(s)</param>
        /// <param name="userSecretsId">User secret identifier (if not specified, not used)</param>
        /// <param name="environments">Other appsettings.{environment}.json environemnts to load</param>
        /// <returns>IConfigurationRoot</returns>
        public static IConfigurationRoot GetConfiguration(string location, string userSecretsId = null, params string[] environments)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .SetBasePath(location)
                .AddEnvironmentVariables();

            if (!string.IsNullOrEmpty(userSecretsId)) configurationBuilder.AddUserSecrets(userSecretsId);
            if (environments == null) return configurationBuilder.Build();
            foreach (var environment in environments)
            {
                configurationBuilder.AddJsonFile($"appsettings.{environment}.json", true);
            }
            return configurationBuilder.Build();
        }

        /// <summary>
        /// Access the appsettings.[{environment}].json configuration settings and optionally the secrets.json
        /// </summary>
        /// <param name="type">Object type to reference its assembly to get the location of the appsettings.json file(s)</param>
        /// <param name="useSecrets">Whether the use a self-defined (in the csproj) secrets.json file</param>
        /// <param name="environments">Other appsettings.{environment}.json environemnts to load</param>
        /// <returns>IConfigurationRoot</returns>
        public static IConfigurationRoot GetConfiguration(Type type, bool useSecrets = true, params string[] environments)
        {
            var assembly = type.GetTypeInfo().Assembly;
            var location = Path.GetDirectoryName(assembly.Location);
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .SetBasePath(location)
                .AddEnvironmentVariables();

            if (useSecrets) configurationBuilder.AddUserSecrets(assembly);
            if (environments == null) return configurationBuilder.Build();
            foreach (var environment in environments)
            {
                configurationBuilder.AddJsonFile($"appsettings.{environment}.json", true);
            }
            return configurationBuilder.Build();
        }

        /// <summary>
        /// Access the appsettings.[{environment}].json configuration settings and optionally the secrets.json
        /// </summary>
        /// <param name="type">Object type to reference its assembly to get the location of the appsettings.json file(s)</param>
        /// <param name="userSecretsId">User secret identifier (if not specified, not used)</param>
        /// <param name="environments">Other appsettings.{environment}.json environemnts to load</param>
        /// <returns>IConfigurationRoot</returns>
        public static IConfigurationRoot GetConfiguration(Type type, string userSecretsId = null, params string[] environments)
        {
            var location = Path.GetDirectoryName(type.GetTypeInfo().Assembly.Location);
            return GetConfiguration(location, userSecretsId, environments);
        }
    }
}
