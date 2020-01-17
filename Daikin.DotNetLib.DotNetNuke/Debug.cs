using System.Web;

namespace Daikin.DotNetLib.DotNetNuke
{
    public static class Debug
    {
        public static bool IsDebugMode(HttpRequest request, string getVar = "dotnetlibdebug")
        {
            var requestVar = request?[getVar];
            if (string.IsNullOrEmpty(requestVar)) return false;
            return (requestVar.ToLower() == "d");
        }
    }
}
