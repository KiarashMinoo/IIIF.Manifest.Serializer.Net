using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IIIF.Manifests.Serializer.ChangeTracking;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;
using IIIF.Manifests.Serializer.Shared.Trackable.Objects;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Exercises change tracking (<see cref="TrackableObject{TTrackableObject}" />'s
///     pull-based <c>HasChanges</c>/<c>GetChanges</c>) and event bubbling
///     (<see cref="TrackableCollection{T}" />'s item-level <c>PropertyChanged</c>
///     subscription) through the real, full-depth Manifest object graph rather than the shallow
///     one-level fixtures in <see cref="ChangeTrackingTests" />:
///     <c>Manifest -&gt; Items (Canvas) -&gt; Items (AnnotationPage) -&gt; Items (Annotation) -&gt;
///     Bodies (TextualBody) -&gt; Format</c> - six levels, crossing three nested
///     <see cref="TrackableCollection{T}" /> instances chained through
///     <c>IReadOnlyCollection&lt;IBaseItem&gt;</c> (<c>Items</c>) and one through
///     <c>IReadOnlyCollection&lt;IBaseResource&gt;</c> (<c>Bodies</c>) - the latter not recognized as
///     an <c>IBaseItem</c>-typed "trackable collection", which is what originally made this path miss
///     changes in <c>GetChanges()</c> even though <c>HasChanges</c> caught them (see
///     <c>TrackableObject.ChangeTracking.cs</c>'s <c>PropagateEnumerableChanges</c>).
/// </summary>
public class DeepHierarchyChangeTrackingTests
{
    private sealed record Hierarchy(
        Manifest Manifest,
        Canvas Canvas,
        AnnotationPage Page,
        Annotation Annotation,
        TextualBody Body
    );

    private static Hierarchy BuildSixLevelHierarchy()
    {
        var body = new TextualBody("hello");
        var annotation = new Annotation("https://example.org/anno/1", body, new AnnotationTarget("https://example.org/canvas/1"));
        var page = new AnnotationPage("https://example.org/page/1");
        page.AddItem(annotation);
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Canvas"), 1000, 800);
        canvas.AddItem(page);
        var manifest = new Manifest("https://example.org/manifest/1", new Label("Manifest"));
        manifest.AddItem(canvas);

        return new Hierarchy(manifest, canvas, page, annotation, body);
    }

    [Fact]
    public void DeepHierarchy_Should_HaveNoChanges_ImmediatelyAfterClearChanges()
    {
        var hierarchy = BuildSixLevelHierarchy();

        hierarchy.Manifest.ClearChanges();

        hierarchy.Manifest.HasChanges.Should().BeFalse();
        hierarchy.Manifest.GetChanges().Should().BeEmpty();
    }

    [Fact]
    public void DeepestBodyPropertyChange_Should_BeDetected_SixLevelsDown()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        hierarchy.Body.SetFormat("text/plain");

        hierarchy.Manifest.HasChanges.Should().BeTrue();
        hierarchy.Manifest.GetChanges().Should().ContainSingle(x =>
            x.Path == "Items[0].Items[0].Items[0].Bodies[0].Format" &&
            x.Kind == IiifChangeKind.Added &&
            (string?)x.CurrentValue == "text/plain");
    }

    [Fact]
    public void CanvasPropertyChange_Should_BeDetected_AtTopLevelPath()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        hierarchy.Canvas.SetHeight(2000);

        hierarchy.Manifest.HasChanges.Should().BeTrue();
        hierarchy.Manifest.GetChanges().Should().ContainSingle(x =>
            x.Path == "Items[0].Height" &&
            x.Kind == IiifChangeKind.Modified);
    }

    [Fact]
    public void AnnotationPagePropertyChange_Should_BeDetected_AtSecondLevelPath()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        hierarchy.Page.SetNext("https://example.org/page/2");

        hierarchy.Manifest.HasChanges.Should().BeTrue();
        hierarchy.Manifest.GetChanges().Should().ContainSingle(x =>
            x.Path == "Items[0].Items[0].Next" &&
            x.Kind == IiifChangeKind.Added);
    }

    [Fact]
    public void AnnotationPropertyChange_Should_BeDetected_AtThirdLevelPath()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        hierarchy.Annotation.SetMotivation("commenting");

        hierarchy.Manifest.HasChanges.Should().BeTrue();
        hierarchy.Manifest.GetChanges().Should().ContainSingle(x =>
            x.Path == "Items[0].Items[0].Items[0].Motivation" &&
            x.Kind == IiifChangeKind.Modified);
    }

    [Fact]
    public void SimultaneousChanges_AtEveryDepth_Should_AllBeReported_Independently()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        hierarchy.Canvas.SetHeight(2000);
        hierarchy.Page.SetNext("https://example.org/page/2");
        hierarchy.Annotation.SetMotivation("commenting");
        hierarchy.Body.SetFormat("text/plain");

        var changes = hierarchy.Manifest.GetChanges();

        changes.Should().HaveCount(4);
        changes.Select(x => x.Path).Should().BeEquivalentTo(
        [
            "Items[0].Height",
            "Items[0].Items[0].Next",
            "Items[0].Items[0].Items[0].Motivation",
            "Items[0].Items[0].Items[0].Bodies[0].Format"
        ]);
    }

    [Fact]
    public void AddingItems_Should_BeDetected_AtEveryCollectionDepth()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        var secondAnnotation = new Annotation("https://example.org/anno/2", new TextualBody("world"), new AnnotationTarget("https://example.org/canvas/1"));
        hierarchy.Page.AddItem(secondAnnotation);

        var secondPage = new AnnotationPage("https://example.org/page/2");
        hierarchy.Canvas.AddItem(secondPage);

        var secondCanvas = new Canvas("https://example.org/canvas/2", new Label("Canvas 2"), 500, 500);
        hierarchy.Manifest.AddItem(secondCanvas);

        var changes = hierarchy.Manifest.GetChanges();

        changes.Should().Contain(x => x.Path == "Items[0].Items[0].Items[1]" && x.Kind == IiifChangeKind.CollectionItemAdded);
        changes.Should().Contain(x => x.Path == "Items[0].Items[1]" && x.Kind == IiifChangeKind.CollectionItemAdded);
        changes.Should().Contain(x => x.Path == "Items[1]" && x.Kind == IiifChangeKind.CollectionItemAdded);
    }

    [Fact]
    public void RemovingAnItem_Should_BeDetected_OnceAcceptedIntoTheBaseline()
    {
        var hierarchy = BuildSixLevelHierarchy();
        var secondCanvas = new Canvas("https://example.org/canvas/2", new Label("Canvas 2"), 500, 500);
        hierarchy.Manifest.AddItem(secondCanvas);
        hierarchy.Manifest.AcceptChanges();

        hierarchy.Manifest.RemoveItem(secondCanvas);

        hierarchy.Manifest.GetChanges().Should().ContainSingle(x =>
            x.Path == "Items[1]" &&
            x.Kind == IiifChangeKind.CollectionItemRemoved);
    }

    [Fact]
    public void AcceptChanges_Should_ClearHasChanges_Recursively_AfterDeepMutation()
    {
        var hierarchy = BuildSixLevelHierarchy();
        hierarchy.Manifest.ClearChanges();

        hierarchy.Canvas.SetHeight(2000);
        hierarchy.Body.SetFormat("text/plain");
        hierarchy.Manifest.HasChanges.Should().BeTrue();

        hierarchy.Manifest.AcceptChanges();

        hierarchy.Manifest.HasChanges.Should().BeFalse();
        hierarchy.Manifest.GetChanges().Should().BeEmpty();

        hierarchy.Body.SetLanguage("en");

        hierarchy.Manifest.HasChanges.Should().BeTrue();
        hierarchy.Manifest.GetChanges().Should().ContainSingle(x => x.Path == "Items[0].Items[0].Items[0].Bodies[0].Language");
    }

    [Fact]
    public void PropertyChanged_Should_Bubble_FromDeepestAnnotation_ToManifest_ThroughThreeCollectionLevels()
    {
        var hierarchy = BuildSixLevelHierarchy();
        var manifestChangedProperties = new List<string?>();
        ((INotifyPropertyChanged)hierarchy.Manifest).PropertyChanged += (_, e) => manifestChangedProperties.Add(e.PropertyName);

        hierarchy.Annotation.SetMotivation("commenting");

        manifestChangedProperties.Should().Contain(nameof(Manifest.Items));
    }

    [Fact]
    public void PropertyChanged_Should_Bubble_FromCanvasTwoLevelsDeep_ToManifest()
    {
        var hierarchy = BuildSixLevelHierarchy();
        var manifestChangedProperties = new List<string?>();
        ((INotifyPropertyChanged)hierarchy.Manifest).PropertyChanged += (_, e) => manifestChangedProperties.Add(e.PropertyName);

        hierarchy.Canvas.SetHeight(3000);

        manifestChangedProperties.Should().Contain(nameof(Manifest.Items));
    }
}
