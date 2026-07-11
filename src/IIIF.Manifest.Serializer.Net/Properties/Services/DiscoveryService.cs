using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Services.Discovery;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services
{
    /// <summary>
    /// IIIF Change Discovery API 1.0 - the top-level "OrderedCollection" activity stream. Only
    /// *points at* its pages via <see cref="First"/>/<see cref="Last"/> - the activities themselves
    /// live on a separate <see cref="DiscoveryCollectionPage"/> ("OrderedCollectionPage"), which
    /// this class previously conflated itself with (it wrongly carried its own "orderedItems").
    /// Has no "profile" field per spec.
    /// </summary>
    [DiscoveryAPI("1.0", Notes = "Change Discovery API 1.0 top-level OrderedCollection.")]
    public class DiscoveryService : UnprefixedBaseItem<DiscoveryService>, IBaseService
    {
        public const string FirstJName = "first";
        public const string LastJName = "last";
        public const string TotalItemsJName = "totalItems";
        public const string SeeAlsoJName = "seeAlso";
        public const string PartOfJName = "partOf";
        public const string RightsJName = "rights";

        string IBaseService.Profile => string.Empty;

        [DiscoveryAPI("1.0")]
        [JsonProperty(FirstJName)]
        public DiscoveryResourceReference? First
        {
            get => GetElementValue(x => x.First);
            private set => SetElementValue(value);
        }

        [DiscoveryAPI("1.0")]
        [JsonProperty(LastJName)]
        public DiscoveryResourceReference Last
        {
            get => GetElementValue(x => x.Last)!;
            private set => SetElementValue(value);
        }

        [DiscoveryAPI("1.0")]
        [JsonProperty(TotalItemsJName)]
        public int? TotalItems
        {
            get => GetElementValue(x => x.TotalItems);
            private set => SetElementValue(value);
        }

        [DiscoveryAPI("1.0")]
        [JsonProperty(SeeAlsoJName)]
        [JsonConverter(typeof(ObjectArrayJsonConverter))]
        public IReadOnlyCollection<DiscoveryDataset> SeeAlso
        {
            get => GetElementValue(x => x.SeeAlso) ?? [];
            private set => SetElementValue(value);
        }

        [DiscoveryAPI("1.0")]
        [JsonProperty(PartOfJName)]
        [JsonConverter(typeof(ObjectArrayJsonConverter))]
        public IReadOnlyCollection<DiscoveryResourceReference> PartOf
        {
            get => GetElementValue(x => x.PartOf) ?? [];
            private set => SetElementValue(value);
        }

        [DiscoveryAPI("1.0")]
        [JsonProperty(RightsJName)]
        public string? Rights
        {
            get => GetElementValue(x => x.Rights);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public DiscoveryService(string id, DiscoveryResourceReference last) : base(id, "OrderedCollection")
        {
            Last = last;
        }

        public DiscoveryService(string context, string id, DiscoveryResourceReference last) : base(id, "OrderedCollection", context)
        {
            Last = last;
        }

        public DiscoveryService SetFirst(DiscoveryResourceReference first)
        {
            First = first;
            return this;
        }

        public DiscoveryService SetTotalItems(int totalItems)
        {
            TotalItems = totalItems;
            return this;
        }

        public DiscoveryService AddSeeAlso(DiscoveryDataset dataset)
        {
            SeeAlso = SeeAlso.With(dataset);
            return this;
        }

        public DiscoveryService AddPartOf(DiscoveryResourceReference partOf)
        {
            PartOf = PartOf.With(partOf);
            return this;
        }

        public DiscoveryService SetRights(string rights)
        {
            Rights = rights;
            return this;
        }
    }
}
