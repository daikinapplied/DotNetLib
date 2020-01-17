using System.Collections.Generic;
using Newtonsoft.Json;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class Feed
    {
        #region Enumerators
        public enum EdgeType
        {
            Feed,
            Posts,
            Tagged
        }

        #endregion

        #region Properties
        [JsonProperty("data")]
        public List<DataFeed> Data { set; get; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        public EdgeType Edge { get; set; }
        #endregion

        #region Methods
        public bool HasData()
        {
            return Data != null && Data.Count > 0;
        }
        #endregion

        #region Functions
        public static Feed FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<Feed>(jsonString);
        }
        #endregion
    }
}
