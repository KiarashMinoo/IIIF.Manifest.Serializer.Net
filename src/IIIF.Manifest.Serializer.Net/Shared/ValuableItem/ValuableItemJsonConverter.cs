using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IIIF.Manifests.Serializer.Shared
{
    public class ValuableItemJsonConverter<TValuableItem> : TrackableObjectJsonConverter<TValuableItem>
        where TValuableItem : ValuableItem<TValuableItem>
    {
        protected override TValuableItem CreateInstance(JToken element, Type objectType, TValuableItem existingValue, bool hasExistingValue, JsonSerializer serializer)
            => (TValuableItem)Activator.CreateInstance(typeof(TValuableItem), element.ToString());

        protected override void EnrichWriteJson(JsonWriter writer, TValuableItem value, JsonSerializer serializer)
        {
            if (value != null && !string.IsNullOrEmpty(value.Value))
                writer.WriteValue(value.Value);
        }
    }
}