using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable.Objects;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
///     The "SpecificResource" target of a <see cref="SearchHitAnnotation" /> - points back at the
///     matching search-result Annotation (<see cref="Source" />) via one or more
///     <see cref="Selector" />s identifying the matched text.
/// </summary>
[SearchAPI("2.0")]
public sealed class SearchHitTarget : TrackableObject<SearchHitTarget>
{
    public const string TypeJName = "type";
    public const string SourceJName = "source";
    public const string SelectorJName = "selector";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private SearchHitTarget()
    {
    }

    [JsonConstructor]
    public SearchHitTarget(string source, IReadOnlyCollection<SearchTextQuoteSelector> selector)
    {
        Type = "SpecificResource";
        Source = source;
        Selector = selector;
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "SpecificResource";
        private set => SetElementValue(value);
    }

    [JsonProperty(SourceJName)]
    public string Source
    {
        get => GetElementValue(x => x.Source)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(SelectorJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<SearchTextQuoteSelector> Selector
    {
        get => GetElementValue(x => x.Selector) ?? [];
        private set => SetElementValue(value);
    }
}