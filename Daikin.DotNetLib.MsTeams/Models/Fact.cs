using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Fact
    {
        #region Properties
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
        #endregion
    }
}
