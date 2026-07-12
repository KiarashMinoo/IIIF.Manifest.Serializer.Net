using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Shared.Exceptions;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     The 3 custom JSON validation exceptions (<see cref="JsonNodeRequiredException{T}" />,
///     <see cref="JsonObjectMustBeJArray{T}" />, <see cref="JsonObjectMustBeJObject{T}" />) had zero test
///     coverage - none of the current hand-rolled converters happen to exercise their throw sites, so
///     these are constructed directly to verify the message shape a caller would eventually see.
/// </summary>
public class JsonExceptionTests
{
    [Fact]
    public void JsonNodeRequiredException_Should_IncludePropertyNameAndTypeInMessage()
    {
        var exception = new JsonNodeRequiredException<Manifest>("id");

        exception.Message.Should().Contain("id").And.Contain(nameof(Manifest)).And.Contain("required");
    }

    [Fact]
    public void JsonObjectMustBeJArray_Should_IncludePropertyNameAndTypeInMessage()
    {
        var exception = new JsonObjectMustBeJArray<Manifest>("items");

        exception.Message.Should().Contain("items").And.Contain(nameof(Manifest)).And.Contain("array");
    }

    [Fact]
    public void JsonObjectMustBeJObject_Should_IncludePropertyNameAndTypeInMessage()
    {
        var exception = new JsonObjectMustBeJObject<Manifest>("requiredStatement");

        exception.Message.Should().Contain("requiredStatement").And.Contain(nameof(Manifest)).And.Contain("object");
    }
}