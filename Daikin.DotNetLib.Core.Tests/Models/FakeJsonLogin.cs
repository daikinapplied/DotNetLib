using Newtonsoft.Json;

namespace Daikin.DotNetLib.Core.Tests.Models
{
    public class FakeJsonLogin
    {
        [JsonProperty("date_time")]
        public string DateTime { get; set; } = "dateTime|UNIX";

        [JsonProperty("ip4")]
        public string Ip4 { get; set; } = "internetIP4";
    }
}
