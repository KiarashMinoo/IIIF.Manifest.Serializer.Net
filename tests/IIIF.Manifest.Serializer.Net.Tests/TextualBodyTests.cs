using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Cookbook Group B: the W3C "TextualBody" resource (<c>type:"TextualBody"</c>, <c>value</c>/
/// <c>format</c>/<c>language</c>) used as an Annotation body for commenting/tagging/transcribing
/// motivations - verified directly against recipes 0019-html-in-annotations and 0021-tagging.
/// </summary>
public class TextualBodyTests
{
    [Fact]
    public void TextualBody_Should_RoundTripThroughPlainJsonConvert()
    {
        var body = new TextualBody("Gänseliesel-Brunnen").SetLanguage("de").SetFormat("text/plain");

        var deserialized = TrackableObject.Parse<TextualBody>(body.Serialize());

        deserialized.Type.Should().Be("TextualBody");
        deserialized.Value.Should().Be("Gänseliesel-Brunnen");
        deserialized.Language.Should().Be("de");
        deserialized.Format.Should().Be("text/plain");
    }

    [Fact]
    public void TextualBody_Should_OmitFormatAndLanguageWhenNotSet()
    {
        var body = new TextualBody("plain comment, no metadata");

        var json = JsonConvert.SerializeObject(body);
        var obj = JObject.Parse(json);

        obj["format"].Should().BeNull();
        obj["language"].Should().BeNull();
    }

    [Fact]
    public void Annotation_Should_RoundTripTextualBodyAsCommentingMotivation()
    {
        // Cookbook recipe 0021-tagging: a "tagging" Annotation whose body is a TextualBody tag.
        var annotation = new Annotation("https://example.org/annotation/tag-1",
                new TextualBody("Gänseliesel-Brunnen").SetLanguage("de").SetFormat("text/plain"),
                new AnnotationTarget("https://example.org/canvas/p1")
                    .SetSelector(Shared.Selectors.FragmentSelector.ForRegion(265, 661, 1260, 1239)))
            .SetMotivation("tagging");

        var canvas = new Canvas("https://example.org/canvas/p1", new Label("p1"), 3024, 4032).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyObj = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyObj["type"]!.ToString().Should().Be("TextualBody");
        bodyObj["value"]!.ToString().Should().Be("Gänseliesel-Brunnen");
        bodyObj["language"]!.ToString().Should().Be("de");
        bodyObj["id"].Should().BeNull();

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedBody = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body;

        roundTrippedBody.Should().BeOfType<TextualBody>();
        var textualBody = (TextualBody)roundTrippedBody;
        textualBody.Value.Should().Be("Gänseliesel-Brunnen");
        textualBody.Format.Should().Be("text/plain");
        textualBody.Language.Should().Be("de");
    }
}
