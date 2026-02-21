using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions
{
    public class GeoreferenceJsonConverter : JsonConverter<Georeference>
    {
        public override void WriteJson(JsonWriter writer, Georeference? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var jObject = new JObject();
            jObject["type"] = value.Type;

            if (!string.IsNullOrEmpty(value.Crs))
                jObject["crs"] = value.Crs;

            if (value.Gcps != null && value.Gcps.Length > 0)
                jObject["gcps"] = JArray.FromObject(value.Gcps, serializer);

            if (value.Transformation != null)
                jObject["transformation"] = JObject.FromObject(value.Transformation, serializer);

            jObject.WriteTo(writer);
        }

        public override Georeference? ReadJson(JsonReader reader, Type objectType, Georeference? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = JObject.Load(reader);
            var type = jObject["type"]?.ToString();

            if (string.IsNullOrEmpty(type))
                throw new JsonSerializationException("Georeference type is required");

            var georeference = new Georeference(type);

            if (jObject["crs"] != null)
                georeference.SetCrs(jObject["crs"].ToString());

            if (jObject["gcps"] != null)
            {
                var gcps = jObject["gcps"].ToObject<GroundControlPoint[]>(serializer);
                georeference.SetGcps(gcps);
            }

            if (jObject["transformation"] != null)
            {
                var transformation = jObject["transformation"].ToObject<Transformation>(serializer);
                georeference.SetTransformation(transformation);
            }

            return georeference;
        }
    }
}