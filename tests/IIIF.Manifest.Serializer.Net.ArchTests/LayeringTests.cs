using System.Linq;
using AwesomeAssertions;
using IIIF.Manifests.Serializer.Nodes;
using NetArchTest.Rules;
using Xunit;

namespace IIIF.Manifests.Serializer.ArchTests;

/// <summary>
/// Enforces the core SDK's layering: <c>Nodes</c> (Manifest/Canvas/Collection/Range/Annotation/...)
/// is the top layer and may depend on <c>Shared</c>, <c>Properties</c>, and <c>Attributes</c>, but
/// none of those lower layers may depend back on <c>Nodes</c>. <c>Shared</c> and <c>Properties</c>
/// legitimately depend on each other (Shared's generic bases like <c>ValuableItem&lt;T&gt;</c> are
/// inherited by Properties types, while Shared's <c>BaseNode&lt;T&gt;</c> composes concrete
/// Properties types like <c>Label</c>/<c>Metadata</c> as property values) - that bidirectional
/// relationship is intentional and NOT tested here as a violation.
///
/// Two specific, documented exceptions to the "no dependency on Nodes" rule are explicitly
/// allow-listed below rather than silently excluded from the query, so a *new* accidental
/// Shared/Properties -&gt; Nodes reference still fails the test:
/// - <c>Shared.Content.Resources.BaseResourceJsonConverter</c> is the polymorphic annotation-body
///   dispatcher; it must know every concrete resource type (Image/Audio/Video/Textual/Embedded/
///   Choice), which live under <c>Nodes.Contents.*</c> - there's no way to dispatch without it.
/// - <c>Properties.Services.Search.SearchResponse</c> reuses <c>Nodes.Contents.Annotation.Annotation</c>
///   directly for its search-result items, since search-hit annotations have the identical
///   {id,type,motivation,body,target} shape as painting annotations (see SDK_VERSIONING_GUIDE.md,
///   Milestone 13) - deliberately not a parallel type.
/// </summary>
public class LayeringTests
{
    private const string NodesNamespace = "IIIF.Manifests.Serializer.Nodes";
    private const string SharedNamespace = "IIIF.Manifests.Serializer.Shared";
    private const string PropertiesNamespace = "IIIF.Manifests.Serializer.Properties";
    private const string AttributesNamespace = "IIIF.Manifests.Serializer.Attributes";

    private static readonly System.Reflection.Assembly CoreAssembly = typeof(Manifest).Assembly;

    [Fact]
    public void Shared_Should_NotDependOnNodes_ExceptTheDocumentedPolymorphicDispatcher()
    {
        var result = Types.InAssembly(CoreAssembly)
            .That().ResideInNamespace(SharedNamespace)
            .Should().NotHaveDependencyOn(NodesNamespace)
            .GetResult();

        var unexpectedViolations = FailingTypeNames(result)
            .Except(["IIIF.Manifests.Serializer.Shared.Content.Resources.BaseResourceJsonConverter"]);

        unexpectedViolations.Should().BeEmpty(
            "only BaseResourceJsonConverter is allowed to depend on Nodes, for polymorphic resource-type dispatch");
    }

    [Fact]
    public void Properties_Should_NotDependOnNodes_ExceptTheDocumentedSearchResponseReuse()
    {
        var result = Types.InAssembly(CoreAssembly)
            .That().ResideInNamespace(PropertiesNamespace)
            .Should().NotHaveDependencyOn(NodesNamespace)
            .GetResult();

        var unexpectedViolations = FailingTypeNames(result)
            .Except(["IIIF.Manifests.Serializer.Properties.Services.Search.SearchResponse"]);

        unexpectedViolations.Should().BeEmpty(
            "only SearchResponse is allowed to depend on Nodes, reusing Annotation directly for search results");
    }

    [Fact]
    public void Attributes_Should_NotDependOnDomainNamespaces()
    {
        var result = Types.InAssembly(CoreAssembly)
            .That().ResideInNamespace(AttributesNamespace)
            .Should().NotHaveDependencyOnAny(NodesNamespace, SharedNamespace, PropertiesNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BecauseOfFailures(result));
    }

    private static string BecauseOfFailures(TestResult result) =>
        result.FailingTypes is null
            ? "no failing types reported"
            : $"the following types violate the rule: {string.Join(", ", result.FailingTypes.Select(t => t.FullName))}";

    private static string[] FailingTypeNames(TestResult result) =>
        result.FailingTypes?.Select(t => t.FullName ?? t.Name).ToArray() ?? [];
}
