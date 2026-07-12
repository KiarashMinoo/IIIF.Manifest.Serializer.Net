using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment.Selector;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     <see cref="EmbeddedContent" />, <see cref="Segment" />,
///     <see cref="IIIF.Manifests.Serializer.Nodes.Contents.Segment.Selector.Selector" />, and
///     <see cref="Layer" /> are all legacy Presentation API 2.x-only concepts with no 3.0 replacement -
///     none of them had any test coverage at all.
/// </summary>
public class LegacyContentTypesTests
{
    [Fact]
    public void EmbeddedContent_Should_ExposeItsOnTargetAndResource()
    {
        var resource = new EmbeddedContentResource("Some transcribed text", "en");
        var content = new EmbeddedContent("https://example.org/annotation/1", resource, "https://example.org/canvas/1");

        content.On.Should().Be("https://example.org/canvas/1");
        content.Resource.Chars.Should().Be("Some transcribed text");
        content.Resource.Language.Should().Be("en");
    }

    [Fact]
    public void EmbeddedContent_Should_RoundTripThroughJsonConvert()
    {
        var resource = new EmbeddedContentResource("Transcript", "en");
        var content = new EmbeddedContent("https://example.org/annotation/1", resource, "https://example.org/canvas/1");

        var json = JsonConvert.SerializeObject(content);
        var obj = JObject.Parse(json);

        obj["@id"]!.ToString().Should().Be("https://example.org/annotation/1");
        obj["@type"]!.ToString().Should().Be("oa:Annotation");
        obj["on"]!.ToString().Should().Be("https://example.org/canvas/1");
        obj["resource"]!["chars"]!.ToString().Should().Be("Transcript");

        var deserialized = JsonConvert.DeserializeObject<EmbeddedContent>(json);
        deserialized!.On.Should().Be("https://example.org/canvas/1");
        deserialized.Resource.Chars.Should().Be("Transcript");
    }

    [Fact]
    public void Segment_Should_DefaultMotivationToScPainting()
    {
        var resource = new SegmentResource("https://example.org/resource/1", ResourceType.Image);
        var segment = new Segment("https://example.org/annotation/1", resource, "https://example.org/canvas/1");

        segment.Motivation.Should().Be("sc:painting");
        segment.On.Should().Be("https://example.org/canvas/1");
    }

    [Fact]
    public void Segment_SetSelector_Should_AttachARegionSelector()
    {
        var resource = new SegmentResource("https://example.org/resource/1", ResourceType.Image);
        var segment = new Segment("https://example.org/annotation/1", resource, "https://example.org/canvas/1");
        var selector = new Selector("https://example.org/selector/1", "oa:FragmentSelector")
            .SetRegion(10, 20, 300, 400);

        segment.SetSelector(selector);

        segment.Selector!.Region.Should().Equal(10, 20, 300, 400);
    }

    [Fact]
    public void Selector_SetRegion_Should_AcceptAnExplicitIntArrayToo()
    {
        var selector = new Selector("https://example.org/selector/1", "oa:FragmentSelector")
            .SetRegion([0, 0, 100, 100]);

        selector.Region.Should().Equal(0, 0, 100, 100);
    }

    [Fact]
    public void Layer_Should_DefaultToTheDeprecatedScLayerType()
    {
        var layer = new Layer("https://example.org/layer/1", new Label("Transcripts"));

        layer.Label.Should().ContainSingle(x => x.Value == "Transcripts");
        layer.OtherContent.Should().BeEmpty();
    }

    [Fact]
    public void Layer_AddOtherContent_Should_AppendAReference()
    {
        var layer = new Layer("https://example.org/layer/1");

        layer.AddOtherContent("https://example.org/list/1");
        layer.AddOtherContent("https://example.org/list/2");

        layer.OtherContent.Should().Equal("https://example.org/list/1", "https://example.org/list/2");
    }

    [Fact]
    public void Layer_RemoveOtherContent_Should_DropOnlyTheMatchingReference()
    {
        var layer = new Layer("https://example.org/layer/1");
        layer.AddOtherContent("https://example.org/list/1");
        layer.AddOtherContent("https://example.org/list/2");

        layer.RemoveOtherContent("https://example.org/list/1");

        layer.OtherContent.Should().ContainSingle().Which.Should().Be("https://example.org/list/2");
    }

    [Fact]
    public void Layer_Should_RoundTripThroughJsonConvert()
    {
        var layer = new Layer("https://example.org/layer/1", new Label("Transcripts"));
        layer.AddOtherContent("https://example.org/list/1");

        var json = JsonConvert.SerializeObject(layer);
        var obj = JObject.Parse(json);

        obj["@id"]!.ToString().Should().Be("https://example.org/layer/1");
        obj["@type"]!.ToString().Should().Be("sc:Layer");
        obj["otherContent"]!.Values<string>().Should().ContainSingle(x => x == "https://example.org/list/1");
    }
}