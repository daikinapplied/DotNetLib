using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Description
    {
        #region Properties
        [JsonProperty("full")]
        public string Full { get; set; }

        [JsonProperty("short")]
        public string Short { get; set; }
        #endregion
    }
}
