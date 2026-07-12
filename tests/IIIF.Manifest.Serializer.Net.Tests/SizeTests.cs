using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     <see cref="Size" /> (an IIIF Image API "sizes" array entry: a plain width/height pair) had zero
///     test coverage.
/// </summary>
public class SizeTests
{
    [Fact]
    public void Constructor_Should_SetWidthAndHeight()
    {
        var size = new Size(1200, 800);

        size.Width.Should().Be(1200);
        size.Height.Should().Be(800);
    }

    [Fact]
    public void Should_RoundTripThroughJsonConvert()
    {
        var size = new Size(600, 400);

        var json = JsonConvert.SerializeObject(size);
        var obj = JObject.Parse(json);

        obj["width"]!.Value<int>().Should().Be(600);
        obj["height"]!.Value<int>().Should().Be(400);

        var deserialized = JsonConvert.DeserializeObject<Size>(json);
        deserialized!.Width.Should().Be(600);
        deserialized.Height.Should().Be(400);
    }
}