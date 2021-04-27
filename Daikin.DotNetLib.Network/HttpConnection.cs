using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Daikin.DotNetLib.Network
{
    public static class HttpConnection
    {
        #region Constants
        public const string AccessTokenScheme = "Bearer";
        public const int ConnectionTimeOut = 60;
        public const string MimeType = "application/json";
        public const int HttpsPort = 443;
        #endregion

        #region Functions
        /// <summary>
        /// Setup a HTTP Client
        /// </summary>
        /// <param name="serverUrl">URL of server to connect via HTTP/HTTPS</param>
        /// <param name="mimeType">MimeType of the communication</param>
        /// <param name="useProxy">Whether to use a proxy</param>
        /// <param name="proxyServer">Proxy server URL</param>
        /// <param name="proxyPort">Proxy server TCP port</param>
        /// <param name="bypassProxyOnLocal">Whether to ignore the proxy server for a local connection (e.g. same computer or LAN)</param>
        /// <param name="accessToken">Access token for OAuth</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response</param>
        /// <param name="tokenHeader">Token reference to include in the HTTP header</param>
        /// <returns>HTTP Client Object</returns>
        public static HttpClient Client(
            string serverUrl, 
            string mimeType = MimeType, 
            bool useProxy = false, 
            string proxyServer = "", 
            int proxyPort = HttpsPort, 
            bool bypassProxyOnLocal = false, 
            string accessToken = "", 
            int httpTimeoutSeconds = ConnectionTimeOut, 
            string tokenHeader = AccessTokenScheme)
        {
            HttpClient httpClient;
            if (useProxy)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    UseProxy = true,
                    Proxy = new WebProxy(proxyServer, proxyPort) { BypassProxyOnLocal = bypassProxyOnLocal }
                    //CookieContainer = new CookieContainer(),
                    //UseDefaultCredentials = false
                };
                httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(serverUrl), Timeout = TimeSpan.FromSeconds(httpTimeoutSeconds) };
            }
            else
            {
                httpClient = new HttpClient { BaseAddress = new Uri(serverUrl), Timeout = TimeSpan.FromSeconds(httpTimeoutSeconds) };
            }

            httpClient.DefaultRequestHeaders.Connection.Clear();
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Daikin.DotNetLib.Network", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mimeType));
            }

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenHeader ?? AccessTokenScheme, accessToken);
            }
            return httpClient;
        }

        /// <summary>
        /// Determine if caller matches an IP address mask (regular expression)
        /// </summary>
        /// <param name="httpContextAccessor">HttpContext.Current with .NET Framework, HttpContext with .NET Core</param>
        /// <param name="ipFilterRegEx">Regular Expression (typically stored in web.config or appsettings.json, such as 127.0.1|::1 for localhost</param>
        /// <param name="requireSecured">Check if the connection should be secured (generally recommended)</param>
        /// <returns></returns>
        public static (bool, string) AllowedCaller(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor, string ipFilterRegEx, bool requireSecured = true)
        {
            var ipAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (string.IsNullOrEmpty(ipAddress)) return (false, "Unable to determine caller IP Address");
            var isSecure = httpContextAccessor.HttpContext.Request.IsHttps;
            if (requireSecured && !isSecure) return (false, "Caller made insecure request");
            return !Regex.IsMatch(ipAddress, ipFilterRegEx) 
                ? (false, $"Invalid caller from {ipAddress}") 
                : (true, "Success");
        }
        #endregion
    }
}
