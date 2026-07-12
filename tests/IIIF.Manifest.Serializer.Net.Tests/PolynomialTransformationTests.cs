using IIIF.Manifests.Serializer.Extensions.Transformations;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     The Georeference extension's <see cref="PolynomialTransformation" /> and
///     <see cref="PolynomialTransformationOption" /> (the "polynomial" of the two georeferencing
///     transformation algorithms - see SDK_VERSIONING_GUIDE.md's Georeference milestone).
///     <see cref="PolynomialTransformationOption.Order" /> previously had no <c>[JsonProperty]</c>
///     attribute and no way to set it via a public API (only a bare <c>private set</c>), so it could
///     never actually be given a non-zero value - fixed in SDK_VERSIONING_GUIDE.md Round 12 by
///     adding <c>[JsonProperty("order")]</c> and a fluent <see cref="PolynomialTransformationOption.SetOrder" />.
/// </summary>
public class PolynomialTransformationTests
{
    [Fact]
    public void Constructor_Should_SetTypeToPolynomial()
    {
        var transformation = new PolynomialTransformation(new PolynomialTransformationOption());

        transformation.Type.Value.Should().Be("polynomial");
    }

    [Fact]
    public void Options_Order_Should_RoundTripThroughJsonConvert()
    {
        var options = JsonConvert.DeserializeObject<PolynomialTransformationOption>("""{"order": 7}""")!;

        options.Order.Should().Be(7);
    }

    [Fact]
    public void Options_SetOrder_Should_BeSettableThroughFluentApi()
    {
        var options = new PolynomialTransformationOption().SetOrder(3);

        options.Order.Should().Be(3);

        var json = JsonConvert.SerializeObject(options);
        JObject.Parse(json)["order"]!.Value<long>().Should().Be(3);
    }

    [Fact]
    public void Should_RoundTripTheTypeThroughJsonConvert()
    {
        var transformation = new PolynomialTransformation(new PolynomialTransformationOption());

        var json = JsonConvert.SerializeObject(transformation);
        var obj = JObject.Parse(json);

        obj["type"]!.ToString().Should().Be("polynomial");

        var deserialized = JsonConvert.DeserializeObject<PolynomialTransformation>(json);
        deserialized!.Type.Value.Should().Be("polynomial");
    }
}