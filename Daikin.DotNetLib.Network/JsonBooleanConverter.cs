using System;
using Newtonsoft.Json;

namespace Daikin.DotNetLib.Network
{
    public class JsonBooleanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var data = reader.Value.ToString().ToLower().Trim();

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (data == "true" || data == "yes" || data == "y" || data == "1") { return true; }
            if (data == "false" || data == "no" || data == "n" || data == "0") { return false; }

            // If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-fied error message.
            return new JsonSerializer().Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
