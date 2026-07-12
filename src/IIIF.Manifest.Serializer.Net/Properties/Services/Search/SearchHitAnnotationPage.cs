using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
///     The nested "AnnotationPage" a <see cref="SearchResponse" />'s <c>annotations</c> field carries -
///     the match-context entries (see <see cref="SearchHitAnnotation" />) for the search results in
///     <see cref="SearchResponse.Items" />.
/// </summary>
[SearchAPI("2.0")]
public class SearchHitAnnotationPage : TrackableObject<SearchHitAnnotationPage>
{
    public const string TypeJName = "type";
    public const string ItemsJName = "items";

    public SearchHitAnnotationPage(IReadOnlyCollection<SearchHitAnnotation> items)
    {
        Type = "AnnotationPage";
        Items = items;
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "AnnotationPage";
        private set => SetElementValue(value);
    }

    [JsonProperty(ItemsJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<SearchHitAnnotation> Items
    {
        get => GetElementValue(x => x.Items) ?? [];
        private set => SetElementValue(value);
    }

    public SearchHitAnnotationPage AddItem(SearchHitAnnotation item)
    {
        Items = Items.With(item);
        return this;
    }
}