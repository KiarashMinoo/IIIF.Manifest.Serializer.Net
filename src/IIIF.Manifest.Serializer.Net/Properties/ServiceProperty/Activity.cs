using IIIF.Manifests.Serializer.Attributes;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty;

/// <summary>
/// Represents an Activity Streams activity for change discovery.
/// </summary>
[DiscoveryAPI("1.0")]
public class Activity
{
    public const string TypeJName = "type";
    public const string ObjectJName = "object";
    public const string EndTimeJName = "endTime";

    [JsonProperty(TypeJName)]
    public string Type { get; set; }

    [JsonProperty(ObjectJName)]
    public ActivityObject Object { get; set; }

    [JsonProperty(EndTimeJName)]
    public string EndTime { get; set; }
}