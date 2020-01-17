using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace RecursiveGeek.DotNetLib.DotNetNuke
{
    public static class Web
    {
        #region Constants
        public const string DefaultDelimiter = "|";
        #endregion

        #region Functions
        public static string RemoveHtml(string s)
        {
            return Regex.Replace(s, "<.*?>", string.Empty);
        }

        public static string GetRemoteHost(HttpRequest request)
        {
            try
            {
                //var userHost = System.Net.Dns.GetHostEntry(Request.UserHostAddress ?? string.Empty).HostName
                var userHost = request.UserHostName ?? (request.UserHostAddress ?? string.Empty);
                return userHost;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string GetPageRef(HttpRequest request, HttpContext context)
        {
            var url = string.Empty;
            if (context != null) url = context.Items["UrlRewrite:OriginalUrl"].ToString();
            if (string.IsNullOrEmpty(url)) url = request.Url.AbsoluteUri;
            var filename = Path.GetFileNameWithoutExtension(url);
            if (filename.Contains("?")) filename = filename.Substring(0, filename.IndexOf('?'));
            return filename;
        }

        public static object ClearApplicationCache(string key, int portalId, int moduleId, string delimiter = DefaultDelimiter)
        {
            return HttpContext.Current.Cache.Remove(CleanseDelimiter(key) + delimiter + portalId + delimiter + moduleId);
        }

        public static void SaveApplicationCache(string key, object val, int portalId, int moduleId, int hours = 24, string delimiter = DefaultDelimiter)
        {
            HttpContext.Current.Cache.Add(CleanseDelimiter(key) + delimiter + portalId + delimiter + moduleId, val, null, DateTime.Now.AddHours(hours), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        public static object GetApplicationCache(string key, int portalId, int moduleId, string delimiter = DefaultDelimiter)
        {
            return HttpContext.Current.Cache.Get(CleanseDelimiter(key) + delimiter + portalId + delimiter + moduleId);
        }

        private static string CleanseDelimiter(string item, string delimiter = DefaultDelimiter)
        {
            return item.Replace(delimiter, string.Empty);
        }
        #endregion
    }
}
