using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Discovery;

/// <summary>
/// A Change Discovery API 1.0 "Dataset" reference - used by the <c>seeAlso</c> field on both the
/// top-level OrderedCollection and an <see cref="ActivityObject"/>, pointing to supplementary
/// machine-readable data about the resource (e.g. an OAI-PMH endpoint or a JSON-LD description).
/// </summary>
[DiscoveryAPI("1.0")]
public class DiscoveryDataset : TrackableObject<DiscoveryDataset>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string FormatJName = "format";
    public const string LabelJName = "label";
    public const string ProfileJName = "profile";

    [JsonProperty(IdJName)]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "Dataset";
        private set => SetElementValue(value);
    }

    [JsonProperty(FormatJName)]
    public string? Format
    {
        get => GetElementValue(x => x.Format);
        private set => SetElementValue(value);
    }

    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(ProfileJName)]
    public string? Profile
    {
        get => GetElementValue(x => x.Profile);
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public DiscoveryDataset(string id)
    {
        Id = id;
        Type = "Dataset";
    }

    public DiscoveryDataset SetFormat(string format)
    {
        Format = format;
        return this;
    }

    public DiscoveryDataset SetLabel(string label) => SetElementValue(x => x.Label, (IReadOnlyCollection<Label>)[new Label(label)]);

    public DiscoveryDataset SetProfile(string profile)
    {
        Profile = profile;
        return this;
    }
}
