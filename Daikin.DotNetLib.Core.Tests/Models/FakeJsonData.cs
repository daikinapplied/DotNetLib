using Newtonsoft.Json;

namespace Daikin.DotNetLib.Core.Tests.Models
{
    public class FakeJsonData
    {
        [JsonProperty("id")]
        public string Id { get; set; } = "personNickName";

        [JsonProperty("email")]
        public string Email { get; set; } = "internetEmail";

        [JsonProperty("gender")]
        public string Gender { get; set; } = "personGender";

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = "nameFirst";

        [JsonProperty("last_name")]
        public string LastName { get; set; } = "nameLast";

        [JsonProperty("last_login")]
        public FakeJsonLogin LastLogin { get; set; } = new FakeJsonLogin();

    }
}
