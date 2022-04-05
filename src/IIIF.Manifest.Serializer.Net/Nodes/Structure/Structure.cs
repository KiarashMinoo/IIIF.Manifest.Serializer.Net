using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Nodes
{
    public class Structure : BaseNode<Structure>
    {
        private readonly List<string> canvases = new List<string>();
        private readonly List<string> ranges = new List<string>();

        public IReadOnlyCollection<string> Canvases => canvases.AsReadOnly();
        public IReadOnlyCollection<string> Ranges => ranges.AsReadOnly();
        public string StartCanvas { get; private set; }

        public Structure(string id) : base(id, "sc:Range")
        {
        }

        public Structure AddCanvas(string canvas) => SetPropertyValue(a => a.canvases, a => a.Canvases, canvases.Attach(canvas));
        public Structure RemoveCanvas(string canvas) => SetPropertyValue(a => a.canvases, a => a.Canvases, canvases.Detach(canvas));

        public Structure AddRange(string range) => SetPropertyValue(a => a.ranges, a => a.Ranges, ranges.Attach(range));
        public Structure RemoveRange(string range) => SetPropertyValue(a => a.ranges, a => a.Ranges, ranges.Detach(range));

        public Structure SetStartCanvas(string startCanvas) => SetPropertyValue(a => a.StartCanvas, startCanvas);
    }
}