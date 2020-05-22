using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace Daikin.DotNetLib.Core.Tests
{
    public class Config
    {
        #region Properties
        public string FacebookAccessToken { get; set; }
        public string FacebookAppSecret { get; set; }
        public string FacebookPageOrId { get; set; }
        public string TeamsWebHookUrl { get; set; }
        public string FakeJsonUrl { get; set; }
        public string FakeJsonToken { get; set; }
        #endregion

        #region Functions
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets("7c94ff42-b17b-4a22-89ff-38d531f03402") // File secrets.json in folder %APPDATA%\Microsoft\UserSecrets
                .AddEnvironmentVariables()
                .Build();
        }

        public static Config GetConfiguration()
        {
            var outputPath = Path.GetDirectoryName(typeof(Config).GetTypeInfo().Assembly.Location);
            var configuration = new Config();
            var iConfig = GetIConfigurationRoot(outputPath);
            iConfig
                .GetSection("Tests")
                .Bind(configuration);
            return configuration;
        }
        #endregion
    }
}
