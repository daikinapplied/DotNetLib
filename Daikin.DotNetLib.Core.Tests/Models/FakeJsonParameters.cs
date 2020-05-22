using Newtonsoft.Json;

namespace Daikin.DotNetLib.Core.Tests.Models
{
    public class FakeJsonParameters
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("delay")]
        public int Delay { get; set; }
        [JsonProperty("consistent")]
        public bool Consistent { get; set; }
    }
}
