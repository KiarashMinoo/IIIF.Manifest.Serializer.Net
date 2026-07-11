# IIIF SDK Multi-Version Design Guide

Status: **implementation complete** (all milestones 0-8 landed; see §9's status line for the
final summary). This document remains the reference for the architecture decisions and the
confirmed IIIF 2.x↔3.0 property mappings — read it before touching version-aware serialization
code.

All §5 mapping rows previously marked "verify against spec" have now been confirmed directly
against `iiif.io/api/presentation/3.0/` and its change log (2026-07-09) — see the updated table.
The four judgment calls that were not spec questions (Sequence's fate, multi-sequence handling,
services write strategy, Obsolete scope) have been decided — see §4 and §6.

## 1. Correction to prior assumptions

There was never a formally published **Presentation API 1.0**. Numbering starts at 2.0; 2.1 is
a minor revision of 2.0; 3.0 is the major rewrite. Informal pre-IIIF "Shared Canvas" documents
are **out of scope** for this SDK unless a concrete need surfaces later.

## 2. Goals

- Read Presentation API **2.0, 2.1, and 3.0** documents — `Manifest` and `Collection` — without
  data loss for anything the model represents.
- Write **whichever version the caller requests** (`IiifSerializerOptions.Version`), defaulting
  to 3.0.
- The public, preferred model surface is **3.0-shaped**. Legacy 2.x concepts are readable but
  are not the way to construct new documents.
- Full v1 scope (nothing below is deferred): `Manifest`, `Collection`, `Canvas`,
  `Structure`/Range, Services (Auth 1.0/2.0, Content Search 2.0, Change Discovery 1.0, Content
  State 1.0), and the three extension packages (navPlace, Georeference, TextGranularity).
- Preserve unknown/extension JSON properties where practical (already partly handled by
  `IAdditionalPropertiesSupport` — verify it still holds after the model reshape).

## 3. Model strategy: reshape around 3.0-native storage

This is the central decision. The object model's **internal storage** becomes 3.0-shaped:
`Items`, `Behavior`, `Rights`, `RequiredStatement`, `Summary`, `PartOf`, top-level `Services`,
etc. are the real backing data.

Legacy 2.x-only concepts (`Sequences`, `Canvas.Images`, `ViewingHint`, `License`, `Attribution`,
`Within`, `Structure.Canvases`/`Ranges`, `Collection.Manifests`/`Collections`/`Members`, ...)
become **computed, read-only views** derived from that same 3.0-native storage:

- **Getter**: derives its value from the new storage on demand (e.g. a legacy `Sequences`
  getter synthesizes a single `Sequence` wrapping `Items.OfType<Canvas>()`). Not tagged
  `[Obsolete]`, or at most a warning-level one — reading old-shaped data must keep working
  cleanly, since this is how a legacy JSON document round-trips through the model.
- **Setter / mutation method** (`SetX`, `AddX`, `RemoveX` tied to a legacy shape): kept in
  place, but marked `[Obsolete("...", error: true)]`. Already-compiled consumers referencing
  the old signature keep running unchanged (Obsolete is compile-time only). New source code
  written against this SDK fails to build if it calls one, unless explicitly suppressed. This
  is the agreed enforcement level — not physical removal, not warning-only.

**Decided (obsolete scope):** legacy getters stay untagged (no `[Obsolete]` at all) — reading a
parsed legacy document must not produce compiler noise. Only setters/mutators get
`[Obsolete(error: true)]`. Do not add warning-level `[Obsolete]` to getters as a discoverability
nudge; it would fire on every legitimate legacy-read code path (e.g. inspecting `.Sequences`
right after parsing a v2 manifest), which is not misuse.
- **Reading a legacy document** writes directly into the new 3.0-native storage during
  deserialization (not into legacy backing fields) — so immediately after parsing a v2 doc,
  `.Items`/`.Behavior`/etc. are already populated, and `.Sequences`/`.ViewingHint`/etc. are the
  computed legacy views reflecting that same data.

Consequence for serialization: once storage is 3.0-native, writing v3 becomes close to a direct
`JsonConvert.SerializeObject` using the model's own `[JsonProperty]`/`[JsonConverter]`
attributes (the same trick the 2.x-native model enjoys today). The hand-written `JObject`-tree
builder currently in `IiifSerializer` shrinks or inverts — it becomes the **legacy (2.0/2.1)
writer**, not the 3.0 writer.

## 4. Obsolete-tagging pattern (generalize the existing `ViewingHint` precedent)

Every legacy property already carries `[PresentationAPI(...)]` metadata
(`IsDeprecated`/`DeprecatedInVersion`/`ReplacedBy`). Apply this consistently, everywhere such
metadata already claims deprecation, but currently lacks a matching C# attribute:

- Legacy **getters**: keep working, no compile break. Add `[Obsolete("message")]` (warning,
  default `error: false`) only where it aids discoverability — never `error: true` on a getter.
- Legacy **setters/mutators**: `[Obsolete("message describing the 3.0 replacement", error: true)]`.

Known targets that currently have deprecation metadata but no `[Obsolete]` at all (from the
codebase scan) — every one of these needs the setter-side tag applied as part of this rewrite:

- `Manifest.Sequences` (getter: computed view) / `Manifest.AddSequence`/`RemoveSequence`
  (setters: obsolete-error) → replaced by `Items`.
- `Collection.Manifests`/`Collections`/`Members` and their `Add*`/`Remove*` → replaced by `Items`.
- `Sequence.Canvases`/`StartCanvas` and their mutators → replaced by `Range`/`start`. The
  `Sequence` class itself has no 3.0 equivalent (see §6) — evaluate whether it survives only as
  a legacy read/write shim.
- `Structure.Canvases`/`Ranges` and mutators → replaced by `Items` (nested `Range`s).
- `Canvas.Images`/`OtherContents` and mutators → replaced by `Items` (`AnnotationPage`).
- `Layer`, `AnnotationList` (whole classes) → no 3.0 replacement exists (superseded by
  `AnnotationPage`/`AnnotationCollection`); keep as legacy-read/write-only types, do not attempt
  to model a 3.0 equivalent that does not exist.
- `BaseNode.ViewingHint`/`SetViewingHint` → already done; use as the reference implementation.
- `BaseNode.License`, `BaseNode.Attribution`, `BaseNode.Within`, `BaseNode.Related` → replacements
  per §5; each needs the same getter/setter split.

## 5. Property mapping — confirmed against `iiif.io/api/presentation/3.0/` (2026-07-09)

Every row below has been checked against the live 3.0 spec and its change-log appendix. Rows
still marked *(design decision, not spec)* are judgment calls, decided in §4/§6 — re-verifying
those against the spec again is not useful, they aren't spec questions.

