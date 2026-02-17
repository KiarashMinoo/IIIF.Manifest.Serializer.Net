using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Provider
{
    /// <summary>
    /// IIIF Provider property - describes the organization providing the resource.
    /// </summary>
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(ProviderJsonConverter))]
    public class Provider : FormatableItem<Provider>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)]
        public string Label { get; }

        public Provider(string id) : base(id, "Agent")
        {
        }

        public Provider(string id, string label) : this(id)
        {
            Label = label;
        }
    }
}