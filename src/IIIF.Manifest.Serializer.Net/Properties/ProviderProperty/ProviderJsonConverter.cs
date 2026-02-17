using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.ProviderProperty
{
    public class ProviderJsonConverter : FormatableItemJsonConverter<Provider>
    {
        public ProviderJsonConverter()
        {
            DisableTypeChecking = true;
        }

        protected override Provider CreateInstance(JToken element, Type objectType, Provider existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!(element is JObject))
                throw new JsonObjectMustBeJObject<Provider>(nameof(Provider));

            var jId = element.TryGetToken(Provider.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Provider>(Provider.IdJName);

            var jLabel = element.TryGetToken(Provider.LabelJName);

            return new Provider(jId.ToString(), jLabel?.ToString());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Provider value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Label))
                {
                    writer.WritePropertyName(Provider.LabelJName);
                    writer.WriteValue(value.Label);
                }
            }
        }
    }
}