using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 8 consistency sweep (SDK_VERSIONING_GUIDE.md §4): License/Attribution/Within/Related
///     were flagged as needing the same legacy-view-plus-obsolete-mutator pattern as Milestones 1-5,
///     but had not actually been reshaped. Rights/RequiredStatement/PartOf are the new 3.0-native
///     properties; License/Attribution/Within/Related become computed legacy views over them, and
///     Related is confirmed (via the 3.0 change log) to map onto the existing Homepage property.
/// </summary>
public class BaseNodeReshapeTests
{
    [Fact]
    public void License_Should_BeAComputedLegacyView_OverRights()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetRights(Rights.CcBy);

        manifest.License!.Value.Should().Be(Rights.CcBy.Value);
    }

    [Fact]
    public void LegacyLicenseJson_Should_DeserializeDirectlyIntoRights()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "license": "http://creativecommons.org/licenses/by/4.0/"
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.Rights!.Value.Should().Be("http://creativecommons.org/licenses/by/4.0/");
        manifest.License!.Value.Should().Be("http://creativecommons.org/licenses/by/4.0/");
    }

    [Fact]
    public void Attribution_Should_BeAComputedLegacyView_OverRequiredStatement()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Provider"), new Description("Courtesy of Example Library")));

        manifest.Attribution.Should().ContainSingle(x => x.Value == "Courtesy of Example Library");
    }

    [Fact]
    public void LegacyAttributionJson_Should_DeserializeIntoRequiredStatement()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "attribution": "Courtesy of Example Library"
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.RequiredStatement.Should().NotBeNull();
        manifest.RequiredStatement!.Value.Should().ContainSingle(x => x.Value == "Courtesy of Example Library");
        manifest.Attribution.Should().ContainSingle(x => x.Value == "Courtesy of Example Library");
    }

    [Fact]
    public void Within_Should_BeAComputedLegacyView_OverPartOf()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddPartOf(new PartOf("https://example.org/collection", "Collection"));

        manifest.Within.Should().ContainSingle(x => x.Id == "https://example.org/collection");
    }

    [Fact]
    public void LegacyWithinJson_Should_DeserializeIntoPartOf()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "within": { "@id": "https://example.org/collection" }
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.PartOf.Should().ContainSingle(x => x.Id == "https://example.org/collection");
        manifest.Within.Should().ContainSingle(x => x.Id == "https://example.org/collection");
    }

    [Fact]
    public void Related_Should_BeAComputedLegacyView_OverHomepage()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.AddHomepage(new Homepage("https://example.org/object-page"));

        manifest.Related.Should().Be("https://example.org/object-page");
    }

    [Fact]
    public void LegacyRelatedJson_Should_DeserializeIntoHomepage()
    {
        const string json = """
                            {
                              "@id": "https://example.org/manifest",
                              "@type": "sc:Manifest",
                              "label": "Test",
                              "related": "https://example.org/object-page"
                            }
                            """;

        var manifest = JsonConvert.DeserializeObject<Manifest>(json)!;

        manifest.Homepage.Should().ContainSingle(x => x.Id == "https://example.org/object-page");
        manifest.Related.Should().Be("https://example.org/object-page");
    }

    [Fact]
    public void IiifSerializer_Should_WriteRightsRequiredStatementPartOf_AsV3()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetRights(Rights.CcBy);
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Provider"), new Description("Courtesy of Example Library")));
        manifest.AddPartOf(new PartOf("https://example.org/collection", "Collection"));

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["rights"]!.ToString().Should().Be(Rights.CcBy.Value);
        obj["requiredStatement"]!["value"]!["none"]![0]!.ToString().Should().Be("Courtesy of Example Library");
        obj["partOf"]![0]!["id"]!.ToString().Should().Be("https://example.org/collection");
    }

    [Fact]
    public void IiifSerializer_Should_ReadRightsRequiredStatementPartOf_FromV3()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Test"] },
                              "rights": "http://creativecommons.org/licenses/by/4.0/",
                              "requiredStatement": {
                                "label": { "none": ["Provider"] },
                                "value": { "none": ["Courtesy of Example Library"] }
                              },
                              "partOf": [ { "id": "https://example.org/collection", "type": "Collection" } ]
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);

        manifest.Rights!.Value.Should().Be("http://creativecommons.org/licenses/by/4.0/");
        manifest.RequiredStatement!.Value.Should().ContainSingle(x => x.Value == "Courtesy of Example Library");
        manifest.PartOf.Should().ContainSingle(x => x.Id == "https://example.org/collection");
    }

    [Fact]
    public void ThreeDotOhConstructedManifest_Should_SerializeAsStructurallyValidLegacyV2Json()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetRights(Rights.CcBy);
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Provider"), new Description("Courtesy of Example Library")));
        manifest.AddPartOf(new PartOf("https://example.org/collection", "Collection"));

        var json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);

        obj["license"]!.ToString().Should().Be(Rights.CcBy.Value);
        obj["attribution"]!.ToString().Should().Be("Courtesy of Example Library");
        obj["within"]!["@id"]!.ToString().Should().Be("https://example.org/collection");
        obj["rights"].Should().BeNull("legacy 2.1 output must not contain 3.0-only keys");
        obj["requiredStatement"].Should().BeNull();
        obj["partOf"].Should().BeNull();
    }

    [Theory]
    [InlineData(nameof(BaseNode<Manifest>.SetLicense))]
    [InlineData(nameof(BaseNode<Manifest>.AddAttribution))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveAttribution))]
    [InlineData(nameof(BaseNode<Manifest>.AddWithin))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveWithin))]
    [InlineData(nameof(BaseNode<Manifest>.SetRelated))]
    [InlineData(nameof(BaseNode<Manifest>.AddDescription))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveDescription))]
    public void LegacyMutators_Should_BeMarkedObsoleteAsWarnings(string methodName)
    {
        var method = typeof(BaseNode<Manifest>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var obsolete = method!.GetCustomAttribute<ObsoleteAttribute>();
        obsolete.Should().NotBeNull();
        obsolete!.IsError.Should().BeFalse("legacy mutators remain callable - deprecated with a warning, not a compile-time error");
    }

    [Theory]
    [InlineData(nameof(BaseNode<Manifest>.SetLicense))]
    [InlineData(nameof(BaseNode<Manifest>.AddAttribution))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveAttribution))]
    [InlineData(nameof(BaseNode<Manifest>.AddWithin))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveWithin))]
    [InlineData(nameof(BaseNode<Manifest>.SetRelated))]
    [InlineData(nameof(BaseNode<Manifest>.AddDescription))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveDescription))]
    [InlineData(nameof(BaseNode<Manifest>.SetViewingHint))]
    public void LegacyMutators_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation(string methodName)
    {
        var method = typeof(BaseNode<Manifest>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var version = method!.GetCustomAttribute<IIIFVersionAttribute>();
        version.Should().NotBeNull("legacy mutators must document their deprecation via an IIIFVersionAttribute-derived attribute");
        version!.IsDeprecated.Should().BeTrue();
        version.DeprecatedInVersion.Should().Be("3.0");
        version.ReplacedBy.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(nameof(BaseNode<Manifest>.SetRights))]
    [InlineData(nameof(BaseNode<Manifest>.SetRequiredStatement))]
    [InlineData(nameof(BaseNode<Manifest>.AddPartOf))]
    [InlineData(nameof(BaseNode<Manifest>.RemovePartOf))]
    [InlineData(nameof(BaseNode<Manifest>.AddHomepage))]
    [InlineData(nameof(BaseNode<Manifest>.SetSummary))]
    [InlineData(nameof(BaseNode<Manifest>.AddSummary))]
    [InlineData(nameof(BaseNode<Manifest>.RemoveSummary))]
    public void ReplacementMutators_Should_NotBeObsolete(string methodName)
    {
        var method = typeof(BaseNode<Manifest>).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        method!.GetCustomAttribute<ObsoleteAttribute>().Should().BeNull();
    }

    [Theory]
    [InlineData(nameof(BaseNode<Manifest>.License))]
    [InlineData(nameof(BaseNode<Manifest>.Attribution))]
    [InlineData(nameof(BaseNode<Manifest>.Within))]
    [InlineData(nameof(BaseNode<Manifest>.Related))]
    [InlineData(nameof(BaseNode<Manifest>.Description))]
    public void LegacyGetters_Should_NotBeObsolete(string propertyName)
    {
        var property = typeof(BaseNode<Manifest>).GetProperty(propertyName);

        property.Should().NotBeNull();
        property!.GetCustomAttribute<ObsoleteAttribute>().Should().BeNull();
    }
}