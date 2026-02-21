using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Audio.Resource
{
    /// <summary>
    /// IIIF Audio resource for A/V content.
    /// </summary>
    [JsonConverter(typeof(AudioResourceJsonConverter))]
    public class AudioResource : BaseResource<AudioResource>
    {
        public const string DurationJName = "duration";

        [JsonProperty(DurationJName)] public double? Duration => GetElementValue(x => x.Duration);

        public AudioResource(string id, string format) : base(id, "dctypes:Sound")
        {
            SetFormat(format);
        }

        public AudioResource SetDuration(double duration) => SetElementValue(a => a.Duration, duration);
    }
}