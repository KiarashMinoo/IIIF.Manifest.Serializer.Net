using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 4 (SDK_VERSIONING_GUIDE.md): Structure reshaped around 3.0-native Items
/// (CanvasReference/RangeReference/nested Structure), with Canvases/Ranges/Members as
/// computed legacy views.
/// </summary>
public class StructureReshapeTests
{
    [Fact]
    public void AddCanvasAndRangeReferences_Should_PopulateItemsAsThePrimary3_0Storage()
    {
        var structure = new Structure("https://example.org/range/1", new Label("Chapter 1"));
        structure.AddCanvasReference("https://example.org/canvas/1");
        structure.AddRangeReference("https://example.org/range/2");

        structure.Items.Should().HaveCount(2);
    }

    [Fact]
    public void CanvasesAndRanges_Should_BeComputedLegacyViews_ReflectingItems()
    {
        var structure = new Structure("https://example.org/range/1", new Label("Chapter 1"));
        structure.AddCanvasReference("https://example.org/canvas/1");
        structure.AddRangeReference("https://example.org/range/2");

        structure.Canvases.Should().ContainSingle(x => x == "https://example.org/canvas/1");
        structure.Ranges.Should().ContainSingle(x => x == "https://example.org/range/2");
        structure.Members.Should().HaveCount(2);
    }

    [Fact]
    public void NestedStructure_Should_SurfaceOnTheLegacyRangesView()
    {
        var parent = new Structure("https://example.org/range/1", new Label("Chapter 1"));
        var child = new Structure("https://example.org/range/1/section-1", new Label("Section 1"));
        parent.AddItem(child);

        parent.Ranges.Should().ContainSingle(x => x == "https://example.org/range/1/section-1");
    }

    [Fact]
    public void LegacyStructureJson_Should_DeserializeDirectlyIntoItems()
    {
        const string json = """
                            {
                              "@id": "https://example.org/range/1",
                              "@type": "sc:Range",
                              "label": "Chapter 1",
                              "canvases": ["https://example.org/canvas/1", "https://example.org/canvas/2"],
                              "ranges": ["https://example.org/range/2"]
                            }
                            """;

        var structure = JsonConvert.DeserializeObject<Structure>(json)!;

        structure.Items.Should().HaveCount(3);
        structure.Canvases.Should().BeEquivalentTo(["https://example.org/canvas/1", "https://example.org/canvas/2"]);
        structure.Ranges.Should().ContainSingle(x => x == "https://example.org/range/2");
    }

    [Fact]
    public void Structure_Should_NotLeak3_0ItemsIntoLegacyV2Json()
    {
        var structure = new Structure("https://example.org/range/1", new Label("Chapter 1"));
        structure.AddCanvasReference("https://example.org/canvas/1");

        var json = JsonConvert.SerializeObject(structure);
        var obj = JObject.Parse(json);

        obj["canvases"].Should().NotBeNull();
        obj["items"].Should().BeNull("Items is 3.0-native storage and must never leak into legacy JSON via reflection-based serialization");
    }

    [Fact]
    public void IiifSerializer_Should_WriteManifestStructuresAsV3Ranges()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.AddItem(new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800));

        var structure = new Structure("https://example.org/range/1", new Label("Chapter 1"));
        structure.AddCanvasReference("https://example.org/canvas/1");
        manifest.AddStructure(structure);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        var structures = obj["structures"] as JArray;
        structures.Should().NotBeNull();
        structures!.Should().ContainSingle();
        structures[0]!["id"]!.ToString().Should().Be("https://example.org/range/1");
        structures[0]!["type"]!.ToString().Should().Be("Range");
        structures[0]!["items"]![0]!["id"]!.ToString().Should().Be("https://example.org/canvas/1");
        structures[0]!["items"]![0]!["type"]!.ToString().Should().Be("Canvas");
    }

    [Fact]
    public void IiifSerializer_Should_ReadNestedV3Range()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Book"] },
                              "structures": [
                                {
                                  "id": "https://example.org/range/1",
                                  "type": "Range",
                                  "label": { "none": ["Chapter 1"] },
                                  "items": [
                                    { "id": "https://example.org/range/1/section-1", "type": "Range", "label": { "none": ["Section 1"] },
                                      "items": [ { "id": "https://example.org/canvas/1", "type": "Canvas" } ] }
                                  ]
                                }
                              ]
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);
        var structure = manifest.Structures.First();

        structure.Ranges.Should().ContainSingle(x => x == "https://example.org/range/1/section-1");
        var nested = structure.Items.OfType<Structure>().Single();
        nested.Canvases.Should().ContainSingle(x => x == "https://example.org/canvas/1");
    }

    [Theory]
    [InlineData(nameof(Structure.AddCanvas))]
    [InlineData(nameof(Structure.RemoveCanvas))]
    [InlineData(nameof(Structure.AddRange))]
    [InlineData(nameof(Structure.RemoveRange))]
    [InlineData(nameof(Structure.AddMember))]
    [InlineData(nameof(Structure.RemoveMember))]
    public void LegacyMutators_Should_BeMarkedObsoleteAsWarnings(string methodName)
    {
        var method = typeof(Structure).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var obsolete = method!.GetCustomAttribute<System.ObsoleteAttribute>();
        obsolete.Should().NotBeNull();
        obsolete!.IsError.Should().BeFalse("legacy mutators remain callable - deprecated with a warning, not a compile-time error");
    }

    [Theory]
    [InlineData(nameof(Structure.AddCanvas))]
    [InlineData(nameof(Structure.RemoveCanvas))]
    [InlineData(nameof(Structure.AddRange))]
    [InlineData(nameof(Structure.RemoveRange))]
    [InlineData(nameof(Structure.AddMember))]
    [InlineData(nameof(Structure.RemoveMember))]
    public void LegacyMutators_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation(string methodName)
    {
        var method = typeof(Structure).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var version = method!.GetCustomAttribute<IIIFVersionAttribute>();
        version.Should().NotBeNull("legacy mutators must document their deprecation via an IIIFVersionAttribute-derived attribute");
        version!.IsDeprecated.Should().BeTrue();
        version.DeprecatedInVersion.Should().Be("3.0");
        version.ReplacedBy.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(nameof(Structure.AddCanvasReference))]
    [InlineData(nameof(Structure.AddRangeReference))]
    public void ReplacementMutators_Should_NotBeObsolete(string methodName)
    {
        var method = typeof(Structure).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        method!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }

    [Theory]
    [InlineData(nameof(Structure.Canvases))]
    [InlineData(nameof(Structure.Ranges))]
    [InlineData(nameof(Structure.Members))]
    public void LegacyGetters_Should_NotBeObsolete(string propertyName)
    {
        var property = typeof(Structure).GetProperty(propertyName);

        property.Should().NotBeNull();
        property!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }
}
