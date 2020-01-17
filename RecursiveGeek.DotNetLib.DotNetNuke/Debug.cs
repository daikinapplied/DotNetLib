using System.Web;

namespace RecursiveGeek.DotNetLib.DotNetNuke
{
    public static class Debug
    {
        public static bool IsDebugMode(HttpRequest request, string getVar = "recursivegeek")
        {
            var requestVar = request?[getVar];
            if (string.IsNullOrEmpty(requestVar)) return false;
            return (requestVar.ToLower() == "d");
        }
    }
}
