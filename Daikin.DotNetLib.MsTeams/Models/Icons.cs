using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Icons
    {
        #region Properties
        [JsonProperty("outline")]
        public string OutlineFilename { get; set; }

        [JsonProperty("color")]
        public string ColorFilename { get; set; }
        #endregion
    }
}
