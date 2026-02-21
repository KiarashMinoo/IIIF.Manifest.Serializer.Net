using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.StructureNode
{
    /// <summary>
    /// IIIF Range/Structure resource - represents a structural section of a Manifest.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Called 'structures' in 2.x, 'Range' in 3.0. Canvases/ranges arrays deprecated in 3.0.")]
    [JsonConverter(typeof(StructureJsonConverter))]
    public class Structure(string id) : BaseNode<Structure>(id, "sc:Range"), IViewingDirectionSupport<Structure>
    {
        public const string CanvasesJName = "canvases";
        public const string RangesJName = "ranges";
        public const string StartCanvasJName = "startCanvas";
        public const string MembersJName = "members";

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<string> Canvases => GetElementValue(x => x.Canvases) ?? [];

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(RangesJName)]
        public IReadOnlyCollection<string> Ranges => GetElementValue(x => x.Ranges) ?? [];

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(MembersJName)]
        public IReadOnlyCollection<object> Members => GetElementValue(x => x.Members) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(StartCanvasJName)]
        public string? StartCanvas => GetElementValue(x => x.StartCanvas);

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection? ViewingDirection => GetElementValue(x => x.ViewingDirection);

        public Structure(string id, Label label) : this(id) => AddLabel(label);

        public Structure AddCanvas(string canvas) => SetElementValue(a => a.Canvases, collection => collection.With(canvas));
        public Structure RemoveCanvas(string canvas) => SetElementValue(a => a.Canvases, collection => collection.Without(canvas));

        public Structure AddRange(string range) => SetElementValue(a => a.Ranges, collection => collection.With(range));
        public Structure RemoveRange(string range) => SetElementValue(a => a.Ranges, collection => collection.Without(range));

        public Structure AddMember(object member) => SetElementValue(a => a.Members, collection => collection.With(member));
        public Structure RemoveMember(object member) => SetElementValue(a => a.Members, collection => collection.Without(member));

        public Structure SetStartCanvas(string startCanvas) => SetElementValue(a => a.StartCanvas, startCanvas);
        public Structure SetViewingDirection(ViewingDirection viewingDirection) => SetElementValue(a => a.ViewingDirection, viewingDirection);
    }
}