| 2.x | 3.0 | Notes / risk |
| --- | --- | --- |
| `@id` | `id` | Direct rename, high confidence. |
| `@type` | `type` | Direct rename; values also change format (`sc:Manifest` → `Manifest`, `dctypes:Image` → `Image`, etc.) — full value-mapping table needed per resource type, not just the key. |
| `viewingHint` | `behavior` | Already implemented; values mostly pass through, some 2.x values have no 3.0 equivalent (verify per value at implementation time). |
| `license` | `rights` | **Confirmed**: `rights` value must be a single string URI (not an array), drawn from CC license URIs, RightsStatements.org URIs, or an extension-registered URI. Rename only, plus that value constraint. |
| `attribution` | `requiredStatement` | **Confirmed structural change** — 3.0 `requiredStatement` is a single JSON object `{"label": {...lang map...}, "value": {...lang map...}}`, not an array/list of pairs. 2.x `attribution` is a bare language map with no label. Mapping: synthesize `label` as a fixed default (e.g. `{"en": ["Attribution"]}`) and carry the 2.x attribution value into `value` verbatim. |
| `description` | `summary` | **Confirmed**: renamed and semantics narrowed to short descriptive text; long-form descriptive content is expected to move to `metadata` instead in idiomatic 3.0, but the SDK's mapping only needs to carry the value through as `summary` — it must not try to auto-split content into `metadata`. |
| `related` | `homepage` | **Confirmed** via change log: "the `related` property was renamed to `homepage` with more specific semantics... Links to other resources instead go into `metadata` as HTML." Implement as a direct rename to `homepage`; do not route to `seeAlso`/`rendering`. |
| `within` | `partOf` | Rename + restructure to an object array with `id`/`type`. |
| `sequences[].canvases` | `items` | **Design decision, not spec** — see §6 multi-sequence handling: first sequence's canvases become `Items`; remaining sequences preserved on a legacy-only side property, not silently dropped, with a diagnostic noting they exist. |
| `startCanvas` | `start` | **Confirmed**: 3.0 `start` can be a full `SpecificResource` (`id`, `type: "SpecificResource"`, `source`, `selector`, e.g. a `PointSelector` with a `t` time offset), not just a bare canvas id. The v1 mapping should support both the plain-id case and the `SpecificResource`+selector case; do not truncate to id-only. |
| `canvas.images` | `canvas.items[0].items` (`AnnotationPage` → `Annotation`) | Already partly implemented in `IiifSerializer`; needs generalizing to Audio/Video/Segment/OtherContent, not just Image. |
| `annotation.resource` | `annotation.body` | Rename + type normalization (`dctypes:Image` → `Image`, etc. — already partly implemented). |
| `annotation.on` | `annotation.target` | Direct rename; verify selector-based targets are preserved, not just flattened to a string id. |
| `otherContent` | `annotations` (on Canvas, pointing to `AnnotationPage`s) | Needs the same `AnnotationPage` restructuring as `images`. |
| `structures`/`Range.canvases`/`Range.ranges` | `structures` (unchanged property name) / `items` (nested inside `Range`) | **Confirmed**: `Manifest.structures` is unchanged as a property name in 3.0 — only the internal shape of each `Range` changes to use `items`. Do not rename the top-level `structures` key. |
| `Collection.manifests`/`collections`/`members` | `items` | Rename + flatten to a single ordered `Items` collection (mixed `Manifest`/`Collection` references), matching how `Canvas`/`Manifest` already unify heterogeneous items. |
| `Collection` paging (`first`/`last`/`next`/`prev`/`total`) | *(no 3.0 replacement)* | **Confirmed** via change log §1.4.4: "paging functionality was removed from the API." Model as legacy-read-only; do not attempt to write it into 3.0 output. |
| `Layer`, `AnnotationList` | *(no 3.0 replacement)* | See §4 — legacy-only, no forward mapping exists. |
| — | `services` (top-level array) | **Confirmed**: inlining `service` directly on a resource remains fully spec-valid in 3.0 — the top-level `services` array is an optional centralization mechanism, not a requirement. A client seeing a stub `service` (only `id`/`type`) should then check `services`, but the SDK does not have to produce that stub form. **Decided (write strategy)**: v1 writes inline everywhere, no cross-resource dedup into `services`. Read side accepts both forms. |
| — | `placeholderCanvas`, `accompanyingCanvas`, `provider`, `timeMode` | New in 3.0, no 2.x equivalent — `provider`/`accompanyingCanvas` already exist in the model; `timeMode` already exists; `placeholderCanvas` already exists as a plain string on `Manifest` — verify it needs no further work. |

## 6. Resource-by-resource plan

- **`Manifest`**: `Items` (Canvas + Range) becomes primary storage. `Sequences` becomes a
  computed single-sequence legacy view (see multi-sequence note in §5). `Structures` stays as
  the manifest-level property name unless §5's verification says otherwise, but its content
  becomes `Range` objects backed by `Items`.
- **`Sequence`**: has no 3.0 concept at all. **Decided: option (a)** — kept purely as a legacy
  read/write shim with no interaction with the new storage beyond what the computed
  `Manifest.Sequences` view constructs on the fly. Smaller blast radius; keeps `AddSequence`'s
  obsolete-error message able to say "construct via Manifest.Items instead" without deleting a
  public type existing consumers may reference by name. Do not remove the standalone class.
- **`Canvas`**: `Items` (`AnnotationPage`/`Annotation`) becomes primary. `Images`/`OtherContents`
  become computed legacy views. `Height`/`Width`/`Duration` are shared, unaffected structurally
  (already fixed the nullable-erasure bug on these this session).
- **`Collection`**: `Items` (heterogeneous `Manifest`/`Collection` references) becomes primary.
  `Manifests`/`Collections`/`Members` become computed legacy views. Paging is legacy-read-only
  per §5.
- **`Structure`**: reshaped into a 3.0 `Range` concept internally (`Items` primary, nested
  `Range`s and `Canvas` references unified), `Canvases`/`Ranges` become computed legacy views.
- **`Layer`/`AnnotationList`**: legacy-only, no reshape — see §4.
- **Services** (`AuthService`/`AuthService1`/`AuthService2`/`SearchService`/
  `AutoCompleteService`/`DiscoveryService`/`ContentStateService`): add version-aware read/write
  for both the 2.x inline-`service` shape and the 3.0 top-level `services` array (see §5's
  inlining recommendation for the v1 write strategy). All the read-side bugs found this session
  (interface-level converter, constructor/`@context` collision, missing setters) are already
  fixed and must stay fixed through this reshape — add regression tests that exercise a full
  service round-trip explicitly, not just through a whole-manifest test.
- **Extensions** (navPlace, Georeference, TextGranularity): these are 3.0-only concepts with no
  legacy shape, so no obsolete/legacy view work is needed — but they currently define
  `*ExtensionAttribute`s that are never applied anywhere (dead code, confirmed by scan). Apply
  them to the relevant extension types, and add tests confirming these extensions still
  round-trip correctly once the core `BaseNode`/`BaseItem` storage changes underneath them.

## 7. Serialization architecture changes

- `IiifSerializer` gains **`Collection`** support: `Serialize(Collection, options)` /
  `DeserializeCollection(json)`, mirroring the existing `Manifest` methods.
- Once storage is 3.0-native, the 3.0 writer for `Manifest`/`Collection`/`Canvas`/`Range` becomes
  close to a direct `JsonConvert.SerializeObject(..., TrackableObject.JsonSerializerSettings)`
  call — the model's own `[JsonProperty]` names and converters ARE the 3.0 shape. The current
  hand-built `JObject`-tree functions (`WriteV3Manifest`, `WriteV3Canvas`, `WriteV3Annotation`,
  `WriteV3Resource`) are expected to shrink dramatically or be replaced entirely.
- A **new legacy writer** (2.0/2.1) takes over the JObject-tree-building role instead — building
  `sequences`/`canvases`/`images`/`resource`/`on` from the 3.0-native storage via the computed
  legacy view properties described in §3/§4.
- `IiifPresentationVersionDetector` already handles `Manifest`/`Collection`/`Canvas` type
  keywords in its structural fallback — extend its test coverage for `Collection`-shaped input
  specifically (not currently tested per the scan) rather than assuming it already works.

## 8. Testing & coverage

