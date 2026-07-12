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
    [InlineData("   ")]
    [InlineData("not-json")]
    [InlineData("[]")]
    [InlineData("null")]
    [InlineData("true")]
    [InlineData("42")]
    [InlineData("\"just a string\"")]
    public void Detect_Should_ReturnUnknown_When_InputIsNotManifestObject(string json)
    {
        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.Unknown);
    }

    [Fact]
    public void Detect_Should_ReturnV2_1_When_ContextIsPresentation2_EvenThoughDocumentIsActually2_0()
    {
        // 2.0 and 2.1 share the exact same @context (confirmed against the live spec - see
        // IiifPresentationVersion.V2_0's remarks), so a genuine 2.0 document is indistinguishable
        // from 2.1 by context and is intentionally, deterministically detected as V2_1.
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/2/context.json",
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "sequences": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V2_1);
    }

    [Fact]
    public void Detect_Should_ReturnV2_0_When_ContextIsTheNonStandardPresentation2_0Url()
    {
        // Not a real published resource (2.0 never had its own distinct context.json) - this
        // exercises the defensive fallback for non-conformant tooling, and is the only way
        // Detect() can ever actually return V2_0 rather than V2_1.
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/2.0/context.json",
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V2_0);
    }

    [Fact]
    public void Detect_Should_ReturnMetadata1_0_When_ContextIsSharedCanvas()
    {
        const string json = """
                            {
                              "@context": "http://www.shared-canvas.org/ns/context.json",
                              "@id": "http://example.org/iiif/book1/manifest.json",
                              "@type": "sc:Manifest",
                              "sequences": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.Metadata_1_0);
    }

    [Fact]
    public void Detect_Should_ReturnV4_0Rc_When_ContextIsPresentation4()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/4/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V4_0_Rc);
    }

    [Fact]
    public void Detect_Should_ReturnV4_0Rc_When_ContextArrayContainsPresentation4()
    {
        const string json = """
                            {
                              "@context": [
                                "https://example.org/extension-context.json",
                                "http://iiif.io/api/presentation/4/context.json"
                              ],
                              "id": "https://example.org/manifest",
                              "type": "Manifest"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V4_0_Rc);
    }

    [Fact]
    public void Detect_Should_ReturnV2_1_When_PayloadHasBothLegacyAndModernStructuralSignals()
    {
        // Mixed/ambiguous payload: both "sequences" (2.x) and "items" (3.0) present, no @context.
        // Documented, deterministic priority: legacy signals (@id/@type/sequences) win over 3.0
        // signals (id/type/items) when both are present with no context to disambiguate.
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "sequences": [],
                              "items": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V2_1);
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

    [Fact]
    public void Detect_Should_ReturnV2_1_When_CollectionHasLegacyType()
    {
        const string json = """
                            {
                              "@id": "https://example.org/collection",
                              "@type": "sc:Collection",
                              "manifests": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V2_1);
    }

    [Fact]
    public void Detect_Should_ReturnV3_When_CollectionHasLatestType()
    {
        const string json = """
                            {
                              "id": "https://example.org/collection",
                              "type": "Collection",
                              "items": []
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V3_0);
    }

    [Fact]
    public void Detect_Should_ReturnV3_When_CollectionContextIsPresentation3()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/collection",
                              "type": "Collection"
                            }
                            """;

        IiifPresentationVersionDetector.Detect(json).Should().Be(IiifPresentationVersion.V3_0);
    }
}
