using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IIIF.Manifests.Serializer.Nodes.Content.Audio.Resource
{
    public class AudioResourceJsonConverter : BaseResourceJsonConverter<AudioResource>
    {
        protected override AudioResource CreateInstance(JToken element, Type objectType, AudioResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(AudioResource.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<AudioResource>(AudioResource.IdJName);
            var jFormat = element.TryGetToken(AudioResource.FormatJName);
            var format = jFormat?.ToString() ?? "audio/mpeg";
            return new AudioResource(jId.ToString(), format);
        }
        protected override AudioResource EnrichReadJson(AudioResource resource, JToken element, Type objectType, AudioResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            resource = base.EnrichReadJson(resource, element, objectType, existingValue, hasExistingValue, serializer);
            var jDuration = element.TryGetToken(AudioResource.DurationJName);
            if (jDuration != null) resource.SetDuration(jDuration.Value<double>());
            return resource;
        }
        protected override void EnrichMoreWriteJson(JsonWriter writer, AudioResource audioResource, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, audioResource, serializer);
            if (audioResource?.Duration != null)
            {
                writer.WritePropertyName(AudioResource.DurationJName);
                writer.WriteValue(audioResource.Duration.Value);
            }
        }
    }
}
