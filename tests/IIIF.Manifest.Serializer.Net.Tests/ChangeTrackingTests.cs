using System.Linq;
using IIIF.Manifests.Serializer.ChangeTracking;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Issue #23 ("SDK Change Tracking"): an EF Core-style change tracker over the whole object
///     graph, generic on <see cref="Shared.Trackable.TrackableObject{TTrackableObject}" /> - see
///     <c>docs/CHANGE_TRACKING.md</c> for the design (pull-based diffing over the pre-existing
///     Original/Modified value pair every property already tracks, rather than an explicit
///     parent-attachment/event-bubbling design).
/// </summary>
public class ChangeTrackingTests
{
    [Fact]
    public void PropertyChange_Should_BeReportedAsModified()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Old title"));
        manifest.ClearChanges();

        manifest.SetLabel([new Label("New title")]);

        manifest.HasChanges.Should().BeTrue();
        manifest.GetChanges().Should().Contain(x => x.Kind == IiifChangeKind.Modified && x.PropertyName == nameof(Manifest.Label));
    }

    [Fact]
    public void SettingAnEquivalentValue_Should_NotCreateAChangeEntry_When_TheValueIsTheSame()
    {
        // A scalar property (not a collection - collection-valued properties are always re-wrapped
        // in a fresh BindingList on every assignment, even when passed the same reference back, so
        // "equivalent value, no change" isn't observable at the collection-property level).
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800);
        canvas.ClearChanges();

        canvas.SetHeight(1000); // same value as construction

        canvas.HasChanges.Should().BeFalse();
        canvas.GetChanges().Should().BeEmpty();
    }

    [Fact]
    public void GetChanges_Should_CaptureOriginalAndCurrentValues()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 1000, 800);
        canvas.ClearChanges();

        canvas.SetHeight(2000);

        var entry = canvas.GetChanges().Single(x => x.PropertyName == nameof(Canvas.Height));
        entry.OriginalValue.Should().Be(1000);
        entry.CurrentValue.Should().Be(2000);
    }

    [Fact]
    public void PropertyChanging_And_PropertyChanged_Should_BothFire()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Old"));
        var changingFired = false;
        var changedFired = false;
        manifest.PropertyChanging += (_, e) =>
        {
            if (e.PropertyName == nameof(Manifest.Label)) changingFired = true;
        };
        manifest.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Manifest.Label)) changedFired = true;
        };

        manifest.SetLabel([new Label("New")]);

        changingFired.Should().BeTrue();
        changedFired.Should().BeTrue();
    }

    [Fact]
    public void CollectionAdd_Should_BeReportedAsCollectionItemAdded()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.ClearChanges();

        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100);
        manifest.AddItem(canvas);

        manifest.HasChanges.Should().BeTrue();
        manifest.GetChanges().Should().ContainSingle(x => x.Kind == IiifChangeKind.CollectionItemAdded && x.Path == "Items[0]" && x.CurrentValue == canvas);
    }

    [Fact]
    public void CollectionRemove_Should_BeReportedAsCollectionItemRemoved()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100);
        manifest.AddItem(canvas);
        manifest.ClearChanges();

        manifest.RemoveItem(canvas);

        manifest.HasChanges.Should().BeTrue();
        manifest.GetChanges().Should().ContainSingle(x => x.Kind == IiifChangeKind.CollectionItemRemoved && x.OriginalValue == canvas);
    }

    [Fact]
    public void ChildPropertyChange_Should_BubbleToParent_ThroughACollection()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
        manifest.AddItem(canvas);
        manifest.ClearChanges();

        canvas.SetLabel([new Label("Updated Page 1")]);

        manifest.HasChanges.Should().BeTrue();
        manifest.GetChanges().Should().Contain(x => x.Path == "Items[0].Label");
    }

    [Fact]
    public void ChildPropertyChange_Should_BubbleToParent_ThroughAHeightChange()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
        manifest.AddItem(canvas);
        manifest.ClearChanges();

        canvas.SetHeight(2000);

        manifest.HasChanges.Should().BeTrue();
        manifest.GetChanges().Should().Contain(x => x.Path == "Items[0].Height");
    }

    [Fact]
    public void ClearChanges_Should_ResetHasChanges_ToFalse()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Old"));
        manifest.SetLabel([new Label("Updated")]);
        manifest.HasChanges.Should().BeTrue();

        manifest.ClearChanges();

        manifest.HasChanges.Should().BeFalse();
        manifest.GetChanges().Should().BeEmpty();
    }

    [Fact]
    public void ClearChanges_Should_RecurseIntoChildrenInsideCollections()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
        manifest.AddItem(canvas);
        canvas.SetHeight(2000);
        manifest.HasChanges.Should().BeTrue();

        manifest.ClearChanges();

        manifest.HasChanges.Should().BeFalse();
        canvas.HasChanges.Should().BeFalse();

        // Future changes on the same child are still tracked normally after a clear.
        canvas.SetHeight(3000);
        manifest.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void AcceptChanges_Should_BehaveIdenticallyToClearChanges()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Old"));
        manifest.SetLabel([new Label("Updated")]);

        manifest.AcceptChanges();

        manifest.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void DeserializedManifest_Should_StartClean()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.AddItem(new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100));
        var json = IiifSerializer.Serialize(manifest);

        var deserialized = IiifSerializer.DeserializeManifest(json);

        deserialized.HasChanges.Should().BeFalse();
        deserialized.GetChanges().Should().BeEmpty();
    }

    [Fact]
    public void NoInfiniteRecursion_When_TheSameChildIsReferencedTwice()
    {
        // Reference the same Canvas object twice within items (an unusual but not impossible
        // construction) - the visited-set cycle guard must prevent double-walking/looping.
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100);
        manifest.AddItem(canvas);
        manifest.AddItem(canvas);
        manifest.ClearChanges();

        canvas.SetHeight(500);

        var act = () => manifest.GetChanges();
        act.Should().NotThrow();
        manifest.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void ChangedManifest_Should_ContainOnlyTheChangedTopLevelProperty()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.AddItem(new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100));
        manifest.ClearChanges();

        manifest.SetRights(Rights.CcBy);

        var changedOnly = manifest.GetChangedManifest();

        changedOnly.Id.Should().Be(manifest.Id);
        changedOnly.Rights.Should().Be(Rights.CcBy);
        changedOnly.Items.Should().BeEmpty("no canvas changed, only the rights property");
    }

    [Fact]
    public void ChangedManifest_Should_ContainOnlyTheChangedChildCanvas()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var unchangedCanvas = new Canvas("https://example.org/canvas/unchanged", new Label("Page 1"), 100, 100);
        var changedCanvas = new Canvas("https://example.org/canvas/changed", new Label("Page 2"), 100, 100);
        manifest.AddItem(unchangedCanvas);
        manifest.AddItem(changedCanvas);
        manifest.ClearChanges();

        changedCanvas.SetLabel([new Label("Updated Page 2")]);

        var changedOnly = manifest.GetChangedManifest();
        var json = IiifSerializer.Serialize(changedOnly);

        changedOnly.Items.Should().ContainSingle();
        json.Should().Contain("Updated Page 2");
        json.Should().NotContain("canvas/unchanged");
    }

    [Fact]
    public void ChangedManifest_Should_ContainANewlyAddedCanvas()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.AddItem(new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100));
        manifest.ClearChanges();

        var newCanvas = new Canvas("https://example.org/canvas/2", new Label("Page 2"), 100, 100);
        manifest.AddItem(newCanvas);

        var changedOnly = manifest.GetChangedManifest();

        changedOnly.Items.Should().ContainSingle().Which.Should().BeSameAs(newCanvas);
    }

    [Fact]
    public void ChangeSet_Should_ReportRemovals_ThatTheChangedManifestCannotRepresent()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100);
        manifest.AddItem(canvas);
        manifest.ClearChanges();

        manifest.RemoveItem(canvas);

        var changeSet = manifest.GetChangeSet();

        changeSet.RootId.Should().Be(manifest.Id);
        changeSet.RootType.Should().Be("Manifest");
        changeSet.Changes.Should().ContainSingle(x => x.Kind == IiifChangeKind.CollectionItemRemoved);
        // The removal is real information in Changes, but a valid IIIF Manifest cannot express
        // "this canvas used to be here" - ChangedManifest has no way to represent it.
        changeSet.ChangedManifest!.Items.Should().BeEmpty();
    }

    [Fact]
    public void SerializeChangedOnly_Should_ProduceTheSameJsonAsSerializingGetChangedManifest()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.ClearChanges();
        manifest.SetRights(Rights.CcBy);

        var direct = IiifSerializer.Serialize(manifest.GetChangedManifest());
        var viaHelper = IiifSerializer.SerializeChangedOnly(manifest);

        viaHelper.Should().Be(direct);
    }

    [Fact]
    public void FullSerialize_Should_RemainUnaffected_ByChangeTrackingState()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
        manifest.AddItem(new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100));
        manifest.ClearChanges();

        var beforeAnyChange = IiifSerializer.Serialize(manifest);
        manifest.SetRights(Rights.CcBy);
        var afterAChange = IiifSerializer.Serialize(manifest);

        // Full Serialize always reflects live state and is unaffected by tracking/clearing -
        // the only difference here is the rights value that was actually set, not any
        // tracking-related side effect.
        afterAChange.Should().Contain("rights");
        beforeAnyChange.Should().NotContain("rights");
    }

    [Fact]
    public void ExtensionObjectChange_Should_Bubble_ThroughNavPlace()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Map"));
        manifest.SetNavPlace(new Extensions.NavPlace("https://example.org/navplace"));
        manifest.ClearChanges();

        manifest.NavPlace!.AddFeature(new Extensions.Feature("https://example.org/feature/1"));

        manifest.HasChanges.Should().BeTrue();
    }
}
