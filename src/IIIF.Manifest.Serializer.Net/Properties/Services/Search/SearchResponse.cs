using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using AnnotationNode = IIIF.Manifests.Serializer.Nodes.Contents.Annotation.Annotation;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
/// The JSON body a Content Search API 2.0 search request returns - an "AnnotationPage" of matching
/// <see cref="Nodes.Contents.Annotation.Annotation"/> results (reusing the core Presentation 3.0
/// Annotation type, since search-result annotations have the same shape as painting annotations),
/// plus optional match-context highlighting, paging, and ignored-parameter reporting.
/// </summary>
[SearchAPI("2.0")]
public class SearchResponse : TrackableObject<SearchResponse>
{
    public const string DefaultContext = "http://iiif.io/api/search/2/context.json";
    public const string ContextJName = "@context";
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string ItemsJName = "items";
    public const string AnnotationsJName = "annotations";
    public const string PartOfJName = "partOf";
    public const string NextJName = "next";
    public const string PrevJName = "prev";
    public const string StartIndexJName = "startIndex";
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
        get => GetElementValue(x => x.Type) ?? "AnnotationPage";
        private set => SetElementValue(value);
    }

    [JsonProperty(ItemsJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<AnnotationNode> Items
    {
        get => GetElementValue(x => x.Items) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(AnnotationsJName)]
    public SearchHitAnnotationPage? Annotations
    {
        get => GetElementValue(x => x.Annotations);
        private set => SetElementValue(value);
    }

    [JsonProperty(PartOfJName)]
    public SearchAnnotationCollectionRef? PartOf
    {
        get => GetElementValue(x => x.PartOf);
        private set => SetElementValue(value);
    }

    [JsonProperty(NextJName)]
    public SearchResourceReference? Next
    {
        get => GetElementValue(x => x.Next);
        private set => SetElementValue(value);
    }

    [JsonProperty(PrevJName)]
    public SearchResourceReference? Prev
    {
        get => GetElementValue(x => x.Prev);
        private set => SetElementValue(value);
    }

    [JsonProperty(StartIndexJName)]
    public int? StartIndex
    {
        get => GetElementValue(x => x.StartIndex);
        private set => SetElementValue(value);
    }

    [JsonProperty(IgnoredJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<string> Ignored
    {
        get => GetElementValue(x => x.Ignored) ?? [];
        private set => SetElementValue(value);
    }

    public SearchResponse(string id)
    {
        Context = DefaultContext;
        Type = "AnnotationPage";
        Id = id;
    }

    public SearchResponse AddItem(AnnotationNode item)
    {
        Items = Items.With(item);
        return this;
    }

    public SearchResponse SetAnnotations(SearchHitAnnotationPage annotations)
    {
        Annotations = annotations;
        return this;
    }

    public SearchResponse SetPartOf(SearchAnnotationCollectionRef partOf)
    {
        PartOf = partOf;
        return this;
    }

    public SearchResponse SetNext(SearchResourceReference next)
    {
        Next = next;
        return this;
    }

    public SearchResponse SetPrev(SearchResourceReference prev)
    {
        Prev = prev;
        return this;
    }

    public SearchResponse SetStartIndex(int startIndex)
    {
        StartIndex = startIndex;
        return this;
    }

    public SearchResponse AddIgnored(string ignored)
    {
        Ignored = Ignored.With(ignored);
        return this;
    }
}
