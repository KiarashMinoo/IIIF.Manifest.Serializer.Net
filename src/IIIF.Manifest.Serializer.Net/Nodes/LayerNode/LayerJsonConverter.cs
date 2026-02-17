using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.LayerNode
{
    public class LayerJsonConverter : BaseNodeJsonConverter<Layer>
    {
        protected override Layer CreateInstance(JToken element, Type objectType, Layer existingValue, 
            bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Layer.IdJName);
            if (jId is null)
                throw new Shared.Exceptions.JsonNodeRequiredException<Layer>(Layer.IdJName);

            return new Layer(jId.ToString());
        }

        protected override Layer EnrichReadJson(Layer layer, JToken element, Type objectType, 
            Layer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            layer = base.EnrichReadJson(layer, element, objectType, existingValue, hasExistingValue, serializer);

            var jOtherContent = element.TryGetToken(Layer.OtherContentJName);
            if (jOtherContent != null)
            {
                if (jOtherContent is JArray array)
                {
                    foreach (var item in array)
                        layer.AddOtherContent(item.ToString());
                }
                else
                {
                    layer.AddOtherContent(jOtherContent.ToString());
                }
            }

            return layer;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Layer layer, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, layer, serializer);

            if (layer.OtherContent.Count > 0)
            {
                writer.WritePropertyName(Layer.OtherContentJName);
                writer.WriteStartArray();
                foreach (var content in layer.OtherContent)
                    writer.WriteValue(content);
                writer.WriteEndArray();
            }
        }
    }
}

