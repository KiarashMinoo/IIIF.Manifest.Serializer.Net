namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// The small Image API value-type wrappers (<see cref="ImageFeature"/>, <see cref="ImageQuality"/>,
/// <see cref="Profile"/>) had zero test coverage - their static named instances were never
/// constructed by any existing test.
/// </summary>
public class ImageApiValueTypesTests
{
    [Fact]
    public void ImageFeature_NamedInstances_Should_ExposeSpecDefinedValues()
    {
        ImageFeature.RegionByPx.Value.Should().Be("regionByPx");
        ImageFeature.SizeUpscaling.Value.Should().Be("sizeUpscaling");
        ImageFeature.RotationArbitrary.Value.Should().Be("rotationArbitrary");
        ImageFeature.Mirroring.Value.Should().Be("mirroring");
        ImageFeature.Cors.Value.Should().Be("cors");
    }

    [Fact]
    public void ImageFeature_Should_SupportCustomValues()
    {
        var custom = new ImageFeature("myCustomFeature");

        custom.Value.Should().Be("myCustomFeature");
    }

    [Fact]
    public void ImageQuality_NamedInstances_Should_ExposeSpecDefinedValues()
    {
        ImageQuality.Default.Value.Should().Be("default");
        ImageQuality.Color.Value.Should().Be("color");
        ImageQuality.Gray.Value.Should().Be("gray");
        ImageQuality.Bitonal.Value.Should().Be("bitonal");
    }

    [Fact]
    public void ImageQuality_WithSameValue_Should_BeEqual()
    {
        ImageQuality.Default.Should().Be(new ImageQuality("default"));
    }

    [Fact]
    public void Profile_NamedInstances_Should_ExposeBoth3_0KeywordsAnd2_xUrls()
    {
        Profile.Level1.Value.Should().Be("level1");
        Profile.ImageApi2Level1.Value.Should().Be("http://iiif.io/api/image/2/level1.json");
        Profile.AuthLogin.Value.Should().Be("http://iiif.io/api/auth/1/login");
        Profile.AuthLogout.Value.Should().Be("http://iiif.io/api/auth/1/logout");
    }

    [Fact]
    public void ImageFeature_Should_RoundTripThroughJsonConvert()
    {
        var json = JsonConvert.SerializeObject(ImageFeature.SizeByWh);

        var deserialized = JsonConvert.DeserializeObject<ImageFeature>(json);

        deserialized!.Value.Should().Be("sizeByWh");
    }
}
