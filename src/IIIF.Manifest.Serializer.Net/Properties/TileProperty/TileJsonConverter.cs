using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.TileProperty
{
    public class TileJsonConverter : TrackableObjectJsonConverter<Tile>
    {
        private Tile SetWidth(JToken element, Tile tile)
        {
            var jWidth = element.TryGetToken(Tile.WidthJName);

            if (jWidth != null)
                tile.SetWidth(jWidth.Value<int>());

            return tile;
        }

        private Tile SetScaleFactor(JToken element, Tile tile)
        {
            var jScaleFactor = element.TryGetToken(Tile.ScaleFactorsJName);

            if (jScaleFactor != null && jScaleFactor is JArray)
            {
                var scaleFactors = jScaleFactor.ToObject<int[]>();
                foreach (var scaleFactor in scaleFactors)
                    tile.AddScaleFactor(scaleFactor);
            }

            return tile;
        }

        protected override Tile CreateInstance(JToken element, Type objectType, Tile existingValue, bool hasExistingValue, JsonSerializer serializer) => new Tile();

        protected override Tile EnrichReadJson(Tile tile, JToken element, Type objectType, Tile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            tile = SetWidth(element, tile);
            tile = SetScaleFactor(element, tile);
            return tile;
        }

        protected sealed override void EnrichWriteJson(JsonWriter writer, Tile tile, JsonSerializer serializer)
        {
            if (tile != null)
            {
                writer.WriteStartObject();

                if (tile.Width != null)
                {
                    writer.WritePropertyName(Tile.WidthJName);
                    writer.WriteValue(tile.Width.Value);
                }

                if (tile.ScaleFactors.Any())
                {
                    writer.WritePropertyName(Tile.ScaleFactorsJName);

                    writer.WriteStartArray();

                    foreach (var scaleFactor in tile.ScaleFactors)
                        writer.WriteValue(scaleFactor);

                    writer.WriteEndArray();
                }

                writer.WriteEndObject();
            }
        }
    }
}