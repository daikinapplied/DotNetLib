using System.Collections.Generic;
using Newtonsoft.Json;

namespace Daikin.DotNetLib.MsTeams.Models
{
    public class Connector
    {
        #region Properties
        [JsonProperty("connectorId")]
        public string ConnectorId { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }
        #endregion

        #region Constructors
        public Connector()
        {
            Scopes = new List<string>();
        }
        #endregion
    }
}
