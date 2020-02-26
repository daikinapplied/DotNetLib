namespace Daikin.DotNetLib.Network
{
    public static class JsonError
    {
        public static string Build(int code, string message)
        {
            return "{\"Error\" : {\"Code\": " + code + ",\"Message\": \"" + message + "\"}";
        }
    }
}
