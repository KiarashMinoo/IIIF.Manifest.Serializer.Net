using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ProviderProperty
{
    /// <summary>
    /// IIIF Provider property - describes the organization providing the resource.
    /// </summary>
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(ProviderJsonConverter))]
    public class Provider : FormattableItem<Provider>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)] public string? Label => GetElementValue(x => x.Label);

        public Provider(string id) : base(id, "Agent")
        {
        }

        public Provider(string id, string label) : this(id)
        {
            SetElementValue(x => x.Label, label);
        }
    }
}