using Newtonsoft.Json;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class Paging
    {
        #region Properties
        [JsonProperty("cursors")]
        public Cursors Cursors { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
        #endregion
    }
}
