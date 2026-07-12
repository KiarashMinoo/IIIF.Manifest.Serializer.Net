using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes;

/// <summary>
///     Minimal reference to a Canvas by id. Used within a Range's Items when only the id is
///     known — 2.x Structure.canvases was always a bare id list, never an embedded Canvas object.
/// </summary>
public class CanvasReference : BaseItem<CanvasReference>
{
    [JsonConstructor]
    public CanvasReference(string id) : base(id, "Canvas")
    {
    }
}