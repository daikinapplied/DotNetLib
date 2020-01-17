namespace RecursiveGeek.DotNetLib.Network
{
    public static class ReverseDns
    {
        public static string Get(string ip)
        {
            string host;

            try
            {
                var ipHostEntry = System.Net.Dns.GetHostEntry(ip);
                host = ipHostEntry.HostName;
            }
            catch
            {
                host = string.Empty;
            }
            return host;
        }
    }
}
