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
    public class VideoResource : BaseResource<VideoResource>, IDimensionSupport<VideoResource>
    {
        public const string DurationJName = "duration";

        [JsonProperty(Constants.HeightJName)] public int? Height => GetElementValue(x => x.Height);

        [JsonProperty(Constants.WidthJName)] public int? Width => GetElementValue(x => x.Width);

        [JsonProperty(DurationJName)] public double? Duration => GetElementValue(x => x.Duration);

        public VideoResource(string id, string format) : base(id, "dctypes:MovingImage")
        {
            SetFormat(format);
        }

        public VideoResource SetHeight(int height) => SetElementValue(a => a.Height, height);
        public VideoResource SetWidth(int width) => SetElementValue(a => a.Width, width);
        public VideoResource SetDuration(double duration) => SetElementValue(a => a.Duration, duration);
    }
}