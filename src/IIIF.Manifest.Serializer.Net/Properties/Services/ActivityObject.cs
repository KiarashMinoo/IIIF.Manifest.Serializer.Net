using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services;

/// <summary>
/// Represents the object of an Activity Streams activity.
/// </summary>
[DiscoveryAPI("1.0")]
public class ActivityObject : TrackableObject<ActivityObject>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";

    [JsonProperty(IdJName)]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type)!;
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public ActivityObject(string id, string type)
    {
        Id = id;
        Type = type;
    }
}