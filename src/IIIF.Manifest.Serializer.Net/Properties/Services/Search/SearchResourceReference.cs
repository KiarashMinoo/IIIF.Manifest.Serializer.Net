using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
/// A plain <c>{"id","type"}</c> resource reference - used by <see cref="SearchResponse"/>'s
/// <c>next</c>/<c>prev</c> paging pointers and by <see cref="SearchAnnotationCollectionRef"/>'s
/// <c>first</c>/<c>last</c>.
/// </summary>
[SearchAPI("2.0")]
public class SearchResourceReference : TrackableObject<SearchResourceReference>
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
    public SearchResourceReference(string id, string type)
    {
        Id = id;
        Type = type;
    }
}
