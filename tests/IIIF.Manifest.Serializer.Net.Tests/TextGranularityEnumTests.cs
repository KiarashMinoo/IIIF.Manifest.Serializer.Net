using IIIF.Manifests.Serializer.Extensions;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 15 (SDK_VERSIONING_GUIDE.md §10, finding 7): the Text Granularity extension defined
/// "character" (not a spec term) instead of "glyph", and was missing "paragraph" entirely - real
/// documents using either term threw in <see cref="TextGranularity.Parse"/>.
/// </summary>
public class TextGranularityEnumTests
{
    [Theory]
    [InlineData("page")]
    [InlineData("block")]
    [InlineData("paragraph")]
    [InlineData("line")]
    [InlineData("word")]
    [InlineData("glyph")]
    public void Parse_Should_AcceptAllSixSpecDefinedLevels(string value)
    {
        var granularity = TextGranularity.Parse(value);

        granularity.Value.Should().Be(value);
    }

    [Fact]
    public void Parse_Should_ThrowForTheRemovedNonSpecCharacterTerm()
    {
        var act = () => TextGranularity.Parse("character");

        act.Should().Throw<System.ArgumentException>();
    }

    [Fact]
    public void Paragraph_Should_BeAccessibleAsAStaticInstance()
    {
        TextGranularity.Paragraph.Value.Should().Be("paragraph");
    }

    [Fact]
    public void Glyph_Should_BeAccessibleAsAStaticInstance()
    {
        TextGranularity.Glyph.Value.Should().Be("glyph");
    }
}
