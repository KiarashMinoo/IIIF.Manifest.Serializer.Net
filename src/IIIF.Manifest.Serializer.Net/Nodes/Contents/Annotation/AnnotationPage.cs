using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation
{
    /// <summary>
    /// IIIF Presentation API 3.0 AnnotationPage. Groups the Annotations painted onto a Canvas
    /// (via <see cref="BaseNode{TBaseNode}.Items"/>), or stands in as a reference to an external
    /// annotation list (the 3.0 replacement for a 2.x Canvas.otherContent reference). Also
    /// <see cref="BaseNode{TBaseNode}.PartOf"/> - used by cookbook recipe 0309-annotation-collection
    /// to point back at the <see cref="AnnotationCollection"/> this page belongs to, alongside
    /// <see cref="Next"/>/<see cref="Prev"/> for W3C-style paging between sibling pages.
    /// </summary>
    [PresentationAPI("3.0", Notes = "No direct 2.x equivalent as a concrete type; 2.x inlined images/otherContent directly on Canvas.")]
    public class AnnotationPage : BaseNode<AnnotationPage>
    {
        public const string NextJName = "next";
        public const string PrevJName = "prev";

        [JsonProperty(NextJName)]
        public string? Next
        {
            get => GetElementValue(x => x.Next);
            private set => SetElementValue(value);
        }

        [JsonProperty(PrevJName)]
        public string? Prev
        {
            get => GetElementValue(x => x.Prev);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public AnnotationPage(string id) : base(id, "AnnotationPage")
        {
        }

        public AnnotationPage SetNext(string next)
        {
            Next = next;
            return this;
        }

        public AnnotationPage SetPrev(string prev)
        {
            Prev = prev;
            return this;
        }
    }
}
