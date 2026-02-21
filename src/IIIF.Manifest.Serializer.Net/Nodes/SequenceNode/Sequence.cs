using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.SequenceNode
{
    /// <summary>
    /// IIIF Sequence resource - an ordered list of Canvases.
    /// </summary>
    /// <remarks>
    /// Deprecated in IIIF Presentation API 3.0. Use items property on Manifest instead.
    /// </remarks>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0",
        ReplacedBy = "Manifest.items", Notes = "Sequences removed in API 3.0; canvases moved to items array")]
    [JsonConverter(typeof(SequenceJsonConverter))]
    public class Sequence : BaseNode<Sequence>, IViewingDirectionSupport<Sequence>
    {
        public const string StartCanvasJName = "startCanvas";
        public const string CanvasesJName = "canvases";

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<Canvas> Canvases => GetElementValue(x => x.Canvases) ?? [];

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "start")]
        [JsonProperty(StartCanvasJName)]
        public StartCanvas? StartCanvas => GetElementValue(x => x.StartCanvas);

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection? ViewingDirection => GetElementValue(x => x.ViewingDirection);

        public Sequence() : this(string.Empty)
        {
        }

        public Sequence(string id) : base(id, "sc:Sequence")
        {
        }

        public Sequence AddCanvas(Canvas canvas) => SetElementValue(a => a.Canvases, collection => collection.With(canvas));
        public Sequence RemoveCanvas(Canvas canvas) => SetElementValue(a => a.Canvases, collection => collection.Without(canvas));

        public Sequence SetStartCanvas(StartCanvas startCanvas) => SetElementValue(a => a.StartCanvas, startCanvas);
        public Sequence SetViewingDirection(ViewingDirection viewingDirection) => SetElementValue(a => a.ViewingDirection, viewingDirection);
    }
}