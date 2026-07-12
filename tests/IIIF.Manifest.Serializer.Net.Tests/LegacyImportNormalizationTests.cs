using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     SDK Phase 2 (issue #6): consolidated legacy-import normalization coverage. Most of the
///     individual mappings this issue asks for (description→summary, attribution→requiredStatement,
///     license→rights, within→partOf, related→homepage, sequences→items, images→AnnotationPage/
///     Annotation) were already implemented and tested per-property in earlier rounds
///     (BaseNodeReshapeTests.cs, ManifestSequenceReshapeTests.cs, CanvasReshapeTests.cs,
///     BehaviorLegacyLeakTests.cs) - this file adds the gaps that audit turned up: a direct
///     description→summary legacy-JSON-in test (the other four all had one, this one didn't), the
///     issue's own literal acceptance examples end-to-end, explicit round-trip tests in both
///     directions, unknown-property preservation, tolerant parsing of partial/minimal documents, and
///     the viewingHint→behavior write-time mapping.
/// </summary>
public class LegacyImportNormalizationTests
{
    [Fact]
    public void LegacyDescriptionJson_Should_DeserializeDirectlyIntoSummary()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "description": "A legacy description"
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.Summary.Should().ContainSingle(x => x.Value == "A legacy description");
        manifest.Description.Should().ContainSingle(x => x.Value == "A legacy description");
    }

    [Fact]
    public void LegacyAttributionAndLicense_Should_NormalizeIntoRequiredStatementAndRights()
    {
        // The issue's own literal acceptance example.
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/2/context.json",
                              "@id": "https://example.org/iiif/book/manifest",
                              "@type": "sc:Manifest",
                              "label": "Book",
                              "attribution": "Provided by Example Library",
                              "license": "http://creativecommons.org/licenses/by/4.0/",
                              "sequences": []
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);

        manifest.RequiredStatement.Should().NotBeNull();
        manifest.RequiredStatement!.Value.Should().ContainSingle(x => x.Value.Contains("Provided by Example Library"));
        manifest.Rights!.Value.Should().Be("http://creativecommons.org/licenses/by/4.0/");
    }

    [Fact]
    public void LegacySequenceCanvasImage_Should_NormalizeIntoItemsCanvasAnnotation()
    {
        // The issue's own literal acceptance example (sequences/canvases/images nested 3 levels
        // deep, exercising Manifest->Sequence->Canvas->Image normalization as one document, not
        // just as separately-tested layers).
        const string json = """
                            {
                              "@id": "https://example.org/iiif/book/manifest",
                              "@type": "sc:Manifest",
                              "label": "Book",
                              "sequences": [{
                                "@type": "sc:Sequence",
                                "canvases": [{
                                  "@id": "https://example.org/canvas/1",
                                  "@type": "sc:Canvas",
                                  "label": "Page 1",
                                  "height": 1000,
                                  "width": 800,
                                  "images": [{
                                    "@type": "oa:Annotation",
                                    "motivation": "sc:painting",
                                    "resource": {
                                      "@id": "https://example.org/page1.jpg",
                                      "@type": "dctypes:Image",
                                      "format": "image/jpeg"
                                    },
                                    "on": "https://example.org/canvas/1"
                                  }]
                                }]
                              }]
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.Items.Should().ContainSingle();
        var canvas = manifest.Items.OfType<Canvas>().Single();
        canvas.Id.Should().Be("https://example.org/canvas/1");
        canvas.Items.Should().NotBeEmpty();
        var annotation = canvas.Items.OfType<AnnotationPage>().SelectMany(x => x.Items).OfType<Annotation>().Single();
        annotation.Target.ToString().Should().Be("https://example.org/canvas/1");
    }

    [Fact]
    public void LegacyManifest_Should_RoundTripToV3_WithoutManualMigration()
    {
        const string legacyJson = """
                                  {
                                    "@context": "http://iiif.io/api/presentation/2/context.json",
                                    "@id": "https://example.org/iiif/book/manifest",
                                    "@type": "sc:Manifest",
                                    "label": "Book",
                                    "description": "A legacy description",
                                    "attribution": "Provided by Example Library",
                                    "license": "http://creativecommons.org/licenses/by/4.0/",
                                    "within": { "@id": "https://example.org/collection" },
                                    "related": "https://example.org/object-page",
                                    "sequences": [{
                                      "@type": "sc:Sequence",
                                      "canvases": [
                                        { "@id": "https://example.org/canvas/1", "@type": "sc:Canvas", "label": "Page 1", "height": 1000, "width": 800 }
                                      ]
                                    }]
                                  }
                                  """;

        var manifest = IiifSerializer.DeserializeManifest(legacyJson);
        var v3Json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(v3Json);

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/3/context.json");
        obj["summary"]!["none"]![0]!.ToString().Should().Be("A legacy description");
        obj["requiredStatement"]!["value"]!["none"]![0]!.ToString().Should().Contain("Provided by Example Library");
        obj["rights"]!.ToString().Should().Be("http://creativecommons.org/licenses/by/4.0/");
        obj["partOf"]![0]!["id"]!.ToString().Should().Be("https://example.org/collection");
        obj["homepage"]![0]!["id"]!.ToString().Should().Be("https://example.org/object-page");
        obj["items"]!.Should().ContainSingle();

        // No manual migration by the caller - legacy-only keys must not leak into 3.0 output.
        obj["description"].Should().BeNull();
        obj["attribution"].Should().BeNull();
        obj["license"].Should().BeNull();
        obj["within"].Should().BeNull();
        obj["related"].Should().BeNull();
        obj["sequences"].Should().BeNull();
    }

    [Fact]
    public void LegacyManifest_Should_RoundTripBackToV2_WithoutManualMigration()
    {
        const string legacyJson = """
                                  {
                                    "@context": "http://iiif.io/api/presentation/2/context.json",
                                    "@id": "https://example.org/iiif/book/manifest",
                                    "@type": "sc:Manifest",
                                    "label": "Book",
                                    "attribution": "Provided by Example Library",
                                    "license": "http://creativecommons.org/licenses/by/4.0/",
                                    "sequences": [{
                                      "@type": "sc:Sequence",
                                      "canvases": [
                                        { "@id": "https://example.org/canvas/1", "@type": "sc:Canvas", "label": "Page 1", "height": 1000, "width": 800 }
                                      ]
                                    }]
                                  }
                                  """;

        var manifest = IiifSerializer.DeserializeManifest(legacyJson);
        var v2Json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(v2Json);

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/2/context.json");
        obj["attribution"]!.ToString().Should().Be("Provided by Example Library");
        obj["license"]!.ToString().Should().Be("http://creativecommons.org/licenses/by/4.0/");
        obj["sequences"]![0]!["canvases"]![0]!["@id"]!.ToString().Should().Be("https://example.org/canvas/1");

        // 3.0-only keys must not appear in legacy output either.
        obj["requiredStatement"].Should().BeNull();
        obj["rights"].Should().BeNull();
        obj["items"].Should().BeNull();
    }

    [Fact]
    public void UnknownProperty_Should_SurviveRoundTrip_ThroughTheLegacyPath()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "sequences": [],
                              "x-vendor-custom-field": "some vendor-specific value"
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;
        var roundTripped = JObject.Parse(JsonConvert.SerializeObject(manifest));

        roundTripped["x-vendor-custom-field"]!.ToString().Should().Be("some vendor-specific value");
    }

    [Fact]
    public void MinimalLegacyManifest_Should_ParseWithoutThrowing_WhenOnlyRequiredFieldsPresent()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Minimal",
                              "sequences": []
                            }
                            """;

        var act = () => IiifSerializer.DeserializeManifest(json);

        act.Should().NotThrow();
        var manifest = IiifSerializer.DeserializeManifest(json);
        manifest.Items.Should().BeEmpty();
        manifest.RequiredStatement.Should().BeNull();
        manifest.Rights.Should().BeNull();
    }

    [Fact]
    public void LegacyManifestWithEmptyCanvasesAndNoImages_Should_ParseWithoutThrowing()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "sequences": [{
                                "@type": "sc:Sequence",
                                "canvases": [
                                  { "@id": "https://example.org/canvas/1", "@type": "sc:Canvas", "label": "Blank page", "height": 1000, "width": 800 }
                                ]
                              }]
                            }
                            """;

        var act = () => JsonConvert.DeserializeObject<Manifest>(json);

        act.Should().NotThrow();
        var manifest = act()!;
        var canvas = manifest.Items.OfType<Canvas>().Single();
        canvas.Items.Should().BeEmpty();
    }

    [Fact]
    public void ViewingHint_Should_MapToBehavior_OnV3Write_WhenBehaviorIsNotAlreadySet()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "viewingHint": "paged",
                              "sequences": []
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;
        var v3Json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(v3Json);

        obj["behavior"]![0]!.ToString().Should().Be("paged");
    }
}