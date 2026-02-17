using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Video.Resource
{
    /// <summary>
    /// IIIF Video resource for A/V content.
    /// </summary>
    [JsonConverter(typeof(VideoResourceJsonConverter))]
    public class VideoResource : BaseResource<VideoResource>, IDimenssionSupport<VideoResource>
    {
        public const string DurationJName = "duration";

        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; private set; }

        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; private set; }

        [JsonProperty(DurationJName)]
        public double? Duration { get; private set; }

        public VideoResource(string id, string format) : base(id, "dctypes:MovingImage")
        {
            SetFormat(format);
        }

        public VideoResource SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public VideoResource SetWidth(int width) => SetPropertyValue(a => a.Width, width);
        public VideoResource SetDuration(double duration) => SetPropertyValue(a => a.Duration, duration);
    }
}