- Add `coverlet.collector` to the test project and a coverage-collection step to
  `.github/workflows/ci.yml` (`dotnet test --collect:"XPlat Code Coverage"` + a report step, e.g.
  ReportGenerator, publishing an artifact/PR summary). **No hard coverage gate for now** — report
  visibility first, revisit a minimum threshold once a real number exists.
- Every milestone below lands with its own tests in the same change — no "write all tests at the
  end" pass. At minimum, per reshaped resource:
  - Round-trip: legacy JSON in → 3.0-native model → legacy view getters reflect the same data.
  - Round-trip: 3.0 JSON in → model → re-serialize as 3.0 → structurally equivalent.
  - Cross-version: legacy JSON in → re-serialize as 3.0 out (the actual point of this SDK).
  - Cross-version: 3.0-constructed model → serialize as legacy 2.0/2.1 → structurally valid
    legacy shape.
  - Obsolete-setter compile-error check via a `csc`/analyzer-driven test where practical, or at
    minimum a reflection check that the attribute is present with `error: true` (extending the
    existing `ObsoleteCompatibilityTests.cs` pattern).
  - Extension/unknown-property preservation still holds after the reshape.

## 9. Milestones

Each milestone ends with passing tests before moving to the next. Do not batch multiple
milestones into one change.

0. **Tooling** — DONE: coverlet + CI coverage report added. No model changes.
1. **Reference reshape — `Canvas`** — DONE: `Items` (inherited from `BaseNode`, now `[JsonIgnore]`
   everywhere so 3.0-native storage never leaks into legacy JSON via reflection) holds
   `AnnotationPage`/`Annotation` (new types, `Nodes/Contents/Annotation/`). `Images`/`Audios`/
   `Videos` are computed legacy views over `Items` filtered by resource body type;
   `Annotations` (new 3.0 property, `annotations` key) is the 3.0-native replacement for
   `otherContent`, with `OtherContents` as its computed legacy view. `AddImage`/`AddAudio`/
   `AddVideo`/`AddOtherContent` are `[Obsolete(error: true)]`; `AddAnnotation`/
   `AddAnnotationPageReference` are the new non-obsolete write API. `IiifSerializer`'s V3
   Canvas writer/reader now read/write `Items`/`Annotations` directly instead of hand-casing
   Image/Audio/Video separately (closes the "generalize to Audio/Video/OtherContent" gap in
   §5). Cookbook/Examples/tests call sites migrated off the obsolete mutators. Tests:
   `CanvasReshapeTests.cs`, `IiifSerializerCanvasReshapeTests.cs`.
2. **`Manifest` + `Sequence`** — DONE: `Items` (inherited, `[JsonIgnore]`) holds `Canvas`
   directly; `Sequences` is a computed legacy view synthesizing a single `Sequence` from
   `Items`/`ViewingDirection`/`Start`. Multi-sequence legacy documents keep the first sequence
   as the 3.0-native view and preserve the rest verbatim on the new `AdditionalSequences`
   (legacy-only, no 3.0 replacement) rather than dropping them — confirmed lossless legacy
   round-trip via test. `SetSequenceId` (new, non-obsolete) sets the synthesized sequence's
   identity — a narrow legacy-JSON-shaping affordance, not itself a deprecated concept.
   `AddSequence`/`RemoveSequence` are `[Obsolete(error: true)]`. Turned out **no separate
   hand-built legacy writer was needed**: because `Sequences` is a plain computed
   `[JsonProperty]` getter (same trick as Canvas.Images in Milestone 1), plain `JsonConvert`
   continues to produce correct 2.0/2.1 JSON — the "add a legacy writer" prediction in this
   section was superseded by the computed-getter-view pattern actually working end-to-end.
   `IiifSerializer`'s V3 reader/writer updated to use `Items`/`AddItem` directly (dropped the
   internal `Sequence` wrapper entirely for 3.0 construction). All Cookbook/Examples/tests call
   sites migrated off `AddSequence`. Tests: `ManifestSequenceReshapeTests.cs`.
3. **`Collection`** — DONE: `Items` (inherited, `[JsonIgnore]`) holds real `Collection` objects
   and minimal label-less `Manifest` stubs (2.x `manifests` only ever carried bare ids; a new
   public, non-obsolete `AddManifestReference(string)` exposes stub creation externally since
   the backing `Manifest(string id)` ctor is `internal`). `Collections`/`Manifests`/`Members`
   are computed legacy views (`Members` maps directly to `Items`, the closest 2.x analogue to
   3.0 `items`). Legacy mutators are `[Obsolete(error: true)]`. Paging is untouched and NOT
   tagged obsolete — it has no 3.0 replacement to redirect to, it's simply excluded from 3.0
   output. `IiifSerializer` gained `Serialize(Collection, options)`/`DeserializeCollection`;
   version detector needed no changes (confirmed by new tests). Tests: `CollectionReshapeTests.cs`.
4. **`Structure`/Range** — DONE: `Items` (inherited, `[JsonIgnore]`) holds new minimal
   `CanvasReference`/`RangeReference` stubs (2.x `canvases`/`ranges` were always bare id lists,
   never embedded objects) or, for the 3.0-preferred path, real nested `Structure` objects.
   `Canvases`/`Ranges`/`Members` are computed legacy views (`Ranges` also surfaces the id of any
   nested `Structure`). Legacy mutators (`AddCanvas`/`AddRange`/`AddMember`/their `Remove*`
   counterparts) are `[Obsolete(error: true)]`, replaced by non-obsolete `AddCanvasReference`/
   `AddRangeReference`. Also fixed a latent bug exposed by the new tests: `Structure` had two
   public constructors and no `[JsonConstructor]` hint, so a standalone `Structure` had in fact
   never been deserializable via plain `JsonConvert` — added `[method: JsonConstructor]` (same
   pattern as `OtherContent`). `IiifSerializer` gained `structures`→`Range` V3 read/write (a gap
   that predates this milestone; `WriteV3Manifest`/`ReadV3Manifest` never touched `structures`
   at all before now). Tests: `StructureReshapeTests.cs`.
5. **Services** — DONE: inline (`service`) read/write for all service types was already mostly
   correct, but writing the "explicit full round-trip" tests this milestone asked for surfaced
   two real bugs, now fixed in `ServiceJsonConverter`: (a) the profile-based type-detection
   fallback only ran when `@type` was *present but unrecognized* — when `@type` was absent
   entirely (the normal case for Image/Auth services, whose constructors in this SDK never set
   one), detection skipped straight to a generic Image-service guess, silently losing
   Auth/Search/Discovery/ContentState data; (b) Auth 1.0 vs 2.0 couldn't be told apart from
   profile alone when `@type` was absent (their JSON shapes overlap enough that deserializing
   as the wrong one didn't throw) — now disambiguated via the always-set `@context`
   (`/auth/1/` vs `/auth/2/`). Added `Manifest.Services` (new, 3.0-only top-level centralized
   array, `AddTopLevelService`/`RemoveTopLevelService` — distinct names from the inherited
   inline `AddService`/`RemoveService` to avoid confusing the two concepts) with full
   `IiifSerializer` V3 read/write; per §5's decision, write always inlines (nothing populates
   this from the inline `service` automatically — no dedup). Tests: `ServiceRoundTripTests.cs`.
