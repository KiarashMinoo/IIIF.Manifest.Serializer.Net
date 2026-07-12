using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Cookbook Group D: Presentation API 3.0 §4.5's <c>timeMode</c> Annotation property
///     (trim/scale/loop), reconciling a temporal-media body's duration against its target's. No
///     cookbook recipe exercises this directly (confirmed by searching every recipe's JSON), but it's
///     a genuine 3.0 spec property, not extension-territory, so it belongs in the SDK regardless.
/// </summary>
public class AnnotationTimeModeTests
{
    [Fact]
    public void Annotation_Should_OmitTimeModeWhenNotSet()
    {
        var annotation = new Annotation("https://example.org/annotation/1",
            new AudioResource("https://example.org/audio.mp3", "audio/mpeg"), "https://example.org/canvas/1");
        var canvas = new Canvas("https://example.org/canvas/1", new Label("c1"), 1, 1).SetDuration(10).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var annotationObj = obj["items"]![0]!["items"]![0]!["items"]![0]!;

        annotationObj["timeMode"].Should().BeNull();
    }

    [Fact]
    public void Annotation_Should_RoundTripTimeModeThroughIiifSerializer()
    {
        var annotation = new Annotation("https://example.org/annotation/1",
                new AudioResource("https://example.org/audio.mp3", "audio/mpeg").SetDuration(5), "https://example.org/canvas/1")
            .SetTimeMode(TimeMode.Loop);
        var canvas = new Canvas("https://example.org/canvas/1", new Label("c1"), 1, 1).SetDuration(20).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var annotationObj = obj["items"]![0]!["items"]![0]!["items"]![0]!;
        annotationObj["timeMode"]!.ToString().Should().Be("loop");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTripped = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single();

        roundTripped.TimeMode.Should().NotBeNull();
        roundTripped.TimeMode!.Value.Should().Be("loop");
    }

    [Fact]
    public void Annotation_Should_RoundTripTimeModeThroughPlainJsonConvert()
    {
        var annotation = new Annotation("https://example.org/annotation/1",
                new AudioResource("https://example.org/audio.mp3", "audio/mpeg"), "https://example.org/canvas/1")
            .SetTimeMode(TimeMode.Trim);

        var deserialized = TrackableObject.Parse<Annotation>(annotation.Serialize());

        deserialized.TimeMode!.Value.Should().Be("trim");
    }
}