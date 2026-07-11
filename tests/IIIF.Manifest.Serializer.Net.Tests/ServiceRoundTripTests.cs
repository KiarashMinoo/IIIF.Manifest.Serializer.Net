using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Properties.Services.Auth2;
using IIIF.Manifests.Serializer.Properties.Services.Discovery;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 5 (SDK_VERSIONING_GUIDE.md): explicit round-trip regression tests per service type,
/// locking in the interface-level-converter / constructor-context-collision / missing-setter
/// fixes made prior to this milestone, plus the new top-level Manifest.Services (3.0-only,
/// centralized services array — no 2.x equivalent, always inlined on write per §5). Auth 2.0 cases
/// updated in Milestone 9 (§10) to use the 4 real service types instead of the old flat AuthService2.
/// </summary>
public class ServiceRoundTripTests
{
    [Fact]
    public void ImageService_Should_RoundTripThroughInlineServiceProperty()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddItem(canvas);
        var service = new Service("http://iiif.io/api/image/2/context.json", "https://example.org/image-service", "http://iiif.io/api/image/2/level2.json")
            .SetHeight(1000).SetWidth(800);
        manifest.AddService(service);

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        deserialized.Service.Should().ContainSingle();
        var roundTripped = deserialized.Service.OfType<Service>().Single();
        roundTripped.Profile.Should().Be("http://iiif.io/api/image/2/level2.json");
        roundTripped.Height.Should().Be(1000);
        roundTripped.Width.Should().Be(800);
    }

    [Fact]
    public void AuthService1_Should_RoundTripThroughInlineServiceProperty()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddService(new AuthService1("https://example.org/auth/login", "http://iiif.io/api/auth/1/login").SetLabel("Login"));

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        var auth = deserialized.Service.OfType<AuthService1>().Single();
        auth.Profile.Should().Be("http://iiif.io/api/auth/1/login");
        auth.Label.Should().Be("Login");
    }

    [Fact]
    public void AuthAccessService2Chain_Should_RoundTripThroughInlineServiceProperty()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        var accessTokenService = new AuthAccessTokenService2("https://example.org/auth/token");
        var accessService = new AuthAccessService2("https://example.org/auth/login", "active", accessTokenService)
            .SetLabel("Login").SetHeading("Sign in")
            .SetLogoutService(new AuthLogoutService2("https://example.org/auth/logout", "Logout"));
        manifest.AddService(new AuthProbeService2("https://example.org/auth/probe", accessService));

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        var probe = deserialized.Service.OfType<AuthProbeService2>().Single();
        var access = probe.AccessServices.Single();
        access.Profile.Should().Be("active");
        access.Heading.Single().Value.Should().Be("Sign in");
        access.AccessTokenService.Id.Should().Be("https://example.org/auth/token");
        access.LogoutService!.Label.Single().Value.Should().Be("Logout");
    }

    [Fact]
    public void AuthAccessService2_External_Should_OmitId()
    {
        var accessTokenService = new AuthAccessTokenService2("https://example.org/auth/token");
        var external = AuthAccessService2.ForExternalProfile(accessTokenService);

        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddService(new AuthProbeService2("https://example.org/auth/probe", external));

        var json = JsonConvert.SerializeObject(manifest);
        var obj = JObject.Parse(json);
        var accessObj = (JObject)obj["service"]!["service"]!;

        accessObj["profile"]!.ToString().Should().Be("external");
        accessObj["id"].Should().BeNull();
    }

    [Fact]
    public void SearchAndAutoCompleteService_Should_RoundTripThroughInlineServiceProperty()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        var search = new SearchService("http://iiif.io/api/search/2/context.json", "https://example.org/search", "http://iiif.io/api/search/0/search")
            .AddService(new AutoCompleteService("http://iiif.io/api/search/2/context.json", "https://example.org/autocomplete", "http://iiif.io/api/search/0/autocomplete"));
        manifest.AddService(search);

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        var roundTripped = deserialized.Service.OfType<SearchService>().Single();
        roundTripped.Profile.Should().Be("http://iiif.io/api/search/0/search");
        roundTripped.Services.Should().ContainSingle();
        roundTripped.Services.First().Profile.Should().Be("http://iiif.io/api/search/0/autocomplete");
    }

    [Fact]
    public void DiscoveryService_Should_RoundTripThroughInlineServiceProperty()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddService(new DiscoveryService("http://iiif.io/api/discovery/1/context.json", "https://example.org/discovery",
            new DiscoveryResourceReference("https://example.org/discovery/page-0", "OrderedCollectionPage")));

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        deserialized.Service.OfType<DiscoveryService>().Should().ContainSingle();
    }

    [Fact]
    public void ContentStateService_Should_RoundTripThroughInlineServiceProperty()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddService(new ContentStateService("http://iiif.io/api/content-state/1/context.json", "https://example.org/content-state", "http://iiif.io/api/content-state/v1/state"));

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        deserialized.Service.OfType<ContentStateService>().Should().ContainSingle();
    }

    [Fact]
    public void TopLevelServices_Should_BeAThreeDotOhOnlyCentralizedArray()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddTopLevelService(new AuthAccessTokenService2("https://example.org/auth/token"));

        manifest.Services.Should().ContainSingle();

        var json = JsonConvert.SerializeObject(manifest);
        var obj = JObject.Parse(json);
        obj["services"].Should().NotBeNull();

        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;
        deserialized.Services.OfType<AuthAccessTokenService2>().Should().ContainSingle(x => x.Id == "https://example.org/auth/token");
    }

    [Fact]
    public void TopLevelServices_Should_RoundTripFromV3Json()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Test"] },
                              "services": [
                                { "id": "https://example.org/auth/token", "type": "AuthAccessTokenService2" }
                              ]
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);

        manifest.Services.Should().ContainSingle();
        manifest.Services.OfType<AuthAccessTokenService2>().Single().Id.Should().Be("https://example.org/auth/token");
    }

    [Fact]
    public void TopLevelServices_Should_WriteAsV3Array()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddTopLevelService(new AuthAccessTokenService2("https://example.org/auth/token"));

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        var services = obj["services"] as JArray;
        services.Should().NotBeNull();
        services!.Should().ContainSingle();
        services[0]!["id"]!.ToString().Should().Be("https://example.org/auth/token");
        services[0]!["type"]!.ToString().Should().Be("AuthAccessTokenService2");
        services[0]!["@id"].Should().BeNull();
        services[0]!["@type"].Should().BeNull();
    }

    [Fact]
    public void TopLevelServices_Should_NotBeConfusedWithInlineService()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddService(new AuthService1("https://example.org/auth/inline", "http://iiif.io/api/auth/1/login"));
        manifest.AddTopLevelService(new AuthAccessTokenService2("https://example.org/auth/toplevel"));

        manifest.Service.Should().ContainSingle(x => ((AuthService1)x).Id == "https://example.org/auth/inline");
        manifest.Services.Should().ContainSingle(x => ((AuthAccessTokenService2)x).Id == "https://example.org/auth/toplevel");
    }
}
