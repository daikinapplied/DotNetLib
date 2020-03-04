using System.Collections.Generic;
using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Manifest
    {
        #region Properties
        // Source: https://docs.microsoft.com/en-us/microsoftteams/platform/webhooks-and-connectors/how-to/connectors-using
        [JsonProperty("$Schema")]
        public string Schema { get; set; } = "https://developer.microsoft.com/json-schemas/teams/v1.5/MicrosoftTeams.schema.json";

        [JsonProperty("manifestVersion")]
        public string ManifestVersion { get; set; } = "1.5";

        [JsonProperty("id")] 
        public string Id { get; set; } = "e9343a03-0a5e-4c1f-95a8-263a565505a5";

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("packageName")]
        public string PackageName { get; set; }

        [JsonProperty("developer")]
        public Developer Developer { get; set; }

        [JsonProperty("description")]
        public Description Description { get; set; }

        [JsonProperty("icons")]
        public Icons Icons { get; set; }

        [JsonProperty("connectors")]
        public List<Connector> Connectors { get; set; }

        [JsonProperty("name")]
        public Description Name { get; set; }

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; } = "#FFFFFF";

        [JsonProperty("needsIdentity")] 
        public bool NeedsIdentity { get; set; } = true;
        #endregion

        #region Constructors
        public Manifest()
        {
            Developer = new Developer();
            Description = new Description();
            Icons = new Icons();
            Connectors = new List<Connector>();
            Name = new Description();
        }
        #endregion
    }
}
