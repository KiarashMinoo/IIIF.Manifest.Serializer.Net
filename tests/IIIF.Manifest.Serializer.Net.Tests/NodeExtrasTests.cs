using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Cookbook M9 prep: <c>Summary</c>, <c>Metadata</c>, <c>Thumbnail</c>, <c>Rendering</c>,
/// <c>Homepage</c>, <c>SeeAlso</c>, and <c>Provider</c> were discovered - while researching real
/// cookbook recipe JSON (0006/0007/0008/0046/0047/0053/0117/0229/0232/0234/0068 among others) - to
/// be completely unwired from <see cref="IiifSerializer"/>'s hand-rolled V3 reader/writer, so
/// building a Manifest/Canvas/Collection/Range with these properties and round-tripping it through
/// IiifSerializer silently dropped every one of them. <c>Summary</c> itself (3.0's replacement for
/// 2.x <c>description</c>) didn't exist as a class member at all. Fixed generically for every
/// BaseNode-derived type (Manifest/Collection/Canvas/Range); Provider is Manifest/Collection-only
/// per spec.
/// </summary>
public class NodeExtrasTests
{
    [Fact]
    public void Manifest_Should_RoundTripSummaryThroughIiifSerializer()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"))
            .SetSummary([new Description("English summary").SetLanguage("en"), new Description("Résumé français").SetLanguage("fr")]);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["summary"]!["en"]![0]!.ToString().Should().Be("English summary");
        obj["summary"]!["fr"]![0]!.ToString().Should().Be("Résumé français");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.Summary.Select(x => x.Value).Should().BeEquivalentTo("English summary", "Résumé français");
    }

    [Fact]
    public void Manifest_Description_Should_BeComputedLegacyViewOfSummary()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"))
            .AddDescription(new Description("Legacy description"));

        manifest.Summary.Should().ContainSingle(x => x.Value == "Legacy description");
        manifest.Description.Should().ContainSingle(x => x.Value == "Legacy description");
    }

    [Fact]
    public void Manifest_Should_RoundTripMetadataRenderingHomepageSeeAlsoThumbnailThroughIiifSerializer()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"))
            .AddMetadata(new Metadata("Author", "Jane Doe", "en"))
            .AddRendering(new Rendering("https://example.org/object.pdf", "PDF version"))
            .AddHomepage(new Homepage("https://example.org/object", "Object page"))
            .AddSeeAlso(new SeeAlso("https://example.org/record.json").SetType("Dataset").SetFormat("application/json").SetProfile("http://example.org/profile"))
            .SetThumbnail(new Thumbnail("https://example.org/thumb.jpg").SetFormat("image/jpeg").SetHeight(200).SetWidth(150));

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["metadata"]![0]!["label"]!["none"]![0]!.ToString().Should().Be("Author");
        obj["metadata"]![0]!["value"]!["en"]![0]!.ToString().Should().Be("Jane Doe");
        obj["rendering"]![0]!["id"]!.ToString().Should().Be("https://example.org/object.pdf");
        obj["homepage"]![0]!["id"]!.ToString().Should().Be("https://example.org/object");
        obj["seeAlso"]![0]!["type"]!.ToString().Should().Be("Dataset");
        obj["seeAlso"]![0]!["profile"]!.ToString().Should().Be("http://example.org/profile");
        obj["thumbnail"]![0]!["height"]!.Value<int>().Should().Be(200);
        obj["thumbnail"]![0]!["width"]!.Value<int>().Should().Be(150);

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.Metadata.Should().ContainSingle(x => x.Label == "Author");
        deserialized.Rendering.Should().ContainSingle(x => x.Id == "https://example.org/object.pdf");
        deserialized.Homepage.Should().ContainSingle(x => x.Id == "https://example.org/object");
        deserialized.SeeAlso.Should().ContainSingle(x => x.Profile == "http://example.org/profile");
        deserialized.Thumbnail.Should().NotBeNull();
        deserialized.Thumbnail!.Height.Should().Be(200);
        deserialized.Thumbnail.Width.Should().Be(150);
    }

    [Fact]
    public void Manifest_Should_RoundTripProviderWithNestedHomepageLogoSeeAlsoThroughIiifSerializer()
    {
        var provider = new Provider("https://example.org/institution", new Label("Example Institution"))
            .AddHomepage(new Homepage("https://example.org/", "Homepage"))
            .AddLogo(new Logo("https://example.org/logo.png").SetFormat("image/png").SetHeight(100).SetWidth(300))
            .AddSeeAlso(new SeeAlso("https://example.org/data.xml").SetType("Dataset"));

        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddProvider(provider);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        var providerObj = obj["provider"]![0]!;
        providerObj["id"]!.ToString().Should().Be("https://example.org/institution");
        providerObj["label"]!["none"]![0]!.ToString().Should().Be("Example Institution");
        providerObj["homepage"]![0]!["id"]!.ToString().Should().Be("https://example.org/");
        providerObj["logo"]![0]!["width"]!.Value<int>().Should().Be(300);
        providerObj["seeAlso"]![0]!["type"]!.ToString().Should().Be("Dataset");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedProvider = deserialized.Provider.Single();
        roundTrippedProvider.Label.Should().ContainSingle(x => x.Value == "Example Institution");
        roundTrippedProvider.Homepage.Should().ContainSingle(x => x.Id == "https://example.org/");
        roundTrippedProvider.Logo.Should().ContainSingle(x => x.Width == 300);
        roundTrippedProvider.SeeAlso.Should().ContainSingle(x => x.Type == "Dataset");
    }

    [Fact]
    public void Canvas_Should_RoundTripMetadataAndThumbnailThroughIiifSerializer()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 100, 100)
            .AddMetadata(new Metadata("Description", "A description of this canvas specifically"))
            .SetThumbnail(new Thumbnail("https://example.org/canvas-thumb.jpg").SetFormat("image/jpeg"));

        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var canvasObj = obj["items"]![0]!;

        canvasObj["metadata"]![0]!["label"]!["none"]![0]!.ToString().Should().Be("Description");
        canvasObj["thumbnail"]![0]!["id"]!.ToString().Should().Be("https://example.org/canvas-thumb.jpg");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedCanvas = (Canvas)deserialized.Items.Single();
        roundTrippedCanvas.Metadata.Should().ContainSingle(x => x.Label == "Description");
        roundTrippedCanvas.Thumbnail.Should().NotBeNull();
    }

    [Fact]
    public void Range_Should_RoundTripThumbnailThroughIiifSerializer()
    {
        var range = new Structure("https://example.org/range/1", new Label("Chapter 1"))
            .SetThumbnail(new Thumbnail("https://example.org/range-thumb.png").SetFormat("image/png"));
        range.AddCanvasReference("https://example.org/canvas/1");

        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 100, 100);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas).AddStructure(range);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["structures"]![0]!["thumbnail"]![0]!["id"]!.ToString().Should().Be("https://example.org/range-thumb.png");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.Structures.Single().Thumbnail.Should().NotBeNull();
    }

    [Fact]
    public void Collection_Should_RoundTripProviderAndMetadataThroughIiifSerializer()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test Collection"))
            .AddMetadata(new Metadata("Type", "Newspaper"))
            .AddProvider(new Provider("https://example.org/institution", new Label("Example Institution")));
        collection.AddManifestReference("https://example.org/manifest-1");

        var json = IiifSerializer.Serialize(collection);
        var obj = JObject.Parse(json);
        obj["metadata"]![0]!["label"]!["none"]![0]!.ToString().Should().Be("Type");
        obj["provider"]![0]!["id"]!.ToString().Should().Be("https://example.org/institution");

        var deserialized = IiifSerializer.DeserializeCollection(json);
        deserialized.Metadata.Should().ContainSingle(x => x.Label == "Type");
        deserialized.Provider.Should().ContainSingle(x => x.Id == "https://example.org/institution");
    }

    [Fact]
    public void ImageResource_Label_Should_RoundTripAsLanguageMapNotBareString()
    {
        // Regression guard for the earlier "label:'Natural Light'" bare-string bug found while
        // implementing Group C - proactively verified here for a non-Choice, standalone Image too.
        var image = new ImageResource("https://example.org/image.jpg", "image/jpeg").SetLabel(new Label("Full Image"));
        var annotation = new Nodes.Contents.Annotation.Annotation("https://example.org/annotation/1", image, "https://example.org/canvas/1");
        var canvas = new Canvas("https://example.org/canvas/1", new Label("c1"), 100, 100).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyObj = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyObj["label"]!["none"]![0]!.ToString().Should().Be("Full Image");
    }
}
