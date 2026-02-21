using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// JSON converter for GeoJSON Geometry objects.
    /// Handles all geometry types: Point, MultiPoint, LineString, MultiLineString,
    /// Polygon, MultiPolygon, and GeometryCollection per the GeoJSON specification (RFC 7946).
    /// </summary>
    public class GeometryJsonConverter : JsonConverter<Geometry>
    {
        public override void WriteJson(JsonWriter writer, Geometry? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var jObject = new JObject
            {
                [Geometry.TypeJName] = value.Type
            };

            if (value.Type == "GeometryCollection")
            {
                if (value.Geometries is { Count: > 0 })
                    jObject[Geometry.GeometriesJName] = JArray.FromObject(value.Geometries, serializer);
                else
                    jObject[Geometry.GeometriesJName] = new JArray();
            }
            else if (value.Coordinates != null)
            {
                jObject[Geometry.CoordinatesJName] = value.Coordinates;
            }

            jObject.WriteTo(writer);
        }

        public override Geometry? ReadJson(JsonReader reader, Type objectType, Geometry? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jObject = JObject.Load(reader);

            var typeToken = jObject[Geometry.TypeJName];
            if (typeToken == null)
                throw new JsonSerializationException("Geometry requires 'type' property.");

            var type = typeToken.ToString();
            var geometry = new Geometry(type);

            if (type == "GeometryCollection")
            {
                var geometriesToken = jObject[Geometry.GeometriesJName];
                if (geometriesToken is JArray gArray)
                {
                    var geometries = gArray.Select(t => t.ToObject<Geometry>(serializer)!).ToArray();
                    geometry.SetGeometries(geometries);
                }
            }
            else
            {
                var coordinatesToken = jObject[Geometry.CoordinatesJName];
                if (coordinatesToken != null)
                {
                    geometry.SetCoordinates(coordinatesToken);
                }
            }

            return geometry;
        }
    }
}