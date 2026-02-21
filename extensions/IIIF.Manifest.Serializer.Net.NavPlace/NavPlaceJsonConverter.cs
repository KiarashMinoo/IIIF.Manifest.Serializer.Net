using System;
using IIIF.Manifests.Serializer.NavPlace;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.NavPlace
{
    public class NavPlaceJsonConverter : JsonConverter<NavPlace>
    {
        public override void WriteJson(JsonWriter writer, NavPlace value, JsonSerializer serializer)
        {
            if (value?.Features == null)
            {
                writer.WriteNull();
                return;
            }

            // Write the FeatureCollection directly
            serializer.Serialize(writer, value.Features);
        }

        public override NavPlace ReadJson(JsonReader reader, Type objectType, NavPlace existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = JObject.Load(reader);
            var features = jObject.ToObject<FeatureCollection>(serializer);
            return new NavPlace(features);
        }
    }
}