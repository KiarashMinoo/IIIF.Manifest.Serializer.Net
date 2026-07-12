using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// <see cref="ManifestHelper"/>'s <c>SetMetadata</c>/<c>GetMetadata</c> convenience extension
/// methods (find-or-create a <c>Metadata</c> entry by label, optionally per-language) had
/// zero test coverage.
/// </summary>
public class ManifestHelperTests
{
    [Fact]
    public void SetMetadata_Should_AddANewEntry_WhenLabelDoesNotExistYet()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));

        manifest.SetMetadata("Creator", "Jane Doe");

        manifest.GetMetadata("Creator")!.Should().ContainSingle(x => x.Value == "Jane Doe");
    }

    [Fact]
    public void SetMetadata_Should_ResetTheValue_WhenLabelAlreadyExistsAndNoLanguageGiven()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetMetadata("Creator", "Jane Doe");

        manifest.SetMetadata("Creator", "John Smith");

        manifest.GetMetadata("Creator")!.Should().ContainSingle(x => x.Value == "John Smith");
    }

    [Fact]
    public void SetMetadata_Should_AddANewLanguageValue_WhenLabelExistsButLanguageDoesNot()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetMetadata("Creator", "Jane Doe", "en");

        manifest.SetMetadata("Creator", "Jeanne Doe", "fr");

        manifest.GetMetadata("Creator")!.Should().HaveCount(2)
            .And.Contain(x => x.Value == "Jane Doe" && x.Language == "en")
            .And.Contain(x => x.Value == "Jeanne Doe" && x.Language == "fr");
    }

    [Fact]
    public void SetMetadata_Should_UpdateTheExistingLanguageValue_WhenBothLabelAndLanguageAlreadyExist()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        manifest.SetMetadata("Creator", "Jane Doe", "en");

        manifest.SetMetadata("Creator", "Jane R. Doe", "en");

        manifest.GetMetadata("Creator")!.Should().ContainSingle(x => x.Value == "Jane R. Doe" && x.Language == "en");
    }

    [Fact]
    public void GetMetadata_Should_ReturnNull_WhenLabelDoesNotExist()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));

        manifest.GetMetadata("Missing").Should().BeNull();
    }
}
