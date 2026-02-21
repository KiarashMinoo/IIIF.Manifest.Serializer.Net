using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty;

/// <summary>
/// Represents an Activity Streams activity for change discovery.
/// </summary>
[DiscoveryAPI("1.0")]
public class Activity : TrackableObject<Activity>
{
    public const string TypeJName = "type";
    public const string ObjectJName = "object";
    public const string EndTimeJName = "endTime";

    [JsonProperty(TypeJName)] public string Type => GetElementValue(x => x.Type)!;

    [JsonProperty(ObjectJName)] public ActivityObject Object => GetElementValue(x => x.Object)!;

    [JsonProperty(EndTimeJName)] public string EndTime => GetElementValue(x => x.EndTime)!;

    public Activity(string type, ActivityObject @object, string endTime)
    {
        SetElementValue(x => x.Type, type);
        SetElementValue(x => x.Object, @object);
        SetElementValue(x => x.EndTime, endTime);
    }
}