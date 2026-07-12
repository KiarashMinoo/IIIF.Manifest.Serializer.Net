using System;
using IIIF.Manifests.Serializer.Helpers;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// <see cref="DatetimeHelper.ParseISO8601String"/> is the lenient ISO-8601 parser used for legacy
/// 2.x navDate values (which appear in a wide range of accuracies, from a bare year down to a
/// full timestamp with offset) - it had zero direct test coverage.
/// </summary>
public class DatetimeHelperTests
{
    [Theory]
    [InlineData("2019-01-01T00:00:00Z")]
    [InlineData("2019-01-01T00:00:00+00:00")]
    [InlineData("20190101T000000Z")]
    [InlineData("2019-01-01")]
    [InlineData("2019-01")]
    [InlineData("2019")]
    public void ParseISO8601String_Should_AcceptEveryReducedAccuracyFormat(string value)
    {
        var act = () => DatetimeHelper.ParseISO8601String(value);

        act.Should().NotThrow();
    }

    [Fact]
    public void ParseISO8601String_Should_ParseAFullTimestampToTheCorrectInstant()
    {
        // DateTimeStyles.AssumeUniversal without AdjustToUniversal converts the assumed-UTC value
        // to the local Kind, so compare via ToUniversalTime() rather than assuming the result's
        // Kind/wall-clock value directly - otherwise this test would be timezone-dependent.
        var parsed = DatetimeHelper.ParseISO8601String("2019-06-15T12:30:00Z");

        parsed.ToUniversalTime().Should().Be(new DateTime(2019, 6, 15, 12, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ParseISO8601String_Should_ThrowForANonConformingString()
    {
        var act = () => DatetimeHelper.ParseISO8601String("not-a-date");

        act.Should().Throw<FormatException>();
    }
}
