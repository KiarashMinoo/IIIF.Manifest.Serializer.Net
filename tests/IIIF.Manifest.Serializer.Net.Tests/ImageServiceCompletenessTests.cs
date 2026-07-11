using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 14 (SDK_VERSIONING_GUIDE.md §10, finding 6): the Image API service descriptor was
/// missing the spec-required "protocol" field, never populated "type" at all, was missing
/// "extraFormats" and Tile.Height, and had no way to produce a standalone info.json document
/// (distinct JSON shape - unprefixed id/type - from the embedded-service form).
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
}
