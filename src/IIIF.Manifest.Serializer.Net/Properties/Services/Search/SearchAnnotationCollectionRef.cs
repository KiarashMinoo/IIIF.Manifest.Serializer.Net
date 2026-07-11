using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
/// The embedded "AnnotationCollection" a <see cref="SearchResponse"/>'s <c>partOf</c> field carries
/// - a compact pointer to the full result set's first/last page and total count.
/// </summary>
[SearchAPI("2.0")]
public class SearchAnnotationCollectionRef : TrackableObject<SearchAnnotationCollectionRef>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string FirstJName = "first";
    public const string LastJName = "last";
    public const string TotalJName = "total";

    [JsonProperty(IdJName)]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "AnnotationCollection";
        private set => SetElementValue(value);
    }

    [JsonProperty(FirstJName)]
    public SearchResourceReference? First
    {
        get => GetElementValue(x => x.First);
        private set => SetElementValue(value);
    }

    [JsonProperty(LastJName)]
    public SearchResourceReference? Last
    {
        get => GetElementValue(x => x.Last);
        private set => SetElementValue(value);
    }

    [JsonProperty(TotalJName)]
    public int? Total
    {
        get => GetElementValue(x => x.Total);
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public SearchAnnotationCollectionRef(string id)
    {
        Id = id;
        Type = "AnnotationCollection";
    }

    public SearchAnnotationCollectionRef SetFirst(SearchResourceReference first)
    {
        First = first;
        return this;
    }

    public SearchAnnotationCollectionRef SetLast(SearchResourceReference last)
    {
        Last = last;
        return this;
    }

    public SearchAnnotationCollectionRef SetTotal(int total)
    {
        Total = total;
        return this;
    }
}
