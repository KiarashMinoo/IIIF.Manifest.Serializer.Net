using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties
{
    public class HomepageJsonConverter : FormatableItemJsonConverter<Homepage>
    {
        public HomepageJsonConverter()
        {
            DisableTypeChecking = true;
        }

        protected override Homepage CreateInstance(JToken element, Type objectType, Homepage existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!(element is JObject))
                throw new JsonObjectMustBeJObject<Homepage>(nameof(Homepage));

            var jId = element.TryGetToken(Homepage.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Homepage>(Homepage.IdJName);

            var jLabel = element.TryGetToken(Homepage.LabelJName);

            return new Homepage(jId.ToString(), jLabel?.ToString());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Homepage value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Label))
                {
                    writer.WritePropertyName(Homepage.LabelJName);
                    writer.WriteValue(value.Label);
                }
            }
        }
    }
}