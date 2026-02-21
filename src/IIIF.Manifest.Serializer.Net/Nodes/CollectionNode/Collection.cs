using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.CollectionNode
{
    /// <summary>
    /// IIIF Collection resource - an ordered list of Manifests and/or Collections.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Supported in 2.x and 3.0. Paging properties added in 2.0.")]
    [JsonConverter(typeof(CollectionJsonConverter))]
    public class Collection : BaseNode<Collection>, IViewingDirectionSupport<Collection>
    {
        public const string CollectionsJName = "collections";
        public const string ManifestsJName = "manifests";
        public const string MembersJName = "members";
        public const string TotalJName = "total";
        public const string FirstJName = "first";
        public const string LastJName = "last";
        public const string NextJName = "next";
        public const string PrevJName = "prev";
        public const string StartIndexJName = "startIndex";

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection? ViewingDirection => GetElementValue(x => x.ViewingDirection);

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CollectionsJName)]
        public IReadOnlyCollection<Collection> Collections => GetElementValue(x => x.Collections) ?? [];

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ManifestsJName)]
        public IReadOnlyCollection<string> Manifests => GetElementValue(x => x.Manifests) ?? [];

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members => GetElementValue(x => x.Members) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(TotalJName)]
        public int? Total => GetElementValue(x => x.Total);

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(FirstJName)]
        public string? First => GetElementValue(x => x.First);

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(LastJName)]
        public string? Last => GetElementValue(x => x.Last);

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(NextJName)]
        public string? Next => GetElementValue(x => x.Next);

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(PrevJName)]
        public string? Prev => GetElementValue(x => x.Prev);

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(StartIndexJName)]
        public int? StartIndex => GetElementValue(x => x.StartIndex);

        internal Collection(string id) : base(id, "sc:Collection")
        {
        }

        public Collection(string id, Label label) : this(id) => AddLabel(label);
        public Collection(string id, IEnumerable<Label> labels) : this(id) => labels.Enumerate(label => AddLabel(label));

        public Collection SetViewingDirection(ViewingDirection viewingDirection) => SetElementValue(a => a.ViewingDirection, viewingDirection);
        public Collection SetTotal(int total) => SetElementValue(a => a.Total, total);
        public Collection SetFirst(string first) => SetElementValue(a => a.First, first);
        public Collection SetLast(string last) => SetElementValue(a => a.Last, last);
        public Collection SetNext(string next) => SetElementValue(a => a.Next, next);
        public Collection SetPrev(string prev) => SetElementValue(a => a.Prev, prev);
        public Collection SetStartIndex(int startIndex) => SetElementValue(a => a.StartIndex, startIndex);

        public Collection AddCollection(Collection collection) => SetElementValue(a => a.Collections, x => x.With(collection));
        public Collection RemoveCollection(Collection collection) => SetElementValue(a => a.Collections, x => x.Without(collection));

        public Collection AddManifest(string manifestId) => SetElementValue(a => a.Manifests, collection => collection.With(manifestId));
        public Collection RemoveManifest(string manifestId) => SetElementValue(a => a.Manifests, collection => collection.With(manifestId));

        public Collection AddMember(object member) => SetElementValue(a => a.Members, collection => collection.With(member));
        public Collection RemoveMember(object member) => SetElementValue(a => a.Members, collection => collection.With(member));
    }
}