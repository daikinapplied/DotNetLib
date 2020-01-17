using Newtonsoft.Json;
using System.Collections.Generic;

namespace Daikin.DotNetLib.Facebook.Models
{
    public class Attachment
    {
        #region Properties
        [JsonProperty("data")]
        public List<DataAttachment> Data { get; set; }
        #endregion
    }
}
