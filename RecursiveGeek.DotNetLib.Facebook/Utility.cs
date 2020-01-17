using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace RecursiveGeek.DotNetLib.Facebook
{
    public static class Utility
    {
        #region Constants
        public const string FacebookUrl = "https://graph.facebook.com";
        public const string FacebookVersion = "v3.3";
        #endregion

        #region Functions
        public static string GetAppSecretProof(string userToken, string appSecret)
        {
            var encoding = new ASCIIEncoding();
            var appSecretBytes = encoding.GetBytes(appSecret);
            var tokenBytes = encoding.GetBytes(userToken);
            var hmac = new HMACSHA256(appSecretBytes);
            var appSecretProof = hmac.ComputeHash(tokenBytes);
            return ByteToString(appSecretProof).ToLower();
        }

        public static string ByteToString(byte[] buff)
        {
            var sbinary = buff.Aggregate(string.Empty, (current, b) => current + b.ToString("X2"));
            return sbinary;
        }

        public static string BuildUrl(bool requireStartSlash, bool requireEndSlash, params string[] pathStrings)
        {
            const string slash = "/";
            var builder = string.Empty;

            foreach (var current in pathStrings)
            {
                var isSlashEnd = builder.Length == 0 || builder.EndsWith(slash);
                var isSlashStart = current.StartsWith(slash);
                if (isSlashEnd && isSlashStart) { builder += current.Substring(1); }
                else if (!isSlashEnd && !isSlashStart) { builder += slash + current; }
                else { builder += current; }
            }

            if (requireStartSlash && !builder.ToLower().Contains("http") && !builder.StartsWith(slash)) { builder = slash + builder; }
            if (requireEndSlash && builder.Length > 0 && !builder.EndsWith(slash) && !builder.Contains("?")) { builder += slash; }
            return builder;
        }

        public static string BuildUrl(params string[] pathStrings)
        {
            return BuildUrl(true, true, pathStrings);
        }

        public static (HttpClient, string) BuildClient(string pageNameOrId, string userToken, string appSecret, string edge, string facebookUrl, string facebookVersion, string fieldList)
        {
            var hostUrl = new Uri(facebookUrl);
            var client = new HttpClient { BaseAddress = hostUrl };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Host = hostUrl.Host;
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            var appSecretProof = GetAppSecretProof(userToken, appSecret);
            var endPoint = BuildUrl(facebookVersion, pageNameOrId, edge);
            endPoint = AddQuery(endPoint, "fields", fieldList);
            endPoint = AddQuery(endPoint, "appsecret_proof", appSecretProof);
            return (client, endPoint);
        }

        private static string AddQuery(string url, string queryVar, string queryVal)
        {
            var newUrl = url.Trim();
            if (!string.IsNullOrEmpty(queryVar) && !string.IsNullOrEmpty(queryVal))
            {
                if (!newUrl.Contains("?")) newUrl += "?";
                if (!newUrl.EndsWith("?")) newUrl += "&";
                newUrl += $"{queryVar}={System.Net.WebUtility.UrlEncode(queryVal)}";
            }
            return newUrl;
        }
        #endregion
    }
}
