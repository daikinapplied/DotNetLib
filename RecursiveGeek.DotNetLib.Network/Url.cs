using System;

namespace RecursiveGeek.DotNetLib.Network
{
    public static class Url
    {
        #region Functions
        public static string Concatenate(string serverBaseUrl, string path)
        {
            return (serverBaseUrl + (!serverBaseUrl.EndsWith("/") && !path.StartsWith("/") ? "/" : string.Empty) + path);
        }

        public static string Concatenate(string serverBaseUrl, string path, string page)
        {
            var url = Concatenate(serverBaseUrl, path);
            return (url + (!url.EndsWith("/") ? "/" : string.Empty) + page);
        }

        public static string Concatenate(string serverBaseUrl, string path, string page, string queryString)
        {
            return Concatenate(serverBaseUrl, path, page) + (!queryString.StartsWith("?") ? "?" : string.Empty) + queryString;
        }

        public static (string, string, string) SplitUrl(string serverFullUrl)
        {
            var serverUri = new Uri(serverFullUrl);
            var serverUrl = serverUri.Scheme + "://" + serverUri.Host;
            var serverPath = serverUri.AbsolutePath;
            var query = serverUri.Query.Substring(1); // eliminate ?
            return (serverUrl, serverPath, query);
        }
        #endregion
    }
}
