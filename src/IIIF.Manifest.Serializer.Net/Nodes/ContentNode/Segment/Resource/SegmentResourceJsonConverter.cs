using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Segment.Resource
{
    public class SegmentResourceJsonConverter : BaseResourceJsonConverter<SegmentResource>
    {
        private SegmentResource SetFull(JToken element, SegmentResource resource)
        {
            var jFull = element.TryGetToken(SegmentResource.FullJName);
            if (jFull != null)
                resource.SetFull(jFull.ToObject<BaseResource>());

            return resource;
        }

        protected override SegmentResource CreateInstance(JToken element, Type objectType, SegmentResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(SegmentResource.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<SegmentResource>(SegmentResource.IdJName);

            var jType = element.TryGetToken(SegmentResource.TypeJName);
            if (jType is null)
                throw new JsonNodeRequiredException<SegmentResource>(SegmentResource.TypeJName);

            var resource = new SegmentResource(jId.ToString(), jType.ToString());
            return resource;
        }

        protected override SegmentResource EnrichReadJson(SegmentResource resource, JToken element, Type objectType, SegmentResource? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            resource = base.EnrichReadJson(resource, element, objectType, existingValue, hasExistingValue, serializer);
            resource = SetFull(element, resource);
            return resource;
        }

    }
}