using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests.Integration;

/// <summary>
/// Tests for IIIF Cookbook Recipe 0001: Simplest Manifest - Single Image File
/// https://iiif.io/api/cookbook/recipe/0001-mvm-image/
/// </summary>
public class Recipe0001Tests
{
    [Fact]
    public void SimplestManifest_Should_SerializeCorrectly()
    {
        // Arrange
        var imageResource = new ImageResource(
                id: "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                format: ImageFormat.Png
            )
            .SetHeight(1800)
            .SetWidth(1200);

        var canvas = new Canvas(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
            label: new Label("p. 1"),
            height: 1800,
            width: 1200
        );

        var image = new Image(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
            resource: imageResource,
            on: canvas.Id
        );

        canvas.AddImage(image);

        var sequence = new Sequence(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/s0"
        );
        sequence.AddCanvas(canvas);

        var manifest = new Manifest(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
            label: new Label("Single Image Example")
        );
        manifest.AddSequence(sequence);

        // Act
        var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
        var jObject = JObject.Parse(json);

        // Assert - Manifest structure
        jObject["@context"].ToString().Should().Be("http://iiif.io/api/presentation/2/context.json");
        jObject["@id"].ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json");
        jObject["@type"].ToString().Should().Be("sc:Manifest");
        jObject["label"].ToString().Should().Be("Single Image Example");

        // Assert - Sequences
        var sequences = jObject["sequences"] as JArray;
        sequences.Should().NotBeNull();
        sequences.Should().HaveCount(1);

        var seq = sequences![0] as JObject;
        seq!["@id"].ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/s0");
        seq["@type"].ToString().Should().Be("sc:Sequence");

        // Assert - Canvas
        var canvases = seq["canvases"] as JArray;
        canvases.Should().NotBeNull();
        canvases.Should().HaveCount(1);

        var canvasObj = canvases![0] as JObject;
        canvasObj!["@id"].ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1");
        canvasObj["@type"].ToString().Should().Be("sc:Canvas");
        canvasObj["label"].ToString().Should().Be("p. 1");
        canvasObj["height"].Value<int>().Should().Be(1800);
        canvasObj["width"].Value<int>().Should().Be(1200);

        // Assert - Images
        var images = canvasObj["images"] as JArray;
        images.Should().NotBeNull();
        images.Should().HaveCount(1);

        var imageObj = images![0] as JObject;
        imageObj!["@id"].ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image");
        imageObj["@type"].ToString().Should().Be("oa:Annotation");
        imageObj["motivation"].ToString().Should().Be("sc:painting");
        imageObj["on"].ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1");

        // Assert - Image Resource
        var resource = imageObj["resource"] as JObject;
        resource.Should().NotBeNull();
        resource!["@id"].ToString().Should().Be("http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png");
        resource["@type"].ToString().Should().Be("dctypes:Image");
        resource["format"].ToString().Should().Be("image/png");
        resource["height"].Value<int>().Should().Be(1800);
        resource["width"].Value<int>().Should().Be(1200);
    }

    [Fact]
    public void SimplestManifest_Should_RoundTrip()
    {
        // Arrange
        var imageResource = new ImageResource(
                id: "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                format: ImageFormat.Png
            )
            .SetHeight(1800)
            .SetWidth(1200);

        var canvas = new Canvas(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
            label: new Label("p. 1"),
            height: 1800,
            width: 1200
        );

        var image = new Image(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
            resource: imageResource,
            on: canvas.Id
        );

        canvas.AddImage(image);

        var sequence = new Sequence(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/s0"
        );
        sequence.AddCanvas(canvas);

        var manifest = new Manifest(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
            label: new Label("Single Image Example")
        );
        manifest.AddSequence(sequence);

        // Act - Serialize to JSON
        var json = JsonConvert.SerializeObject(manifest);

        // Act - Deserialize back
        var deserializedManifest = JsonConvert.DeserializeObject<Manifest>(json);

        // Assert
        deserializedManifest.Should().NotBeNull();
        deserializedManifest!.Id.Should().Be(manifest.Id);
        deserializedManifest.Label.Should().HaveCount(1);
        deserializedManifest.Label.First().Value.Should().Be("Single Image Example");

        deserializedManifest.Sequences.Should().HaveCount(1);
        var deserializedSequence = deserializedManifest.Sequences.First();
        deserializedSequence.Canvases.Should().HaveCount(1);

        var deserializedCanvas = deserializedSequence.Canvases.First();
        deserializedCanvas.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1");
        deserializedCanvas.Height.Should().Be(1800);
        deserializedCanvas.Width.Should().Be(1200);

        deserializedCanvas.Images.Should().HaveCount(1);
        var deserializedImage = deserializedCanvas.Images.First();
        deserializedImage.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image");
        deserializedImage.On.Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1");
        deserializedImage.Motivation.Should().Be("sc:painting");

        deserializedImage.Resource.Should().NotBeNull();
        deserializedImage.Resource!.Id.Should().Be("http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png");
        deserializedImage.Resource.Height.Should().Be(1800);
        deserializedImage.Resource.Width.Should().Be(1200);
    }

