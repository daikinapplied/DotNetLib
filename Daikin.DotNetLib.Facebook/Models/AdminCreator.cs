using Newtonsoft.Json;

namespace Daikin.DotNetLib.Facebook.Models
{
    public class AdminCreator
    {
        [JsonProperty("name")]
        public string Name { get; set;}

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
