using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RecursiveGeek.DotNetLib.Network
{
    public class JsonNullToEmptyStringResolver : DefaultContractResolver
    {
        #region Methods
        // This doesn't work with nested complex data types
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                .Select(p => {
                    var jp = base.CreateProperty(p, memberSerialization);
                    jp.ValueProvider = new JsonNullToEmptyStringValueProvider(p);
                    return jp;
                }).ToList();
        }
        #endregion
    }
}
