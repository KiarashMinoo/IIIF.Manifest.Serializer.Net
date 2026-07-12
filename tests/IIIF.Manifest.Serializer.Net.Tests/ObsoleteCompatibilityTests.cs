using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

public class ObsoleteCompatibilityTests
{
    [Fact]
    public void ViewingHint_Should_BeReadableFromLegacyJson()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/2/context.json",
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Legacy Manifest",
                              "viewingHint": "paged",
                              "sequences": []
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);

#pragma warning disable CS0618
        manifest.ViewingHint.Should().NotBeNull();
        manifest.ViewingHint!.Value.Should().Be("paged");
#pragma warning restore CS0618
    }

    [Fact]
    public void SerializeV3_Should_MapLegacyViewingHintToBehavior()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/2/context.json",
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Legacy Manifest",
                              "viewingHint": "paged",
                              "sequences": []
                            }
                            """;
        var manifest = IiifSerializer.DeserializeManifest(json);

        var v3 = JObject.Parse(IiifSerializer.Serialize(manifest));

        v3["behavior"]!.Values<string>().Should().ContainSingle("paged");
        v3["viewingHint"].Should().BeNull();
    }

    [Fact]
    public void SerializeV2_Should_MapBehaviorToViewingHint_When_ValueIsSharedBetweenBoth()
    {
        // Behavior is the only 3.0-native storage - no SetViewingHint call - so the legacy v2.1
        // writer must derive viewingHint from behavior for values valid in both (issue #7's
        // "behavior -> viewingHint where safe" downgrade rule).
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"))
            .AddBehavior(Behavior.Paged);

        var v2 = JObject.Parse(IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1)));

        v2["viewingHint"]!.ToString().Should().Be("paged");
    }

    [Fact]
    public void SerializeV2_Should_OmitViewingHint_When_BehaviorHasNo2xEquivalent()
    {
        // "auto-advance" exists only in Behavior, not ViewingHint - must be omitted on legacy
        // write, never invented/guessed, per issue #7's "otherwise omit" downgrade rule.
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"))
            .AddBehavior(Behavior.AutoAdvance);

        var v2 = JObject.Parse(IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1)));

        v2["viewingHint"].Should().BeNull();
    }

    [Fact]
    public void SetViewingHint_Should_BePublicWriteApi()
    {
        var method = typeof(BaseNode<Manifest>).GetMethod(
            "SetViewingHint",
            BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
    }

    [Fact]
    public void Behavior_Should_BePublicWriteApi()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Latest Manifest"))
            .AddBehavior(Behavior.Paged);

        manifest.Behavior.Select(x => x.Value).Should().ContainSingle("paged");
    }
}
