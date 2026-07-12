using IIIF.Manifests.Serializer.Properties.Services;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 14 (SDK_VERSIONING_GUIDE.md §10, finding 6): the Image API service descriptor was
///     missing the spec-required "protocol" field, never populated "type" at all, was missing
///     "extraFormats" and Tile.Height, and had no way to produce a standalone info.json document
///     (distinct JSON shape - unprefixed id/type - from the embedded-service form).
/// </summary>
public class ImageServiceCompletenessTests
{
    [Fact]
    public void Service_Should_DefaultToImageService3_AndWriteProtocol()
    {
        var service = new Service("http://iiif.io/api/image/3/context.json", "https://example.org/image-service", "level2");

        var obj = JObject.Parse(service.Serialize());

        obj["type"].Should().BeNull();
        obj["@type"]!.ToString().Should().Be("ImageService3");
        obj["protocol"]!.ToString().Should().Be("http://iiif.io/api/image");
    }

    [Fact]
    public void Service_AsImageService2_Should_SwitchType()
    {
        var service = new Service("http://iiif.io/api/image/2/context.json", "https://example.org/image-service", "level2")
            .AsImageService2();

        var obj = JObject.Parse(service.Serialize());

        obj["@type"]!.ToString().Should().Be("ImageService2");
    }

    [Fact]
    public void Service_Should_RoundTripExtraFormatsAndTileHeight()
    {
        var service = new Service("http://iiif.io/api/image/3/context.json", "https://example.org/image-service", "level2")
            .AddExtraFormat("webp")
            .AddTile(new Tile().SetWidth(512).SetHeight(256).AddScaleFactor(1).AddScaleFactor(2));

        var obj = JObject.Parse(service.Serialize());

        obj["extraFormats"]![0]!.ToString().Should().Be("webp");
        obj["tiles"]![0]!["height"]!.Value<int>().Should().Be(256);
    }

    [Fact]
    public void Service_ToInfoJson_Should_WriteUnprefixedIdAndType_ButKeepContextPrefixed()
    {
        var service = new Service("http://iiif.io/api/image/3/context.json", "https://example.org/image-service", "level2");

        var obj = JObject.Parse(service.ToInfoJson());

        obj["id"]!.ToString().Should().Be("https://example.org/image-service");
        obj["type"]!.ToString().Should().Be("ImageService3");
        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/image/3/context.json");
        obj["@id"].Should().BeNull();
        obj["@type"].Should().BeNull();
    }

    [Fact]
    public void Service_FromInfoJson_Should_RoundTrip_ThroughToInfoJson()
    {
        var service = new Service("http://iiif.io/api/image/3/context.json", "https://example.org/image-service", "level2")
            .SetWidth(6000)
            .SetHeight(4000)
            .AddTile(new Tile().SetWidth(512).AddScaleFactor(1).AddScaleFactor(2));

        var infoJson = service.ToInfoJson();
        var parsed = Service.FromInfoJson(infoJson);

        parsed.Id.Should().Be(service.Id);
        parsed.Type.Should().Be("ImageService3");
        parsed.Profile.Should().Be("level2");
        parsed.Width.Should().Be(6000);
        parsed.Height.Should().Be(4000);
        parsed.Tiles.Should().ContainSingle(x => x.Width == 512);
    }

    [Fact]
    public void Service_FromInfoJson_Should_ParseARawStandaloneInfoJsonDocument()
    {
        // The issue's own literal example from GitHub issue #8.
        const string infoJson = """
                                {
                                  "@context": "http://iiif.io/api/image/3/context.json",
                                  "id": "https://example.org/iiif/page1",
                                  "type": "ImageService3",
                                  "protocol": "http://iiif.io/api/image",
                                  "profile": "level2",
                                  "width": 6000,
                                  "height": 4000,
                                  "tiles": [{ "width": 512, "scaleFactors": [1, 2, 4, 8] }]
                                }
                                """;

        var service = Service.FromInfoJson(infoJson);

        service.Id.Should().Be("https://example.org/iiif/page1");
        service.Profile.Should().Be("level2");
        service.Width.Should().Be(6000);
        service.Height.Should().Be(4000);
        service.Tiles.Should().ContainSingle(x => x.Width == 512);
    }

    [Fact]
    public void Service_FromInfoJson_Should_Throw_When_JsonIsBlank()
    {
        var act = () => Service.FromInfoJson("   ");

        act.Should().Throw<ArgumentException>();
    }
}