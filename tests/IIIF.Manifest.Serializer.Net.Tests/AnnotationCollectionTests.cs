using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Cookbook Group F: the W3C AnnotationCollection - recipe 0309-annotation-collection's standalone
///     <c>anno_coll.json</c> document (distinct from the IIIF <see cref="Collection" /> resource type).
/// </summary>
public class AnnotationCollectionTests
{
    [Fact]
    public void AnnotationCollection_Should_RoundTripThroughIiifSerializer()
    {
        var annotationCollection = new AnnotationCollection("https://example.org/anno_coll.json", new Label("Newspaper layout markup"))
            .SetTotal(8)
            .SetFirst("https://example.org/anno_p1.json")
            .SetLast("https://example.org/anno_p2.json");

        var json = IiifSerializer.Serialize(annotationCollection);
        var obj = JObject.Parse(json);

        obj["type"]!.ToString().Should().Be("AnnotationCollection");
        obj["label"]!["none"]![0]!.ToString().Should().Be("Newspaper layout markup");
        obj["total"]!.Value<int>().Should().Be(8);
        obj["first"]!.ToString().Should().Be("https://example.org/anno_p1.json");
        obj["last"]!.ToString().Should().Be("https://example.org/anno_p2.json");

        var deserialized = IiifSerializer.DeserializeAnnotationCollection(json);
        deserialized.Id.Should().Be("https://example.org/anno_coll.json");
        deserialized.Total.Should().Be(8);
        deserialized.First.Should().Be("https://example.org/anno_p1.json");
        deserialized.Last.Should().Be("https://example.org/anno_p2.json");
        deserialized.Label.Should().ContainSingle(x => x.Value == "Newspaper layout markup");
    }

    [Fact]
    public void AnnotationCollection_Should_OmitPagingPropertiesWhenNotSet()
    {
        var annotationCollection = new AnnotationCollection("https://example.org/anno_coll.json", new Label("Minimal"));

        var json = IiifSerializer.Serialize(annotationCollection);
        var obj = JObject.Parse(json);

        obj["total"].Should().BeNull();
        obj["first"].Should().BeNull();
        obj["last"].Should().BeNull();
    }
}