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
    /// IIIF Range/Structure resource - represents a structural section of a Manifest.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Called 'structures' in 2.x, 'Range' in 3.0. Canvases/ranges arrays deprecated in 3.0.")]
    public class Structure(string id) : BaseNode<Structure>(id, "sc:Range"), IViewingDirectionSupport<Structure>
    {
        public const string CanvasesJName = "canvases";
        public const string RangesJName = "ranges";
        public const string StartCanvasJName = "startCanvas";
        public const string MembersJName = "members";

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<string> Canvases
        {
            get => GetElementValue(x => x.Canvases) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(RangesJName)]
        public IReadOnlyCollection<string> Ranges
        {
            get => GetElementValue(x => x.Ranges) ?? [];
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

        public Structure AddCanvas(string canvas)
        {
            Canvases = Canvases.With(canvas);
            return this;
        }

        public Structure RemoveCanvas(string canvas)
        {
            Canvases = Canvases.Without(canvas);
            return this;
        }

        public Structure AddRange(string range)
        {
            Ranges = Ranges.With(range);
            return this;
        }

        public Structure RemoveRange(string range)
        {
            Ranges = Ranges.Without(range);
            return this;
        }

        public Structure AddMember(object member)
        {
            Members = Members.With(member);
            return this;
        }

        public Structure RemoveMember(object member)
        {
            Members = Members.Without(member);
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
    }
}