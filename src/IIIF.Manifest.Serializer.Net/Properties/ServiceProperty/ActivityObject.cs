using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty;

/// <summary>
/// Represents the object of an Activity Streams activity.
/// </summary>
[DiscoveryAPI("1.0")]
public class ActivityObject : TrackableObject<ActivityObject>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";

    [JsonProperty(IdJName)] public string Id => GetElementValue(x => x.Id)!;

    [JsonProperty(TypeJName)] public string Type => GetElementValue(x => x.Type)!;

    public ActivityObject(string id, string type)
    {
        SetElementValue(x => x.Id, id);
        SetElementValue(x => x.Type, type);
    }
}