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
    /// IIIF Range/Structure resource - represents a structural section of a Manifest.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Called 'structures' in 2.x, 'Range' in 3.0. Canvases/ranges arrays deprecated in 3.0.")]
    [method: JsonConstructor]
    public class Structure(string id) : BaseNode<Structure>(id, "sc:Range"), IViewingDirectionSupport<Structure>
    {
        public const string CanvasesJName = "canvases";
        public const string RangesJName = "ranges";
        public const string StartCanvasJName = "startCanvas";
        public const string MembersJName = "members";

        /// <summary>
        /// Legacy (2.x) view of referenced canvas ids. Computed from
        /// <see cref="BaseNode{TBaseNode}.Items"/> (the 3.0-native storage) — each id backs a
        /// minimal <see cref="CanvasReference"/> (2.x only ever carried the bare id).
        /// </summary>
        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<string> Canvases
        {
            get => Items.OfType<CanvasReference>().Select(x => x.Id).ToList();
            private set => ReplaceItemsOfType<CanvasReference>((value ?? []).Select(id => new CanvasReference(id)));
        }

        /// <summary>
        /// Legacy (2.x) view of referenced range ids. Computed from
        /// <see cref="BaseNode{TBaseNode}.Items"/> — includes both bare
        /// <see cref="RangeReference"/> entries (from a legacy read) and the id of any nested
        /// <see cref="Structure"/> added via the 3.0-preferred API.
        /// </summary>
        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(RangesJName)]
        public IReadOnlyCollection<string> Ranges
        {
            get => Items.Select(x => x switch
                {
                    RangeReference r => r.Id,
                    Structure s => s.Id,
                    _ => null
                })
                .OfType<string>()
                .ToList();
            private set => ReplaceItemsOfType<RangeReference>((value ?? []).Select(id => new RangeReference(id)));
        }

        /// <summary>
        /// Legacy (2.1) view of the ordered, mixed canvas/range list. Computed directly from
        /// <see cref="BaseNode{TBaseNode}.Items"/>.
        /// </summary>
        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members
        {
            get => Items.Select(x => (object)x).ToList();
            private set => ReplaceMembers(value ?? []);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(StartCanvasJName)]
        public string? StartCanvas
        {
            get => GetElementValue(x => x.StartCanvas);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection? ViewingDirection
        {
            get => GetElementValue(x => x.ViewingDirection);
            private set => SetElementValue(value);
        }

        public Structure(string id, Label label) : this(id) => AddLabel(label);

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddCanvasReference instead.", error: true)]
        public Structure AddCanvas(string canvas)
        {
            AddItem(new CanvasReference(canvas));
            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddCanvasReference instead.", error: true)]
        public Structure RemoveCanvas(string canvas)
        {
            RemoveCanvasReference(canvas);
            return this;
        }

        /// <summary>
        /// Adds a reference to a Canvas by id — the 3.0-preferred replacement for the legacy
        /// bare-id <c>canvases</c> array.
        /// </summary>
        public Structure AddCanvasReference(string canvasId)
        {
            AddItem(new CanvasReference(canvasId));
            return this;
        }

        public Structure RemoveCanvasReference(string canvasId)
        {
            var existing = Items.OfType<CanvasReference>().FirstOrDefault(x => x.Id == canvasId);
            if (existing is not null)
            {
                RemoveItem(existing);
            }

            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddRangeReference instead.", error: true)]
        public Structure AddRange(string range)
        {
            AddItem(new RangeReference(range));
            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddRangeReference instead.", error: true)]
        public Structure RemoveRange(string range)
        {
            RemoveRangeReference(range);
            return this;
        }

        /// <summary>
        /// Adds a reference to another Range by id — the 3.0-preferred replacement for the
        /// legacy bare-id <c>ranges</c> array. To embed a full nested Range instead, use
        /// <see cref="BaseNode{TBaseNode}.AddItem{TItem}"/> with a <see cref="Structure"/>.
        /// </summary>
        public Structure AddRangeReference(string rangeId)
        {
            AddItem(new RangeReference(rangeId));
            return this;
        }

        public Structure RemoveRangeReference(string rangeId)
        {
            var existing = Items.OfType<RangeReference>().FirstOrDefault(x => x.Id == rangeId);
            if (existing is not null)
            {
                RemoveItem(existing);
            }

            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct items directly via AddItem instead.", error: true)]
        public Structure AddMember(object member)
        {
            AddMemberCore(member);
            return this;
        }

        [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct items directly via AddItem instead.", error: true)]
        public Structure RemoveMember(object member)
        {
            switch (member)
            {
                case CanvasReference or RangeReference or Structure:
                    RemoveItem((IBaseItem)member);
                    break;
                case string id:
                    RemoveCanvasReference(id);
                    break;
            }

            return this;
        }

        public Structure SetStartCanvas(string startCanvas)
        {
            StartCanvas = startCanvas;
            return this;
        }

        public Structure SetViewingDirection(ViewingDirection viewingDirection)
        {
            ViewingDirection = viewingDirection;
            return this;
        }

        private void AddMemberCore(object member)
        {
            switch (member)
            {
                case CanvasReference canvasRef:
                    AddItem(canvasRef);
                    break;
                case RangeReference rangeRef:
                    AddItem(rangeRef);
                    break;
                case Structure nested:
                    AddItem(nested);
                    break;
                case string id:
                    AddItem(new CanvasReference(id));
                    break;
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
                AddMemberCore(member);
            }
        }
    }
}
