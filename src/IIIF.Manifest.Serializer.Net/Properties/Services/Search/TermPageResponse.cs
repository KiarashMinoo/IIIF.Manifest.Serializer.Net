using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
/// The JSON body a Content Search API 2.0 autocomplete request returns - a "TermPage" of
/// <see cref="SearchTerm"/> suggestions.
/// </summary>
[SearchAPI("2.0")]
public class TermPageResponse : TrackableObject<TermPageResponse>
{
    public const string DefaultContext = "http://iiif.io/api/search/2/context.json";
    public const string ContextJName = "@context";
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string ItemsJName = "items";
    public const string IgnoredJName = "ignored";

    [JsonProperty(ContextJName)]
    public string Context
    {
        get => GetElementValue(x => x.Context) ?? DefaultContext;
        private set => SetElementValue(value);
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
        get => GetElementValue(x => x.Type) ?? "TermPage";
        private set => SetElementValue(value);
    }

    [JsonProperty(ItemsJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<SearchTerm> Items
    {
        get => GetElementValue(x => x.Items) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(IgnoredJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<string> Ignored
    {
        get => GetElementValue(x => x.Ignored) ?? [];
        private set => SetElementValue(value);
    }

    public TermPageResponse(string id)
    {
        Context = DefaultContext;
        Type = "TermPage";
        Id = id;
    }

    public TermPageResponse AddItem(SearchTerm item)
    {
        Items = Items.With(item);
        return this;
    }

    public TermPageResponse AddIgnored(string ignored)
    {
        Ignored = Ignored.With(ignored);
        return this;
    }
}
