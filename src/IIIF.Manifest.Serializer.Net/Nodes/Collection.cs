using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    /// <summary>
    /// IIIF Collection resource - an ordered list of Manifests and/or Collections.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Supported in 2.x and 3.0. Paging properties added in 2.0.")]
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
        public ViewingDirection? ViewingDirection
        {
            get => GetElementValue(x => x.ViewingDirection);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CollectionsJName)]
        public IReadOnlyCollection<Collection> Collections
        {
            get => GetElementValue(x => x.Collections) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ManifestsJName)]
        public IReadOnlyCollection<string> Manifests
        {
            get => GetElementValue(x => x.Manifests) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members
        {
            get => GetElementValue(x => x.Members) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(TotalJName)]
        public int? Total
        {
            get => GetElementValue(x => x.Total);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(FirstJName)]
        public string? First
        {
            get => GetElementValue(x => x.First);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(LastJName)]
        public string? Last
        {
            get => GetElementValue(x => x.Last);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(NextJName)]
        public string? Next
        {
            get => GetElementValue(x => x.Next);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(PrevJName)]
        public string? Prev
        {
            get => GetElementValue(x => x.Prev);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", Notes = "Paging property")]
        [JsonProperty(StartIndexJName)]
        public int? StartIndex
        {
            get => GetElementValue(x => x.StartIndex);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        internal Collection(string id) : base(id, "sc:Collection")
        {
        }

        public Collection(string id, Label label) : this(id) => AddLabel(label);
        public Collection(string id, IReadOnlyCollection<Label> labels) : this(id) => SetLabel(labels);

        public Collection SetViewingDirection(ViewingDirection viewingDirection)
        {
            ViewingDirection = viewingDirection;
            return this;
        }

        public Collection SetTotal(int total)
        {
            Total = total;
            return this;
        }

        public Collection SetFirst(string first)
        {
            First = first;
            return this;
        }

        public Collection SetLast(string last)
        {
            Last = last;
            return this;
        }

        public Collection SetNext(string next)
        {
            Next = next;
            return this;
        }

        public Collection SetPrev(string prev)
        {
            Prev = prev;
            return this;
        }

        public Collection SetStartIndex(int startIndex)
        {
            StartIndex = startIndex;
            return this;
        }

        public Collection AddCollection(Collection collection)
        {
            Collections = Collections.With(collection);
            return this;
        }

        public Collection RemoveCollection(Collection collection)
        {
            Collections = Collections.Without(collection);
            return this;
        }

        public Collection AddManifest(string manifestId)
        {
            Manifests = Manifests.With(manifestId);
            return this;
        }

        public Collection RemoveManifest(string manifestId)
        {
            Manifests = Manifests.Without(manifestId);
            return this;
        }

        public Collection AddMember(object member)
        {
            Members = Members.With(member);
            return this;
        }

        public Collection RemoveMember(object member)
        {
            Members = Members.Without(member);
            return this;
        }
    }
}