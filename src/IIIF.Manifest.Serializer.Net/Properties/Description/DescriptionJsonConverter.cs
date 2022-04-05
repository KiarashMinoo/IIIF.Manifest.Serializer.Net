using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IIIF.Manifests.Serializer.Properties
{
    public class DescriptionJsonConverter : ValuableItemJsonConverter<Description>
    {
        private Description SetLanguage(JToken jToken, Description description)
        {
            if (jToken is JObject jObject && jObject.TryGetValue(Description.LanguageJName, out JToken jLanguage) && jLanguage != null)
                description.SetLanguage(jLanguage.ToString());

            return description;
        }

        protected override Description CreateInstance(JToken element, Type objectType, Description existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (element is JObject)
                return new Description(element.TryGetToken(Description.ValueJName).ToString());

            return base.CreateInstance(element, objectType, existingValue, hasExistingValue, serializer);
        }

        protected override Description EnrichReadJson(Description item, JToken element, Type objectType, Description existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = base.EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);

            item = SetLanguage(element, item);

            return item;
        }

        protected override void EnrichWriteJson(JsonWriter writer, Description description, JsonSerializer serializer)
        {
            if (description != null)
            {
                if (!string.IsNullOrEmpty(description.Value) && !string.IsNullOrEmpty(description.Language))
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName(Description.ValueJName);
                    writer.WriteValue(description.Value);

                    writer.WritePropertyName(Description.LanguageJName);
                    writer.WriteValue(description.Language);

                    writer.WriteEndObject();
                }
                else
                    base.EnrichWriteJson(writer, description, serializer);
            }
        }
    }
}