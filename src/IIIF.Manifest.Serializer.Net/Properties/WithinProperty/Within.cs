using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Within
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(WithinJsonConverter))]
    public class Within : BaseItem<Within>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)]
        public string Label { get; private set; }

        public Within(string id) : base(id)
        {
        }

        public Within SetLabel(string label) => SetPropertyValue(a => a.Label, label);
    }
}