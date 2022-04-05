using IIIF.Manifests.Serializer.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IIIF.Manifests.Serializer.Shared
{
    public class FormatableItemJsonConverter<TFormatableItem> : BaseItemJsonConverter<TFormatableItem>
        where TFormatableItem : FormatableItem<TFormatableItem>
    {
        public FormatableItemJsonConverter() => DisableTypeChecking = true;

        private TFormatableItem SetFormat(JToken element, TFormatableItem formatableItem)
        {
            var jFormat = element.TryGetToken(FormatableItem<TFormatableItem>.FormatJName);
            if (jFormat != null)
                formatableItem.SetFormat(jFormat.ToString());

            return formatableItem;
        }

        protected override TFormatableItem EnrichReadJson(TFormatableItem item, JToken element, Type objectType, TFormatableItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = base.EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);

            item = SetFormat(element, item);

            return item;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, TFormatableItem value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Format))
                {
                    writer.WritePropertyName(FormatableItem<TFormatableItem>.FormatJName);
                    writer.WriteValue(value.Format);
                }
            }
        }
    }
}