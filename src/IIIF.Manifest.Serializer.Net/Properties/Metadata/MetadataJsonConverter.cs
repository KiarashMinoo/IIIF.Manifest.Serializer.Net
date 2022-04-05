using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace IIIF.Manifests.Serializer.Properties
{
    public class MetadataJsonConverter : TrackableObjectJsonConverter<Metadata>
    {
        protected override Metadata CreateInstance(JToken element, Type objectType, Metadata existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jLabel = element[Metadata.LabelJName];
            if (jLabel is null)
                throw new JsonNodeRequiredException<Metadata>(Metadata.LabelJName);

            var jValue = element[Metadata.ValueJName];
            if (jValue is null)
                throw new JsonNodeRequiredException<Metadata>(Metadata.ValueJName);

            if (jValue is JArray)
            {
                var metadataValues = jValue.ToObject<MetadataValue[]>();
                var first = metadataValues[0];
                var metadata = new Metadata(jLabel.ToString(), first);

                foreach (var metadataValue in metadataValues.Except(new[] { first }))
                    metadata.AddValue(metadataValue);

                return metadata;
            }
            else
                return new Metadata(jLabel.ToString(), jValue.ToString());
        }

        protected sealed override void EnrichWriteJson(JsonWriter writer, Metadata metadata, JsonSerializer serializer)
        {
            if (metadata != null)
            {
                writer.WriteStartObject();

                if (!string.IsNullOrEmpty(metadata.Label))
                {
                    writer.WritePropertyName(Metadata.LabelJName);
                    writer.WriteValue(metadata.Label);
                }

                if (metadata.Value.Any())
                {
                    writer.WritePropertyName(Metadata.ValueJName);

                    if (metadata.Value.Count == 1)
                        serializer.Serialize(writer, metadata.Value.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var value in metadata.Value)
                            serializer.Serialize(writer, value);

                        writer.WriteEndArray();
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}