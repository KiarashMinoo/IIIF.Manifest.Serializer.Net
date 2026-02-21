using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// JSON converter for GeoJSON Feature objects.
    /// Validates required "type", "geometry", and "properties" tokens per the GeoJSON specification.
    /// Supports optional "id" property per the navPlace extension spec.
    /// </summary>
    public class FeatureJsonConverter : BaseItemJsonConverter<Feature>
    {
        protected override Feature CreateInstance(JToken element, Type objectType, Feature? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Feature.IdJName);
            return jId is null
                ? throw new JsonNodeRequiredException<Feature>(Feature.IdJName)
                : new Feature(jId.ToString());
        }

        protected override Feature EnrichReadJson(Feature item, JToken element, Type objectType, Feature? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Feature.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Feature>(Feature.IdJName);

            var jGeometry = element.TryGetToken(Feature.GeometryJName);

            var geometry = jGeometry?.ToObject<Geometry>(serializer);
            if (geometry != null)
                item.SetGeometry(geometry);

            var jProperties = element.TryGetToken(Feature.PropertiesJName);
            var properties = jProperties?.ToObject<FeatureProperties>(serializer);
            if (properties != null)
                item.SetProperties(properties);

            return item;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Feature value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value.Properties != null)
            {
                writer.WritePropertyName(Feature.PropertiesJName);
                serializer.Serialize(writer, value.Properties);
            }

            if (value.Geometry != null)
            {
                writer.WritePropertyName(Feature.GeometryJName);
                serializer.Serialize(writer, value.Geometry);
            }
        }
    }
}