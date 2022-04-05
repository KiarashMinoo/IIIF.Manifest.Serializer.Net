using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IIIF.Manifests.Serializer.Nodes
{
    public class EmbeddedContentResourceJsonConverter : BaseResourceJsonConverter<EmbeddedContentResource>
    {
        protected override EmbeddedContentResource CreateInstance(JToken element, Type objectType, EmbeddedContentResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return base.CreateInstance(element, objectType, existingValue, hasExistingValue, serializer);
        }
    }
}