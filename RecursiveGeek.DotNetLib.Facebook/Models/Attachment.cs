using Newtonsoft.Json;
using System.Collections.Generic;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class Attachment
    {
        #region Properties
        [JsonProperty("data")]
        public List<DataAttachment> Data { get; set; }
        #endregion
    }
}
