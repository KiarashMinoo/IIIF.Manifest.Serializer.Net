using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.AnnotationList
{
    public class AnnotationListJsonConverter : BaseNodeJsonConverter<AnnotationList>
    {
        protected override AnnotationList CreateInstance(JToken element, Type objectType, 
            AnnotationList existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(AnnotationList.IdJName);
            if (jId is null)
                throw new Shared.Exceptions.JsonNodeRequiredException<AnnotationList>(AnnotationList.IdJName);

            return new AnnotationList(jId.ToString());
        }

        protected override AnnotationList EnrichReadJson(AnnotationList annotationList, JToken element, 
            Type objectType, AnnotationList existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            annotationList = base.EnrichReadJson(annotationList, element, objectType, existingValue, 
                hasExistingValue, serializer);

            var jResources = element.TryGetToken(AnnotationList.ResourcesJName);
            if (jResources != null && jResources is JArray array)
            {
                foreach (var item in array)
                    annotationList.AddResource(item.ToObject<object>());
            }

            var jWithin = element.TryGetToken(AnnotationList.WithinJName);
            if (jWithin != null)
            {
                if (jWithin.Type == JTokenType.String)
                    annotationList.SetWithinLayer(jWithin.ToString());
                else if (jWithin is JObject obj)
                {
                    var layerId = obj.TryGetToken("@id");
                    if (layerId != null)
                        annotationList.SetWithinLayer(layerId.ToString());
                }
            }

            return annotationList;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, AnnotationList annotationList, 
            JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, annotationList, serializer);

            if (annotationList.Resources.Count > 0)
            {
                writer.WritePropertyName(AnnotationList.ResourcesJName);
                writer.WriteStartArray();
                foreach (var resource in annotationList.Resources)
                    serializer.Serialize(writer, resource);
                writer.WriteEndArray();
            }

            if (!string.IsNullOrEmpty(annotationList.WithinLayer))
            {
                writer.WritePropertyName(AnnotationList.WithinJName);
                writer.WriteValue(annotationList.WithinLayer);
            }
        }
    }
}

