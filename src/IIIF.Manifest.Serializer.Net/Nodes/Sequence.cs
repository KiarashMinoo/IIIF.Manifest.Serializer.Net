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
    /// IIIF Sequence resource - an ordered list of Canvases.
    /// </summary>
    /// <remarks>
    /// Deprecated in IIIF Presentation API 3.0. Use items property on Manifest instead.
    /// </remarks>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0",
        ReplacedBy = "Manifest.items", Notes = "Sequences removed in API 3.0; canvases moved to items array")]
    public class Sequence : BaseNode<Sequence>, IViewingDirectionSupport<Sequence>
    {
        public const string StartCanvasJName = "startCanvas";
        public const string CanvasesJName = "canvases";

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(CanvasesJName)]
        public IReadOnlyCollection<Canvas> Canvases
        {
            get => GetElementValue(x => x.Canvases) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "start")]
        [JsonProperty(StartCanvasJName)]
        public StartCanvas? StartCanvas
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

        [JsonConstructor]
        public Sequence() : this(string.Empty)
        {
        }

        public Sequence(string id) : base(id, "sc:Sequence")
        {
        }

        public Sequence AddCanvas(Canvas canvas)
        {
            Canvases = Canvases.With(canvas);
            return this;
        }

        public Sequence RemoveCanvas(Canvas canvas)
        {
            Canvases = Canvases.Without(canvas);
            return this;
        }

        public Sequence SetStartCanvas(StartCanvas startCanvas)
        {
            StartCanvas = startCanvas;
            return this;
        }

        public Sequence SetViewingDirection(ViewingDirection viewingDirection)
        {
            ViewingDirection = viewingDirection;
            return this;
        }
    }
}