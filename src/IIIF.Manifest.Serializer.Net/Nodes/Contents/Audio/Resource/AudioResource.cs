using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource
{
    /// <summary>
    /// IIIF Audio resource for A/V content.
    /// </summary>
    public class AudioResource : BaseResource<AudioResource>
    {
        public const string DurationJName = "duration";

        [JsonProperty(DurationJName)]
        public double? Duration
        {
            get => GetElementValue(x => x.Duration);
            private set => SetElementValue(value);
        }

        public AudioResource(string id, string format) : base(id,  ResourceType.Sound)
        {
            SetFormat(format);
        }

        public AudioResource SetDuration(double duration)
        {
            Duration = duration;
            return this;
        }
    }
}