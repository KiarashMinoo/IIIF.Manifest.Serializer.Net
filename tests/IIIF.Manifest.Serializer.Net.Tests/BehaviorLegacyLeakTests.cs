using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Round 2 (SDK_VERSIONING_GUIDE.md §10, Milestone 20): <c>BaseNode.Behavior</c> was missing the
///     <c>[JsonIgnore]</c> that its siblings (Rights/RequiredStatement/PartOf) got in Milestone 8,
///     so a plain <c>JsonConvert.SerializeObject</c> (the 2.x write path) leaked a spurious "behavior"
///     key into V2.0/V2.1 JSON. Also newly wires "behavior" into IiifSerializer's V3 Canvas/Range
///     writers, which previously dropped it entirely (a separate, narrower pre-existing gap).
/// </summary>
public class BehaviorLegacyLeakTests
{
    [Fact]
    public void Manifest_Should_NotLeakBehaviorIntoV2_1Json()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddBehavior(Behavior.Paged);

        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);

        obj["behavior"].Should().BeNull();
    }

    [Fact]
    public void Collection_Should_NotLeakBehaviorIntoV2_1Json()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test")).AddBehavior(Behavior.MultiPart);

        var json = IiifSerializer.Serialize(collection, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);

        obj["behavior"].Should().BeNull();
    }

    [Fact]
    public void Canvas_Should_NotLeakBehaviorViaPlainJsonConvert()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800).AddBehavior(Behavior.FacingPages);

        var json = JsonConvert.SerializeObject(canvas);
        var obj = JObject.Parse(json);

        obj["behavior"].Should().BeNull();
    }

    [Fact]
    public void Manifest_Should_RoundTripBehaviorThroughV3()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddBehavior(Behavior.Paged);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["behavior"]![0]!.ToString().Should().Be("paged");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.Behavior.Should().ContainSingle(x => x.Value == "paged");
    }

    [Fact]
    public void Canvas_Should_RoundTripBehaviorThroughV3()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800).AddBehavior(Behavior.FacingPages);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var deserialized = IiifSerializer.DeserializeManifest(json);

        var roundTrippedCanvas = deserialized.Items.OfType<Canvas>().Single();
        roundTrippedCanvas.Behavior.Should().ContainSingle(x => x.Value == "facing-pages");
    }

    [Fact]
    public void Structure_Should_RoundTripBehaviorThroughV3()
    {
        var structure = new Structure("https://example.org/range/1", new Label("Chapter 1")).AddBehavior(Behavior.Individuals);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"))
            .AddItem(new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800))
            .AddStructure(structure);

        var json = IiifSerializer.Serialize(manifest);
        var deserialized = IiifSerializer.DeserializeManifest(json);

        deserialized.Structures.Should().ContainSingle(x => x.Behavior.Count == 1 && x.Behavior.Any(b => b.Value == "individuals"));
    }
}