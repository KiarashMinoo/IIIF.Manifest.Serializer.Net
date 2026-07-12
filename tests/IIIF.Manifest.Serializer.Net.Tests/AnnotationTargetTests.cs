using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Cookbook Group A: generalizes Annotation.Target (and Manifest.Start, which shares the exact
///     same shape) to a SpecificResource wrapping a source + selector (FragmentSelector/PointSelector/
///     ImageApiSelector/SvgSelector), matching real cookbook recipes 0015/0040/0068/0135/0261/0299/
///     0306/0309/0326/0540/0599 (verified directly against github.com/IIIF/cookbook-recipes JSON, not
///     just the recipe prose). Implicit string conversion keeps every pre-existing bare-URI call site
///     compiling unchanged.
/// </summary>
public class AnnotationTargetTests
{
    [Fact]
    public void Manifest_Start_Should_WriteBareIdWhenNoSelector()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).SetStart("https://example.org/canvas/1");

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["start"]!.Type.Should().Be(JTokenType.String);
        obj["start"]!.ToString().Should().Be("https://example.org/canvas/1");
    }

    [Fact]
    public void Manifest_Start_Should_RoundTripPointSelectorForTimeOffset()
    {
        // Cookbook recipe 0015-start: start mid-video at t=120.5 seconds.
        var target = new AnnotationTarget("https://example.org/canvas/segment1", "Canvas")
            .SetSelector(PointSelector.ForTemporalPoint(120.5));
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).SetStart(target);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["start"]!["type"]!.ToString().Should().Be("SpecificResource");
        obj["start"]!["selector"]!["type"]!.ToString().Should().Be("PointSelector");
        obj["start"]!["selector"]!["t"]!.Value<double>().Should().Be(120.5);

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.Start!.SourceId.Should().Be("https://example.org/canvas/segment1");
        ((PointSelector)deserialized.Start.Selector!).T.Should().Be(120.5);
    }

    [Fact]
    public void AnnotationTarget_Should_RoundTripPointSelectorForSpatialPoint()
    {
        // Cookbook recipe 0135-annotating-point-in-canvas.
        var target = new AnnotationTarget("https://example.org/canvas.json")
            .SetSelector(PointSelector.ForSpatialPoint(3385, 1464));

        var deserialized = TrackableObject.Parse<AnnotationTarget>(target.Serialize());

        var selector = (PointSelector)deserialized.Selector!;
        selector.X.Should().Be(3385);
        selector.Y.Should().Be(1464);
        selector.T.Should().BeNull();
    }

    [Fact]
    public void AnnotationTarget_Should_RoundTripFragmentSelectorWithNestedPartOf()
    {
        // Cookbook recipe 0306-linking-annotations-to-manifests: target source is a Canvas with
        // partOf pointing back at its Manifest.
        var target = new AnnotationTarget("https://example.org/canvas-1", "Canvas")
            .SetPartOf("https://example.org/manifest.json")
            .SetSelector(FragmentSelector.ForRegion(300, 800, 1200, 1200));

        var obj = JObject.Parse(target.Serialize());
        obj["type"]!.ToString().Should().Be("SpecificResource");
        obj["source"]!["partOf"]![0]!["id"]!.ToString().Should().Be("https://example.org/manifest.json");
        obj["selector"]!["value"]!.ToString().Should().Be("xywh=300,800,1200,1200");

        var deserialized = TrackableObject.Parse<AnnotationTarget>(target.Serialize());
        deserialized.PartOfId.Should().Be("https://example.org/manifest.json");
        ((FragmentSelector)deserialized.Selector!).Value.Should().Be("xywh=300,800,1200,1200");
    }

    [Fact]
    public void AnnotationTarget_Should_RoundTripSvgSelector()
    {
        // Cookbook recipe 0261-non-rectangular-commenting.
        const string svg = "<svg xmlns='http://www.w3.org/2000/svg'><path d='M1,1 L2,2' /></svg>";
        var target = new AnnotationTarget("https://example.org/canvas/p1").SetSelector(new SvgSelector(svg));

        var deserialized = TrackableObject.Parse<AnnotationTarget>(target.Serialize());

        ((SvgSelector)deserialized.Selector!).Value.Should().Be(svg);
    }

    [Fact]
    public void SpecificResource_Should_WriteImageApiSelectorCroppingAnEmbeddedImage()
    {
        // Cookbook recipe 0299-region: body = SpecificResource wrapping an Image, cropped via
        // ImageApiSelector's "region" parameter.
        var image = new ImageResource("https://example.org/full/max/0/default.jpg", "image/jpeg").SetHeight(4999).SetWidth(3536);
        var body = new SpecificResource(image).SetId("https://example.org/body/b1")
            .SetSelector(new ImageApiSelector().SetRegion(1768, 2423, 1768, 2080));
        var annotation = new Annotation("https://example.org/annotation/1", body, "https://example.org/canvas/p1");
        var canvas = new Canvas("https://example.org/canvas/p1", new Label("p1"), 4999, 3536).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyObj = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyObj["type"]!.ToString().Should().Be("SpecificResource");
        bodyObj["source"]!["type"]!.ToString().Should().Be("Image");
        bodyObj["source"]!["id"]!.ToString().Should().Be("https://example.org/full/max/0/default.jpg");
        bodyObj["selector"]!["type"]!.ToString().Should().Be("ImageApiSelector");
        bodyObj["selector"]!["region"]!.ToString().Should().Be("1768,2423,1768,2080");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedBody = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body;
        roundTrippedBody.Should().BeOfType<SpecificResource>();
        ((SpecificResource)roundTrippedBody).Source.Should().BeOfType<ImageResource>();
    }

    [Fact]
    public void Annotation_Should_RoundTripMultipleTargets()
    {
        // Cookbook recipes 0540/0599: a link annotation opening several canvases at once.
        var annotation = new Annotation("https://example.org/annotation/1",
                new EmbeddedContentResource("Compare these pages", "en"),
                "https://example.org/canvas/1")
            .AddTarget("https://example.org/canvas/2");

        annotation.Targets.Should().HaveCount(2);

        var deserialized = TrackableObject.Parse<Annotation>(annotation.Serialize());
        deserialized.Targets.Select(t => t.SourceId).Should().BeEquivalentTo(
            "https://example.org/canvas/1", "https://example.org/canvas/2");
    }
}