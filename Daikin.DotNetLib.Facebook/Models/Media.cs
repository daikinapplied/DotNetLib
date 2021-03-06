﻿using Newtonsoft.Json;

namespace Daikin.DotNetLib.Facebook.Models
{
    public class Media
    {
        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; } // e.g., video content URL
    }
}
