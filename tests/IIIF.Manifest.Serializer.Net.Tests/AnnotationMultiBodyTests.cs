using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Cookbook Group I (discovered while researching recipes for CookbookCatalog M9, not in the
/// original A-H gap analysis): the W3C Annotation Model allows <c>body</c> to be a single value or
/// an array of sibling resources (distinct from <c>Choice</c>, which models mutually-exclusive
/// alternatives) - recipe 0022-linking-with-a-hotspot pairs a TextualBody with a SpecificResource
/// link as two sibling bodies on one "linking" Annotation.
/// </summary>
public class AnnotationMultiBodyTests
{
    [Fact]
    public void Annotation_Should_RoundTripMultipleBodiesThroughIiifSerializer()
    {
        var textBody = new TextualBody("A link to a close up.").SetLanguage("en").SetFormat("text/plain");
        var linkBody = new SpecificResource(new BaseResource("https://example.org/canvas/p2", "Canvas"));

        var annotation = new Annotation("https://example.org/annotation/1", textBody, "https://example.org/canvas/p1")
            .AddBody(linkBody)
            .SetMotivation("linking");

        var canvas = new Canvas("https://example.org/canvas/p1", new Label("p1"), 100, 100).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyToken = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyToken.Should().BeOfType<JArray>();
        var bodyArray = (JArray)bodyToken;
        bodyArray.Should().HaveCount(2);
        bodyArray[0]!["type"]!.ToString().Should().Be("TextualBody");
        bodyArray[1]!["type"]!.ToString().Should().Be("SpecificResource");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedAnnotation = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single();

        roundTrippedAnnotation.Bodies.Should().HaveCount(2);
        roundTrippedAnnotation.Bodies.OfType<TextualBody>().Should().ContainSingle(x => x.Value == "A link to a close up.");
        roundTrippedAnnotation.Bodies.OfType<SpecificResource>().Should().ContainSingle();
    }

    [Fact]
    public void Annotation_Should_CollapseSingleBodyToBareObjectNotArray()
    {
        var annotation = new Annotation("https://example.org/annotation/1",
            new TextualBody("Just one body"), "https://example.org/canvas/p1");
        var canvas = new Canvas("https://example.org/canvas/p1", new Label("p1"), 100, 100).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyToken = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyToken.Should().BeOfType<JObject>();
    }
}
