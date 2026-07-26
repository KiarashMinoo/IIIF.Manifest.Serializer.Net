using IIIF.Manifests.Serializer.Shared.Content;

namespace IIIF.Manifests.Serializer.Nodes.Contents.OtherContent;

public sealed class OtherContent(string id) : BaseContent<OtherContent>(id, "sc:AnnotationList");