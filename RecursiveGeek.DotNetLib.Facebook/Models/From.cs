using Newtonsoft.Json;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class From
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
