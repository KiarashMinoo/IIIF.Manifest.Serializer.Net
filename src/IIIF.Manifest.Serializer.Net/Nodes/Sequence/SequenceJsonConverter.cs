using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Sequence
{
    public class SequenceJsonConverter : BaseNodeJsonConverter<Sequence>
    {
        private Sequence ConstructSequence(JToken element)
        {
            var jId = element.TryGetToken(Sequence.IdJName);
            return jId is null ? new Sequence() : new Sequence(jId.ToString());
        }

        private Sequence SetStartCanvas(JToken element, Sequence sequence)
        {
            var jStartCanvas = element.TryGetToken(Sequence.StartCanvasJName);
            if (jStartCanvas != null)
                sequence.SetStartCanvas(jStartCanvas.ToObject<StartCanvas>());

            return sequence;
        }

        private Sequence SetCanvases(JToken element, Sequence sequence)
        {
            var jCanvases = element.TryGetToken(Sequence.CanvasesJName);
            if (jCanvases is null)
                throw new JsonNodeRequiredException<Sequence>(Sequence.CanvasesJName);

            if (!(jCanvases is JArray))
                throw new JsonObjectMustBeJArray<Sequence>(Sequence.CanvasesJName);

            var canvases = jCanvases.ToObject<Canvas.Canvas[]>();
            foreach (var canvas in canvases)
                sequence.AddCanvas(canvas);

            return sequence;
        }

        protected override Sequence CreateInstance(JToken element, Type objectType, Sequence existingValue, bool hasExistingValue, JsonSerializer serializer) => ConstructSequence(element);

        protected override Sequence EnrichReadJson(Sequence sequence, JToken element, Type objectType, Sequence existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            sequence = base.EnrichReadJson(sequence, element, objectType, existingValue, hasExistingValue, serializer);

            sequence = SetCanvases(element, sequence);
            sequence = SetStartCanvas(element, sequence);
            sequence = sequence.SetViewingDirection(element);

            return sequence;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Sequence sequence, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, sequence, serializer);

            if (sequence != null)
            {
                if (sequence.Canvases.Any())
                {
                    writer.WritePropertyName(Sequence.CanvasesJName);

                    writer.WriteStartArray();

                    foreach (var canvas in sequence.Canvases)
                        serializer.Serialize(writer, canvas);

                    writer.WriteEndArray();
                }

                if (sequence.StartCanvas != null)
                {
                    writer.WritePropertyName(Sequence.StartCanvasJName);
                    serializer.Serialize(writer, sequence.StartCanvas);
                }

                if (sequence.ViewingDirection != null)
                {
                    writer.WritePropertyName(Constants.ViewingDirectionJName);
                    serializer.Serialize(writer, sequence.ViewingDirection);
                }
            }
        }
    }
}