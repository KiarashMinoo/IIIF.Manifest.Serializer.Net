using System;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Metadata.MetadataValue
{
    public class MetadataValueJsonConverter : TrackableObjectJsonConverter<MetadataValue>
    {
        protected override MetadataValue CreateInstance(JToken element, Type objectType, MetadataValue existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (element is JObject)
            {
                var jValue = element[MetadataValue.ValueJName];
                if (jValue is null)
                    throw new JsonNodeRequiredException<MetadataValue>(MetadataValue.ValueJName);

                var jLanguage = element[MetadataValue.LanguageJName];
                if (jLanguage != null)
                    return new MetadataValue(jValue.ToString(), jLanguage.ToString());
                else
                    return new MetadataValue(jValue.ToString());
            }

            return new MetadataValue(element.ToString());
        }

        protected sealed override void EnrichWriteJson(JsonWriter writer, MetadataValue value, JsonSerializer serializer)
        {
            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Language) && !string.IsNullOrEmpty(value.Value))
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName(MetadataValue.LanguageJName);
                    writer.WriteValue(value.Language);

                    writer.WritePropertyName(MetadataValue.ValueJName);
                    writer.WriteValue(value.Value);

                    writer.WriteEndObject();
                }
                else if (!string.IsNullOrEmpty(value.Value))
                {
                    writer.WriteValue(value.Value);
                }
            }
        }
    }
}