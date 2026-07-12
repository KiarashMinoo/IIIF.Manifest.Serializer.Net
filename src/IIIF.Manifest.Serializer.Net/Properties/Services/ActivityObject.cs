using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Services.Discovery;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services;

/// <summary>
///     Represents the object of an Activity Streams activity.
/// </summary>
[DiscoveryAPI("1.0")]
public class ActivityObject : TrackableObject<ActivityObject>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string CanonicalJName = "canonical";
    public const string SeeAlsoJName = "seeAlso";
    public const string ProviderJName = "provider";

    [JsonConstructor]
    public ActivityObject(string id, string type)
    {
        Id = id;
        Type = type;
    }

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

    [DiscoveryAPI("1.0")]
    [JsonProperty(CanonicalJName)]
    public string? Canonical
    {
        get => GetElementValue(x => x.Canonical);
        private set => SetElementValue(value);
    }

    [DiscoveryAPI("1.0")]
    [JsonProperty(SeeAlsoJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<DiscoveryDataset> SeeAlso
    {
        get => GetElementValue(x => x.SeeAlso) ?? [];
        private set => SetElementValue(value);
    }

    [DiscoveryAPI("1.0")]
    [JsonProperty(ProviderJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<DiscoveryAgent> Provider
    {
        get => GetElementValue(x => x.Provider) ?? [];
        private set => SetElementValue(value);
    }

    public ActivityObject SetCanonical(string canonical)
    {
        Canonical = canonical;
        return this;
    }

    public ActivityObject AddSeeAlso(DiscoveryDataset dataset)
    {
        SeeAlso = SeeAlso.With(dataset);
        return this;
    }

    public ActivityObject AddProvider(DiscoveryAgent provider)
    {
        Provider = Provider.With(provider);
        return this;
    }
}