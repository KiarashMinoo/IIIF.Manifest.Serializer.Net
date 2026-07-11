using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Properties.Services.Auth2;
using IIIF.Manifests.Serializer.Properties.Services.Discovery;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 9 (SDK_VERSIONING_GUIDE.md §10): cross-cutting service bugs found while comparing the
/// SDK against https://github.com/IIIF/awesome-iiif - the top-level Manifest.Services (3.0-only)
/// array previously dropped "type" entirely instead of unprefixing it, and mis-renamed "@context" to
/// a nonexistent "context" key. Search/AutoComplete/Discovery services (which postdate the
/// Presentation 3.0 "no @ prefix" convention and never had a prefixed form) now model id/type via
/// UnprefixedBaseItem instead of inheriting BaseItem's @id/@type.
/// </summary>
public class Milestone9ServiceFixTests
{
    [Fact]
    public void SearchService_Should_WriteUnprefixedIdAndTypeInTopLevelServicesArray()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddTopLevelService(new SearchService("http://iiif.io/api/search/2/context.json", "https://example.org/search", "http://iiif.io/api/search/2/search"));

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var service = ((JArray)obj["services"]!)[0]!;

        service["id"]!.ToString().Should().Be("https://example.org/search");
        service["type"]!.ToString().Should().Be("SearchService2");
        service["@context"]!.ToString().Should().Be("http://iiif.io/api/search/2/context.json");
        service["@id"].Should().BeNull();
        service["@type"].Should().BeNull();
        service["context"].Should().BeNull();
    }

    [Fact]
    public void SearchService_Should_RoundTripThroughV3TopLevelServicesArray()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddTopLevelService(new SearchService("http://iiif.io/api/search/2/context.json", "https://example.org/search", "http://iiif.io/api/search/2/search"));

        var json = IiifSerializer.Serialize(manifest);
        var deserialized = IiifSerializer.DeserializeManifest(json);

        var search = deserialized.Services.OfType<SearchService>().Single();
        search.Id.Should().Be("https://example.org/search");
        search.Type.Should().Be("SearchService2");
        search.Profile.Should().Be("http://iiif.io/api/search/2/search");
    }

    [Fact]
    public void DiscoveryService_Should_RoundTripThroughV3TopLevelServicesArray()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddTopLevelService(new DiscoveryService("http://iiif.io/api/discovery/1/context.json", "https://example.org/discovery",
            new DiscoveryResourceReference("https://example.org/discovery/page-0", "OrderedCollectionPage")));

        var json = IiifSerializer.Serialize(manifest);
        var deserialized = IiifSerializer.DeserializeManifest(json);

        var discovery = deserialized.Services.OfType<DiscoveryService>().Single();
        discovery.Type.Should().Be("OrderedCollection");
    }

    [Fact]
    public void HandWrittenV3Json_WithUnprefixedTypeField_Should_DispatchToSearchService()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Test"] },
                              "services": [
                                { "id": "https://example.org/search", "type": "SearchService2", "profile": "http://iiif.io/api/search/2/search" }
                              ]
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);

        var search = manifest.Services.OfType<SearchService>().Single();
        search.Id.Should().Be("https://example.org/search");
        search.Profile.Should().Be("http://iiif.io/api/search/2/search");
    }

    [Fact]
    public void AuthAccessTokenService2_Should_WriteUnprefixedShape_EvenUnderV2_1Serialize()
    {
        // Milestone 11 replaced the old flat AuthService2 (which inherited BaseItem's @id/@type)
        // with the 4 real Auth 2.0 service types on UnprefixedBaseItem. Auth 2.0 has no 2.x form at
        // all, so its embedded-service shape doesn't depend on the enclosing document's declared
        // Presentation API version - this locks in that it always writes unprefixed id/type.
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddItem(canvas);
        manifest.AddService(new AuthAccessTokenService2("https://example.org/auth/token"));

        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);
        var service = obj["service"]!;

        service["id"]!.ToString().Should().Be("https://example.org/auth/token");
        service["type"]!.ToString().Should().Be("AuthAccessTokenService2");
        service["@id"].Should().BeNull();
    }
}
