using Newtonsoft.Json;
using RecursiveGeek.DotNetLib.Network;

namespace RecursiveGeek.DotNetLib.Core.Tests.Models.JsonPlaceHolder
{
    // Used for https://jsonplaceholder.typicode.com/
    public class Post
    {
        #region Properties
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
        #endregion
    }
}
