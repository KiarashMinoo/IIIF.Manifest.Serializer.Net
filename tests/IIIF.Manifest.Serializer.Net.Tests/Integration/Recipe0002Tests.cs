using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests.Integration;

/// <summary>
/// Tests for IIIF Cookbook Recipe 0002: Simplest Manifest - Single Audio File
/// https://iiif.io/api/cookbook/recipe/0002-mvm-audio/
/// </summary>
public class Recipe0002Tests
{
    [Fact]
    public void SimplestAudioManifest_Should_SerializeCorrectly()
    {
        // Arrange
        var audioResource = new AudioResource(
                id: "https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4",
                format: "audio/mp4"
            )
            .SetDuration(1985.024);

        var canvas = new Canvas(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas",
            label: new Label("Mahler, Symphony No. 3: CD 1"),
            height: 1,
            width: 1
        )
        .SetDuration(1985.024);

        var audio = new Audio(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation",
            resource: audioResource,
            on: canvas.Id
        );

        canvas.AddAudio(audio);

        var sequence = new Sequence(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/s0"
        );
        sequence.AddCanvas(canvas);

        var manifest = new Manifest(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
            label: new Label("Simplest Audio Example 1")
        );
        manifest.AddSequence(sequence);

        // Act
        var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
        var jObject = JObject.Parse(json);

        // Assert - Manifest structure
        jObject["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/2/context.json");
        jObject["@id"]!.ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json");
        jObject["@type"]!.ToString().Should().Be("sc:Manifest");
        jObject["label"]!.ToString().Should().Be("Simplest Audio Example 1");

        // Assert - Sequence and Canvas
        var sequences = jObject["sequences"] as JArray;
        sequences.Should().NotBeNull();
        sequences.Should().HaveCount(1);

        var canvases = sequences![0]!["canvases"] as JArray;
        canvases.Should().NotBeNull();
        canvases.Should().HaveCount(1);

        var canvasObj = canvases![0] as JObject;
        canvasObj.Should().NotBeNull();
        canvasObj!["@id"]!.ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas");
        canvasObj["duration"]!.Value<double>().Should().Be(1985.024);

        // v2 canvas keeps dimensions required by this model even for audio-only canvases.
        canvasObj["height"]!.Value<int>().Should().Be(1);
        canvasObj["width"]!.Value<int>().Should().Be(1);

        // Assert - Audio annotation and resource
        var audios = canvasObj["Audios"] as JArray;
        audios.Should().NotBeNull();
        audios.Should().HaveCount(1);

        var audioObj = audios![0] as JObject;
        audioObj.Should().NotBeNull();
        audioObj!["@id"]!.ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation");
        audioObj["@type"]!.ToString().Should().Be("oa:Annotation");
        audioObj["motivation"]!.ToString().Should().Be("sc:painting");
        audioObj["on"]!.ToString().Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas");

        var resource = audioObj["resource"] as JObject;
        resource.Should().NotBeNull();
        resource!["@id"]!.ToString().Should().Be("https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4");
        resource["@type"]!.ToString().Should().Be("dctypes:Sound");
        resource["format"]!.ToString().Should().Be("audio/mp4");
        resource["duration"]!.Value<double>().Should().Be(1985.024);
    }

    [Fact]
    public void SimplestAudioManifest_Should_RoundTrip()
    {
        // Arrange
        var audioResource = new AudioResource(
                id: "https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4",
                format: "audio/mp4"
            )
            .SetDuration(1985.024);

        var canvas = new Canvas(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas",
            label: new Label("Mahler, Symphony No. 3: CD 1"),
            height: 1,
            width: 1
        )
        .SetDuration(1985.024);

        var audio = new Audio(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation",
            resource: audioResource,
            on: canvas.Id
        );

        canvas.AddAudio(audio);

        var sequence = new Sequence(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/s0"
        );
        sequence.AddCanvas(canvas);

        var manifest = new Manifest(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
            label: new Label("Simplest Audio Example 1")
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
        deserializedManifest.Label.First().Value.Should().Be("Simplest Audio Example 1");

        deserializedManifest.Sequences.Should().HaveCount(1);
        var deserializedCanvas = deserializedManifest.Sequences.First().Canvases.First();
        deserializedCanvas.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas");
        deserializedCanvas.Duration.Should().Be(1985.024);

        deserializedCanvas.Audios.Should().HaveCount(1);
        var deserializedAudio = deserializedCanvas.Audios.First();
        deserializedAudio.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation");
        deserializedAudio.On.Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas");
        deserializedAudio.Resource.Should().NotBeNull();
        deserializedAudio.Resource!.Duration.Should().Be(1985.024);
        deserializedAudio.Resource.Format.Should().Be("audio/mp4");
    }
}
