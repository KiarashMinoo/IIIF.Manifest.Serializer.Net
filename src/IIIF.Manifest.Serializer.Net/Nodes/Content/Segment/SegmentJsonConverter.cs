using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Content.Segment.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Content.Segment
{
    public class SegmentJsonConverter : BaseContentJsonConverter<Segment>
    {
        private Segment SetSelector(JToken element, Segment segment)
        {
            var jSelector = element.TryGetToken(Segment.SelectorJName);
            if (jSelector != null)
                segment.SetSelector(jSelector.ToObject<Selector.Selector>());

            return segment;
        }

        protected override Segment CreateInstance(JToken element, Type objectType, Segment existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Segment.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Segment>(Segment.IdJName);

            var jType = element.TryGetToken(Segment.TypeJName);
            if (jType is null)
                throw new JsonNodeRequiredException<Segment>(Segment.TypeJName);

            var jResource = element.TryGetToken(Segment.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<Segment>(Segment.ResourceJName);

            var jOn = element.TryGetToken(Segment.OnJName);
            if (jOn is null)
                throw new JsonNodeRequiredException<Segment>(Segment.OnJName);

            return new Segment(jId.ToString(), jResource.ToObject<SegmentResource>(), jOn.ToString());
        }

        protected override Segment EnrichReadJson(Segment segment, JToken element, Type objectType, Segment existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            segment = base.EnrichReadJson(segment, element, objectType, existingValue, hasExistingValue, serializer);
            segment = SetSelector(element, segment);
            return segment;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Segment segment, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, segment, serializer);

            if (segment != null)
            {
                if (!string.IsNullOrEmpty(segment.On))
                {
                    writer.WritePropertyName(Segment.OnJName);
                    writer.WriteValue(segment.On);
                }

                if (segment.Resource != null)
                {
                    writer.WritePropertyName(Segment.ResourceJName);
                    serializer.Serialize(writer, segment.Resource);
                }

                if (segment.Selector != null)
                {
                    writer.WritePropertyName(Segment.SelectorJName);
                    serializer.Serialize(writer, segment.Selector);
                }

                if (!string.IsNullOrEmpty(segment.Motivation))
                {
                    writer.WritePropertyName(Segment.MotivationJName);
                    writer.WriteValue(segment.Motivation);
                }
            }
        }
    }
}