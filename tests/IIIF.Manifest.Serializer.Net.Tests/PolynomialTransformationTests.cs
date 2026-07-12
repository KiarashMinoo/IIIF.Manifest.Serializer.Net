using IIIF.Manifests.Serializer.Extensions.Transformations;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// The Georeference extension's <see cref="PolynomialTransformation"/> and
/// <see cref="PolynomialTransformationOption"/> (the "polynomial" of the two georeferencing
/// transformation algorithms - see SDK_VERSIONING_GUIDE.md's Georeference milestone) had zero test
/// coverage, and writing these tests surfaced a real, currently-dead bug.
///
/// <para>
/// <strong>Known bug, not fixed here</strong> (the test-generation task this file was written under
/// scopes to tests only, never production code): <see cref="PolynomialTransformationOption.Order"/>
/// has no <c>[JsonProperty]</c> attribute and no public constructor parameter, only a
/// <c>private set</c> - so <c>Order</c> can never actually be given a non-zero value through any
/// public API (not via JSON deserialization, since Newtonsoft's default contract resolver only
/// treats a private setter as writable when <c>[JsonProperty]</c> is explicitly present on the
/// member, and every other settable property in this codebase does carry that attribute for
/// exactly this reason; not via a fluent setter, since none exists). A repo-wide search confirms
/// nothing in the codebase currently constructs a <c>PolynomialTransformationOption</c> with a real
/// order value either, so this is dead-on-arrival rather than an active regression. Flagged for a
/// future fix (add <c>[JsonProperty("order")]</c> plus either a public constructor parameter or a
/// fluent <c>SetOrder</c>, matching this codebase's established pattern) rather than fixed here.
/// </para>
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
    public void Options_Order_Should_DefaultToZero_AndCannotCurrentlyBeSet_KnownBug()
    {
        // No [JsonProperty]/public constructor/fluent setter exists for Order (see class-level
        // remarks), so JSON deserialization silently leaves it at its default regardless of input.
        var options = JsonConvert.DeserializeObject<PolynomialTransformationOption>("""{"order": 7}""")!;

        options.Order.Should().Be(0);
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
