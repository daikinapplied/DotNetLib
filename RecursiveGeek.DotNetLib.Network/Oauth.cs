using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace RecursiveGeek.DotNetLib.Network
{
    public static class Oauth
    {
        #region Functions
        /// <summary>
        /// Used to encode a challenge string, which may be passed along as part of authorizing in OAuth, and verified once control returns from the OAuth server
        /// </summary>
        /// <param name="codeVerifier">Recommend a randomly generated 64 character string</param>
        /// <returns>SHA256 encoded URL-safe string</returns>
        public static string GetCodeChallenge(string codeVerifier)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
            var transformedCodeVerifier = Base64Url.Encode(hashedBytes);
            return transformedCodeVerifier;
        }

        /// <summary>
        /// Get Authentication Header
        /// </summary>
        /// <param name="key">Basic Key</param>
        /// <returns>Header with Authorization basic key</returns>
        public static HttpRequestHeaders GetAuthenticationHeader(string key)
        {
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", "Basic " + key);
            return header;
        }

        /// <summary>
        /// Get the Authentication Header for a client secret for OAuth2
        /// </summary>
        /// <param name="clientId">OAuth2 Client ID</param>
        /// <param name="secret">OAuth2 Client Secret</param>
        /// <returns>OAuth2 Request Header</returns>
        public static HttpRequestHeaders GetClientSecretAuthenticationHeader(string clientId, string secret)
        {
            var encodeKey = $"{clientId}:{secret}";
            var plainTextBytes = Encoding.UTF8.GetBytes(encodeKey);
            var base64Bytes = Convert.ToBase64String(plainTextBytes);
            return GetAuthenticationHeader(base64Bytes);
        }

        /// <summary>
        /// Call the OAuth2 Server and get a response
        /// </summary>
        /// <param name="method">WebApi Method Type</param>
        /// <param name="authorizationServerUrl">Authorization Server Url</param>
        /// <param name="clientUrl">Client Url (caller)</param>
        /// <param name="content">Content (Data)</param>
        /// <param name="headers">Headers to pass to the Authorization Server</param>
        /// <returns>Response from WebApi call</returns>
        public static HttpResponseMessage GetApiResponseCall(HttpMethod method, string authorizationServerUrl, string clientUrl, StringContent content, HttpRequestHeaders headers)
        {
            var httpClient = HttpConnection.Client(authorizationServerUrl);
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(authorizationServerUrl)
            };

            foreach (var header in headers)
            {
                httpRequestMessage.Headers.Add(header.Key, header.Value);
            }

            if (method == HttpMethod.Put || method == HttpMethod.Post)
            {
                httpRequestMessage.Content = content;
            }

            return httpClient.SendAsync(httpRequestMessage).Result;
        }

        /// <summary>
        /// Get Access Token from OAuth2 Authorization Server
        /// </summary>
        /// <param name="authorizationServerUrl">OAuth2 server</param>
        /// <param name="clientUrl">Client Url (Caller)</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <returns>Access token and expiration (if successful, otherwise empty string for access token)</returns>
        public static (string, DateTime) GetAccessToken(string authorizationServerUrl, string clientUrl, string clientId, string clientSecret)
        {
            var secret = new NetworkCredential(string.Empty, clientSecret).Password;
            var headers = GetClientSecretAuthenticationHeader(clientId, secret);
            var content = $"grant_type=client_credentials&client_id={clientId}";
            var stringContent = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = GetApiResponseCall(HttpMethod.Post, authorizationServerUrl, clientUrl, stringContent, headers);
            if (!response.IsSuccessStatusCode) return (string.Empty, DateTime.Now);
            var data = response.Content.ReadAsStringAsync().Result;
            var accessToken = Json.Parse(data)["access_token"].ToString();
            var expiresIn = DateTime.Now.AddSeconds((int)Json.Parse(data)["expires_in"] - 10);
            return (accessToken, expiresIn);
        }
        #endregion
    }
}