6. **Extensions** — DONE, and this milestone surfaced the most significant bug found in the
   whole reshape effort. Applied `NavPlaceExtensionAttribute`/`GeoreferenceExtensionAttribute`/
   `TextGranularityExtensionAttribute` to the relevant extension properties/methods/types (they
   were confirmed genuinely dead code beforehand — zero references anywhere).
   **Critical fix**: writing the "confirm these extensions round-trip" tests this milestone
   asked for proved they did **not round-trip at all** — `SetNavPlace`/`SetTransformation`/
   `SetResourceCoords`/`SetTextGranularity` store into `ElementDescriptors` with
   `IsAdditional=true`, but nothing ever serialized that data into JSON (no `[JsonExtensionData]`
   or equivalent existed), so extension data set via any of these three packages was silently
   dropped on every serialize — the packages did not do what they were shipped to do. Fixed by:
   1. Adding a `[JsonExtensionData]`-backed `AdditionalPropertiesDictionary` bridge to
      `TrackableObject<T>` that surfaces `IsAdditional=true` entries for write and accepts
      unmapped JSON keys back into `ElementDescriptors` for read.
   2. Excluding `JToken` from the "wrap enumerable values in `BindingList<T>`" heuristic in
      `SetElementValue` — `JObject` implements `IEnumerable`, so without this exclusion a raw
      JToken value read back via extension data got corrupted into a single-item
      `BindingList<JToken>` instead of stored as a scalar.
   3. Lazily converting the raw JToken/primitive stored by extension-data reads to the real
      requested type on first typed access in `GetElementValue` (applying whatever
      `JsonConverter` that type declares, e.g. `ValuableItemJsonConverter<TextGranularity>`),
      caching the converted result in place.
   Also fixed two smaller pre-existing bugs found by the same tests: `ServiceJsonConverter`'s
   Auth1/Auth2 profile-based detection (same class of bug as Milestone 5's fix, just for a
   different code path) and `BaseResource`'s own `JsonConstructor` binding a raw JSON string
   directly to a `ResourceType`-typed parameter without going through its converter. Tests:
   `ExtensionAttributeTests.cs`.
7. **Cookbook/Examples migration** — DONE: `ExampleCatalogTests.cs` now round-trips every
   `Manifest`/`Collection` example (the large majority of the ~20 cookbook/demo recipes) through
   `IiifSerializer` in both directions and both versions (build → write v2.1 → write v3.0 →
   read each back → re-write in the other version). `Layer`/`AnnotationList` examples (no 3.0
   concept, no `IiifSerializer` support by design) still fall back to a plain-JSON validity
   check. This migration surfaced no new bugs — a good signal that Milestones 1–6's fixes were
   thorough. All 142 tests pass.
8. **Consistency sweep** — DONE. Grepped every `IsDeprecated = true` site and cross-checked
   against `[Obsolete]` coverage. Milestones 1-5's own targets (Canvas/Manifest/Collection/
   Structure/Sequence) were all correctly tagged. But the sweep caught a real gap: **§4's own
   `BaseNode.License`/`Attribution`/`Within`/`Related` targets had never actually been reshaped**
   — their write APIs (`SetLicense`, `AddAttribution`/`RemoveAttribution`, `AddWithin`/
   `RemoveWithin`, `SetRelated`) were still fully open, directly violating the user's explicit
   final requirement that obsolete properties be read-only. Fixed now: added 3.0-native `Rights`/
   `RequiredStatement`/`PartOf` (new types; `Related` maps onto the already-existing `Homepage`
   per the confirmed change-log mapping), made the four legacy properties computed views, and
   tagged the legacy mutators `[Obsolete(error: true)]`. This also caught two more bugs during
   testing: (a) `Rights`/`RequiredStatement`/`PartOf` initially leaked into legacy V2 JSON via
   plain `JsonConvert` reflection — fixed with `[JsonIgnore]`, the same pattern as `Items`; (b)
   a pre-existing bug in `ObjectArrayJsonConverter`: deserializing an explicit JSON `null` for
   any `IReadOnlyCollection<T>` property produced a list containing **one null element** instead
   of an empty list — harmless for old code that just stored the list, but a
   `NullReferenceException` waiting to happen for the new computed setters that actually iterate
   the collection. Fixed at the source. Tests: `BaseNodeReshapeTests.cs`. Coverage push:
   `CoveragePushTests.cs` (Collection paging, Structure viewing-direction/start). Overall line
   coverage: **48.7% → 63.5%** (from the pre-session baseline in `coverage-report/SummaryGithub.md`).
   Final count: **170 tests, all passing**, solution builds clean.

**Known follow-up, not fixed this pass** (flagged for a future session, not blocking): `Behavior`
(3.0-only, like `Rights`/`RequiredStatement`/`PartOf`) is not yet `[JsonIgnore]`d and likely has
the same legacy-JSON-leak risk when the plain-`JsonConvert` legacy write path is used directly
(as opposed to `IiifSerializer.Serialize(..., V2_1)`, which is unaffected). Also, `Properties/Related.cs`
(a `Related : FormattableItem<Related>` value type) is dead code predating this session —
`BaseNode.Related` has always been a plain `string?`, never referencing that type.

### Status of Milestones 0-8: complete (superseded by the final status at the end of this document)

## 10. Extended standards coverage (Auth/Search/Discovery/Content State/Image API, extensions)

Follow-up pass, prompted by a deep comparison against `github.com/IIIF/awesome-iiif`'s "Standards"
section (Auth, Content Search, Change Discovery, Content State, Image APIs + navPlace/Georeference/
Text Granularity extensions) — the awesome-iiif list itself is a directory of external tools, not
a spec, but its Standards section is the right checklist of what a manifest/service-descriptor
library like this one should model. The Presentation API core (already fully reshaped in §§1-9)
was explicitly out of scope for this comparison. Findings, in priority order:

1. **Cross-cutting bugs** (found first, affect everything below):
   - `WriteV3Service` (`IiifSerializer.cs`) does `token.Remove("@type")` instead of renaming it to
     `type` — every service written into the new `Manifest.Services` top-level array (Milestone 5)
     silently loses its type in V3 output. Real regression from this session's own work.
   - `Service` (Image) and `AuthService2` never populate `Type` in any constructor — a manifest
     built with this library embedding an image or Auth 2.0 service never identifies what kind of
     service it is via `type`/`@type` at all.
   - `AuthService2`/`SearchService`/`AutoCompleteService`/`DiscoveryService`/`ContentStateService`
     all inherit `BaseItem<T>`, which hardcodes `@id`/`@type` — but Auth 2.0, Search 2.0, Discovery
     1.0, and Content State 1.0 all postdate Presentation 3.0 and use unprefixed `id`/`type` in
     their own spec examples (only Auth 1.0, contemporaneous with Presentation 2.x, correctly uses
     `@id`/`@type`). `Activity`/`ActivityObject` already get this right by declaring their own
     unprefixed properties instead of inheriting `BaseItem`.
2. **Content State 1.0 — 0% modeled, biggest gap.** No `ContentState` object exists at all (an
   Annotation with `motivation: "contentState"`, `target` polymorphic over Canvas/Manifest/
   SpecificResource+Selector/bare URI), and no base64url `iiif-content` encode/decode helper —
   pure client-side logic, no server needed, squarely in scope for a manifest-authoring library
   wanting to build/parse IIIF deep links. The existing `ContentStateService` class models a
   `{"type": "ContentStateService", "profile"}` shape that doesn't correspond to anything in the
   actual spec (Content State 1.0 has no discoverable "service" concept at all).
