# Change Tracking

An EF Core-style change-tracking subsystem (issue #23) over the entire SDK object graph -
Manifest, Collection, Canvas, Range, Annotation, AnnotationPage, content resources, services, and
extension objects (navPlace/Georeference/TextGranularity) alike, since all of them derive from the
same `TrackableObject<T>` base. This document covers the lifecycle, how parent/child propagation
and collection changes work, changed-only output for delta/time-scale storage, and this feature's
deliberate limitations.

## Design: pull-based, not push/event-bubbling

The issue's own suggested shape included an `IIiifObjectGraphTrackable` interface with
`Parent`/`ParentPath`/`AttachParent`/`DetachParent` - an explicit parent-attachment, event-bubbling
design. This SDK implements something simpler instead: **`HasChanges`/`GetChanges()` walk the
object graph fresh on every call**, pulling from the same Original/Modified value pair every
property already maintains via `TrackableObject<T>`'s existing `ElementDescriptors` (used for other
purposes independent of this feature - see `Shared/Trackable/TrackableObject.cs`).

Why pull, not push:

- **No wiring can go stale or be missed.** A child mutated through a reference held outside its
  parent - `manifest.AddItem(canvas); manifest.ClearChanges(); canvas.SetHeight(2000);` - is still
  correctly detected by `manifest.HasChanges`, because the check re-walks the graph from `manifest`
  down to `canvas` every time, rather than relying on `canvas` having been told at `AddItem` time
  "notify this specific parent when you change." There's no call site that could forget to wire up
  attachment, and no risk of a child being silently orphaned from tracking if it's later moved
  between two different parent collections.
- **No event-subscription lifecycle to manage.** A push design needs to subscribe when a child is
  added and unsubscribe when removed, correctly, at every single mutation call site across the
  whole SDK (`AddItem`, `RemoveItem`, `AddAnnotation`, `SetPlaceholderCanvas`, ...) - a push design
  done incompletely is worse than no design at all, since it would silently miss changes in exactly
  the call sites that weren't updated. Pull-based tracking needs zero changes to any of those
  existing methods.
- **Trivial cycle safety.** A visited-set threaded through the recursive walk (see "Cycle
  protection" below) is enough; a push design needs the same protection *plus* correct
  subscribe/unsubscribe bookkeeping to avoid leaking event handlers.

## Lifecycle

1. **Tracking is always on** - there's no explicit "start tracking" step. Every `TrackableObject<T>`
   (every model type) implements `IIiifChangeTrackable` automatically.
2. **A freshly-constructed object reports `HasChanges == false`.** Properties set during
   construction (e.g. `Manifest`'s required `id`/`label`, `Canvas`'s required `height`/`width`) are
   each a property's *first-ever* assignment, which `ElementDescriptor`'s own Original/Modified pair
   treats as "this is the baseline," not a change - by design, so building a new object doesn't
   itself look like a pending edit.
3. **`ClearChanges()`** (alias: `AcceptChanges()` - this SDK treats them as identical, since there's
   no separate "pending delete" concept to reconcile) establishes the *current* state as the new
   baseline, recursively through every reachable descendant. After it returns, `HasChanges` is
   `false` across the whole subtree, and future mutations are tracked freshly from that point.
4. **Deserialization starts clean by default** - `IiifSerializer.DeserializeManifest`/
   `DeserializeCollection`/`DeserializeAnnotationCollection`, and the shared
   `TrackableObject.Parse<T>`/`TryParse<T>` used by other document types (`SearchResponse`,
   `ContentState`, `DiscoveryCollectionPage`, ...), all call `ClearChanges()` on the result before
   returning it. A document just loaded from storage has no "pending edits" yet.

## A gap the design itself doesn't fully cover on its own

`ElementDescriptor`'s Original/Modified pair being "first-ever assignment = baseline" is exactly
right for construction-time properties (point 2 above) - but it under-reports one real case: a
property that was **never touched before an explicit `ClearChanges()`**, then set for the first
time *after* that call. Two concrete examples this SDK's own tests caught by actually running them,
not by assuming the design was correct:

```csharp
var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
manifest.ClearChanges();               // Items was never set - nothing to "reset" yet

var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 100, 100);
manifest.AddItem(canvas);              // Items's *first-ever* assignment

manifest.HasChanges.Should().BeTrue(); // must be true - and naively isn't, without the fix below
```

```csharp
var manifest = new Manifest("https://example.org/manifest", new Label("Book"));
manifest.ClearChanges();               // Rights was never set

manifest.SetRights(Rights.CcBy);       // Rights's *first-ever* assignment

manifest.HasChanges.Should().BeTrue(); // same issue, for a scalar property this time
```

Fixed with two small pieces of bookkeeping, both populated in `ClearChangesCore`:

- **`_keysAtLastClear`** (a `HashSet<string>`, `null` until the first `ClearChanges()` call):
  snapshots *which* `ElementDescriptors` keys existed at clear time. Any key set for the first time
  *after* that point - even though `ElementDescriptor.IsModified` itself says "unmodified" - is
  still reported as a `Modified` change, with `OriginalValue = null` (there was genuinely nothing
  there before).
- **`_collectionBaselines`** (a `Dictionary<string, List<object>>`): a snapshot, by reference, of
  exactly which items an `IBaseItem`-typed collection (see below) contained at clear time - the
  actual diffing baseline for `CollectionItemAdded`/`CollectionItemRemoved`, used instead of
  `ElementDescriptor.OriginalValue` for these collections specifically, for the same reason.

## Collections: value collections vs. structural children

A single generic rule decides how a collection-valued property is represented in `GetChanges()`,
based on whether the collection's element type implements `IBaseItem` (the SDK's own existing
marker interface for "things that live in an `Items`-style collection" - `Manifest`/`Collection`/
`Canvas`/`Structure`/`Annotation`/content resources all implement it):

