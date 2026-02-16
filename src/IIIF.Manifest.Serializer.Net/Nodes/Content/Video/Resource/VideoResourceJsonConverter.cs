using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace IIIF.Manifests.Serializer.Nodes.Content.Video.Resource
{
    public class VideoResourceJsonConverter : BaseResourceJsonConverter<VideoResource>
    {
        protected override VideoResource CreateInstance(JToken element, Type objectType, VideoResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(VideoResource.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<VideoResource>(VideoResource.IdJName);
            var jFormat = element.TryGetToken(VideoResource.FormatJName);
            var format = jFormat?.ToString() ?? "video/mp4";
            return new VideoResource(jId.ToString(), format);
        }
        protected override VideoResource EnrichReadJson(VideoResource resource, JToken element, Type objectType, VideoResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            resource = base.EnrichReadJson(resource, element, objectType, existingValue, hasExistingValue, serializer);
            var jHeight = element.TryGetToken(Constants.HeightJName);
            if (jHeight != null) resource.SetHeight(jHeight.Value<int>());
            var jWidth = element.TryGetToken(Constants.WidthJName);
            if (jWidth != null) resource.SetWidth(jWidth.Value<int>());
            var jDuration = element.TryGetToken(VideoResource.DurationJName);
            if (jDuration != null) resource.SetDuration(jDuration.Value<double>());
            return resource;
        }
        protected override void EnrichMoreWriteJson(JsonWriter writer, VideoResource videoResource, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, videoResource, serializer);
            if (videoResource?.Height != null)
            {
                writer.WritePropertyName(Constants.HeightJName);
                writer.WriteValue(videoResource.Height.Value);
            }
            if (videoResource?.Width != null)
            {
                writer.WritePropertyName(Constants.WidthJName);
                writer.WriteValue(videoResource.Width.Value);
            }
            if (videoResource?.Duration != null)
            {
                writer.WritePropertyName(VideoResource.DurationJName);
                writer.WriteValue(videoResource.Duration.Value);
            }
        }
    }
}
