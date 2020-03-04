using System.Collections.Generic;
using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class WebHookRequest
    {
        #region Properties
        [JsonProperty("@type")]
        public string Type { get; set; } = "MessageCard";

        [JsonProperty("@context")]
        public string Context { get; set; } = "https://schema.org/extensions";

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("themeColor")]
        public string ThemeColor { get; set; }

        [JsonProperty("sections")]
        public List<Section> Sections { get; set; }
        #endregion

        #region Constructors
        public WebHookRequest()
        {
            Sections = new List<Section>();
        }
        #endregion
    }
}
