using Newtonsoft.Json;

namespace Daikin.DotNetLib.Facebook.Models
{
    public class From
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
