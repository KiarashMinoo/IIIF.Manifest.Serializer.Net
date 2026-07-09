using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    /// <summary>
    /// Minimal reference to a Range (Structure) by id. Used within a Range's Items when only
    /// the id is known — 2.x Structure.ranges was always a bare id list, never an embedded
    /// Range object.
    /// </summary>
    public class RangeReference : BaseItem<RangeReference>
    {
        [JsonConstructor]
        public RangeReference(string id) : base(id, "Range")
        {
        }
    }
}