    [Fact]
    public void SimplestManifest_With_MultipleCanvases_Should_Work()
    {
        // Arrange - Create a manifest with multiple canvases (book-like structure)
        var manifest = new Manifest(
            id: "https://example.org/iiif/book1/manifest",
            label: new Label("Multi-page Book Example")
        );

        var sequence = new Sequence(id: "https://example.org/iiif/book1/sequence/normal");

        // Add multiple pages
        for (int i = 1; i <= 3; i++)
        {
            var imageResource = new ImageResource(
                    id: $"https://example.org/images/page{i}.jpg",
                    format: "image/jpeg"
                )
                .SetHeight(2000)
                .SetWidth(1500);

            var canvas = new Canvas(
                id: $"https://example.org/iiif/book1/canvas/p{i}",
                label: new Label($"Page {i}"),
                height: 2000,
                width: 1500
            );

            var image = new Image(
                id: $"https://example.org/iiif/book1/annotation/p{i:D4}-image",
                resource: imageResource,
                on: canvas.Id
            );

            canvas.AddImage(image);
            sequence.AddCanvas(canvas);
        }

        manifest.AddSequence(sequence);

        // Act
        var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
        var jObject = JObject.Parse(json);

        // Assert
        var sequences = jObject["sequences"] as JArray;
        sequences.Should().HaveCount(1);

        var canvases = sequences![0]["canvases"] as JArray;
        canvases.Should().HaveCount(3);

        for (int i = 0; i < 3; i++)
        {
            var canvasObj = canvases![i] as JObject;
            canvasObj!["label"].ToString().Should().Be($"Page {i + 1}");
            canvasObj["height"].Value<int>().Should().Be(2000);
            canvasObj["width"].Value<int>().Should().Be(1500);
        }
    }

    [Fact]
    public void SimplestManifest_Should_Have_RequiredFields()
    {
        // Arrange
        var imageResource = new ImageResource(
                id: "https://example.org/image.jpg",
                format: "image/jpeg"
            )
            .SetHeight(1000)
            .SetWidth(800);

        var canvas = new Canvas(
            id: "https://example.org/canvas/1",
            label: new Label("Test Canvas"),
            height: 1000,
            width: 800
        );

        var image = new Image(
            id: "https://example.org/annotation/1",
            resource: imageResource,
            on: canvas.Id
        );

        canvas.AddImage(image);

        var sequence = new Sequence();
        sequence.AddCanvas(canvas);

        var manifest = new Manifest(
            id: "https://example.org/manifest.json",
            label: new Label("Test Manifest")
        );
        manifest.AddSequence(sequence);

        // Act
        var json = JsonConvert.SerializeObject(manifest);
        var jObject = JObject.Parse(json);

        // Assert - Required fields per IIIF Presentation API 2.0
        jObject["@context"].Should().NotBeNull("@context is required");
        jObject["@id"].Should().NotBeNull("@id is required");
        jObject["@type"].Should().NotBeNull("@type is required");
        jObject["label"].Should().NotBeNull("label is required");
        jObject["sequences"].Should().NotBeNull("sequences is required for v2");

        var sequences = jObject["sequences"] as JArray;
        var seq = sequences![0] as JObject;
        seq!["canvases"].Should().NotBeNull("canvases is required in sequence");

        var canvases = seq["canvases"] as JArray;
        var canvasObj = canvases![0] as JObject;
        canvasObj!["@id"].Should().NotBeNull();
        canvasObj["@type"].Should().NotBeNull();
        canvasObj["label"].Should().NotBeNull();
        canvasObj["height"].Should().NotBeNull("height is required on canvas");
        canvasObj["width"].Should().NotBeNull("width is required on canvas");
    }
}