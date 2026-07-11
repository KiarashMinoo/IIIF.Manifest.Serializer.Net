using System.Linq;
using IIIF.Manifests.Serializer.Nodes.Contents.ContentState;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 10 (SDK_VERSIONING_GUIDE.md §10, finding 2): IIIF Content State API 1.0 was 0%
/// modeled before this milestone - no way to build/parse the deep-linking "content state" object,
/// nor to encode/decode the base64url string real viewers pass around via `iiif-content`.
/// </summary>
public class ContentStateTests
{
    [Fact]
    public void ContentState_Should_WriteBareUriTarget_WhenOnlyIdSet()
    {
        var contentState = new ContentState(new ContentStateTarget("https://example.org/manifest/canvas1"));

        var json = contentState.Serialize();
        var obj = JObject.Parse(json);

        obj["type"]!.ToString().Should().Be("Annotation");
        obj["motivation"]!.ToString().Should().Be("contentState");
        obj["target"]!.Type.Should().Be(JTokenType.String);
        obj["target"]!.ToString().Should().Be("https://example.org/manifest/canvas1");
    }

    [Fact]
    public void ContentState_Should_WriteTypedResourceReference_WhenResourceTypeSet()
    {
        var contentState = new ContentState(new ContentStateTarget("https://example.org/manifest/canvas1", "Canvas"));

        var obj = JObject.Parse(contentState.Serialize());

        obj["target"]!.Type.Should().Be(JTokenType.Object);
        obj["target"]!["id"]!.ToString().Should().Be("https://example.org/manifest/canvas1");
        obj["target"]!["type"]!.ToString().Should().Be("Canvas");
    }

    [Fact]
    public void ContentState_Should_WriteRegionAsMediaFragmentSuffixOnId_NotASpecificResource()
    {
        // Content State 1.0 §5.1 has no SpecificResource/selector form for region-targeting at
        // all - its own spec example expresses a region as a plain Media Fragments suffix on the
        // target id, so the bare-string/typed-reference constructor already covers this case.
        var target = new ContentStateTarget("https://example.org/object1/canvas7#xywh=1000,2000,1000,2000", "Canvas")
            .SetPartOf("https://example.org/object1/manifest");
        var contentState = new ContentState(target);

        var obj = JObject.Parse(contentState.Serialize());
        var targetObj = (JObject)obj["target"]!;

        targetObj["id"]!.ToString().Should().Be("https://example.org/object1/canvas7#xywh=1000,2000,1000,2000");
        targetObj["type"]!.ToString().Should().Be("Canvas");
        targetObj["partOf"]![0]!["id"]!.ToString().Should().Be("https://example.org/object1/manifest");
    }

    [Fact]
    public void ContentState_Should_WriteSpecificResourceWithPointSelectorAndPartOf()
    {
        var target = new ContentStateTarget("https://example.org/iiif/id1/canvas1", "Canvas")
            .SetPointSelector(14.5)
            .SetPartOf("https://example.org/iiif/id1/manifest");
        var contentState = new ContentState(target).SetId("https://example.org/import/2");

        var obj = JObject.Parse(contentState.Serialize());
        var targetObj = (JObject)obj["target"]!;

        targetObj["type"]!.ToString().Should().Be("SpecificResource");
        var source = (JObject)targetObj["source"]!;
        source["id"]!.ToString().Should().Be("https://example.org/iiif/id1/canvas1");
        source["type"]!.ToString().Should().Be("Canvas");
        source["partOf"]![0]!["id"]!.ToString().Should().Be("https://example.org/iiif/id1/manifest");

        var selector = (JObject)targetObj["selector"]!;
        selector["type"]!.ToString().Should().Be("PointSelector");
        selector["t"]!.Value<double>().Should().Be(14.5);
    }

    [Fact]
    public void ContentState_Should_RoundTripThroughSerializeAndParse()
    {
        var target = new ContentStateTarget("https://example.org/manifest/canvas1", "Canvas")
            .SetPointSelector(14.5)
            .SetPartOf("https://example.org/manifest");
        var original = new ContentState(target).SetId("https://example.org/content-state/1");

        var deserialized = TrackableObject.Parse<ContentState>(original.Serialize());

        deserialized.Id.Should().Be("https://example.org/content-state/1");
        deserialized.Motivation.Should().Be("contentState");
        var roundTripped = deserialized.Target.Single();
        roundTripped.Id.Should().Be("https://example.org/manifest/canvas1");
        roundTripped.ResourceType.Should().Be("Canvas");
        roundTripped.PointSelector!.T.Should().Be(14.5);
        roundTripped.PartOfId.Should().Be("https://example.org/manifest");
        roundTripped.PartOfType.Should().Be("Manifest");
    }

    [Fact]
    public void ContentState_Should_RoundTripMultipleTargetsAsArray()
    {
        var contentState = new ContentState(
            new ContentStateTarget("https://example.org/canvas/1"),
            new ContentStateTarget("https://example.org/canvas/2"));

        var obj = JObject.Parse(contentState.Serialize());
        obj["target"]!.Type.Should().Be(JTokenType.Array);

        var deserialized = TrackableObject.Parse<ContentState>(contentState.Serialize());
        deserialized.Target.Select(t => t.Id).Should().BeEquivalentTo(
            "https://example.org/canvas/1", "https://example.org/canvas/2");
    }

    [Fact]
    public void ContentStateCodec_Should_RoundTripThroughBase64UrlEncoding()
    {
        var contentState = new ContentState(
            new ContentStateTarget("https://example.org/manifest/canvas1", "Canvas").SetPointSelector(10.5))
            .SetId("https://example.org/content-state/1");

        var encoded = ContentStateCodec.Encode(contentState);

        encoded.Should().NotContain("+").And.NotContain("/").And.NotContain("=");

        var decoded = ContentStateCodec.Decode(encoded);

        decoded.Id.Should().Be("https://example.org/content-state/1");
        var target = decoded.Target.Single();
        target.Id.Should().Be("https://example.org/manifest/canvas1");
        target.PointSelector!.T.Should().Be(10.5);
    }

    [Fact]
    public void ContentStateCodec_Decode_Should_ThrowOnBlankInput()
    {
        var act = () => ContentStateCodec.Decode("");
        act.Should().Throw<ArgumentException>();
    }
}
