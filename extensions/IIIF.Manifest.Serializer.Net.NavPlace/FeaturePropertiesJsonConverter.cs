using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// JSON converter for GeoJSON Feature properties.
    /// Reads/writes the properties object which may contain label and other custom properties.
    /// </summary>
    public class FeaturePropertiesJsonConverter : TrackableObjectJsonConverter<FeatureProperties>
    {
        protected override FeatureProperties CreateInstance(JToken element, Type objectType, FeatureProperties? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new FeatureProperties();
        }

        protected override FeatureProperties EnrichReadJson(FeatureProperties item, JToken element, Type objectType, FeatureProperties? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = base.EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);

            var jLabel = element.TryGetToken(FeatureProperties.LabelJName);
            if (jLabel is JArray)
            {
                var labels = jLabel.ToObject<Label[]>();
                if (labels is not null)
                {
                    foreach (var label in labels)
                        item.AddLabel(label);
                }
            }

            var summaryToken = element.TryGetToken(FeatureProperties.SummaryJName);
            if (summaryToken != null)
                item.SetSummary(summaryToken.ToString());

            return item;
        }

        protected override void EnrichWriteJson(JsonWriter writer, FeatureProperties value, JsonSerializer serializer)
        {
            base.EnrichWriteJson(writer, value, serializer);

            if (value.Label.Count > 0)
            {
                writer.WritePropertyName(FeatureProperties.LabelJName);

                writer.WriteStartArray();

                foreach (var label in value.Label)
                {
                    writer.WriteValue(label);
                }

                writer.WriteEndArray();
            }

            if (!string.IsNullOrWhiteSpace(value.Summary))
            {
                writer.WritePropertyName(FeatureProperties.SummaryJName);
                writer.WriteValue(value.Summary);
            }
        }
    }
}