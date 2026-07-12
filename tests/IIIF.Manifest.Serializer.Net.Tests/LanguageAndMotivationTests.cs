namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// <see cref="Language"/> and <see cref="Motivation"/>'s static named instances had zero test
/// coverage.
/// </summary>
public class LanguageAndMotivationTests
{
    [Fact]
    public void Language_NamedInstances_Should_ExposeBcp47Tags()
    {
        Language.English.Value.Should().Be("en");
        Language.EnglishUs.Value.Should().Be("en-US");
        Language.French.Value.Should().Be("fr");
        Language.Japanese.Value.Should().Be("ja");
        Language.None.Value.Should().Be("none");
    }

    [Fact]
    public void Language_Should_SupportCustomTags()
    {
        var custom = new Language("cy");

        custom.Value.Should().Be("cy");
    }

    [Fact]
    public void Motivation_NamedInstances_Should_ExposePresentationAndWebAnnotationMotivations()
    {
        Motivation.Painting.Value.Should().Be("painting");
        Motivation.Supplementing.Value.Should().Be("supplementing");
        Motivation.Commenting.Value.Should().Be("commenting");
        Motivation.Tagging.Value.Should().Be("tagging");
        Motivation.Linking.Value.Should().Be("linking");
    }

    [Fact]
    public void Motivation_ScPainting_Should_ExposeThe2_xPrefixedForm()
    {
        Motivation.ScPainting.Value.Should().Be("sc:painting");
    }

    [Fact]
    public void Motivation_Should_RoundTripThroughJsonConvert()
    {
        var json = JsonConvert.SerializeObject(Motivation.Tagging);

        var deserialized = JsonConvert.DeserializeObject<Motivation>(json);

        deserialized!.Value.Should().Be("tagging");
    }
}
