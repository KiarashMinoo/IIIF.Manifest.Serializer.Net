using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Discovery;

/// <summary>
///     A plain <c>{"id","type"}</c> resource reference - used throughout Change Discovery API 1.0 for
///     <c>first</c>/<c>last</c>/<c>partOf</c>/<c>next</c>/<c>prev</c> page pointers, and for an
///     <see cref="Activity" />'s <c>target</c> (the destination of a Move activity).
/// </summary>
[DiscoveryAPI("1.0")]
public class DiscoveryResourceReference : TrackableObject<DiscoveryResourceReference>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";

    [JsonConstructor]
    public DiscoveryResourceReference(string id, string type)
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
}