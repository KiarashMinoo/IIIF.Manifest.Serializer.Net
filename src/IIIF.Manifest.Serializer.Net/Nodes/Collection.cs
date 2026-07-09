using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Legacy (2.x) view of nested Collections. Computed from
        /// <see cref="BaseNode{TBaseNode}.Items"/> (the 3.0-native storage).
        /// </summary>
        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CollectionsJName)]
        public IReadOnlyCollection<Collection> Collections
        {
            get => Items.OfType<Collection>().ToList();
            private set => ReplaceItemsOfType(value ?? []);
        }

        /// <summary>
        /// Legacy (2.x) view of referenced Manifest ids. Computed from
        /// <see cref="BaseNode{TBaseNode}.Items"/> — each id backs a minimal 3.0-native
        /// <see cref="Manifest"/> stub (2.x only ever carried the bare id, never a full object).
        /// </summary>
        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ManifestsJName)]
        public IReadOnlyCollection<string> Manifests
        {
            get => Items.OfType<Manifest>().Select(x => x.Id).ToList();
            private set => ReplaceItemsOfType((value ?? []).Select(id => new Manifest(id)));
        }

        /// <summary>
        /// Legacy (2.1) view of the ordered, mixed Manifest/Collection list — the closest 2.x
        /// analogue to 3.0 <c>items</c>. Computed directly from <see cref="BaseNode{TBaseNode}.Items"/>.
        /// </summary>
        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members
        {
            get => Items.Select(x => (object)x).ToList();
            private set => ReplaceMembers(value ?? []);
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

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct items directly via AddItem instead.", error: true)]
        public Collection AddCollection(Collection collection)
        {
            AddItem(collection);
            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct items directly via AddItem instead.", error: true)]
        public Collection RemoveCollection(Collection collection)
        {
            RemoveItem(collection);
            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddManifestReference instead.", error: true)]
        public Collection AddManifest(string manifestId)
        {
            AddItem(new Manifest(manifestId));
            return this;
        }

        /// <summary>
        /// Adds a reference to a Manifest by id — the 3.0-preferred replacement for the legacy
        /// bare-id <c>manifests</c> array. 2.x never carried more than the id for these
        /// references, so this constructs a minimal, label-less Manifest stub internally.
        /// </summary>
        public Collection AddManifestReference(string manifestId)
        {
            AddItem(new Manifest(manifestId));
            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddManifestReference instead.", error: true)]
        public Collection RemoveManifest(string manifestId)
        {
            var existing = Items.OfType<Manifest>().FirstOrDefault(x => x.Id == manifestId);
            if (existing is not null)
            {
                RemoveItem(existing);
            }

            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct items directly via AddItem instead.", error: true)]
        public Collection AddMember(object member)
        {
            switch (member)
            {
                case Manifest manifest:
                    AddItem(manifest);
                    break;
                case Collection collection:
                    AddItem(collection);
                    break;
                case string manifestId:
                    AddItem(new Manifest(manifestId));
                    break;
            }

            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct items directly via AddItem instead.", error: true)]
        public Collection RemoveMember(object member)
        {
            switch (member)
            {
                case Manifest or Collection:
                    RemoveItem((IBaseItem)member);
                    break;
                case string manifestId:
                    RemoveManifestItem(manifestId);
                    break;
            }

            return this;
        }

        private void RemoveManifestItem(string manifestId)
        {
            var existing = Items.OfType<Manifest>().FirstOrDefault(x => x.Id == manifestId);
            if (existing is not null)
            {
                RemoveItem(existing);
            }
        }

        private void ReplaceItemsOfType<TItem>(IEnumerable<TItem> replacements) where TItem : IBaseItem
        {
            foreach (var existing in Items.OfType<TItem>().ToList())
            {
                RemoveItem(existing);
            }

            foreach (var item in replacements)
            {
                AddItem(item);
            }
        }

        private void ReplaceMembers(IEnumerable<object> members)
        {
            foreach (var existing in Items.ToList())
            {
                RemoveItem(existing);
            }

            foreach (var member in members)
            {
                switch (member)
                {
                    case Manifest manifest:
                        AddItem(manifest);
                        break;
                    case Collection collection:
                        AddItem(collection);
                        break;
                    case string manifestId:
                        AddItem(new Manifest(manifestId));
                        break;
                }
            }
        }
    }
}