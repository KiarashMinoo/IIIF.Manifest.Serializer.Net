using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// JSON converter for GeoJSON Point coordinates.
    /// A position is an array of numbers: [longitude, latitude] or [longitude, latitude, altitude].
    /// </summary>
    public class PointJsonConverter : JsonConverter<Point>
    {
        public override void WriteJson(JsonWriter writer, Point? value, JsonSerializer serializer)
        {
            if (value?.Coordinates == null)
            {
                writer.WriteNull();
                return;
            }

            var array = new JArray();
            foreach (var coord in value.Coordinates)
            {
                array.Add(coord);
            }

            array.WriteTo(writer);
        }

        public override Point? ReadJson(JsonReader reader, Type objectType, Point? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var token = JToken.Load(reader);
            if (token is not JArray jArray || jArray.Count < 2)
                throw new JsonSerializationException("Point coordinates must be an array with at least 2 elements [longitude, latitude].");

            var longitude = jArray[0].Value<double>();
            var latitude = jArray[1].Value<double>();

            if (jArray.Count >= 3)
            {
                var altitude = jArray[2].Value<double>();
                return new Point(longitude, latitude, altitude);
            }

            return new Point(longitude, latitude);
        }
    }
}
