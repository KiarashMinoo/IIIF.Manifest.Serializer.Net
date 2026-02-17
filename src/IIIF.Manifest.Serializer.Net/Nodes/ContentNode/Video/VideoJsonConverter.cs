using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Video.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Video
{
    public class VideoJsonConverter : BaseContentJsonConverter<Video>
    {
        protected override Video CreateInstance(JToken element, Type objectType, Video existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var id = string.Empty;
            var jId = element.TryGetToken(Video.IdJName);
            if (jId != null)
                id = jId.ToString();

            var jResource = element.TryGetToken(Video.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<Video>(Video.ResourceJName);

            var jOn = element.TryGetToken(Video.OnJName);
            if (jOn is null)
                throw new JsonNodeRequiredException<Video>(Video.OnJName);

            return new Video(id, jResource.ToObject<VideoResource>(), jOn.ToString());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Video video, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, video, serializer);

            if (video != null)
            {
                if (!string.IsNullOrEmpty(video.On))
                {
                    writer.WritePropertyName(Video.OnJName);
                    writer.WriteValue(video.On);
                }

                if (video.Resource != null)
                {
                    writer.WritePropertyName(Video.ResourceJName);
                    serializer.Serialize(writer, video.Resource);
                }

                if (!string.IsNullOrEmpty(video.Motivation))
                {
                    writer.WritePropertyName(Video.MotivationJName);
                    writer.WriteValue(video.Motivation);
                }
            }
        }
    }
}