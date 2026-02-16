using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IIIF.Manifests.Serializer.Properties.Size
{
    public class SizeJsonConverter : TrackableObjectJsonConverter<Size>
    {
        protected override Size CreateInstance(JToken element, Type objectType, Size existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jWidth = element.TryGetToken(Size.WidthJName);
            var jHeight = element.TryGetToken(Size.HeightJName);
            var width = jWidth?.Value<int>() ?? 0;
            var height = jHeight?.Value<int>() ?? 0;
            return new Size(width, height);
        }
        protected override Size EnrichReadJson(Size size, JToken element, Type objectType, Size existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return size;
        }
        protected override void EnrichWriteJson(JsonWriter writer, Size size, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(Size.WidthJName);
            writer.WriteValue(size.Width);
            writer.WritePropertyName(Size.HeightJName);
            writer.WriteValue(size.Height);
            writer.WriteEndObject();
        }
    }
}
