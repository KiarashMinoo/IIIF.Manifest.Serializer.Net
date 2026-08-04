using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF AccompanyingCanvas property - references a canvas that accompanies the manifest.
/// </summary>
[PresentationAPI("2.0")]
public sealed class AccompanyingCanvas(string id) : BaseItem<AccompanyingCanvas>(id, "sc:Canvas")
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private AccompanyingCanvas() : this(null!)
    {
    }
}