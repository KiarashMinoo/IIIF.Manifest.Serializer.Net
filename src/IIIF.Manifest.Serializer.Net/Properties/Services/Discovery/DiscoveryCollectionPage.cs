using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Discovery;

/// <summary>
/// A Change Discovery API 1.0 "OrderedCollectionPage" - a single page of <see cref="Activity"/>
/// entries. Distinct from the top-level <see cref="DiscoveryService"/> (which models the
/// "OrderedCollection" and only ever *points at* pages via <c>first</c>/<c>last</c>, never embeds
/// their <c>orderedItems</c> directly) - the two were previously conflated into one class.
/// </summary>
[DiscoveryAPI("1.0")]
public class DiscoveryCollectionPage : TrackableObject<DiscoveryCollectionPage>
{
    public const string DefaultContext = "http://iiif.io/api/discovery/1/context.json";
    public const string ContextJName = "@context";
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string PartOfJName = "partOf";
    public const string NextJName = "next";
    public const string PrevJName = "prev";
    public const string StartIndexJName = "startIndex";
    public const string OrderedItemsJName = "orderedItems";

    [JsonProperty(ContextJName)]
    public string? Context
    {
        get => GetElementValue(x => x.Context);
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
        get => GetElementValue(x => x.Type) ?? "OrderedCollectionPage";
        private set => SetElementValue(value);
    }

    [JsonProperty(PartOfJName)]
    public DiscoveryResourceReference? PartOf
    {
        get => GetElementValue(x => x.PartOf);
        private set => SetElementValue(value);
    }

    [JsonProperty(NextJName)]
    public DiscoveryResourceReference? Next
    {
        get => GetElementValue(x => x.Next);
        private set => SetElementValue(value);
    }

    [JsonProperty(PrevJName)]
    public DiscoveryResourceReference? Prev
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

    [JsonProperty(OrderedItemsJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Activity> OrderedItems
    {
        get => GetElementValue(x => x.OrderedItems) ?? [];
        private set => SetElementValue(value);
    }

    public DiscoveryCollectionPage(string id, IReadOnlyCollection<Activity> orderedItems)
    {
        Type = "OrderedCollectionPage";
        Id = id;
        OrderedItems = orderedItems;
    }

    public DiscoveryCollectionPage SetContext(string context)
    {
        Context = context;
        return this;
    }

    public DiscoveryCollectionPage SetPartOf(DiscoveryResourceReference partOf)
    {
        PartOf = partOf;
        return this;
    }

    public DiscoveryCollectionPage SetNext(DiscoveryResourceReference next)
    {
        Next = next;
        return this;
    }

    public DiscoveryCollectionPage SetPrev(DiscoveryResourceReference prev)
    {
        Prev = prev;
        return this;
    }

    public DiscoveryCollectionPage SetStartIndex(int startIndex)
    {
        StartIndex = startIndex;
        return this;
    }

    public DiscoveryCollectionPage AddActivity(Activity activity)
    {
        OrderedItems = OrderedItems.With(activity);
        return this;
    }
}
