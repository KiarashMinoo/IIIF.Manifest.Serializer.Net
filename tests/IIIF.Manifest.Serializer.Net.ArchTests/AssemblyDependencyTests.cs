using System.Linq;
using System.Reflection;
using AwesomeAssertions;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using Xunit;

namespace IIIF.Manifests.Serializer.ArchTests;

/// <summary>
/// Guards the solution's known-good assembly reference graph: the core SDK never depends on any
/// extension, and the 3 extension packages only depend on each other in the one direction they
/// actually need (Georeference reuses navPlace's <c>NavPlace</c>/<c>Feature</c> FeatureCollection
/// shape for its Annotation body, so Georeference -&gt; NavPlace is expected and allowed; nothing
/// else cross-references). A `dotnet build` would already refuse a genuine circular
/// <c>ProjectReference</c>, so this is a regression guard against a *new*, one-directional but
/// architecturally-wrong reference being added later (e.g. TextGranularity accidentally depending
/// on NavPlace), not a check that could ever catch a true build-breaking cycle.
/// </summary>
public class AssemblyDependencyTests
{
    private static readonly Assembly CoreAssembly = typeof(Manifest).Assembly;
    private static readonly Assembly NavPlaceAssembly = typeof(NavPlace).Assembly;
    private static readonly Assembly GeoreferenceAssembly = typeof(GeoreferenceAnnotation).Assembly;
    private static readonly Assembly TextGranularityAssembly = typeof(TextGranularity).Assembly;

    [Fact]
    public void CoreAssembly_Should_NotReferenceAnyExtension()
    {
        var referenced = ReferencedNames(CoreAssembly);

        referenced.Should().NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.NavPlace"))
            .And.NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.Georeference"))
            .And.NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.TextGranularity"));
    }

    [Fact]
    public void NavPlaceAssembly_Should_OnlyReferenceCore()
    {
        var referenced = ReferencedNames(NavPlaceAssembly);

        referenced.Should().NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.Georeference"))
            .And.NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.TextGranularity"));
    }

    [Fact]
    public void TextGranularityAssembly_Should_OnlyReferenceCore()
    {
        var referenced = ReferencedNames(TextGranularityAssembly);

        referenced.Should().NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.NavPlace"))
            .And.NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.Georeference"));
    }

    [Fact]
    public void GeoreferenceAssembly_Should_NotReferenceTextGranularity()
    {
        var referenced = ReferencedNames(GeoreferenceAssembly);

        referenced.Should().NotContain(n => n.StartsWith("IIIF.Manifest.Serializer.Net.TextGranularity"));
    }

    private static string[] ReferencedNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty).ToArray();
}
