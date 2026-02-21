using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(HomepageJsonConverter))]
    public class Homepage : FormattableItem<Homepage>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)] public string? Label => GetElementValue(x => x.Label);

        public Homepage(string id) : base(id)
        {
        }

        public Homepage(string id, string label) : base(id)
        {
            SetElementValue(x => x.Label, label);
        }
    }
}