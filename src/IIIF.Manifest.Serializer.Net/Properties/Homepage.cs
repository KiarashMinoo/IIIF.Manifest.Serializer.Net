using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(HomepageJsonConverter))]
    public class Homepage : FormatableItem<Homepage>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)]
        public string Label { get; }

        public Homepage(string id) : base(id)
        {
        }

        public Homepage(string id, string label) : base(id)
        {
            Label = label;
        }
    }
}