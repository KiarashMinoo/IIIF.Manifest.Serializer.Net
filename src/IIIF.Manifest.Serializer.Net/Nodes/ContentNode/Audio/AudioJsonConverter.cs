using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Audio.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Audio
{
    public class AudioJsonConverter : BaseContentJsonConverter<Audio>
    {
        protected override Audio CreateInstance(JToken element, Type objectType, Audio existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var id = string.Empty;
            var jId = element.TryGetToken(Audio.IdJName);
            if (jId != null)
                id = jId.ToString();

            var jResource = element.TryGetToken(Audio.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<Audio>(Audio.ResourceJName);

            var jOn = element.TryGetToken(Audio.OnJName);
            if (jOn is null)
                throw new JsonNodeRequiredException<Audio>(Audio.OnJName);

            return new Audio(id, jResource.ToObject<AudioResource>(), jOn.ToString());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Audio audio, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, audio, serializer);

            if (audio != null)
            {
                if (!string.IsNullOrEmpty(audio.On))
                {
                    writer.WritePropertyName(Audio.OnJName);
                    writer.WriteValue(audio.On);
                }

                if (audio.Resource != null)
                {
                    writer.WritePropertyName(Audio.ResourceJName);
                    serializer.Serialize(writer, audio.Resource);
                }

                if (!string.IsNullOrEmpty(audio.Motivation))
                {
                    writer.WritePropertyName(Audio.MotivationJName);
                    writer.WriteValue(audio.Motivation);
                }
            }
        }
    }
}