3. **Auth 2.0 — one flat class standing in for four distinct service types.** Spec defines
   `AuthProbeService2`, `AuthAccessService2`, `AuthAccessTokenService2`, `AuthLogoutService2` with
   different required/optional fields (`id` must be *absent* for `external`-profile access
   services; `errorHeading`/`errorNote` on Probe/AccessToken; `label`/`heading`/`note`/
   `confirmLabel` are language maps, not plain strings) — plus the response shapes
   (`AuthProbeResult2`: `status`/`substitute`/`location`/`heading`/`note`; `AuthAccessToken2`/
   `AuthAccessTokenError2`: `accessToken`/`expiresIn`/`messageId`/error `profile` codes) are
   unmodeled entirely.
4. **Discovery 1.0 — no real paging.** `DiscoveryService` is missing `first`/`last` (spec-required)
   /`totalItems`/`seeAlso`/`partOf`/`rights`, and there's no distinct `OrderedCollectionPage` type
   (collection and page are conflated). `Activity` is missing `id`/`startTime`/`summary`/`actor`/
   `target` (the last makes `Move` activities structurally unrepresentable). `ActivityObject` is
   missing `canonical`/`seeAlso`/`provider`.
5. **Search 2.0 — response shapes unmodeled.** The search-result `AnnotationPage` (hits, `ignored`
   params, `partOf`/`next`/`prev`/`startIndex` paging) and autocomplete's `TermPage` (`items` of
   `Term`: `value`/`total`/`label`/`language`/`service`) have no C# types. The existing `profile`
   property on `SearchService`/`AutoCompleteService` isn't actually part of the 2.0 spec (a Search
   1.0 holdover) — harmless as an extension point, just not spec-required.
6. **Image API — `Service` (embedded descriptor) incomplete, no standalone info.json type.**
   Missing `protocol` (spec-required), `extraFormats`, `Tile.Height`. No type represents a
   freestanding info.json document (different JSON shape than the embedded-service form: `id`/
   `type` unprefixed at top level vs. `@id`/`@type` when embedded via `BaseItem`).
7. **Text Granularity extension — wrong enum values.** Spec defines `page`/`block`/`paragraph`/
   `line`/`word`/`glyph`; the code has `character` (not a spec term at all) instead of `glyph`,
   and is missing `paragraph` entirely. Real documents using `"glyph"`/`"paragraph"` throw in
   `Parse`.
8. **navPlace — missing `geometries`** on `Geometry` for the `GeometryCollection` case (an array of
   nested `Geometry` objects, distinct from `coordinates`). Otherwise verified complete.
9. **Georeference — top-level Annotation wrapper unmodeled.** The `transformation`/`resourceCoords`
   property-level pieces are correct, but the spec's actual Georeference construct is a full W3C
   Annotation (`motivation: "georeferencing"`, `target`: `SpecificResource`+`Selector` over the
   Canvas/Image, `body`: the FeatureCollection) — currently `transformation` is just an
   additional-property settable directly on any `BaseNode`, with no annotation wrapper at all.
10. **Dead code**: `Properties/Services/AuthService.cs` (context `/auth/0/`) duplicates
    `AuthService1`, is unreachable via `ServiceJsonConverter`, and predates the "there was never a
    Presentation 1.0" correction from §1 — same class of leftover as the `Related.cs` dead code
    noted in §8's follow-up. `ServiceJsonConverter`'s Auth-1.0-only `@type` switch cases
    (`AuthCookieService1`/etc.) don't correspond to any `@type` the spec actually defines (Auth 1.0
    typing is profile-driven, not `@type`-driven) — not incorrect, just documentation-shaped
    fiction; the profile-based fallback is what actually resolves these in practice.

Milestones 9-19 below track implementing this list, same discipline as §§1-9: tests land with
each change, `[Obsolete(error: true)]` on anything a fix genuinely replaces, no regressions.

### Milestone 9: DONE — cross-cutting service bugs (type write/dispatch)

Fixed the three bugs from finding 1 above, scoped tightly to avoid redoing work that Milestones
10/11 will replace anyway:

- `WriteV3Service` (`IiifSerializer.cs`) now renames `@type` → `type` instead of removing it, and
  no longer mis-renames `@context` → `context` (a second bug found while fixing the first — `@context`
  is a JSON-LD keyword that stays prefixed in Presentation 3.0; only `id`/`type` lose the `@`).
- `SearchService`, `AutoCompleteService`, `DiscoveryService` now inherit a new `UnprefixedBaseItem<T>`
  (`Shared/UnprefixedBaseItem.cs`) instead of `BaseItem<T>` — same shape/constructor pattern, but
  `IdJName`/`TypeJName` are unprefixed (`id`/`type`) with `@context` staying prefixed, matching how
  `Activity`/`ActivityObject` already modeled their own properties. `ContentStateService` was left on
  `BaseItem` for now since Milestone 10 replaces it outright with a real Content State 1.0 object.
  `Service` (Image) and `AuthService2` were left as-is too — both already have a real `Type`-population
  gap, but `AuthService2` is being split into 4 types in Milestone 11 and `Service`'s `type` is
  version-dependent (`ImageService2` vs `ImageService3`, no single correct hardcoded value) and is
  addressed properly in Milestone 14 instead of hardcoding a guess now.
- `ServiceJsonConverter`'s type-dispatch switch now checks both `@type` and unprefixed `type` (a V3
  manifest's top-level `services` array always writes id/type unprefixed on the wire regardless of
  which shape the leaf C# class models internally), and normalizes the token to whichever shape the
  detected leaf class's constructor actually binds against via two new helpers, `WithPrefixedIdType`/
  `WithUnprefixedIdType` — both no-ops when the token already matches, so the existing profile-based
  Auth1/Auth2 fallback path needed no behavioral changes beyond calling them.
- `IiifSerializer.ReadV3Service` no longer blindly renames every service's id/context/type to the
  `@`-prefixed shape before dispatch (that blind reversal was only safe while every leaf class was
  `BaseItem`-shaped; it would have silently broken constructor binding for the newly-unprefixed
  classes) — detection/normalization now lives entirely in `ServiceJsonConverter`, which knows which
  shape each leaf type needs.

Tests: `Milestone9ServiceFixTests.cs` (5 new tests — V3 top-level array write shape for
Search/Discovery services, round-trip through `IiifSerializer`, hand-written-JSON dispatch with
unprefixed `type`, and a regression lock confirming `AuthService2` still writes its pre-existing
`@id`-prefixed shape for the plain 2.x/inline-service path). Full suite: **175 tests, all passing**,
0 build warnings/errors introduced.

### Milestone 10: DONE — Content State 1.0 object + base64url encode/decode

Added the object that finding 2 said was entirely missing: `Nodes/Contents/ContentState/`:

- `ContentState.cs` — the W3C-Annotation-shaped root object (`id?`, `type: "Annotation"`,
  `motivation: "contentState"`, `target`), built on `TrackableObject<T>` directly (not `BaseItem`,
  matching `Activity`/`ActivityObject`'s precedent for shapes that don't fit either the `@`-prefixed
  or unprefixed `BaseItem` conventions). `Target` reuses `ObjectArrayJsonConverter` so a single
  target serializes bare and multiple targets serialize as an array, per spec.
- `ContentStateTarget.cs` + `ContentStateTargetJsonConverter.cs` — models the 3 target shapes the
  spec allows (bare URI string; typed resource reference `{id,type}`; full SpecificResource wrapping
  a `source` and/or `selector`), with the converter picking the minimal correct shape on write based
  on which fields (`ResourceType`, `Selector`, `PartOfId`) are set, and dispatching on read by
  inspecting whether the token is a string, a plain object, or a `"type":"SpecificResource"` object.
