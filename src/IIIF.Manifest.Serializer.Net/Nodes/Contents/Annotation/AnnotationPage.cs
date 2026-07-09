using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation
{
    /// <summary>
    /// IIIF Presentation API 3.0 AnnotationPage. Groups the Annotations painted onto a Canvas
    /// (via <see cref="BaseNode{TBaseNode}.Items"/>), or stands in as a reference to an external
    /// annotation list (the 3.0 replacement for a 2.x Canvas.otherContent reference).
    /// </summary>
    [PresentationAPI("3.0", Notes = "No direct 2.x equivalent as a concrete type; 2.x inlined images/otherContent directly on Canvas.")]
    public class AnnotationPage : BaseNode<AnnotationPage>
    {
        [JsonConstructor]
        public AnnotationPage(string id) : base(id, "AnnotationPage")
        {
        }
    }
}
