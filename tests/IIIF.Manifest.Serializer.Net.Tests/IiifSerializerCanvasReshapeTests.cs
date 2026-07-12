using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 1 (SDK_VERSIONING_GUIDE.md): IiifSerializer round-trips through the reshaped
///     Canvas (Items-native, generalized to Video/otherContent-as-annotations, not just Image/Audio).
/// </summary>
public class IiifSerializerCanvasReshapeTests
{
    [Fact]
    public void Serialize_Should_MapVideoAnnotationToV3VideoBody()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Video"), 720, 1280).SetDuration(60.5);
        var resource = new VideoResource("https://example.org/video.mp4", "video/mp4").SetHeight(720).SetWidth(1280).SetDuration(60.5);
        canvas.AddAnnotation(new Annotation("https://example.org/annotation/1", resource, canvas.Id));

        var manifest = new Manifest("https://example.org/manifest", new Label("Video Example"));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");

        var json = IiifSerializer.Serialize(manifest);
        var body = JObject.Parse(json)["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        body["type"]!.ToString().Should().Be("Video");
        body["format"]!.ToString().Should().Be("video/mp4");
        body["duration"]!.Value<double>().Should().Be(60.5);
    }

    [Fact]
    public void Serialize_Should_WriteCanvasAnnotationsAsAnnotationPageReferences()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
        canvas.AddAnnotationPageReference(new AnnotationPage("https://example.org/annotation-list/transcript"));

        var manifest = new Manifest("https://example.org/manifest", new Label("Transcript Example"));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");

        var json = IiifSerializer.Serialize(manifest);
        var canvasObj = JObject.Parse(json)["items"]![0]!;

        var annotationsRef = canvasObj["annotations"]![0]!;
        annotationsRef["id"]!.ToString().Should().Be("https://example.org/annotation-list/transcript");
        annotationsRef["type"]!.ToString().Should().Be("AnnotationPage");
    }

    [Fact]
    public void ThreeDotOhConstructedCanvas_Should_SerializeAsStructurallyValidLegacyV2Json()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
        var resource = new ImageResource("https://example.org/image.png", "image/png").SetHeight(1000).SetWidth(800);
        canvas.AddAnnotation(new Annotation("https://example.org/annotation/1", resource, canvas.Id));

        var manifest = new Manifest("https://example.org/manifest", new Label("Single Image Example"));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");

        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var canvasObj = JObject.Parse(json)["sequences"]![0]!["canvases"]![0]!;

        canvasObj["items"].Should().BeNull("legacy 2.1 output must not contain 3.0-native items");
        var images = canvasObj["images"] as JArray;
        images.Should().NotBeNull();
        images!.Should().HaveCount(1);
        images[0]!["resource"]!["format"]!.ToString().Should().Be("image/png");
    }

    [Fact]
    public void DeserializeManifest_Should_ReadAnnotationsReferenceFromV3Json()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Transcript Example"] },
                              "items": [
                                {
                                  "id": "https://example.org/canvas/1",
                                  "type": "Canvas",
                                  "label": { "none": ["Page 1"] },
                                  "height": 1000,
                                  "width": 800,
                                  "annotations": [
                                    { "id": "https://example.org/annotation-list/transcript", "type": "AnnotationPage" }
                                  ]
                                }
                              ]
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);
        var canvas = manifest.Sequences.First().Canvases.First();

        canvas.Annotations.Should().ContainSingle(x => x.Id == "https://example.org/annotation-list/transcript");
        canvas.OtherContents.Should().ContainSingle();
    }
}