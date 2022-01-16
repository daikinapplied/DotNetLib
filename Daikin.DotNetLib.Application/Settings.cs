using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Daikin.DotNetLib.Application
{
    public class Settings
    {
        #region Fields
        private IConfiguration _configuration;
        #endregion

        #region Properties
        public static string KeyVaultUrl { get; set; }
        #endregion

        #region Constructors
        public Settings(IConfigurationBuilder builder)
        {
            //_configuration = builder.Build();
            _configuration = builder
                .AddSettingsConfiguration()
                .Build();
        }

        public Settings(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();
            _configuration = builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddSettingsConfiguration()
                .Build();
        }

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Settings(Microsoft.Extensions.Hosting.HostBuilderContext context, IConfigurationBuilder builder)
        {
            // For now, this approach presumes the caller has used CreateDefaultBuilder, which includes the set up of other settings (to avoid duplications)
            _configuration = context.Configuration;
            builder.AddAzureKeyVault(AzureSecretClient(), new KeyVaultSecretManager());
        }

        //public Settings(ConfigurationManager config)
        //{
        //    // .NET 6
        //    _configuration = config
        //        .AddSettingsConfiguration()
        //        .Build();
        //}
        #endregion

        #region Methods
        public string ReadString(string variableName, ILogger log = null, bool hideSettingValue = false)
        {
            var setting = string.Empty;
            if (_configuration != null) setting = _configuration[variableName]; // Try the recommended way first
            if (string.IsNullOrEmpty(setting)) setting = Environment.GetEnvironmentVariable(variableName); // Backup method
            if (log != null) log.LogInformation($"{nameof(Settings)} {variableName}={(hideSettingValue ? "(hidden)" : setting)}");
            return setting;
        }

        public int ReadInt(string variableName, ILogger log = null, bool hideSettingValue = false)
        {
            try { return Convert.ToInt32(ReadString(variableName, log, hideSettingValue)); } catch { return -1; }
        }

        public bool ReadBool(string variableName, ILogger log = null, bool hideSettingValue = false)
        {
            try { return Convert.ToBoolean(ReadString(variableName, log, hideSettingValue)); } catch { return false; }
        }
        #endregion

        #region Functions

        public static SecretClient AzureSecretClient()
        {
            var keyVaultUrl = AzureKeyVaultUrl();
            return IsRunningLocal ? new SecretClient(new Uri(keyVaultUrl), new InteractiveBrowserCredential()) : new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
        }

        public static string ApplicationEnvironment()
        {
            try { return Environment.GetEnvironmentVariable(Constants.AspCoreEnvironmentVar) ?? Constants.EnvironmentDev; } catch (Exception ex) { return ex.Message; }
        }

        public static bool IsEnvironmentPrd()
        {
            return ApplicationEnvironment().Equals(Constants.EnvironmentPrd, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsEnvironmentUat()
        {
            return ApplicationEnvironment().Equals(Constants.EnvironmentUat, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsEnvironmentStg()
        {
            return ApplicationEnvironment().Equals(Constants.EnvironmentStg, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsEnvironmentDev()
        {
            return ApplicationEnvironment().Equals(Constants.EnvironmentDev, StringComparison.OrdinalIgnoreCase);
        }

        public static string AzureKeyVaultUrl() {
            if (string.IsNullOrEmpty(KeyVaultUrl)) throw new Exception($"Accessing Undefined Azure Key Vault URL.  Set value of {nameof(KeyVaultUrl)} first.");
            return KeyVaultUrl;
        } 

        public static string RunningUnder
        {
            get
            {
                string runningUnder;
                try { runningUnder = System.Diagnostics.Process.GetCurrentProcess().ProcessName; } catch (Exception ex) { runningUnder = ex.Message; }
                return runningUnder;
            }
        }

        public static bool IsIis => RunningUnder.ToLower() == "w3wp";
        public static bool IsIisExpress => RunningUnder.ToLower() == "iisexpress";
        public static bool IsKestrel => string.Equals(RunningUnder, System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name, StringComparison.CurrentCultureIgnoreCase);
        public static bool IsAzureFunctionLocal => Environment.GetEnvironmentVariable("IS_RUNNING_LOCALLY") == "true"; // Set in local.settings.json
        public static bool IsUnitTest => RunningUnder.ToLower().Contains("test");
        public static bool IsRunningLocal => IsIisExpress || IsAzureFunctionLocal || IsUnitTest;
        #endregion
    }
}
