using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Structure
{
    /// <summary>
    /// IIIF Range/Structure resource - represents a structural section of a Manifest.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Called 'structures' in 2.x, 'Range' in 3.0. Canvases/ranges arrays deprecated in 3.0.")]
    [JsonConverter(typeof(StructureJsonConverter))]
    public class Structure : BaseNode<Structure>, IViewingDirectionSupport<Structure>
    {
        public const string CanvasesJName = "canvases";
        public const string RangesJName = "ranges";
        public const string StartCanvasJName = "startCanvas";
        public const string MembersJName = "members";

        private readonly List<string> canvases = new List<string>();
        private readonly List<string> ranges = new List<string>();
        private readonly List<object> members = new List<object>();

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<string> Canvases => canvases.AsReadOnly();

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(RangesJName)]
        public IReadOnlyCollection<string> Ranges => ranges.AsReadOnly();

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members => members.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(StartCanvasJName)]
        public string StartCanvas { get; private set; }

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection ViewingDirection { get; private set; }

        public Structure(string id) : base(id, "sc:Range")
        {
        }

        public Structure(string id, Label label) : this(id) => AddLabel(label);

        public Structure AddCanvas(string canvas) => SetPropertyValue(a => a.canvases, a => a.Canvases, canvases.Attach(canvas));
        public Structure RemoveCanvas(string canvas) => SetPropertyValue(a => a.canvases, a => a.Canvases, canvases.Detach(canvas));

        public Structure AddRange(string range) => SetPropertyValue(a => a.ranges, a => a.Ranges, ranges.Attach(range));
        public Structure RemoveRange(string range) => SetPropertyValue(a => a.ranges, a => a.Ranges, ranges.Detach(range));

        public Structure AddMember(object member) => SetPropertyValue(a => a.members, a => a.Members, members.Attach(member));
        public Structure RemoveMember(object member) => SetPropertyValue(a => a.members, a => a.Members, members.Detach(member));

        public Structure SetStartCanvas(string startCanvas) => SetPropertyValue(a => a.StartCanvas, startCanvas);
        public Structure SetViewingDirection(ViewingDirection viewingDirection) => SetPropertyValue(a => a.ViewingDirection, viewingDirection);
    }
}