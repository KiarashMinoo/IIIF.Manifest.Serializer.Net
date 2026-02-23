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

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(ObjectJName)]
    public ActivityObject Object
    {
        get => GetElementValue(x => x.Object)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(EndTimeJName)]
    public string EndTime
    {
        get => GetElementValue(x => x.EndTime)!;
        private set => SetElementValue(value);
    }

    public Activity(string type, ActivityObject @object, string endTime)
    {
        Type = type;
        Object = @object;
        EndTime = endTime;
    }
}