- `ContentStateFragmentSelector.cs` — the Media Fragments region selector (`xywh=...`), the only
  selector shape modeled (matches this SDK's existing region-selection precedent elsewhere).
- `ContentStateCodec.cs` — `Encode`/`Decode` for the base64url "content state string" used in the
  `iiif-content` query parameter: UTF-8 JSON → standard base64 → strip padding → `+`/`/` → `-`/`_`
  (and the reverse for decode).

Deliberately left untouched: the existing `ContentStateService` class (`Properties/Services/`) —
finding 2 noted its `{"type":"ContentStateService","profile"}` shape doesn't correspond to
anything in the real spec (Content State 1.0 has no "service" concept at all), but it's an inert,
already-tested extension point rather than something actively wrong, so removing it wasn't part of
this milestone's scope (no spec-shaped replacement exists to migrate callers to).

Tests: `ContentStateTests.cs` (7 new tests — all 3 target write shapes, full serialize/parse
round-trip incl. selector + partOf, multi-target array round-trip, base64url codec round-trip, and
a blank-input error case). Full suite: **182 tests, all passing**, 0 build warnings/errors introduced.

### Milestone 11: DONE — Auth 2.0 split into 4 real service types + response shapes

Replaced the old flat `AuthService2` (deleted) with `Properties/Services/Auth2/`:

- `AuthProbeService2`, `AuthAccessService2`, `AuthAccessTokenService2`, `AuthLogoutService2` — each
  with its own actual required/optional fields per spec, instead of one class with every field
  optional. `AuthProbeService2`/`AuthAccessTokenService2`/`AuthLogoutService2` have no `profile`
  field at all (satisfy `IBaseService.Profile` via an explicit interface implementation returning
  `string.Empty`, which Newtonsoft's reflection-based serializer never picks up as a real member —
  confirmed no stray `"profile": ""` appears in output). `AuthAccessService2.Id` is nullable
  (required for `active`/`kiosk`, must be *absent* for `external`) — required loosening
  `UnprefixedBaseItem<T>`'s `id` constructor parameter from `string` to `string?` (the `Id` property
  itself was already null-tolerant at runtime; only the compile-time constraint changed, and
  `NullValueHandling.Ignore` already omits a null `Id` from output for free). `AuthProbeService2`'s
  and `AuthAccessService2`'s nested service arrays reuse the inherited `Service`/`AddService`
  machinery directly, with `[JsonIgnore]`d convenience accessors (`AccessServices`/
  `AccessTokenService`/`LogoutService`) layered on top.
- `Responses/AuthProbeResult2`, `AuthAccessToken2`, `AuthAccessTokenError2`, plus a small
  `AuthResourceReference` (`{id,type}`) used by `AuthProbeResult2`'s `substitute`/`location` — these
  are HTTP/`postMessage` response payloads, not embedded services, so they're plain
  `TrackableObject<T>` (not `IBaseService`) and live in their own `Responses/` folder.
- `LanguageMapJsonConverter` (`Shared/`) — new. Auth 2.0's `label`/`heading`/`note`/`confirmLabel`/
  `errorHeading`/`errorNote` fields are all real language maps per spec, but this SDK's existing
  `BaseNode.Label` only becomes a `{"none": [...]}` language map when routed through
  `IiifSerializer`'s hand-built V3 writer — embedded services never go through that path (they
  serialize via their own `[JsonProperty]`/plain `JsonConvert`), so without a dedicated converter
  Auth 2.0's label fields would've written as bare strings. Reads leniently (map, bare array, or
  bare string) and writes `{"none": [...]}`, matching this SDK's established (if spec-simplified —
  single "none" bucket, not real BCP47 tags) language-map convention.

