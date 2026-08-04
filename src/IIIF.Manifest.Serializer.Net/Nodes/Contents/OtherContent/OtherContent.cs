using IIIF.Manifests.Serializer.Shared.Content;

namespace IIIF.Manifests.Serializer.Nodes.Contents.OtherContent;

public sealed class OtherContent(string id) : BaseContent<OtherContent>(id, "sc:AnnotationList")
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private OtherContent() : this(null!)
    {
    }
}