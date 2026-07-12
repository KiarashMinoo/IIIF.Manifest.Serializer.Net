using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
///     One "contextualizing" match-context entry in a <see cref="SearchResponse" />'s <c>annotations</c>
///     field - identifies exactly where within a search-result Annotation's body the query matched.
/// </summary>
[SearchAPI("2.0")]
public class SearchHitAnnotation : TrackableObject<SearchHitAnnotation>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string MotivationJName = "motivation";
    public const string TargetJName = "target";

    [JsonConstructor]
    public SearchHitAnnotation(SearchHitTarget target)
    {
        Type = "Annotation";
        Motivation = "contextualizing";
        Target = target;
    }

    [JsonProperty(IdJName)]
    public string? Id
    {
        get => GetElementValue(x => x.Id);
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "Annotation";
        private set => SetElementValue(value);
    }

    [JsonProperty(MotivationJName)]
    public string Motivation
    {
        get => GetElementValue(x => x.Motivation) ?? "contextualizing";
        private set => SetElementValue(value);
    }

    [JsonProperty(TargetJName)]
    public SearchHitTarget Target
    {
        get => GetElementValue(x => x.Target)!;
        private set => SetElementValue(value);
    }

    public SearchHitAnnotation SetId(string id)
    {
        Id = id;
        return this;
    }
}