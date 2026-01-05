using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Sequence
{
    [JsonConverter(typeof(SequenceJsonConverter))]
    public class Sequence : BaseNode<Sequence>, IViewingDirectionSupport<Sequence>
    {
        public const string StartCanvasJName = "startCanvas";
        public const string CanvasesJName = "canvases";

        private readonly List<Canvas.Canvas> canvases = new List<Canvas.Canvas>();

        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<Canvas.Canvas> Canvases => canvases.AsReadOnly();

        [JsonProperty(StartCanvasJName)]
        public StartCanvas StartCanvas { get; private set; }

        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection ViewingDirection { get; private set; }

        public Sequence() : this(string.Empty)
        {
        }

        public Sequence(string id) : base(id, "sc:Sequence")
        {
        }

        public Sequence AddCanvas(Canvas.Canvas canvas) => SetPropertyValue(a => a.canvases, a => a.Canvases, canvases.Attach(canvas));
        public Sequence RemoveCanvas(Canvas.Canvas canvas) => SetPropertyValue(a => a.canvases, a => a.Canvases, canvases.Detach(canvas));

        public Sequence SetStartCanvas(StartCanvas startCanvas) => SetPropertyValue(a => a.StartCanvas, startCanvas);
        public Sequence SetViewingDirection(ViewingDirection viewingDirection) => SetPropertyValue(a => a.ViewingDirection, viewingDirection);
    }
}