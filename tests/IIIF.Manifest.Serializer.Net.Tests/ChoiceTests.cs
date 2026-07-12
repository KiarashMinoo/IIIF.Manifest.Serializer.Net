using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Cookbook Group C: the Web Annotation Model's "Choice" body type (<c>type:"Choice"</c>,
///     <c>items:[...]</c>) - verified against recipes 0033-choice (image alternatives with per-item
///     labels), 0346-multilingual-annotation-body (TextualBody alternatives), and 0434-choice-av
///     (Sound alternatives). Group C also required adding <c>label</c> support to
///     <see cref="BaseResource{TBaseResource}" /> itself, since content resources used as Choice items
///     carry their own label per spec (e.g. "Natural Light" vs "X-Ray").
/// </summary>
public class ChoiceTests
{
    [Fact]
    public void Choice_Should_AlwaysWriteItemsAsArrayEvenWithOneItem()
    {
        var choice = new Choice([new TextualBody("only one")]);

        var json = choice.Serialize();
        var obj = JObject.Parse(json);

        obj["items"].Should().BeOfType<JArray>();
        ((JArray)obj["items"]!).Should().ContainSingle();
    }

    [Fact]
    public void Choice_Should_RoundTripThroughPlainJsonConvertWithMixedItemTypes()
    {
        var choice = new Choice([
            new TextualBody("Koto with a cover being carried").SetLanguage("en").SetFormat("text/plain"),
            new TextualBody("袋に収められた琴").SetLanguage("ja").SetFormat("text/plain")
        ]);

        var deserialized = TrackableObject.Parse<Choice>(choice.Serialize());

        deserialized.Type.Should().Be("Choice");
        deserialized.Items.Should().HaveCount(2);
        deserialized.Items.OfType<TextualBody>().Select(x => x.Language).Should().BeEquivalentTo("en", "ja");
    }

    [Fact]
    public void Annotation_Should_RoundTripChoiceOfImagesWithLabelsThroughIiifSerializer()
    {
        // Cookbook recipe 0033-choice: painting Annotation whose body offers Natural Light vs X-Ray.
        var natural = new ImageResource("https://example.org/natural/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(1271).SetWidth(2000).SetLabel(new Label("Natural Light"));
        var xray = new ImageResource("https://example.org/xray/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(1271).SetWidth(2000).SetLabel(new Label("X-Ray"));

        var annotation = new Annotation("https://example.org/annotation/1", new Choice([natural, xray]),
            "https://example.org/canvas/p1");
        var canvas = new Canvas("https://example.org/canvas/p1", new Label("p1"), 1271, 2000).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyObj = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyObj["type"]!.ToString().Should().Be("Choice");
        var items = (JArray)bodyObj["items"]!;
        items.Should().HaveCount(2);
        items[0]!["id"]!.ToString().Should().Be("https://example.org/natural/full/max/0/default.jpg");
        items[0]!["label"]!["none"]![0]!.ToString().Should().Be("Natural Light");
        items[0]!["@id"].Should().BeNull();

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedBody = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body;

        roundTrippedBody.Should().BeOfType<Choice>();
        var roundTrippedChoice = (Choice)roundTrippedBody;
        roundTrippedChoice.Items.Should().HaveCount(2);
        roundTrippedChoice.Items.OfType<ImageResource>().Select(x => x.Label.Single().Value)
            .Should().BeEquivalentTo("Natural Light", "X-Ray");
    }

    [Fact]
    public void Annotation_Should_RoundTripChoiceOfSoundAlternativesThroughIiifSerializer()
    {
        // Cookbook recipe 0434-choice-av: several audio format alternatives for the same content.
        var alac = new AudioResource("https://fixtures.iiif.io/audio/egbe-iyawo.m4a", "audio/alac")
            .SetDuration(16.0).SetLabel(new Label("ALAC"));
        var mp3 = new AudioResource("https://fixtures.iiif.io/audio/egbe-iyawo.mp3", "audio/mpeg")
            .SetDuration(16.0).SetLabel(new Label("MP3"));

        var annotation = new Annotation("https://example.org/annotation/1", new Choice([alac, mp3]),
            "https://example.org/canvas/1");
        var canvas = new Canvas("https://example.org/canvas/1", new Label("c1"), 1, 1).SetDuration(16.0).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedBody = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body;

        var choice = (Choice)roundTrippedBody;
        choice.Items.OfType<AudioResource>().Select(x => x.Format).Should().BeEquivalentTo("audio/alac", "audio/mpeg");
        choice.Items.OfType<AudioResource>().Select(x => x.Label.Single().Value).Should().BeEquivalentTo("ALAC", "MP3");
    }
}