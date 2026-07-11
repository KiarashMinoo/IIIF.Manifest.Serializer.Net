using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation;

/// <summary>
/// The W3C Web Annotation Protocol's "AnnotationCollection" - a paged list of Annotations, distinct
/// from both the IIIF <see cref="Nodes.Collection"/> (a list of Manifests/Collections) and
/// <see cref="AnnotationPage"/> (one page of that list). Cookbook recipe 0309-annotation-collection
/// is its own standalone top-level document (<c>anno_coll.json</c>), separate from the Manifest
/// that references it via each <see cref="AnnotationPage"/>'s <c>partOf</c>.
/// </summary>
[PresentationAPI("3.0", Notes = "W3C Web Annotation Protocol paging concept, distinct from the IIIF Collection resource.")]
public class AnnotationCollection : BaseNode<AnnotationCollection>
{
    public const string TotalJName = "total";
    public const string FirstJName = "first";
    public const string LastJName = "last";

    [JsonProperty(TotalJName)]
    public int? Total
    {
        get => GetElementValue(x => x.Total);
        private set => SetElementValue(value);
    }

    [JsonProperty(FirstJName)]
    public string? First
    {
        get => GetElementValue(x => x.First);
        private set => SetElementValue(value);
    }

    [JsonProperty(LastJName)]
    public string? Last
    {
        get => GetElementValue(x => x.Last);
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    internal AnnotationCollection(string id) : base(id, "AnnotationCollection")
    {
    }

    public AnnotationCollection(string id, Label label) : this(id) => AddLabel(label);

    public AnnotationCollection SetTotal(int total)
    {
        Total = total;
        return this;
    }

    public AnnotationCollection SetFirst(string first)
    {
        First = first;
        return this;
    }

    public AnnotationCollection SetLast(string last)
    {
        Last = last;
        return this;
    }
}