| Element type | Treatment | Example |
| --- | --- | --- |
| Implements `IBaseItem` | **Structural**: diffed by reference against the collection baseline; each newly-present item is `CollectionItemAdded`, each vanished item is `CollectionItemRemoved`, and items present in both baseline and current are recursed into for their *own* internal changes (see "Parent/child propagation" below). | `Manifest.Items`, `Manifest.Structures` |
| Anything else | **Value-like**: the whole property is reported as one `Modified` entry when its `ElementDescriptor` shows a change, with no per-item diffing. | `Label`, `Metadata`, `SeeAlso`, `Homepage`, `Rendering` |

This split exists because a value collection like `Label` is conceptually "the manifest's label,"
not "a list of graph children" - `manifest.SetLabel([new Label("New title")])` should report one
`Modified` entry for `Label`, not a `CollectionItemRemoved`+`CollectionItemAdded` pair for the old
and new `Label` instances (which, technically, *are* different objects by reference, since every
assignment through a collection-valued property setter re-wraps its value in a fresh internal
`BindingList<T>` - see `SetElementValue` in `TrackableObject.cs`). `IiifChangeKind.CollectionCleared`
exists in the enum (matching the issue's suggested shape) but this implementation never
synthesizes it - clearing every item from a structural collection already produces one
`CollectionItemRemoved` entry per item, which fully captures the removal; a wrapper entry would be
redundant, not additional information.

## Parent/child propagation

Because the walk is pull-based, "propagation" is really just recursion with path-prefixing, not a
distinct mechanism:

- For a **structural collection** item present in both the baseline and current snapshot (same
  reference, not added/removed), if that item itself `HasChanges`, its own `GetChanges()` entries
  are re-added to the parent's result list with the parent's path segment prepended - e.g. a
  `Canvas` at `Items[0]` whose `Height` changed produces `Items[0].Height` on the `Manifest`, not a
  separate synthetic "child changed" wrapper entry. The concrete leaf `IiifChangeKind` (`Modified`,
  `CollectionItemAdded`, ...) is preserved through the prefixing, since it's more useful to a caller
  than a generic `ChildChanged` marker.
- For a **single-reference** property (not a collection) whose `ElementDescriptor` shows no direct
  change but whose current value is itself a trackable object with pending changes (e.g.
  `RequiredStatement`, or an attached extension object like `NavPlace`), the same prefixing applies
  one level up.
- **Cycle protection**: every walk (`HasChanges`, `GetChanges()`, `ClearChanges()`) threads a
  reference-equality `HashSet<object>` of already-visited objects through the recursion. Revisiting
  an object (a genuine cycle, or the same object reachable twice, e.g. added to the same collection
  twice) contributes no further entries on the second visit - whatever it has was already accounted
  for. IIIF manifests are normally tree-shaped with no back-references, so this is a defensive
  backstop rather than something expected to trigger in practice, but it's tested directly (see
  `ChangeTrackingTests.NoInfiniteRecursion_When_TheSameChildIsReferencedTwice`).

