using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions
{
    public class NavPlaceJsonConverter : BaseItemJsonConverter<NavPlace>
    {
        protected override NavPlace CreateInstance(JToken element, Type objectType, NavPlace? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Feature.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Feature>(Feature.IdJName);

            return new NavPlace(jId.ToString());
        }

        protected override NavPlace EnrichReadJson(NavPlace item, JToken element, Type objectType, NavPlace? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jFeatures = element.TryGetToken(NavPlace.FeaturesJName);

            if (jFeatures is JArray)
            {
                var features = jFeatures.ToObject<Feature[]>();
                if (features != null)
                    item.SetFeatures(features);
            }

            return item;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, NavPlace value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            var features = value.Features;
            if (features.Count > 0)
            {
                writer.WritePropertyName(NavPlace.FeaturesJName);

                writer.WriteStartArray();

                foreach (var feature in features)
                    serializer.Serialize(writer, feature);

                writer.WriteEndArray();
            }
        }
    }
}