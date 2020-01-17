using Newtonsoft.Json;
using System.Collections.Generic;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class DataAttachment
    {
        #region Properties
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("subattachments")]
        public Attachment Subattachments { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
        #endregion
    }
}
