using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes;

/// <summary>
///     Minimal reference to a Canvas by id. Used within a Range's Items when only the id is
///     known — 2.x Structure.canvases was always a bare id list, never an embedded Canvas object.
/// </summary>
public sealed class CanvasReference : BaseItem<CanvasReference>
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private CanvasReference()
    {
    }

    [JsonConstructor]
    public CanvasReference(string id) : base(id, "Canvas")
    {
    }
}