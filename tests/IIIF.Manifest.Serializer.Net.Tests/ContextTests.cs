namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// <see cref="Context"/>'s 11 static named instances (one per IIIF/W3C context URL this SDK
/// recognizes) had zero test coverage.
/// </summary>
public class ContextTests
{
    [Fact]
    public void NamedInstances_Should_ExposeThePresentationApiContextUrls()
    {
        Context.Presentation2.Value.Should().Be("http://iiif.io/api/presentation/2/context.json");
        Context.Presentation3.Value.Should().Be("http://iiif.io/api/presentation/3/context.json");
    }

    [Fact]
    public void NamedInstances_Should_ExposeTheImageApiContextUrls()
    {
        Context.Image2.Value.Should().Be("http://iiif.io/api/image/2/context.json");
        Context.Image3.Value.Should().Be("http://iiif.io/api/image/3/context.json");
    }

    [Fact]
    public void NamedInstances_Should_ExposeTheAuthApiContextUrls()
    {
        Context.Auth1.Value.Should().Be("http://iiif.io/api/auth/1/context.json");
        Context.Auth2.Value.Should().Be("http://iiif.io/api/auth/2/context.json");
    }

    [Fact]
    public void NamedInstances_Should_ExposeTheSearchDiscoveryContentStateAndWebAnnotationContextUrls()
    {
        Context.Search1.Value.Should().Be("http://iiif.io/api/search/1/context.json");
        Context.Search2.Value.Should().Be("http://iiif.io/api/search/2/context.json");
        Context.Discovery1.Value.Should().Be("http://iiif.io/api/discovery/1/context.json");
        Context.ContentState1.Value.Should().Be("http://iiif.io/api/content-state/1/context.json");
        Context.WebAnnotation.Value.Should().Be("http://www.w3.org/ns/anno.jsonld");
    }

    [Fact]
    public void Should_SupportCustomValues()
    {
        var custom = new Context("https://example.org/custom-context.json");

        custom.Value.Should().Be("https://example.org/custom-context.json");
    }
}
