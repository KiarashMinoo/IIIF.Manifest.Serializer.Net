using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Structure
{
    public class StructureJsonConverter : BaseNodeJsonConverter<Structure>
    {
        private Structure ConstructStructure(JToken element)
        {
            var jId = element.TryGetToken(Structure.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Structure>(Structure.IdJName);

            return new Structure(jId.ToString());
        }

        private Structure SetCanvases(JToken element, Structure structure)
        {
            var jCanvases = element.TryGetToken(Structure.CanvasesJName);
            if (jCanvases != null)
            {
                if (!(jCanvases is JArray))
                    throw new JsonObjectMustBeJArray<Structure>(Structure.CanvasesJName);

                foreach (var canvas in jCanvases.ToObject<string[]>())
                    structure.AddCanvas(canvas);
            }

            return structure;
        }

        private Structure SetRanges(JToken element, Structure structure)
        {
            var jRanges = element.TryGetToken(Structure.RangesJName);
            if (jRanges != null)
            {
                if (!(jRanges is JArray))
                    throw new JsonObjectMustBeJArray<Structure>(Structure.RangesJName);

                foreach (var range in jRanges.ToObject<string[]>())
                    structure.AddRange(range);
            }

            return structure;
        }

        private Structure SetStartCanvas(JToken element, Structure structure)
        {
            var jStartCanvas = element.TryGetToken(Structure.StartCanvasJName);
            if (jStartCanvas != null)
                structure.SetStartCanvas(jStartCanvas.ToString());

            return structure;
        }

        protected override Structure CreateInstance(JToken element, Type objectType, Structure existingValue, bool hasExistingValue, JsonSerializer serializer)
            => ConstructStructure(element);

        protected override Structure EnrichReadJson(Structure structure, JToken element, Type objectType, Structure existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            structure = base.EnrichReadJson(structure, element, objectType, existingValue, hasExistingValue, serializer);
            structure = SetCanvases(element, structure);
            structure = SetRanges(element, structure);
            structure = SetStartCanvas(element, structure);
            return structure;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Structure structure, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, structure, serializer);

            if (structure != null)
            {
                if (structure.Canvases.Any())
                {
                    writer.WritePropertyName(Structure.CanvasesJName);
                    writer.WriteStartArray();
                    foreach (var canvas in structure.Canvases)
                        writer.WriteValue(canvas);
                    writer.WriteEndArray();
                }

                if (structure.Ranges.Any())
                {
                    writer.WritePropertyName(Structure.RangesJName);
                    writer.WriteStartArray();
                    foreach (var range in structure.Ranges)
                        writer.WriteValue(range);
                    writer.WriteEndArray();
                }

                if (!string.IsNullOrEmpty(structure.StartCanvas))
                {
                    writer.WritePropertyName(Structure.StartCanvasJName);
                    writer.WriteValue(structure.StartCanvas);
                }
            }
        }
    }
}
