using IIIF.Manifests.Serializer;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

public class IiifPresentationVersionDetectorTests
{
    [Fact]
    public void Detect_Should_ReturnV3_When_ContextIsPresentation3()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V3_0);
    }

    [Fact]
    public void Detect_Should_ReturnV2_1_When_ContextIsPresentation2()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/2/context.json",
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V2_1);
    }

    [Fact]
    public void Detect_Should_Read_ContextArray()
    {
        const string json = """
                            {
                              "@context": [
                                "https://example.org/context.json",
                                "http://iiif.io/api/presentation/3/context.json"
                              ],
                              "id": "https://example.org/manifest",
                              "type": "Manifest"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V3_0);
    }

    [Fact]
    public void Detect_Should_ReturnV2_1_When_LegacyShapeHasSequences()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "sequences": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V2_1);
    }

    [Fact]
    public void Detect_Should_ReturnV3_When_LatestShapeHasItems()
    {
        const string json = """
                            {
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "items": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V3_0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-json")]
    [InlineData("[]")]
    public void Detect_Should_ReturnUnknown_When_InputIsNotManifestObject(string json)
    {
        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.Unknown);
    }

    [Fact]
    public void Detect_Should_AcceptParsedTokens()
    {
        var token = JObject.Parse("""
                                  {
                                    "id": "https://example.org/manifest",
                                    "type": "Manifest"
                                  }
                                  """);

        IiifPresentationVersionDetector.Detect(token).Should().Be(IiifPresentationVersion.V3_0);
    }
}
