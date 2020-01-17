using Newtonsoft.Json;

namespace Daikin.DotNetLib.Core.Tests.Models.JsonPlaceHolder
{
    public class Comment
    {
        #region Properties

        [JsonProperty("postid")]
        public int PostId { get; set; }
        
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
        #endregion
    }
}
