using Newtonsoft.Json;

namespace Daikin.DotNetLib.Core.Tests.Models
{
    // https://fakejson.com/documentation#request_structure
    public class FakeJsonRequest
    {
        #region Properties
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("parameters")]
        public FakeJsonParameters Parameters { get; set; } = new FakeJsonParameters();

        [JsonProperty("data")]
        public FakeJsonData Data { get; set; } = new FakeJsonData();
        #endregion
    }
}
