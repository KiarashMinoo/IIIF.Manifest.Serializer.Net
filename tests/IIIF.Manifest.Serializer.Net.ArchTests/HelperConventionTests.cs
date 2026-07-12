using System.Linq;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using IIIF.Manifests.Serializer.Nodes;
using NetArchTest.Rules;
using Xunit;

namespace IIIF.Manifests.Serializer.ArchTests;

/// <summary>
/// Enforces the convention already followed by every hand-written type in
/// <c>IIIF.Manifests.Serializer.Helpers</c> (<see cref="IIIF.Manifests.Serializer.Helpers.CollectionHelper"/>,
/// <see cref="IIIF.Manifests.Serializer.Helpers.DatetimeHelper"/>,
/// <see cref="IIIF.Manifests.Serializer.Helpers.JsonHelper"/>, etc.): a "Helpers" type is a stateless
/// bag of static methods, never an instantiable class - so any future addition to that namespace is
/// caught immediately if it accidentally isn't declared <c>static</c>.
///
/// Compiler-synthesized nested types are excluded by name (any type whose simple name contains
/// <c>&lt;</c> - never valid in hand-written C#): <c>AdditionalPropertiesHelper</c>/
/// <c>CollectionHelper</c> use C#'s "extension" member blocks
/// (<c>extension&lt;T&gt;(T target) { ... }</c>), which the compiler backs with a synthesized
/// generic marker type (e.g. <c>AdditionalPropertiesHelper+&lt;G&gt;$&lt;hash&gt;`1</c>) that is
/// never <c>static</c> even though the hand-written extension block and its containing class both
/// are - <see cref="CompilerGeneratedAttribute"/> is not applied to this particular synthesized
/// shape, so filtering on that attribute (tried first) does not exclude it; filtering by name does.
/// </summary>
public class HelperConventionTests
{
    private static readonly System.Reflection.Assembly CoreAssembly = typeof(Manifest).Assembly;

    [Fact]
    public void TypesInHelpersNamespace_Should_BeStatic()
    {
        var result = Types.InAssembly(CoreAssembly)
            .That().ResideInNamespace("IIIF.Manifests.Serializer.Helpers")
            .Should().BeStatic()
            .GetResult();

        var handWrittenViolations = (result.FailingTypes ?? Enumerable.Empty<System.Type>())
            .Where(t => !t.Name.Contains('<'))
            .Select(t => t.FullName)
            .ToArray();

        handWrittenViolations.Should().BeEmpty(
            "every hand-written type in the Helpers namespace must be a static class");
    }
}
