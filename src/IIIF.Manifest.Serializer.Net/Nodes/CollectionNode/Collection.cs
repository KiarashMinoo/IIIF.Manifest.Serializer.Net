using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Collection
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

        private readonly List<Collection> collections = new List<Collection>();
        private readonly List<string> manifests = new List<string>();
        private readonly List<object> members = new List<object>();

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection ViewingDirection { get; private set; }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CollectionsJName)]
        public IReadOnlyCollection<Collection> Collections => collections.AsReadOnly();

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ManifestsJName)]
        public IReadOnlyCollection<string> Manifests => manifests.AsReadOnly();

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members => members.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(TotalJName)]
        public int? Total { get; private set; }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(FirstJName)]
        public string First { get; private set; }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(LastJName)]
        public string Last { get; private set; }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(NextJName)]
        public string Next { get; private set; }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(PrevJName)]
        public string Prev { get; private set; }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(StartIndexJName)]
        public int? StartIndex { get; private set; }

        internal Collection(string id) : base(id, "sc:Collection")
        {
        }

        public Collection(string id, Label label) : this(id) => AddLabel(label);
        public Collection(string id, IEnumerable<Label> labels) : this(id) => labels.Enumerate(label => AddLabel(label));

        public Collection SetViewingDirection(ViewingDirection viewingDirection) => SetPropertyValue(a => a.ViewingDirection, viewingDirection);
        public Collection SetTotal(int total) => SetPropertyValue(a => a.Total, total);
        public Collection SetFirst(string first) => SetPropertyValue(a => a.First, first);
        public Collection SetLast(string last) => SetPropertyValue(a => a.Last, last);
        public Collection SetNext(string next) => SetPropertyValue(a => a.Next, next);
        public Collection SetPrev(string prev) => SetPropertyValue(a => a.Prev, prev);
        public Collection SetStartIndex(int startIndex) => SetPropertyValue(a => a.StartIndex, startIndex);

        public Collection AddCollection(Collection collection) => SetPropertyValue(a => a.collections, a => a.Collections, collections.Attach(collection));
        public Collection RemoveCollection(Collection collection) => SetPropertyValue(a => a.collections, a => a.Collections, collections.Detach(collection));

        public Collection AddManifest(string manifestId) => SetPropertyValue(a => a.manifests, a => a.Manifests, manifests.Attach(manifestId));
        public Collection RemoveManifest(string manifestId) => SetPropertyValue(a => a.manifests, a => a.Manifests, manifests.Detach(manifestId));

        public Collection AddMember(object member) => SetPropertyValue(a => a.members, a => a.Members, members.Attach(member));
        public Collection RemoveMember(object member) => SetPropertyValue(a => a.members, a => a.Members, members.Detach(member));
    }
}

