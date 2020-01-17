using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Channels;  // System.ServiceModel reference library for RemoteEndpointMessageProperty
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace Daikin.DotNetLib.Windows
{
    public static class Web
    {
        #region Functions
        public static string ReadConfigSetting(string key)
        {
            try
            {
                return WebConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool WriteConfigSetting(string key, string value)
        {
            try
            {
                var configFile = WebConfigurationManager.OpenMachineConfiguration();
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            if (!request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                return HttpContext.Current?.Request.UserHostAddress;
            }
            var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            return prop.Address;
        }

        public static StringBuilder ServerInformation(HttpRequestMessage message, string userContext, string servers)
        {
            var eventDetails = new StringBuilder();

            eventDetails.AppendLine("~~SERVER CONFIG~~");
            eventDetails.AppendLine("Servers: " + servers);
            eventDetails.AppendLine("User Context: " + userContext);

            eventDetails.AppendLine("~~HTTP CLIENT~~");
            eventDetails.AppendLine("Client IP: " + ClientIp(message));

            eventDetails.AppendLine("~~HTTP REQUEST~~");
            eventDetails.AppendLine(message.Method.Method + " " + message.RequestUri + " HTTP/" + message.Version);
            foreach (var header in message.Headers)
            {
                var valueList = string.Empty;
                foreach (var value in header.Value)
                {
                    if (!string.IsNullOrEmpty(valueList)) valueList += ", ";
                    valueList += value;
                }
                eventDetails.AppendLine(header.Key + ": " + valueList);
            }
            return eventDetails;
        }

        public static string ServerUrl()
        {
            var serverUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
            if (HttpContext.Current.Request.ApplicationPath != null)
            {
                serverUrl += HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
            }

            return serverUrl;
        }

        public static void LogHttpRequest(string source, HttpRequestMessage message, string userContext, string server)
        {
            const int statusCode = 271727;
            var eventDetails = ServerInformation(message, userContext, server);
            if (message.Content != null)
            {
                message.Content.ReadAsByteArrayAsync().ContinueWith
                (
                    task =>
                    {
                        string content;
                        using (var reader = new StreamReader(message.Content.ReadAsStreamAsync().Result))
                        {
                            reader.BaseStream.Seek(0, SeekOrigin.Begin);
                            content = reader.ReadToEnd();
                        }
                        eventDetails.AppendLine();
                        eventDetails.AppendLine(content);
                        EventLog.Post(source, statusCode, eventDetails.ToString());
                    }
                );
            }
            else
            {
                eventDetails.AppendLine();
                eventDetails.AppendLine();
                eventDetails.AppendLine("(No Content");
                EventLog.Post(source, statusCode, eventDetails.ToString());
            }
        }
        #endregion
    }
}
