# Change Tracking

The SDK provides pull-based, recursive change tracking through `IIiifChangeTrackable`:

```csharp
public interface IIiifChangeTrackable
{
    bool HasChanges { get; }
    IReadOnlyCollection<IiifChangeEntry> GetChanges();
    void ClearChanges();
    void AcceptChanges();
}
```

Every model derived from `TrackableObject<T>` and every `TrackableCollection<T>` receives this behavior from the non-generic `TrackableObject` base.

## Descriptor-only architecture

Change state is stored where the value lives:

- object properties use `TrackableObject.ElementDescriptors`;
- collection items use `TrackableCollection<T>` item descriptors.

There are no parallel key snapshots or collection-baseline dictionaries. Recursive graph operations inspect descriptors directly.

| `ModificationType` | Change entry | Original | Current |
|---|---|---|---|
| `Added` | `Added` or `CollectionItemAdded` | `null` | current value |
| `Changed` | `Modified` | accepted value | current value |
| `Removed` | `Removed` or `CollectionItemRemoved` | accepted value | `null` |
| `Unchanged` | no direct entry | accepted value | accepted value |
| `Unknown` | no direct entry | — | — |

Assigning the accepted value again returns a property descriptor to `Unchanged`. Adding and then removing a new collection item cancels the pending change.

## Recursive tracking

The non-generic `TrackableObject` owns the shared recursive operations:

- `HasChangesCore`
- `GetChangesCore`
- `ClearChangesCore`
- nested-object and enumerable traversal
- reference-cycle detection
- path prefixing
- duplicate-safe reference matching

`TrackableCollection<T>` overrides the three core operations to interpret its indexed descriptors. This makes objects and collections follow the same recursion contract without a generic dispatch interface.

A shared reference is visited once per operation. Cycles do not recurse indefinitely.

## Collection semantics

`IBaseItem` collections such as `Items` and `Structures` report structural entries:

```text
Items[0]                         CollectionItemAdded
Items[1]                         CollectionItemRemoved
Items[0].Label                   Modified
Items[0].Items[0].Height         Modified
```

Reference matching is occurrence-aware. If the same object reference appears twice and one occurrence is removed, exactly one removal is reported.

Value-like collections such as `Label`, `Metadata`, `Homepage`, and `Rendering` remain property-level values when replaced. Mutations to trackable descendants still recurse normally.

## Paths

Paths use SDK-style property and bracket notation:

- `Rights`
- `Items[0]`
- `Items[0].Height`
- `Items[0].Items[0].Items[0].Bodies[0].Format`

Paths are relative to the object on which `GetChanges()` is invoked.

## Accepting changes

`ClearChanges()` and `AcceptChanges()` are equivalent. They recursively:

1. remove accepted property/item deletions;
2. make current values the new baseline;
3. reset descriptor states to `Unchanged`;
4. accept changes in every reachable child.

Objects produced by `TrackableObject.Parse<T>` start clean because deserialization establishes a baseline automatically.

## Example

```csharp
var manifest = new Manifest(id, new Label("Book"));
manifest.ClearChanges();

var canvas = new Canvas(canvasId, new Label("Page 1"), 1000, 800);
manifest.AddItem(canvas);

manifest.HasChanges; // true
var changes = manifest.GetChanges();
// Items[0] => CollectionItemAdded

manifest.AcceptChanges();
manifest.HasChanges; // false

canvas.SetHeight(2000);
// manifest.GetChanges(): Items[0].Height => Modified
```

## Changed manifests

`Manifest.GetChangedManifest()` creates a best-effort partial manifest containing changed top-level values and changed/added canvases. Removals cannot always be represented in a valid IIIF Manifest; use `GetChangeSet().Changes` for the complete record.