Cross-cutting fixes needed to make the above work:
- `WriteV3Service` (`IiifSerializer.cs`): removed the `Rename(token, "@context", "context")` call
  found while implementing this milestone — a **second** real bug in the code Milestone 9 had just
  touched. `@context` is a JSON-LD keyword that stays prefixed in Presentation 3.0 (only `id`/`type`
  drop the `@`); renaming it to a nonexistent `context` key would have silently corrupted every
  service written into the top-level `Manifest.Services` array that carries its own `@context`
  (Auth 2.0's four types all do).
- `IiifSerializer.ReadV3Service`: simplified to a pass-through — the previous blind
  `id→@id`/`context→@context`/`type→@type` rename (a holdover from when every leaf service class was
  `BaseItem`-shaped) would have broken constructor binding for the Search/AutoComplete/Discovery
  classes Milestone 9 already moved to unprefixed shape. Detection/normalization now lives entirely
  in `ServiceJsonConverter`.
- `ServiceJsonConverter`'s Auth 1.0-vs-2.0 profile-based fallback branch was simplified to just Auth
  1.0: real Auth 2.0 JSON always carries an explicit `type` (unlike this SDK's Auth 1.0 classes,
  which by design never set one — Auth 1.0 typing is profile-driven per spec), so with every Auth
  2.0 constructor now populating a real `type`, the switch-based primary dispatch always succeeds
  and the fallback path is never reached for Auth 2.0 in practice.
- `ServiceJsonConverter`'s recursion-guard contract resolver (`LeafContractResolver`) previously
  suppressed `ServiceJsonConverter` for **any** type that inherited it via the
  interface-attribute-inheritance quirk noted in the existing code comment — correct for preventing
  a leaf type from recursing into itself, but wrong once a leaf type nests *other* polymorphic
  services (`AuthProbeService2.Service` holds `AuthAccessService2`; `AuthAccessService2.Service`
  holds `AuthAccessTokenService2`/`AuthLogoutService2`): the nested `IReadOnlyCollection<IBaseService>`
  element type needs the converter to stay *active* to dispatch to the right concrete type. Fixed by
  only suppressing when `objectType` is a concrete class, not when it's the `IBaseService` interface
  itself — caught by a real `JsonSerializationException` ("cannot instantiate interface") when
  running the full suite, not by reasoning alone.
- Existing callers migrated off the deleted `AuthService2`: `DemoCatalog.cs`, `CookbookCatalog.cs`
  (both now build a real Probe→Access→AccessToken+Logout chain), and the `ServiceRoundTripTests.cs`/
  `Milestone9ServiceFixTests.cs` tests that referenced it by name.

Tests: `Auth2ServiceTests.cs` (8 new — no-profile-field checks, error heading/note round trips,
logout label requirement, all 3 response types incl. substitute/location) plus rewrites of the
Auth2-related cases in `ServiceRoundTripTests.cs` and `Milestone9ServiceFixTests.cs`. Full suite:
**191 tests, all passing**, 0 build warnings/errors introduced.

**Known follow-up, not fixed this pass**: language maps everywhere in this SDK collapse every value
into a single `"none"` bucket rather than real BCP47 language tags (`"en"`, etc.) — a pre-existing,
deliberate simplification (see `IiifSerializer.WriteLanguageMap`) that `LanguageMapJsonConverter`
intentionally matches for consistency rather than fixing in isolation for just Auth 2.0.

### Milestone 12: DONE — Discovery 1.0 paging + Activity completeness

Split the previously-conflated collection/page model in `Properties/Services/DiscoveryService.cs`
and `Properties/Services/Discovery/`:

- `DiscoveryService` now models only the top-level "OrderedCollection": `First`/`Last`
  (`DiscoveryResourceReference`, a plain `{id,type}` pointer — `Last` is spec-required),
  `TotalItems`, `SeeAlso` (`DiscoveryDataset`), `PartOf`, `Rights`. Its previous `OrderedItems` and
  fabricated `Profile` field were removed — per spec the collection only *points at* pages via
  first/last, it never embeds their activities directly, and Discovery 1.0 has no profile concept
  (same explicit-interface-implementation trick as Milestone 11's profile-less Auth 2.0 services).
- `DiscoveryCollectionPage` (new) — the actual "OrderedCollectionPage": `PartOf`/`Next`/`Prev`
  (`DiscoveryResourceReference`), `StartIndex`, `OrderedItems` (the real activity list, moved here
  from `DiscoveryService`). Plain `TrackableObject<T>`, not `IBaseService` — a page is a standalone
  fetchable resource, never embedded as a service.
- `DiscoveryDataset`/`DiscoveryAgent` (new, shared) — the spec's `seeAlso` (`Dataset`: id/type/
  format/label/profile) and `provider`/`actor` (`Agent`-shaped: id/type/label) reference shapes,
  used by both `DiscoveryService`/`ActivityObject` (seeAlso) and `ActivityObject`/`Activity`
  (provider/actor).
- `Activity` gained `Id`, `StartTime`, `Summary`, `Actor` (`DiscoveryAgent`), and `Target`
  (`DiscoveryResourceReference`) — `Target` specifically makes "Move" activities representable
  (spec: `object` carries the *source* location, `target` the *destination*; previously only
  `object` existed, so a Move's new location had nowhere to go).
- `ActivityObject` gained `Canonical`, `SeeAlso`, `Provider`.

Existing callers (`CookbookCatalog.cs`, `ServiceRoundTripTests.cs`, `Milestone9ServiceFixTests.cs`)
migrated from the old `new DiscoveryService(context, id, profile)` to
`new DiscoveryService(context, id, lastPageReference)`.

Tests: `DiscoveryServiceTests.cs` (5 new — collection has no orderedItems and writes first/last/
totalItems/rights, page round-trips paging fields + activities, a full Move-activity round trip
(object=source, target=destination), Activity id/startTime/summary/actor round trip, ActivityObject
canonical/seeAlso/provider round trip). Full suite: **196 tests, all passing**, 0 build
warnings/errors introduced.

### Milestone 13: DONE — Search 2.0 response shapes (AnnotationPage hits, TermPage)

Finding 5 noted the search-result and autocomplete-result *response bodies* (as opposed to the
already-modeled `SearchService`/`AutoCompleteService` descriptors) were entirely unmodeled. Added
`Properties/Services/Search/`:

- `SearchResponse` — the search "AnnotationPage": `Items` reuses the core
  `Nodes.Contents.Annotation.Annotation` type directly (search-result annotations have the same
  `{id,type,motivation,body,target}` shape as painting annotations), plus `Annotations` (nested
  match-context page), `PartOf`/`Next`/`Prev`/`StartIndex` paging, `Ignored`.
- `SearchHitAnnotationPage`/`SearchHitAnnotation`/`SearchHitTarget`/`SearchTextQuoteSelector` — the
  `annotations` field's nested "contextualizing" match entries, each pointing back at its
  search-result Annotation via a `SpecificResource` + one or more W3C `TextQuoteSelector`s
  (`prefix`/`exact`/`suffix`). Deliberately a *separate* small type tree from the core `Annotation`
  rather than extending it — `Annotation.Target` is a plain `string` everywhere else in this SDK
  (Canvas painting, etc.) and making it polymorphic to also accept a `SpecificResource` object would
  be a much larger, riskier cross-cutting change for a shape that's specific to search hit-highlighting.
- `SearchAnnotationCollectionRef`/`SearchResourceReference` — the embedded `partOf` "AnnotationCollection"
  (id/first/last/total) and the plain `{id,type}` pointers `next`/`prev` use.
- `TermPageResponse`/`SearchTerm` — the autocomplete "TermPage": `SearchTerm.Value` is the only
  required field (a bare `{"value":"..."}` is valid); `Type`/`Total`/`Label`/`Language`/`Service`
  (reusing the existing polymorphic `IBaseService` handling) are all optional per spec.

**Known follow-up, not fixed this pass**: `Annotation.Body` (`IBaseResource`) has no
polymorphic-dispatch `JsonConverter` for plain `JsonConvert`/`TrackableObject.Parse` round trips —
it only resolves correctly through `IiifSerializer`'s hand-built V3 Canvas reader, which knows the
concrete resource type from context. This is a pre-existing gap (not introduced by this milestone)
that surfaced because `SearchResponse` is a standalone response object with no `IiifSerializer`
integration of its own; `SearchResponseTests.cs` works around it by not round-tripping `Items`
through `Parse`, testing the new paging/hit-highlighting fields independently instead.

Tests: `SearchResponseTests.cs` (3 new — required context/type/items write shape, full hit-highlighting
+ paging + ignored-params round trip, minimal-vs-extended Term round trip). Full suite: **199 tests,
all passing**, 0 build warnings/errors introduced.

### Milestone 14: DONE — Image API service completeness

Finding 6's gaps in `Properties/Services/Service.cs` (the embedded Image API service descriptor,
deliberately left untouched in Milestone 9 since — unlike every other service this pass fixed — it
genuinely spans *two* spec versions in one class):

- **`Type` now actually gets populated** (was `string.Empty` since the class's original writing,
  the same bug Milestone 9 fixed for `AuthService2`). Defaults to `"ImageService3"` (steers new code
  to the current spec, per the session's overall mandate), with `AsImageService2()`/`AsImageService3()`
  fluent toggles — `Service` is unlike Auth2/Search/Discovery's classes, each of which maps to
  exactly one spec version, so its `type` can't be inferred from context and must be an explicit choice.
- **`Protocol`** (spec-required, fixed `"http://iiif.io/api/image"` in both 2.x and 3.0) — read-only
  computed property, since a descriptor that isn't Image-API-protocol-compliant has no reason to
  exist.
- **`ExtraFormats`** (was entirely missing) — added with the same `ImageFormat` value-object pattern
  already used for `PreferredFormats`.
- **`Tile.Height`** (was missing; spec: optional, defaults to `Width` when omitted).
- **`Service.ToInfoJson()`** (new) — the research noted "no type represents a freestanding info.json
  document" (unprefixed `id`/`type`, vs. the `@id`/`@type` this class already correctly uses when
  embedded inline in a Presentation resource for 2.x-era backwards compatibility — confirmed via the
  live spec page that embedding *may* still use `@id`/`@type`, so the existing shape wasn't wrong,
  just incomplete). Rather than a whole parallel class for an identical data model, added a
  conversion method that unprefixes `id`/`type` while leaving `@context` alone (still a JSON-LD
  keyword, same rule as Milestones 9/11's `@context`-handling fixes).

Tests: `ImageServiceCompletenessTests.cs` (4 new — default-to-3.0 + protocol write, the 2.0 toggle,
extraFormats/Tile.Height round trip, `ToInfoJson()`'s unprefixed-id/type-but-prefixed-context shape).
Full suite: **203 tests, all passing**, 0 build warnings/errors introduced.

### Milestone 15: DONE — fix Text Granularity enum values

Verified the exact 6 Text Granularity Levels directly from the spec source
(`github.com/IIIF/api/source/extension/text-granularity/index.md`, since the live iiif.io page
404'd): `page`, `block`, `paragraph`, `line`, `word`, `glyph`. `extensions/IIIF.Manifest.Serializer.Net.TextGranularity/TextGranularity.cs`
had `character` (not a term the spec defines at all) instead of `glyph`, and was missing `paragraph`
entirely — real OCR/transcription documents using either term would throw in `Parse`. Renamed
`Character` → `Glyph` and added `Paragraph`; no call sites referenced `.Character` anywhere in this
repo, so no migration was needed beyond the enum itself. This is a straight correctness fix (not a
version-legacy split — Text Granularity has only ever had one shape), so no `[Obsolete]` shim: `"character"`
now throws in `Parse`, same as any other invalid value.

Tests: `TextGranularityEnumTests.cs` (9 new — all 6 spec levels parse correctly, the removed
`"character"` term now throws, `Paragraph`/`Glyph` are reachable as static instances). Full suite:
**212 tests, all passing**, 0 build warnings/errors introduced.

### Milestone 16: DONE — navPlace GeometryCollection.geometries

`extensions/IIIF.Manifest.Serializer.Net.NavPlace/Geometry.cs` had no way to represent RFC 7946
§3.1.8's "GeometryCollection" case — an array of complete nested `Geometry` objects under
`geometries`, which (per spec) is mutually exclusive with the `coordinates` member every other
geometry type uses. Added a `Geometries` property (`IReadOnlyCollection<Geometry>`) plus
`SetGeometries`/`AddGeometry`/`RemoveGeometry`; empty by default so non-collection geometry types
(`Point`/`LineString`/etc.) never write a stray `"geometries": []`.

Tests: `NavPlaceGeometryCollectionTests.cs` (3 new — collection writes `geometries` not
`coordinates`, nested geometries round-trip including a nested Polygon's own coordinate structure,
and a plain Point confirms it never writes a `geometries` field). Full suite: **215 tests, all
passing**, 0 build warnings/errors introduced.

### Milestone 17: DONE — model Georeference Annotation wrapper

Verified the exact Annotation shape from the spec source
(`github.com/IIIF/api/source/extension/georef/index.md`, §§3.2-3.4 + full example). Added to
`extensions/IIIF.Manifest.Serializer.Net.Georeference/`:

- `GeoreferenceAnnotation` (new) — the actual top-level construct: fixed `type: "Annotation"` /
  `motivation: "georeferencing"`, a `Target`, and a `Body` (reuses the navPlace extension's
  `NavPlace` FeatureCollection type directly, since both are the same shape and the Georeference
  project already references NavPlace for exactly this reason).
- `GeoreferenceTarget` + `GeoreferenceTargetJsonConverter` (new) — the 3 target shapes the spec
  allows (§3.3): a bare Canvas/Image-Service URI, a full resource object (`id`/`type`/`height`/
  `width`), or a SpecificResource wrapping a `GeoreferenceSvgSelector` for targeting a specific
  region — same polymorphic-converter pattern as Milestone 10's `ContentStateTarget`.
- `GeoreferenceSvgSelector` (new) — the W3C SvgSelector (`type`/`value`) the spec prefers over
  other selector shapes for Georeference Annotations (§3.3.2).
- **Cross-cutting fix**: `TransformationExtensions`'s generic constraint was narrowed to
  `BaseNode<TNode>` — meaning `SetTransformation`/`Transformation` (which the spec places on the
  Annotation body FeatureCollection, §3.6) could never actually be called on `NavPlace`, since
  `NavPlace` is a `BaseItem`, not a `BaseNode`. This was finding 9's literal complaint ("transformation
  is just an additional-property settable directly on any BaseNode, with no annotation wrapper at
  all") — confirmed by attempting to call it on a `NavPlace` instance and finding it wouldn't
  compile. Widened the constraint to `TrackableObject<TNode>` (the only base every settable type,
  including `NavPlace`, actually shares), which required no other changes since
  `TrackableObject<T>` already implements `IAdditionalPropertiesSupport<T>` unconditionally.

**Known follow-up, not fixed this pass**: the reused `NavPlace`/`Feature` types still default to
`BaseItem`'s `@id`/`@type`/Presentation-2.x-context shape rather than the unprefixed `id`/`type`
the spec's own Georeference Annotation body example shows — a pre-existing NavPlace-package-wide
convention question (its default context, id/type prefixing), not something specific to modeling
the Annotation wrapper this milestone was scoped to.

Tests: `GeoreferenceAnnotationTests.cs` (5 new — required context/type/motivation write shape, full
resource-target + transformation round trip, SpecificResource+SvgSelector write shape and round
trip, and a direct regression test locking in that `Transformation` is now callable on the
FeatureCollection body itself). Full suite: **220 tests, all passing**, 0 build warnings/errors
introduced.

### Milestone 18: DONE — remove dead AuthService.cs (Auth 0.x)

Deleted `Properties/Services/AuthService.cs` (Auth API 0.x, `@context: /auth/0/`) — confirmed
unreachable: not referenced by `ServiceJsonConverter`'s type-dispatch switch or its profile-based
fallback (only `AuthService1`/`AuthService2` are), not referenced by any test or example, and
predates the "there was never a Presentation 1.0" correction from §1 (same class of pre-session
leftover as the `Related.cs` dead code noted in §8, and consistent with Milestone 11 deleting the
old flat `AuthService2` for the same reason — genuinely unreachable code, not a legacy shape
needing preservation). Grep confirmed zero remaining references before deletion; full suite still
**220 tests, all passing** with no changes needed elsewhere.

Left as-is (informational only, not a defect): `ServiceJsonConverter`'s `AuthCookieService1`/
`AuthTokenService1`/`AuthLogoutService1` `@type` switch cases don't correspond to any `@type` value
Auth 1.0 actually defines (its typing is profile-driven per spec) — harmless "documentation-shaped
fiction" per finding 10, since the profile-based fallback immediately below is what actually
resolves Auth 1.0 services in practice, and removing the dead cases would change nothing observable.

### Milestone 19: DONE — final verification sweep + guide update

Final checks across the whole solution (all 6 projects: core library, 3 extension packages,
examples/Cookbook, tests):

- `dotnet build` (full solution, clean): **0 warnings, 0 errors**.
- `dotnet test`: **220 tests, all passing**.
- Coverage (coverlet + ReportGenerator, `coverage-report/SummaryGithub.md`): overall line coverage
  **72.3%** (2726/3766 lines), branch coverage 69.5% — up from the 63.5% baseline recorded at the
  end of Milestone 8, despite this pass adding a large volume of new, mostly-well-tested code
  (Content State, the 4 Auth 2.0 types + 3 responses, Discovery paging, Search responses, Image
  service completeness, Text Granularity, navPlace GeometryCollection, the Georeference wrapper).
  Lowest-covered new areas are mostly `[JsonConstructor]`-only classes' rarely-hit branches
  (`SearchAnnotationCollectionRef`, `AuthLogoutService2`) rather than untested surface area.

## Status: all 19 milestones (0-19) complete.

Summary of what changed since §§0-8 (the Presentation API core versioning work): the
`github.com/IIIF/awesome-iiif` comparison in §10 found the *surrounding* standards (Auth, Search,
Discovery, Content State, Image, plus the navPlace/Georeference/Text Granularity extensions) were
materially incomplete or, in a few cases, actively wrong (missing `type` on write, a fabricated
enum value, a conflated collection/page model). Milestones 9-18 closed each of those gaps with the
same discipline as the core work: 3.0-native-first modeling, `[JsonIgnore]`d computed views where a
type spans conventions, tests landing with every change, and — critically — several of the
"cross-cutting bugs" found in Milestone 9 turned out to have siblings only surfaced by later
milestones exercising the same code paths differently (the `@context` mis-rename found while fixing
Milestone 9's own `@type` bug; the `ServiceJsonConverter` recursion-guard gap found only once Auth
2.0 introduced genuinely nested polymorphic services in Milestone 11). No regressions were
introduced in any of the 9 milestones — every existing test continued passing throughout, and each
milestone's own new tests are additive.
