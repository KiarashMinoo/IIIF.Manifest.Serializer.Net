using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Cookbook Group G remainder: recipe 0309-annotation-collection wires an externally-referenced
///     <see cref="AnnotationPage" /> back to its owning <see cref="AnnotationCollection" /> via
///     PartOf (already generically available on every BaseNode, just not previously wired into the
///     hand-rolled Canvas writer/reader) plus W3C-style <see cref="AnnotationPage.Next" />/
///     <see cref="AnnotationPage.Prev" /> paging between sibling pages.
/// </summary>
public class AnnotationPagePartOfTests
{
    [Fact]
    public void Canvas_Should_RoundTripAnnotationPageReferenceWithPartOfAndNext()
    {
        var image = new ImageResource("https://example.org/p1/full/max/0/default.jpg", "image/jpeg");
        var painting = new Annotation("https://example.org/annotation/p1", image, "https://example.org/canvas/p1");
        var canvas = new Canvas("https://example.org/canvas/p1", new Label("p. 1"), 5000, 3602).AddAnnotation(painting);

        var externalPage = new AnnotationPage("https://example.org/anno_p1.json")
            .AddPartOf(new PartOf("https://example.org/anno_coll.json", "AnnotationCollection"))
            .SetNext("https://example.org/anno_p2.json");
        canvas.AddAnnotationPageReference(externalPage);

        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var annotationsRef = obj["items"]![0]!["annotations"]![0]!;

        annotationsRef["id"]!.ToString().Should().Be("https://example.org/anno_p1.json");
        annotationsRef["type"]!.ToString().Should().Be("AnnotationPage");
        annotationsRef["partOf"]![0]!["id"]!.ToString().Should().Be("https://example.org/anno_coll.json");
        annotationsRef["partOf"]![0]!["type"]!.ToString().Should().Be("AnnotationCollection");
        annotationsRef["next"]!.ToString().Should().Be("https://example.org/anno_p2.json");
        annotationsRef["prev"].Should().BeNull();

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedPage = ((Canvas)deserialized.Items.Single()).Annotations.Single();

        roundTrippedPage.Id.Should().Be("https://example.org/anno_p1.json");
        roundTrippedPage.PartOf.Should().ContainSingle(x => x.Id == "https://example.org/anno_coll.json" && x.Type == "AnnotationCollection");
        roundTrippedPage.Next.Should().Be("https://example.org/anno_p2.json");
        roundTrippedPage.Prev.Should().BeNull();
    }

    [Fact]
    public void Canvas_Should_RoundTripAnnotationPageReferenceWithPrevOnly()
    {
        var canvas = new Canvas("https://example.org/canvas/p2", new Label("p. 2"), 5000, 3602);
        var externalPage = new AnnotationPage("https://example.org/anno_p2.json").SetPrev("https://example.org/anno_p1.json");
        canvas.AddAnnotationPageReference(externalPage);

        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedPage = ((Canvas)deserialized.Items.Single()).Annotations.Single();

        roundTrippedPage.Prev.Should().Be("https://example.org/anno_p1.json");
        roundTrippedPage.Next.Should().BeNull();
        roundTrippedPage.PartOf.Should().BeEmpty();
    }
}