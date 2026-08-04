using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes;

/// <summary>
///     Minimal reference to a Range (Structure) by id. Used within a Range's Items when only
///     the id is known — 2.x Structure.ranges was always a bare id list, never an embedded
///     Range object.
/// </summary>
public sealed class RangeReference : BaseItem<RangeReference>
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private RangeReference()
    {
    }

    [JsonConstructor]
    public RangeReference(string id) : base(id, "Range")
    {
    }
}