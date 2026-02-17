using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Nodes.StructureNode;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.ManifestNode
{
    public class ManifestJsonConverter : BaseNodeJsonConverter<Manifest>
    {
        private Manifest ConstructManifest(JToken element)
        {
            var jId = element.TryGetToken(Manifest.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Manifest>(Manifest.IdJName);

            return new Manifest(jId.ToString());
        }

        private Manifest SetNavDate(JToken element, Manifest manifest)
        {
            var jNavDate = element.TryGetToken(Manifest.NavDateJName);
            if (jNavDate != null)
            {
                if (jNavDate.Type == Newtonsoft.Json.Linq.JTokenType.Date)
                    manifest.SetNavDate(jNavDate.Value<DateTime>());
                else
                    manifest.SetNavDate(DatetimeHelper.ParseISO8601String(jNavDate.ToString()));
            }

            return manifest;
        }

        private Manifest SetSequences(JToken element, Manifest manifest)
        {
            var jSequences = element.TryGetToken(Manifest.SequencesJName);
            if (jSequences is null)
                return manifest;

            if (!(jSequences is JArray))
                throw new JsonObjectMustBeJArray<Manifest>(Manifest.SequencesJName);

            var sequences = jSequences.ToObject<Sequence[]>();
            foreach (var sequence in sequences)
                manifest.AddSequence(sequence);

            return manifest;
        }

        private Manifest SetStructures(JToken element, Manifest manifest)
        {
            var jStructures = element.TryGetToken(Manifest.StructuresJName);
            if (jStructures != null)
            {
                if (!(jStructures is JArray))
                    throw new JsonObjectMustBeJArray<Manifest>(Manifest.StructuresJName);

                var structures = jStructures.ToObject<Structure[]>();
                foreach (var structure in structures)
                    manifest.AddStructure(structure);
            }

            return manifest;
        }

        private Manifest SetStart(JToken element, Manifest manifest)
        {
            var jStart = element.TryGetToken(Manifest.StartJName);
            if (jStart != null)
            {
                // Start can be either a string ID or an object with @id
                if (jStart.Type == JTokenType.String)
                {
                    manifest.SetStart(jStart.Value<string>());
                }
                else if (jStart is JObject startObj)
                {
                    var id = startObj.TryGetToken("@id")?.Value<string>() ?? startObj.TryGetToken("id")?.Value<string>();
                    if (!string.IsNullOrEmpty(id))
                    {
                        manifest.SetStart(id);
                    }
                }
            }

            return manifest;
        }

        private Manifest SetPlaceholderCanvas(JToken element, Manifest manifest)
        {
            var jPlaceholderCanvas = element.TryGetToken(Manifest.PlaceholderCanvasJName);
            if (jPlaceholderCanvas != null)
            {
                // PlaceholderCanvas can be either a string ID or an object with @id
                if (jPlaceholderCanvas.Type == JTokenType.String)
                {
                    manifest.SetPlaceholderCanvas(jPlaceholderCanvas.Value<string>());
                }
                else if (jPlaceholderCanvas is JObject placeholderObj)
                {
                    var id = placeholderObj.TryGetToken("@id")?.Value<string>() ?? placeholderObj.TryGetToken("id")?.Value<string>();
                    if (!string.IsNullOrEmpty(id))
                    {
                        manifest.SetPlaceholderCanvas(id);
                    }
                }
            }

            return manifest;
        }

        protected override Manifest CreateInstance(JToken element, Type objectType, Manifest existingValue, bool hasExistingValue, JsonSerializer serializer) => ConstructManifest(element);

        protected override Manifest EnrichReadJson(Manifest manifest, JToken element, Type objectType, Manifest existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            manifest = base.EnrichReadJson(manifest, element, objectType, existingValue, hasExistingValue, serializer);

            manifest = SetNavDate(element, manifest);
            manifest = SetSequences(element, manifest);
            manifest = manifest.SetViewingDirection(element);
            manifest = SetStructures(element, manifest);
            manifest = SetStart(element, manifest);
            manifest = SetPlaceholderCanvas(element, manifest);

            return manifest;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Manifest manifest, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, manifest, serializer);

            if (manifest != null)
            {
                if (manifest.NavDate != null)
                {
                    writer.WritePropertyName(Manifest.NavDateJName);
                    writer.WriteValue(manifest.NavDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture));
                }

                if (manifest.Sequences.Any())
                {
                    writer.WritePropertyName(Manifest.SequencesJName);

                    writer.WriteStartArray();

                    foreach (var sequence in manifest.Sequences)
                        serializer.Serialize(writer, sequence);

                    writer.WriteEndArray();
                }

                if (manifest.Structures.Any())
                {
                    writer.WritePropertyName(Manifest.StructuresJName);

                    writer.WriteStartArray();

                    foreach (var structure in manifest.Structures)
                        serializer.Serialize(writer, structure);

                    writer.WriteEndArray();
                }

                if (manifest.ViewingDirection != null)
                {
                    writer.WritePropertyName(Constants.ViewingDirectionJName);
                    serializer.Serialize(writer, manifest.ViewingDirection);
                }

                if (!string.IsNullOrEmpty(manifest.Start))
                {
                    writer.WritePropertyName(Manifest.StartJName);
                    writer.WriteValue(manifest.Start);
                }

                if (!string.IsNullOrEmpty(manifest.PlaceholderCanvas))
                {
                    writer.WritePropertyName(Manifest.PlaceholderCanvasJName);
                    writer.WriteValue(manifest.PlaceholderCanvas);
                }
            }
        }
    }
}