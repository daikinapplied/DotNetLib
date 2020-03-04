using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Developer
    {
        #region Properties
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("websiteUrl")]
        public string WebsiteUrl { get; set; }

        [JsonProperty("privateUrl")]
        public string PrivacyUrl { get; set; }

        [JsonProperty("termsOfUseUrl")]
        public string TermsOfServiceUrl { get; set; }
        #endregion
    }
}
