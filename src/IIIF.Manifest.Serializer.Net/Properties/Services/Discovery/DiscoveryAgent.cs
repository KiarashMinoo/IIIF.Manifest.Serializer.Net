using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable.Objects;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Discovery;

/// <summary>
///     A Change Discovery API 1.0 agent reference - used by the <c>provider</c> field on an
///     <see cref="ActivityObject" /> (type <c>"Agent"</c>) and by an <see cref="Activity" />'s
///     <c>actor</c> (type <c>"Application"</c>/<c>"Organization"</c>/<c>"Person"</c>); both share the
///     same <c>{id,type,label}</c> shape.
/// </summary>
[DiscoveryAPI("1.0")]
public sealed class DiscoveryAgent : TrackableObject<DiscoveryAgent>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string LabelJName = "label";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private DiscoveryAgent()
    {
    }

    [JsonConstructor]
    public DiscoveryAgent(string id, string type)
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

    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    public DiscoveryAgent SetLabel(string label)
    {
        return SetElementValue(x => x.Label, (IReadOnlyCollection<Label>)[new Label(label)]);
    }
}