## Changed-only output for delta/time-scale storage

Two related APIs, both currently implemented on `Manifest` only (the primary/only document type
every one of the issue's own examples exercises - Collection/AnnotationCollection could follow the
same pattern later if a real need arises):

- **`Manifest.GetChangedManifest()`** returns a **best-effort, valid partial `Manifest`** - always
  keeps `id`; includes `rights`/`requiredStatement`/`summary` if changed; includes each `Canvas`
  under `Items` that was either newly added or has its own internal changes, and *only* those
  (unchanged canvases are omitted entirely). This is a foundational reconstruction covering the
  properties this feature's own examples test, not an exhaustive copy of every possible Manifest
  property - extend `Manifest.ChangeTracking.cs` incrementally as real needs surface, the same
  "gap-fill against real coverage" discipline the rest of this SDK follows.
- **`Manifest.GetChangeSet()`** returns an `IiifChangeSet` envelope: `RootId`/`RootType`/
  `CreatedAtUtc`, the complete `Changes` list from `GetChanges()` (**every** change, including
  removals), and the same `ChangedManifest` as a convenience.
- **`IiifSerializer.SerializeChangedOnly(manifest, options)`** is a named-entry-point convenience
  equivalent to `Serialize(manifest.GetChangedManifest(), options)`.

### Why removals can't appear in `ChangedManifest`

A valid IIIF Manifest has no way to express "this canvas used to be here, now it's gone" - there's
no tombstone/deletion-marker concept in the spec. `GetChangedManifest()` therefore never represents
removals; **`IiifChangeSet.Changes`** is the reliable source for them (`IiifChangeKind.
CollectionItemRemoved` entries), since it's a flat record of raw facts, not a reconstructed
document. A consumer building a delta-persistence or event-sourcing pipeline should treat
`Changes` as the durable event log and `ChangedManifest` as a convenience "preview" for the
add/modify subset only:

```csharp
var changeSet = manifest.GetChangeSet();

// Apply changeSet.Changes to an event store / audit log - this is complete, including removals.
foreach (var change in changeSet.Changes)
    eventStore.Append(manifest.Id, change);

// Use ChangedManifest only for the "what does the updated object look like" preview -
// never assume it reflects removals.
if (changeSet.ChangedManifest is { } changed)
    cache.Upsert(changed);
```

## Full serialization is unaffected

`IiifSerializer.Serialize(manifest)`/`Serialize(manifest, options)` always serialize the complete,
live object graph regardless of tracked-change state - `SerializeChangedOnly` is a separate, opt-in
method. Calling `HasChanges`/`GetChanges()`/`ClearChanges()` never mutates data, only the
tracking bookkeeping (`ElementDescriptors`' Original/Modified pair, `_keysAtLastClear`,
`_collectionBaselines`).

## Limitations and non-goals

- **Not exhaustive property coverage in `GetChangedManifest()`** - see above; extend as needed.
- **No database/event-sourcing storage implementation** - this SDK produces the change model
  (`IiifChangeEntry`/`IiifChangeSet`); persisting it anywhere is entirely up to the consumer.
- **No EF Core dependency** - this is a from-scratch, SDK-native tracker conceptually inspired by
  EF Core's change tracker, not built on top of it.
- **`IiifChangeKind.CollectionCleared` is never synthesized** - see "Collections" above.
- **Value-collection properties never get per-item diffing** - `Label`/`Metadata`/etc. always
  report as one wholesale `Modified` entry; this is a deliberate simplification, not a bug, since
  these properties are constructor-required in practice (never touched for the "first time after
  clear" edge case that needed fixing for `Items`/`Rights`).
- **UI data-binding beyond the standard `INotifyPropertyChanging`/`INotifyPropertyChanged`
  interfaces** is out of scope - this SDK doesn't target any specific UI framework.
