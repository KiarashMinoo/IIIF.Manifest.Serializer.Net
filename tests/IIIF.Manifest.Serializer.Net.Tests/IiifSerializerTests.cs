using System.Linq;
using IIIF.Manifests.Serializer;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

public class IiifSerializerTests
{
    [Fact]
    public void Serialize_Should_WriteLegacyV2_When_VersionIsV2_1()
    {
        var manifest = CreateImageManifest();

        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/2/context.json");
        obj["@id"]!.ToString().Should().Be(manifest.Id);
        obj["@type"]!.ToString().Should().Be("sc:Manifest");
        obj["sequences"].Should().BeOfType<JArray>();
        obj["items"].Should().BeNull();
    }

    [Fact]
    public void Serialize_Should_WriteLatestV3_ByDefault()
    {
        var manifest = CreateImageManifest();

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/3/context.json");
        obj["id"]!.ToString().Should().Be(manifest.Id);
        obj["type"]!.ToString().Should().Be("Manifest");
        obj["label"]!["none"]!.Values<string>().Should().ContainSingle("Single Image Example");
        obj["items"].Should().BeOfType<JArray>();
        obj["@id"].Should().BeNull();
        obj["@type"].Should().BeNull();
        obj["sequences"].Should().BeNull();
    }

    [Fact]
    public void Serialize_Should_MapImageAnnotationToV3AnnotationPage()
    {
        var manifest = CreateImageManifest();

        var json = IiifSerializer.Serialize(manifest);
        var annotation = JObject.Parse(json)["items"]![0]!["items"]![0]!["items"]![0]!;

        annotation["type"]!.ToString().Should().Be("Annotation");
        annotation["motivation"]!.ToString().Should().Be("painting");
        annotation["target"]!.ToString().Should().Be("https://example.org/canvas/1");
        annotation["body"]!["type"]!.ToString().Should().Be("Image");
        annotation["body"]!["format"]!.ToString().Should().Be("image/png");
        annotation["resource"].Should().BeNull();
        annotation["on"].Should().BeNull();
    }

    [Fact]
    public void Serialize_Should_MapAudioAnnotationToV3SoundBody()
    {
        var manifest = CreateAudioManifest();

        var json = IiifSerializer.Serialize(manifest);
        var body = JObject.Parse(json)["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        body["type"]!.ToString().Should().Be("Sound");
        body["format"]!.ToString().Should().Be("audio/mp4");
        body["duration"]!.Value<double>().Should().Be(10.5);
    }

    [Fact]
    public void DeserializeManifest_Should_ReadV2Manifest()
    {
        var manifest = CreateImageManifest();
        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));

        var result = IiifSerializer.DeserializeManifest(json);

        result.Id.Should().Be(manifest.Id);
        result.Sequences.Should().HaveCount(1);
        result.Sequences.First().Canvases.First().Images.Should().HaveCount(1);
    }

    [Fact]
    public void DeserializeManifest_Should_NormalizeSimpleV3Manifest()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Single Image Example"] },
                              "behavior": ["paged"],
                              "items": [
                                {
                                  "id": "https://example.org/canvas/1",
                                  "type": "Canvas",
                                  "label": { "none": ["p. 1"] },
                                  "height": 1000,
                                  "width": 800,
                                  "items": [
                                    {
                                      "id": "https://example.org/canvas/1/page",
                                      "type": "AnnotationPage",
                                      "items": [
                                        {
                                          "id": "https://example.org/annotation/1",
                                          "type": "Annotation",
                                          "motivation": "painting",
                                          "body": {
                                            "id": "https://example.org/image.png",
                                            "type": "Image",
                                            "format": "image/png",
                                            "height": 1000,
                                            "width": 800
                                          },
                                          "target": "https://example.org/canvas/1"
                                        }
                                      ]
                                    }
                                  ]
                                }
                              ]
                            }
                            """;

        var result = IiifSerializer.DeserializeManifest(json);

        result.Id.Should().Be("https://example.org/manifest");
        result.Label.First().Value.Should().Be("Single Image Example");
        result.Behavior.First().Value.Should().Be("paged");
        result.Sequences.Should().HaveCount(1);
        var canvas = result.Sequences.First().Canvases.First();
        canvas.Label.First().Value.Should().Be("p. 1");
        canvas.Images.Should().HaveCount(1);
        canvas.Images.First().Resource.Format.Should().Be("image/png");
    }

    [Fact]
    public void DeserializeManifest_Should_Throw_When_VersionCannotBeDetected()
    {
        var act = () => IiifSerializer.DeserializeManifest("{ \"label\": \"No version\" }");

        act.Should().Throw<JsonSerializationException>();
    }

    private static Manifest CreateImageManifest()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p. 1"), 1000, 800);
        var resource = new ImageResource("https://example.org/image.png", ImageFormat.Png)
            .SetHeight(1000)
            .SetWidth(800);
        canvas.AddAnnotation(new Annotation("https://example.org/annotation/1", resource, canvas.Id));

        var manifest = new Manifest("https://example.org/manifest", new Label("Single Image Example"));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");
        return manifest;
    }

    private static Manifest CreateAudioManifest()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Audio"), 1, 1)
            .SetDuration(10.5);
        var resource = new AudioResource("https://example.org/audio.mp4", "audio/mp4")
            .SetDuration(10.5);
        canvas.AddAnnotation(new Annotation("https://example.org/annotation/1", resource, canvas.Id));

        var manifest = new Manifest("https://example.org/manifest", new Label("Audio Example"));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");
        return manifest;
    }
}
