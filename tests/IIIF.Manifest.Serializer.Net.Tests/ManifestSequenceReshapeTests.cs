using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 2 (SDK_VERSIONING_GUIDE.md): Manifest reshaped around 3.0-native Items (Canvas),
/// with Sequences as a computed single-sequence legacy view; Sequence itself stays a legacy-only
/// shim (decision §6). Multi-sequence documents are preserved (first-wins + diagnostic), not
/// silently truncated.
/// </summary>
public class ManifestSequenceReshapeTests
{
    private static Canvas CreateCanvas(string id) =>
        new Canvas(id, new Label("Page"), 1000, 800)
            .AddAnnotation(new Annotation($"{id}/annotation", new ImageResource($"{id}/image.png", "image/png"), id));

    [Fact]
    public void AddItem_Should_PopulateItemsAsThePrimary3_0Storage()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        var canvas = CreateCanvas("https://example.org/canvas/1");

        manifest.AddItem(canvas);

        manifest.Items.Should().ContainSingle();
        manifest.Items.OfType<Canvas>().Should().ContainSingle(x => x.Id == canvas.Id);
    }

    [Fact]
    public void Sequences_Should_BeAComputedLegacyView_ReflectingItems()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        var canvas = CreateCanvas("https://example.org/canvas/1");
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");

        manifest.Sequences.Should().ContainSingle();
        var sequence = manifest.Sequences.First();
        sequence.Id.Should().Be("https://example.org/sequence/normal");
        sequence.Canvases.Should().ContainSingle(x => x.Id == canvas.Id);
    }

    [Fact]
    public void ViewingDirectionAndStart_Should_MirrorOntoTheSynthesizedSequence()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddItem(CreateCanvas("https://example.org/canvas/1"));
        manifest.SetViewingDirection(ViewingDirection.Ltr);
        manifest.SetStart("https://example.org/canvas/1");

        var sequence = manifest.Sequences.First();
        sequence.ViewingDirection!.Value.Should().Be("left-to-right");
        sequence.StartCanvas!.Id.Should().Be("https://example.org/canvas/1");
    }

    [Fact]
    public void LegacyManifestJson_Should_DeserializeDirectlyIntoItems()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "sequences": [
                                {
                                  "@id": "https://example.org/sequence/normal",
                                  "@type": "sc:Sequence",
                                  "viewingDirection": "left-to-right",
                                  "canvases": [
                                    { "@id": "https://example.org/canvas/1", "@type": "sc:Canvas", "label": "p1", "height": 1000, "width": 800 }
                                  ]
                                }
                              ]
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.Items.OfType<Canvas>().Should().ContainSingle(x => x.Id == "https://example.org/canvas/1");
        manifest.ViewingDirection!.Value.Should().Be("left-to-right");

        // The legacy view getter reflects the same data.
        manifest.Sequences.Should().ContainSingle();
        manifest.Sequences.First().Id.Should().Be("https://example.org/sequence/normal");
    }

    [Fact]
    public void MultiSequenceLegacyDocument_Should_PreserveExtraSequences_NotSilentlyDropThem()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "sequences": [
                                {
                                  "@id": "https://example.org/sequence/1",
                                  "@type": "sc:Sequence",
                                  "canvases": [ { "@id": "https://example.org/canvas/1", "@type": "sc:Canvas", "label": "p1", "height": 1000, "width": 800 } ]
                                },
                                {
                                  "@id": "https://example.org/sequence/2",
                                  "@type": "sc:Sequence",
                                  "canvases": [ { "@id": "https://example.org/canvas/2", "@type": "sc:Canvas", "label": "p2", "height": 1000, "width": 800 } ]
                                }
                              ]
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        // First sequence wins the 3.0-native Items.
        manifest.Items.OfType<Canvas>().Should().ContainSingle(x => x.Id == "https://example.org/canvas/1");

        // The second sequence is not silently dropped.
        manifest.AdditionalSequences.Should().ContainSingle(x => x.Id == "https://example.org/sequence/2");
        manifest.Sequences.Should().HaveCount(2);

        // Re-serializing as legacy loses no sequences.
        var json2 = JsonConvert.SerializeObject(manifest);
        JObject.Parse(json2)["sequences"]!.Should().HaveCount(2);
    }

    [Fact]
    public void Manifest_Should_NotLeak3_0ItemsIntoLegacyV2Json()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddItem(CreateCanvas("https://example.org/canvas/1"));

        var json = JsonConvert.SerializeObject(manifest);
        var obj = JObject.Parse(json);

        obj["sequences"].Should().NotBeNull();
        obj["items"].Should().BeNull("Items is 3.0-native storage and must never leak into legacy JSON via reflection-based serialization");
    }

    [Fact]
    public void ThreeDotOhConstructedManifest_Should_SerializeAsStructurallyValidLegacyV2Json()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddItem(CreateCanvas("https://example.org/canvas/1"));
        manifest.SetSequenceId("https://example.org/sequence/normal");

        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);

        obj["items"].Should().BeNull();
        var sequences = obj["sequences"] as JArray;
        sequences.Should().NotBeNull();
        sequences!.Should().ContainSingle();
        sequences[0]!["@id"]!.ToString().Should().Be("https://example.org/sequence/normal");
        sequences[0]!["canvases"]![0]!["@id"]!.ToString().Should().Be("https://example.org/canvas/1");
    }

    [Fact]
    public void LegacyManifest_Should_CrossSerializeAsV3()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "sequences": [
                                {
                                  "@id": "https://example.org/sequence/normal",
                                  "@type": "sc:Sequence",
                                  "canvases": [ { "@id": "https://example.org/canvas/1", "@type": "sc:Canvas", "label": "p1", "height": 1000, "width": 800 } ]
                                }
                              ]
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;
        var v3Json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(v3Json);

        obj["items"]!.Should().HaveCount(1);
        obj["items"]![0]!["id"]!.ToString().Should().Be("https://example.org/canvas/1");
        obj["sequences"].Should().BeNull();
    }

    [Theory]
    [InlineData(nameof(Manifest.AddSequence))]
    [InlineData(nameof(Manifest.RemoveSequence))]
    public void LegacyMutators_Should_BeMarkedObsoleteAsWarnings(string methodName)
    {
        var method = typeof(Manifest).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var obsolete = method!.GetCustomAttribute<System.ObsoleteAttribute>();
        obsolete.Should().NotBeNull();
        obsolete!.IsError.Should().BeFalse("legacy mutators remain callable - deprecated with a warning, not a compile-time error");
    }

    [Theory]
    [InlineData(nameof(Manifest.AddSequence))]
    [InlineData(nameof(Manifest.RemoveSequence))]
    public void LegacyMutators_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation(string methodName)
    {
        var method = typeof(Manifest).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var version = method!.GetCustomAttribute<IIIFVersionAttribute>();
        version.Should().NotBeNull("legacy mutators must document their deprecation via an IIIFVersionAttribute-derived attribute");
        version!.IsDeprecated.Should().BeTrue();
        version.DeprecatedInVersion.Should().Be("3.0");
        version.ReplacedBy.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SequencesGetter_Should_NotBeObsolete()
    {
        var property = typeof(Manifest).GetProperty(nameof(Manifest.Sequences));

        property.Should().NotBeNull();
        property!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }

    [Fact]
    public void SetSequenceId_Should_NotBeObsolete()
    {
        var method = typeof(Manifest).GetMethod(nameof(Manifest.SetSequenceId), BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        method!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }
}
