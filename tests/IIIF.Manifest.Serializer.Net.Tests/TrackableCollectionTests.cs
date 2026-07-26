using System.ComponentModel;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Direct unit tests for <see cref="TrackableCollection{T}" /> - the collection type every
///     enumerable-valued property is wrapped in by <c>TrackableObject{T}.SetElementValue</c>. Covers
///     append/remove/clear mechanics and the item-change-to-collection-event bubbling that a parent's
///     <c>SetElementValue</c> delegate wiring relies on to learn that a child already inside one of
///     its collections has changed.
/// </summary>
public class TrackableCollectionTests
{
    private sealed class NotifyingItem : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void ChangeValue()
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(ChangeValue)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChangeValue)));
        }
    }

    [Fact]
    public void Add_Should_AppendItemsInOrder_WhenCollectionStartsEmpty()
    {
        var collection = new TrackableCollection<string>();

        collection.Add("first");
        collection.Add("second");
        collection.Add("third");

        collection.Should().Equal("first", "second", "third");
    }

    [Fact]
    public void Add_Should_RaiseCollectionChanged_WithAddTypeAndAppendedIndex()
    {
        var collection = new TrackableCollection<string>();
        TrackableCollectionChangedEventArgs? raised = null;
        collection.CollectionChanged += (_, e) => raised = e;

        collection.Add("only item");

        raised.Should().NotBeNull();
        raised!.CollectionChangedType.Should().Be(CollectionChangedType.Add);
        raised.Index.Should().Be(0);
    }

    [Fact]
    public void Remove_Should_RaiseCollectionChanged_WithRemoveType()
    {
        var item = new NotifyingItem();
        var collection = new TrackableCollection<NotifyingItem>([item]);
        TrackableCollectionChangedEventArgs? raised = null;
        collection.CollectionChanged += (_, e) => raised = e;

        collection.Remove(item);

        raised.Should().NotBeNull();
        raised!.CollectionChangedType.Should().Be(CollectionChangedType.Remove);
    }

    [Fact]
    public void ItemPropertyChange_Should_RaiseCollectionChanged_WithModifyType_ForItemAlreadyInCollection()
    {
        var item = new NotifyingItem();
        var collection = new TrackableCollection<NotifyingItem>([item]);
        TrackableCollectionChangedEventArgs? raised = null;
        collection.CollectionChanged += (_, e) => raised = e;

        item.ChangeValue();

        raised.Should().NotBeNull();
        raised!.CollectionChangedType.Should().Be(CollectionChangedType.Modify);
    }

    [Fact]
    public void ItemPropertyChange_Should_NotRaiseCollectionChanged_AfterItemIsRemoved()
    {
        var item = new NotifyingItem();
        var collection = new TrackableCollection<NotifyingItem>([item]);
        collection.Remove(item);
        var raiseCount = 0;
        collection.CollectionChanged += (_, _) => raiseCount++;

        item.ChangeValue();

        raiseCount.Should().Be(0);
    }

    [Fact]
    public void Clear_Should_EmptyCollection_WhenMultipleItemsArePresent()
    {
        var collection = new TrackableCollection<string>(["a", "b", "c"]);

        collection.Clear();

        collection.Should().BeEmpty();
    }
}
