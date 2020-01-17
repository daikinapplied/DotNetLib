using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RecursiveGeek.DotNetLib.Network
{
    // ReSharper disable once UnusedMember.Global
    public class WebApi
    {
        #region Functions
        /// <summary>
        /// Output Debug Information
        /// </summary>
        /// <param name="response">Response object</param>
        /// <param name="serverUrl">Endpoint Server URL (with Path and Query)</param>
        /// <param name="useProxy">Use Proxy? (e.g. Fiddler)</param>
        /// <param name="proxyServer">Proxy Server URL (e.g. Fiddler)</param>
        /// <param name="proxyPort">Proxy Server Port (e.g. Fiddler)</param>
        private static void DebugOutput(HttpResponseMessage response, string serverUrl, bool useProxy, string proxyServer, int proxyPort)
        {
            #if DEBUG
            System.Diagnostics.Debug.WriteLine("HTTP HEADER");
            System.Diagnostics.Debug.WriteLine("Host: " + serverUrl + (useProxy ? " (via " + proxyServer + ":" + proxyPort + ")" : string.Empty));
            System.Diagnostics.Debug.WriteLine("Status Code: " + response.StatusCode + " (" + (response.IsSuccessStatusCode ? "Status" : "Failure") + ")");
            System.Diagnostics.Debug.WriteLine("Reason Phrase: " + response.ReasonPhrase);
            System.Diagnostics.Debug.WriteLine("Version: " + response.Version);
            //System.Diagnostics.Debug.WriteLine("Content: " + response.Content);
            System.Diagnostics.Debug.WriteLine("Headers: " + response.Headers);
            #endif
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Object Input, Object Output
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Used for passing an object as an input and receiving an object output from a WebApi call
        /// </summary>
        /// <typeparam name="TInput">Object Input (converted to JSON for transmision)</typeparam>
        /// <typeparam name="TOutput">Object Output (converted from JSON from transmission)</typeparam>
        /// <param name="serverUrl">Server URL with optional Path and Querystring</param>
        /// <param name="data">Object input to pass along to the WebApi call</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return object from WebApi call</returns>
        /// <remarks>WebApi only allows for one object each way</remarks>
        public static (TOutput, HttpResponseMessage) Call<TInput, TOutput>(string serverUrl, TInput data, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            try
            {
                using (var httpClient = HttpConnection.Client(serverUrl, "application/json", useProxy, proxyServer, proxyPort, false, accessToken, httpTimeoutSeconds))
                {
                    if (method == null) { method = HttpMethod.Put; }
                    var httpRequestMessage = new HttpRequestMessage { Method = method, RequestUri = new Uri(serverUrl) };
                    if (method == HttpMethod.Post || method == HttpMethod.Put)
                    {
                        httpRequestMessage.Content = new StringContent(Json.ObjectToString(data), Encoding.UTF8, "application/json");
                    }
                    var response = httpClient.SendAsync(httpRequestMessage).Result;
                    DebugOutput(response, serverUrl, useProxy, proxyServer, proxyPort);
                    return (!response.IsSuccessStatusCode 
                                ? default 
                                : Json.ObjectFromString<TOutput>(response.Content.ReadAsStringAsync().Result),
                            response);
                }
            }
            catch (Exception ex)
            {
                return (default, new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = NewlineCleanse(ex.Message) });
            }
        }

        /// <summary>
        /// Used for passing an object as an input and receiving an object output from a WebApi call
        /// </summary>
        /// <typeparam name="TInput">Object Input (converted to JSON for transmision)</typeparam>
        /// <typeparam name="TOutput">Object Output (converted from JSON from transmission)</typeparam>
        /// <param name="serverUrl">Server URL without path</param>
        /// <param name="serverPath">Server Path (method path/call)</param>
        /// <param name="data">Object input to pass along to the WebApi call</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return object from WebApi call</returns>
        /// <remarks>WebApi only allows for one object each way</remarks>
        public static (TOutput, HttpResponseMessage) Call<TInput, TOutput>(string serverUrl, string serverPath, TInput data, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            var serverFullUrl = Url.Concatenate(serverUrl, serverPath);
            var (output, response) = Call<TInput, TOutput>(serverFullUrl, data, method, accessToken, useProxy, proxyServer, proxyPort, httpTimeoutSeconds);
            return (output, response);
        }


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // No Object Input, Object Output
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Used for only receiving an output (no input) from a WebApi
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="serverUrl">Server URL with optional Path and Querystring</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return object from WebApi call</returns>
        public static (TOutput, HttpResponseMessage) Call<TOutput>(string serverUrl, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            try
            {
                using (var httpClient = HttpConnection.Client(serverUrl, "application/json", useProxy, proxyServer, proxyPort, false, accessToken, httpTimeoutSeconds))
                {
                    if (method == null) { method = HttpMethod.Put; }
                    var httpRequestMessage = new HttpRequestMessage { Method = method, RequestUri = new Uri(serverUrl) };
                    var response = httpClient.SendAsync(httpRequestMessage).Result;
                    #if DEBUG
                    DebugOutput(response, serverUrl, useProxy, proxyServer, proxyPort);
                    #endif
                    return (!response.IsSuccessStatusCode 
                                ? default 
                                : Json.ObjectFromString<TOutput>(response.Content.ReadAsStringAsync().Result),
                            response);
                }
            }
            catch (Exception ex)
            {
                return (default, new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = NewlineCleanse(ex.Message) });
            }
        }

        /// <summary>
        /// Used for only receiving an output (no input) from a WebApi
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="serverUrl">Server URL without path</param>
        /// <param name="serverPath">Server Path (method path/call)</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return object from WebApi call</returns>
        public static (TOutput, HttpResponseMessage) Call<TOutput>(string serverUrl, string serverPath, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            var serverFullUrl = Url.Concatenate(serverUrl, serverPath);
            var (output, response) = Call<TOutput>(serverFullUrl, method, accessToken, useProxy, proxyServer, proxyPort, httpTimeoutSeconds);
            return (output, response);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Object Input, string Output
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Used for passing an object as an input and receiving a string output from a WebApi call - useful for debugging return calls to determine json structure
        /// </summary>
        /// <typeparam name="TInput">Object Input (converted to JSON for transmission)</typeparam>
        /// <param name="serverUrl">Server URL with optional Path and Querystring</param>
        /// <param name="data">Object input to pass along to the WebApi call</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return string (Json) from WebApi call</returns>
        public static string Call<TInput>(string serverUrl, TInput data, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            try
            {
                using (var httpClient = HttpConnection.Client(serverUrl, "application/json", useProxy, proxyServer, proxyPort, false, accessToken, httpTimeoutSeconds))
                {
                    if (method == null) { method = HttpMethod.Put; }
                    var httpRequestMessage = new HttpRequestMessage { Method = method, RequestUri = new Uri(serverUrl) };
                    if (method == HttpMethod.Post || method == HttpMethod.Put)
                    {
                        httpRequestMessage.Content = new StringContent(Json.ObjectToString(data), Encoding.UTF8, "application/json");
                    }
                    var response = httpClient.SendAsync(httpRequestMessage).Result;
                    #if DEBUG
                    DebugOutput(response, serverUrl, useProxy, proxyServer, proxyPort);
                    #endif
                    return !response.IsSuccessStatusCode 
                        ? $"API Call Failure: {response.StatusCode} - {NewlineCleanse(response.ReasonPhrase)}" 
                        : response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                return ex.Message; // Unable to make a connection
            }
        }

        /// <summary>
        /// Used for passing an object as an input and receiving a string output from a WebApi call - useful for debugging return calls to determine json structure
        /// </summary>
        /// <typeparam name="TInput">Object Input (converted to JSON for transmission)</typeparam>
        /// <param name="serverUrl">Server URL without path</param>
        /// <param name="serverPath">Server Path (method path/call)</param>
        /// <param name="data">Object input to pass along to the WebApi call</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return string (Json) from WebApi call</returns>
        public static string Call<TInput>(string serverUrl, string serverPath, TInput data, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            var serverFullUrl = Url.Concatenate(serverUrl, serverPath);
            return Call(serverFullUrl, data, method, accessToken, useProxy, proxyServer, proxyPort, httpTimeoutSeconds);
        }


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // No Object Input, string Output
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// Used for only receiving an output (no input) from a WebApi - useful for debugging return calls to determine json structure
        /// </summary>
        /// <param name="serverUrl">Server URL with optional Path and Querystring</param>
        /// <param name="method">WebApi method type GET, POST, PUT (PUT is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return string (Json) from WebApi call</returns>
        public static string Call(string serverUrl, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            try
            {
                using (var httpClient = HttpConnection.Client(serverUrl, "application/json", useProxy, proxyServer, proxyPort, false, accessToken, httpTimeoutSeconds))
                {
                    if (method == null) { method = HttpMethod.Put; }
                    var httpRequestMessage = new HttpRequestMessage { Method = method, RequestUri = new Uri(serverUrl) };
                    var response = httpClient.SendAsync(httpRequestMessage).Result;
                    #if DEBUG
                    DebugOutput(response, serverUrl, useProxy, proxyServer, proxyPort);
                    #endif
                    return !response.IsSuccessStatusCode 
                        ? $"API Call Failure: {response.StatusCode} - {NewlineCleanse(response.ReasonPhrase)}" 
                        : response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                return ex.Message; // Unable to make a connection
            }
        }

        /// <summary>
        /// Used for only receiving an output (no input) from a WebApi - useful for debugging return calls to determine json structure
        /// </summary>
        /// <param name="serverUrl">Server URL without path</param>
        /// <param name="serverPath">Server Path (method path/call)</param>
        /// <param name="method">WebApi method type GET, POST, PUT (GET is default)</param>
        /// <param name="accessToken">Access token for OAuth (Optional)</param>
        /// <param name="useProxy">Whether to use a proxy (Optional)</param>
        /// <param name="proxyServer">Proxy server URL (Optional)</param>
        /// <param name="proxyPort">Proxy server TCP port (Optional)</param>
        /// <param name="httpTimeoutSeconds">Number of seconds to wait for the HTTP response (Optional)</param>
        /// <returns>return string (Json) from WebApi call</returns>
        public static string Call(string serverUrl, string serverPath, HttpMethod method = null, string accessToken = "", bool useProxy = false, string proxyServer = "", int proxyPort = 443, int httpTimeoutSeconds = 60)
        {
            var serverFullUrl = Url.Concatenate(serverUrl, serverPath);
            if (method == null) method = HttpMethod.Get;
            return Call(serverFullUrl, method, accessToken, useProxy, proxyServer, proxyPort, httpTimeoutSeconds);
        }

        private static string NewlineCleanse(string s)
        {
            return s.Replace("\r", string.Empty).Replace("\n", " > ");
        }
        #endregion

    }
}
