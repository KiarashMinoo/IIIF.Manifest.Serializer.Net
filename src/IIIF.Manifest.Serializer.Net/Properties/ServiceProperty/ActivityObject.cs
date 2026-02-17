using IIIF.Manifests.Serializer.Attributes;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty;

/// <summary>
/// Represents the object of an Activity Streams activity.
/// </summary>
[DiscoveryAPI("1.0")]
public class ActivityObject
{
    public const string IdJName = "id";
    public const string TypeJName = "type";

    [JsonProperty(IdJName)]
    public string Id { get; set; }

    [JsonProperty(TypeJName)]
    public string Type { get; set; }
}