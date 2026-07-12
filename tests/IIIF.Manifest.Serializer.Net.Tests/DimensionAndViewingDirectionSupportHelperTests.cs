using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     <see cref="IDimenssionSupportHelper" /> and <see cref="IViewingDirectionSupportHelper" /> are the
///     shared JSON-read helpers behind the hand-rolled legacy 2.x converters (they read a "height"/
///     "width"/"viewingDirection" token off a raw <see cref="JToken" /> and apply it via the target's own
///     fluent setter) - neither had any direct test coverage, and writing these tests surfaced a real,
///     currently-dead bug (see the two tests below).
///     <para>
///         <strong>Known bug, not fixed here</strong> (the test-generation task this file was written under
///         scopes to tests only, never production code): <see cref="IDimenssionSupportHelper.SetHeight{T}" />
///         and <c>SetWidth</c> call <c>item.SetHeight(jHeight.Value&lt;int&gt;())</c> /
///         <c>item.SetWidth(...)</c> - but neither <c>BaseItem&lt;T&gt;</c> nor
///         <see cref="IDimensionSupport{TItem}" /> declares a <c>SetHeight(int)</c>/<c>SetWidth(int)</c>
///         overload, so that call actually re-resolves back to this SAME <c>SetHeight(JToken)</c>/
///         <c>SetWidth(JToken)</c> extension method, via <c>JToken</c>'s implicit <c>int</c> conversion
///         operator wrapping the value in a bare <c>JValue</c>. The recursive call's own
///         <c>element.TryGetToken("height")</c> then returns null (a <c>JValue</c> is not a <c>JObject</c>,
///         see <see cref="IIIF.Manifests.Serializer.Helpers.JsonHelper.TryGetToken" />), so the method is a
///         silent no-op - it never actually sets anything. Currently harmless: a repo-wide search confirms
///         neither method is called anywhere else in the codebase (dead code), but it would silently fail
///         the moment anyone wires it up. Flagged for a future fix (e.g. drop through to
///         <c>SetElementValue</c> directly, matching every other legacy read helper's pattern) rather than
///         left unfixed - fixing it here would violate this task's "test files only" scope.
///     </para>
/// </summary>
public class DimensionAndViewingDirectionSupportHelperTests
{
    [Fact]
    public void SetHeight_Should_ApplyTheHeightTokenWhenPresent_ButCurrentlyDoesNot_KnownBug()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1, 1);
        var element = JObject.Parse("""{"height": 2000}""");

        canvas.SetHeight(element);

        // Documents the current (buggy) no-op behavior - see the class-level remarks. The intended
        // behavior is canvas.Height == 2000.
        canvas.Height.Should().Be(1);
    }

    [Fact]
    public void SetWidth_Should_ApplyTheWidthTokenWhenPresent_ButCurrentlyDoesNot_KnownBug()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1, 1);
        var element = JObject.Parse("""{"width": 1500}""");

        canvas.SetWidth(element);

        // Documents the current (buggy) no-op behavior - see the class-level remarks. The intended
        // behavior is canvas.Width == 1500.
        canvas.Width.Should().Be(1);
    }

    [Fact]
    public void SetViewingDirection_Should_ApplyTheViewingDirectionTokenWhenPresent()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        var element = JObject.Parse("""{"viewingDirection": "right-to-left"}""");

        manifest.SetViewingDirection(element);

        manifest.ViewingDirection!.Value.Should().Be("right-to-left");
    }

    [Fact]
    public void SetViewingDirection_Should_LeaveViewingDirectionNull_WhenTokenMissing()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test"));
        var element = JObject.Parse("{}");

        manifest.SetViewingDirection(element);

        manifest.ViewingDirection.Should().BeNull();
    }
}