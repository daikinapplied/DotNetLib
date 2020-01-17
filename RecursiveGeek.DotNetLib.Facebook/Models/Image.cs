using Newtonsoft.Json;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class Image
    {
        #region Enumerators
        public enum MediaStyle
        {
            Unknown,
            Image,
            Video
        }
        #endregion

        #region Properties
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("src")]
        public string Source { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("tag")]
        public object Tag { get; set; } // Not used by Facebook

        public MediaStyle MediaType { get; set; } = MediaStyle.Unknown;

        public string Url { get; set; }
        #endregion
    }
}
