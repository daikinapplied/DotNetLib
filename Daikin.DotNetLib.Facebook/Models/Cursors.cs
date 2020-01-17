using Newtonsoft.Json;

namespace Daikin.DotNetLib.Facebook.Models
{
    public class Cursors
    {
        #region Properties
        [JsonProperty("before")]
        public string Before { get; set; }

        [JsonProperty("after")]
        public string After { get; set; }
        #endregion
    }
}
