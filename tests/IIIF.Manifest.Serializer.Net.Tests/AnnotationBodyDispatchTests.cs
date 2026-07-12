using System.Linq;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties.Services.Search;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Round 2 (SDK_VERSIONING_GUIDE.md §11, Milestone 22): <c>Annotation.Body</c> (declared
///     <see cref="IBaseResource" />) had no polymorphic-dispatch <c>JsonConverter</c>, so a standalone
///     <c>Annotation</c> round-tripped through plain <c>JsonConvert</c>/<c>TrackableObject.Parse</c>
///     (not <c>IiifSerializer</c>'s hand-built V3 Canvas reader) threw trying to instantiate the
///     interface directly - explicitly flagged as a "Known follow-up" in both Milestones 13 and (via
///     round 2's independent re-verification) still true beforehand.
/// </summary>
public class AnnotationBodyDispatchTests
{
    [Fact]
    public void Annotation_Should_RoundTripImageBodyThroughPlainJsonConvert()
    {
        var annotation = new Annotation("https://example.org/annotation/1",
            new ImageResource("https://example.org/image.jpg", "image/jpeg").SetHeight(100).SetWidth(200),
            "https://example.org/canvas/1");

        var json = JsonConvert.SerializeObject(annotation);
        var deserialized = JsonConvert.DeserializeObject<Annotation>(json)!;

        var body = deserialized.Body.Should().BeOfType<ImageResource>().Subject;
        body.Height.Should().Be(100);
        body.Width.Should().Be(200);
    }

    [Fact]
    public void Annotation_Should_RoundTripAudioBodyThroughTrackableObjectParse()
    {
        var annotation = new Annotation("https://example.org/annotation/2",
            new AudioResource("https://example.org/audio.mp3", "audio/mpeg").SetDuration(30.5),
            "https://example.org/canvas/1");

        var deserialized = TrackableObject.Parse<Annotation>(annotation.Serialize());

        var body = deserialized.Body.Should().BeOfType<AudioResource>().Subject;
        body.Duration.Should().Be(30.5);
    }

    [Fact]
    public void Annotation_Should_RoundTripVideoBody()
    {
        var annotation = new Annotation("https://example.org/annotation/3",
            new VideoResource("https://example.org/video.mp4", "video/mp4").SetHeight(480).SetWidth(640).SetDuration(60),
            "https://example.org/canvas/1");

        var deserialized = TrackableObject.Parse<Annotation>(annotation.Serialize());

        var body = deserialized.Body.Should().BeOfType<VideoResource>().Subject;
        body.Duration.Should().Be(60);
    }

    [Fact]
    public void Annotation_Should_RoundTripEmbeddedTextBody_WithNoId()
    {
        var annotation = new Annotation("https://example.org/annotation/4",
            new EmbeddedContentResource("hello world", "en"),
            "https://example.org/canvas/1");

        var json = annotation.Serialize();
        json.Should().NotContain("cnt:ContextAsText");
        json.Should().Contain("cnt:ContentAsText");

        var deserialized = TrackableObject.Parse<Annotation>(json);

        var body = deserialized.Body.Should().BeOfType<EmbeddedContentResource>().Subject;
        body.Chars.Should().Be("hello world");
        body.Language.Should().Be("en");
    }

    [Fact]
    public void SearchResponse_Items_Should_NowRoundTripThroughTrackableObjectParse()
    {
        // Previously called out in Milestone 13 as a limitation this specific test worked around.
        var annotation = new Annotation("https://example.org/annotation/anno-bird",
            new EmbeddedContentResource("A bird in the hand is worth two in the bush", "en"),
            "https://example.org/canvas1#xywh=100,100,250,20");
        var response = new SearchResponse("https://example.org/search?q=bird").AddItem(annotation);

        var deserialized = TrackableObject.Parse<SearchResponse>(response.Serialize());

        deserialized.Items.Single().Id.Should().Be("https://example.org/annotation/anno-bird");
        deserialized.Items.Single().Body.Should().BeOfType<EmbeddedContentResource>();
    }
}