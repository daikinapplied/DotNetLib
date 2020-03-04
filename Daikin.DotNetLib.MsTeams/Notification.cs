using System.Net.Http;
using Daikin.DotNetLib.MsTeams.Models;

namespace Daikin.DotNetLib.MsTeams
{
    public static class Notification
    {
        public static (bool, string) Send(string webHookUrl, WebHookRequest request)
        {
            try
            {
                var result = Network.WebApi.Call(webHookUrl, request, HttpMethod.Post);
                return (true, result);
            }
            catch
            {
                return (false, string.Empty);
            }
        }
    }
}
