using IIIF.Manifests.Serializer;

namespace IIIF.Manifests.Serializer.Tests;

public class IiifSerializerOptionsTests
{
    [Fact]
    public void Default_Should_TargetLatestPresentationVersion()
    {
        IiifSerializerOptions.Default.Version.Should().Be(IiifPresentationVersion.V3_0);
    }

    [Fact]
    public void Constructor_Should_SetVersionAndLegacyPreservation()
    {
        var options = new IiifSerializerOptions(
            IiifPresentationVersion.V2_1,
            preserveLegacyProperties: true);

        options.Version.Should().Be(IiifPresentationVersion.V2_1);
        options.PreserveLegacyProperties.Should().BeTrue();
    }
}
