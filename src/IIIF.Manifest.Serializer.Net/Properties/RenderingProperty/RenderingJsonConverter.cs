using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Rendering
{
    public class RenderingJsonConverter : FormatableItemJsonConverter<Rendering>
    {
        public RenderingJsonConverter()
        {
            DisableTypeChecking = true;
        }

        protected override Rendering CreateInstance(JToken element, Type objectType, Rendering existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!(element is JObject))
                throw new JsonObjectMustBeJObject<Rendering>(nameof(Rendering));

            var jId = element.TryGetToken(Rendering.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Rendering>(Rendering.IdJName);

            var jLabel = element.TryGetToken(Rendering.LabelJName);
            if (jLabel is null)
                throw new JsonNodeRequiredException<Rendering>(Rendering.LabelJName);

            return new Rendering(jId.ToString(), jLabel.ToString());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Rendering value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Label))
                {
                    writer.WritePropertyName(Rendering.LabelJName);
                    writer.WriteValue(value.Label);
                }
            }
        }
    }
}