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

### Status of Milestones 0-19: complete (superseded by the final status at the end of this document)

## 11. Round 2: re-comparison against awesome-iiif

A second deep pass (research delegated to an agent, prompted with round 1's own findings so it
wouldn't re-report them) re-checked the same 6 APIs + 3 extensions against the current code,
independently sanity-checked round 1's explicitly-flagged "Known follow-up, not fixed this pass"
notes (legitimate round-2 candidates by definition), and confirmed no new standards/extensions
exist in awesome-iiif's Standards section or `IIIF/api`'s extension registry beyond what round 1
already covered. Found 5 concrete gaps, tracked as Milestones 20-24:

### Milestone 20: DONE — fix Behavior leaking into legacy V2.x JSON

`BaseNode.Behavior` was missing the `[JsonIgnore]` its Milestone-8 siblings (`Rights`/
`RequiredStatement`/`PartOf`) got — flagged as a suspected risk in §8's own follow-up note, now
confirmed by direct inspection: `IiifSerializer`'s 2.x write path is plain
`JsonConvert.SerializeObject(node, TrackableObject.JsonSerializerSettings)`, which walks the whole
object graph via reflection, so any `Manifest`/`Collection`/`Canvas` (any `BaseNode`) with
`Behavior` set leaked a spurious `"behavior"` key into 2.0/2.1 JSON that the 2.x spec doesn't define
at all.

Fixed by adding `[JsonIgnore]`, matching the established pattern exactly. This also surfaced a
second, narrower pre-existing gap while fixing the first: `WriteV3Canvas`/`WriteV3Range` (the V3
Canvas/Structure hand-rolled writers) never wrote `"behavior"` at all, unlike `WriteV3Manifest`/
`WriteV3Collection` which already did inline. Factored the duplicated Manifest/Collection
inline logic into shared `WriteV3Behavior`/`ReadV3Behavior` helpers (mirroring the existing
`WriteV3RightsRequiredStatementPartOf` pattern) and wired them into all four `BaseNode` writers/readers
(Manifest, Collection, Canvas, Range) uniformly, so Canvas/Range now support `behavior` in V3 output
for the first time.

Tests: `BehaviorLegacyLeakTests.cs` (6 new — Manifest/Collection no-leak-into-V2.1, Canvas no-leak
via plain `JsonConvert`, and full V3 round trips for Manifest/Canvas/Structure). Full suite: **226
tests, all passing**, 0 build warnings/errors introduced.

### Milestone 21: DONE — Content State PointSelector, plus a real correction to Milestone 10

Round 2's research flagged a missing `PointSelector` (AV deep-linking, spec §5.2). Fetching the
actual spec source (`source/content-state/1.0/index.md`) to model it precisely turned up something
bigger: **Milestone 10's `ContentStateFragmentSelector` doesn't correspond to anything in the real
spec.** `grep`-ing the entire spec document for `FragmentSelector` returns zero matches — Content
State 1.0's only documented region-targeting pattern (§5.1) is a plain Media Fragments suffix on the
target `id` itself (`"id": ".../canvas7#xywh=1000,2000,1000,2000"`), with `partOf` sitting directly
on that same object; `SpecificResource` only appears once in the whole spec, for the PointSelector
case. So Milestone 10 invented a wrapper shape (`{"type":"SpecificResource","source":...,
"selector":{"type":"FragmentSelector",...}}`) that no real Content State 1.0 consumer would produce
or expect - the same class of defect as finding 2's original complaint about `ContentStateService`.

Fixed by:
- Deleting `ContentStateFragmentSelector.cs` outright (no external consumers existed yet - added and
  removed within the same session, never released).
- Adding `ContentStatePointSelector` (new, spec-accurate: `{"type":"PointSelector","t":14.5}`) and
  renaming `ContentStateTarget.Selector` → `PointSelector` of that type.
- Fixing `ContentStateTargetJsonConverter`: the SpecificResource wrapper is now triggered *only* by
  `PointSelector` being set (not by `PartOfId`, which the old code incorrectly also treated as a
  wrapper trigger) - `partOf` now always lives on the resource-reference object itself, matching
  both of the spec's own examples (§5.1's bare target, §5.2's SpecificResource `source`), never on
  the wrapper.
- Region-targeting needs no new code at all - the existing bare-URI/typed-reference constructor
  already covers it; callers just include the `#xywh=...` fragment in the `id` they pass.

Tests: rewrote the affected cases in `ContentStateTests.cs` to match the corrected shapes (region
via fragment-on-id + direct `partOf`, PointSelector via the SpecificResource wrapper, codec round
trip using a time offset instead of a region). Full suite: **227 tests, all passing**, 0 build
warnings/errors introduced.

### Milestone 22: DONE — polymorphic dispatch for Annotation.Body

Added `BaseResourceJsonConverter` (`Shared/Content/Resources/`), attached to the `IBaseResource`
interface, dispatching on `@type`/`type` to `ImageResource`/`AudioResource`/`VideoResource`/
`EmbeddedContentResource`, falling back to plain `BaseResource` for anything unrecognized (e.g. a
`SegmentResource.Full` reference). Needed the identical recursion-guard `LeafContractResolver`
pattern Milestone 9 built for `ServiceJsonConverter` (suppress the converter for concrete leaf
types, keep it active for the bare `IBaseResource` interface type) — copied deliberately rather than
reinvented, since that exact bug (and fix) was already worked out and tested.

This closes the "Known follow-up" flagged in Milestone 13 and re-confirmed by round 2's independent
check: a standalone `Annotation` (e.g. a Content Search 2.0 result in `SearchResponse.Items`)
previously threw trying to instantiate the `IBaseResource` interface directly when round-tripped
through plain `JsonConvert`/`TrackableObject.Parse` rather than `IiifSerializer`'s hand-built V3
Canvas reader. `SearchResponseTests.cs`'s hit-highlighting test, which had explicitly worked around
this by not exercising `Items`, now does.

Building this surfaced one more independent, real bug: `EmbeddedContentResource`'s constructor
passed its own type-string literal as the *id* parameter (via `BaseResource`'s single-arg overload)
and misspelled it (`"cnt:ContextAsText"` instead of `"cnt:ContentAsText"`) - so the resource's
`@type` was never actually being set at all, and its `@id` was garbage. Fixed to pass an empty id
(matching the spec convention that an embedded literal text body has no dereferenceable `@id`) and
the correctly-spelled type.

Tests: `AnnotationBodyDispatchTests.cs` (5 new — Image/Audio/Video/EmbeddedContent body round trips
through plain `JsonConvert`/`TrackableObject.Parse`, plus a direct regression test confirming
`SearchResponse.Items` now round-trips too); `SearchResponseTests.cs` strengthened to actually
exercise `Items` in its round-trip test instead of working around the gap. Full suite: **232 tests,
all passing**, 0 build warnings/errors introduced.

### Milestone 23: DONE — switch navPlace Feature/NavPlace to unprefixed id/type

`extensions/IIIF.Manifest.Serializer.Net.NavPlace/{Feature.cs,NavPlace.cs}` inherited `BaseItem`
(`@id`/`@type`), flagged in Milestone 17's "Known follow-up" and re-confirmed by round 2's
independent check. Verified directly against the spec source
(`source/extension/navplace/index.md`): every example uses unprefixed `id`/`type` throughout,
confirming navPlace (like Search 2.0/Discovery 1.0/Auth 2.0, fixed the same way in Milestone 9)
postdates Presentation 3.0 with no 2.x form. Switched both classes from `BaseItem<T>` to
`UnprefixedBaseItem<T>` — a pure base-class swap requiring no call-site changes, since
`UnprefixedBaseItem`'s constructor overloads were deliberately built (Milestone 9) to mirror
`BaseItem`'s exactly. This also incidentally fixes `NavPlace`/`Feature`'s default `@context` (was
Presentation 2.x's URI via `BaseItem.DefaultContext`; now Presentation 3.0's, matching
`UnprefixedBaseItem.DefaultContext`) — spotted during round 2's own research pass while comparing
output against the spec's Georeference full-example.

Updated `GeoreferenceAnnotationTests.cs` (Milestone 17), which had a comment explicitly noting the
old `@type`-prefixed shape as a deferred limitation — now asserts the corrected unprefixed shape.

**Known follow-up, not fixed this pass**: embedded `Feature`/`NavPlace` instances still always write
some `@context` value (falling back to the class default when unset), whereas the spec's own
examples show no `@context` at all on an embedded FeatureCollection/Feature (only the top-level
Manifest carries `@context`, as a combined array). Suppressing `@context` entirely for the
common embedded case would need a per-class override hook `UnprefixedBaseItem` doesn't currently
expose; out of scope for a targeted id/type-shape fix.

Tests: `NavPlaceUnprefixedShapeTests.cs` (3 new — `NavPlace`/`Feature` write unprefixed id/type, full
round trip including nested `Geometry`/`FeatureProperties`). Full suite: **235 tests, all passing**,
0 build warnings/errors introduced.

### Milestone 24: DONE — remove dead Related.cs + final verification sweep

Deleted `Properties/Related.cs` (a `Related : FormattableItem<Related>` value type) — confirmed
unreferenced: `BaseNode.Related` has always been a plain computed `string?` view over `Homepage`
(see §8), never touching this type. Flagged as dead code in §8's own follow-up note and
re-confirmed by round 2's independent check; same class of pre-session/pre-round leftover as
`AuthService.cs` (Milestone 18).

Final checks across the whole solution (all 6 projects: core library, 3 extension packages,
examples/Cookbook, tests):

- `dotnet build` (full solution, clean): **0 warnings, 0 errors**.
- `dotnet test`: **235 tests, all passing**.
- Coverage (coverlet + ReportGenerator): overall line coverage **72.5%** (2751/3794 lines), branch
  coverage 69.8% — up slightly from Milestone 19's 72.3%, consistent with round 2 adding a modest
  amount of new, well-tested code (`ContentStatePointSelector`, `BaseResourceJsonConverter`) without
  materially changing the codebase's size.

Summary of what changed across both rounds: round 1 (§§0-9) reshaped the Presentation API core
around 3.0-native storage with computed legacy views for 2.x compatibility, then (§10) found the
*surrounding* standards (Auth, Search, Discovery, Content State, Image, plus the navPlace/
Georeference/Text Granularity extensions) materially incomplete or, in a few cases, actively wrong
(missing `type` on write, a fabricated enum value, a conflated collection/page model) and closed
each gap with the same discipline: 3.0-native-first modeling, `[JsonIgnore]`d computed views where a
type spans conventions, tests landing with every change. Round 2 (§11) re-verified round 1's own
work by fetching primary spec sources directly rather than trusting memory, which caught real
defects invented *during* round 1 itself — most notably Milestone 10's `ContentStateFragmentSelector`,
a wrapper shape that appears nowhere in the actual Content State 1.0 spec (fixed in Milestone 21) —
alongside independently-flagged "Known follow-up" items that were legitimate round-2 candidates by
round 1's own admission (Behavior's legacy-JSON leak, Annotation.Body's missing polymorphic
dispatch, navPlace's `@id`/`@type` shape). No regressions were introduced across either round —
every existing test kept passing throughout, and each milestone's own new tests are additive. The
recurring lesson across both rounds: several "cross-cutting" fixes turned out to have siblings only
surfaced once a *later* milestone exercised the same code path differently (the `@context`
mis-rename found while fixing Milestone 9's own `@type` bug; the `ServiceJsonConverter`
recursion-guard gap found only once Auth 2.0 introduced genuinely nested polymorphic services in
Milestone 11; `BaseResourceJsonConverter` needing the identical recursion-guard pattern in
Milestone 22) — a reason to keep verifying against primary sources and real round trips rather than
declaring a section "done" from reasoning alone.

## Round 3: IIIF Cookbook recipe fidelity (Milestones 1-10, this round's own numbering)

Scope: implement every real recipe in `github.com/IIIF/cookbook-recipes` (73 folders; 2 -
`0231-transcript-meta-recipe` and `0466-link-for-loading-manifest` - are prose-only with no
manifest JSON, leaving 71 real recipes) as a faithful C# builder, originally all in one file,
`examples/IIIF.Manifest.Serializer.Net.Cookbook/CookbookCatalog.cs` (later split into thematic
recipe-set files - see Round 4 below). `iiif.io/demos/` was checked twice (once on request) and
confirmed to be an external-links showcase page with no manifest content of its own - not a source
for examples.

**Groups A-H (Milestones 1-8): new SDK modeling required by at least one recipe**, found by
fetching every recipe's real JSON directly (not just its prose) and diffing against the SDK's
existing capability:

- **A** - `Annotation.Target`/`Manifest.Start` generalized from a bare string to a polymorphic
  `AnnotationTarget` (bare URI, typed reference, or SpecificResource+selector), plus four selector
  types (`FragmentSelector`, `PointSelector`, `ImageApiSelector`, `SvgSelector`) and a
  `SpecificResource` body wrapper. Implicit `string`→`AnnotationTarget` conversion kept every
  pre-existing call site compiling. Found and fixed two real Newtonsoft bugs here: a
  `[JsonConstructor]` parameter binding by wire-name to the wrong (plural) contract, and an
  unguarded null-token cast in the target converters (proactively fixed in three sibling
  converters, not just the one that failed).
- **B** - `TextualBody` (W3C inline text body: `type`/`value`/`format`/`language`, no `id`),
  distinct from the legacy `EmbeddedContentResource` (`cnt:ContentAsText`).
- **C** - `Choice` body type (`type:"Choice"`, `items:[...]`, always an array even with one item -
  given its own `ChoiceJsonConverter` rather than the collapsing `ObjectArrayJsonConverter` used
  elsewhere). Required by both language alternatives (0346) and format/quality alternatives (0434) -
  same shape, different motivations.
- **D** - `Annotation.TimeMode` (trim/scale/loop) - a genuine spec property with no recipe
  exercising it directly, added anyway since "compare against the spec" was the mandate for the
  awesome-iiif pass and applies equally here.
- **E** - `Canvas.PlaceholderCanvas` (a full embedded Canvas). Found `Manifest.PlaceholderCanvas`
  already existed but was modeled as a bare string tagged `2.0` - both wrong (3.0-only, and always
  a full Canvas object per spec) - fixed alongside adding the Canvas-level property.
- **F** - `AnnotationCollection` (W3C paged list: `total`/`first`/`last`) - a standalone top-level
  document type, distinct from both the IIIF `Collection` resource and `AnnotationPage`. Given its
  own `Serialize`/`DeserializeAnnotationCollection` entry points mirroring `Collection`'s.
- **G** - `AnnotationPage.PartOf`/`Next`/`Prev` (paging linkage back to an `AnnotationCollection`) -
  `PartOf` already existed generically on every `BaseNode<T>` but was never wired into the
  hand-rolled Canvas writer/reader for `AnnotationPage` references specifically.
- **H** - navPlace's `Feature` made to implement `IBaseResource` so a bare GeoJSON Feature can
  stand in as an Annotation body (0139) - distinct from navPlace's usual Manifest/Canvas-level
  property. Since core cannot reference the navPlace extension assembly, added a
  `ResourceTypeRegistry` (extension types self-register their type-name from a static constructor)
  that both `BaseResourceJsonConverter` and `IiifSerializer`'s hand-rolled reader consult as a
  fallback.
- **Group I** (found only while writing the catalog, not in the original gap analysis) -
  `Annotation.Body` also needed array support (multiple sibling bodies, not `Choice`'s mutually
  exclusive alternatives - recipes 0022/0103/0258/0377), added via the identical
  `Bodies`+computed-`Body` pattern already used for `Targets`/`Target`.

**Milestone 9: the 71-recipe catalog itself**, built from real JSON fetched directly (six parallel
research passes covering all 71 recipes, cross-checked against a handful fetched by hand earlier in
the session) rather than from recipe prose alone. Writing the catalog surfaced a second wave of
gaps - all in `IiifSerializer`'s hand-rolled V3 reader/writer, not in the data model itself - found
by literally dumping the SDK's own V3 output for several recipes and diffing against the real JSON,
the same discipline as round 2:

- `Summary` (3.0's rename of 2.x `description`) **did not exist as a class member at all** -
  discovered only because recipe 0006 needed it. Added as the 3.0-native storage on `BaseNode<T>`,
  with `Description` becoming a computed legacy view (a straight rename, unlike
  `Attribution`→`RequiredStatement`'s restructuring).
- `Metadata`, `Thumbnail`, `Rendering`, `Homepage`, `SeeAlso`, `Provider` were **all completely
  unwired from `IiifSerializer`** - real properties existed on `BaseNode<T>` and could be set via
  fluent methods, but the hand-rolled V3 Manifest/Collection/Canvas/Range writer and reader never
  touched any of them, so every one of these properties was silently dropped on any V3 round trip.
  Wired generically (`WriteV3NodeExtras`/`ReadV3NodeExtras`) for all four `BaseNode` types;
  `Provider` wired separately since it's Manifest/Collection-only per spec. `Thumbnail`/`Logo` also
  gained `Height`/`Width` (`IDimensionSupport`), and `SeeAlso` gained `Profile`/`Label`/`Type`.
- `Label` **had no per-entry language support at all** (only `Description`/`MetadataValue` did) -
  discovered when recipes 0006/0010/0011's multi-language labels couldn't be expressed. Added
  `Label.Language` (inert on `Label`'s own `ValuableItemJsonConverter`, consulted only by
  `IiifSerializer`'s own language-map-grouping helpers) and taught every `label`-writing call site
  to group by language instead of dumping everything under `"none"`.
- An embedded per-resource `service` (e.g. an Image API service on a painting body) wrote with raw
  `@id`/`@context`/`@type` and collapsed to a bare object instead of an array when there was only
  one - wrong on both counts per every real recipe with an image service (the vast majority of the
  catalog). Fixed by rebuilding `service` explicitly in `WriteV3Resource` via a new
  `IBaseResource.Service` default-interface member (so every existing resource type gets it for
  free) and a `WriteV3EmbeddedResourceService` variant that strips `@context` - kept distinct from
  the pre-existing `WriteV3Service` used for `Manifest.Services`, which an existing test asserts
  *does* keep its own `@context`. The read side had the same bug in the opposite direction: `service`
  was never read back into the body resource at all.
- `Canvas.Annotations` (secondary/commenting/tagging AnnotationPages) **always wrote as a bare
  `{id,type}` stub**, even when the `AnnotationPage` object had items added to it - silently
  dropping the actual annotation content for roughly a third of the catalog (every recipe with a
  secondary tagging/commenting/supplementing page: 0019, 0021, 0045, 0074, 0103, 0135, 0139, 0219,
  0258, 0261, 0266, 0326, 0346, 0377, 0464, and others). Only a couple of recipes (0269, 0306)
  deliberately use the external-reference form - the rest embed fully. Fixed by having
  `WriteV3AnnotationPageReference` check whether the page has items and embed
  (`WriteV3AnnotationPage`-shaped) instead of stubbing when it does; the reader gained the
  symmetric `items` parsing.

None of these were caught by the existing test suite before this round, because no prior test
built a Manifest with `Summary`/`Metadata`/`Thumbnail`/etc. and then round-tripped it through
`IiifSerializer` (as opposed to plain `JsonConvert`) - the gap was invisible until the cookbook
catalog started exercising exactly that path at scale.

**Milestone 10**: final sweep. Full solution build (6 projects): 0 warnings, 0 errors. Full test
suite: 329 tests, all passing, including a dedicated round-trip test per new SDK feature and an
automatic per-catalog-entry round-trip test for all 78 `CookbookCatalog` entries (71 recipe numbers;
a few recipes with two structurally distinct manifests, or a manifest plus a standalone
AnnotationCollection/ContentState document, contribute more than one catalog entry). Spot-checked
several catalog entries' actual `IiifSerializer` output against the real recipe JSON directly
(not just "does it round-trip without throwing") to catch the `service`/`Canvas.Annotations` gaps
above - a reminder that a passing round-trip test proves internal consistency, not fidelity to an
external reference document.

## Round 4: structural refactor - file-size reduction, Facade/Strategy/Registry patterns

Scope: two files had grown past ~1400 lines over the course of Round 3 (`IiifSerializer.cs` from
the V3 read/write logic, `CookbookCatalog.cs` from the 71-recipe catalog). Both were mechanically
split with **zero behavior change** - every existing test passed before and after each split, no
logic was altered, only relocated.

- **`src/IIIF.Manifest.Serializer.Net/IiifSerializer.cs`** (1461 → 126 lines): now holds only the
  public `Serialize`/`Deserialize` overloads and 2.x/3.0 version dispatch - a thin **Facade** over
  the internal per-type V3 read/write logic, which moved into 13 sibling `partial class` files, one
  per IIIF resource-type responsibility: `IiifSerializer.Manifest.cs`, `.Collection.cs`,
  `.AnnotationCollection.cs`, `.Canvas.cs`, `.Range.cs`, `.Annotation.cs` (the largest remaining
  file at 366 lines, by design - the most tightly-coupled recursive body-dispatch logic),
  `.NodeExtras.cs` (rights/requiredStatement/partOf/summary/metadata/thumbnail/rendering/homepage/
  seeAlso/behavior, generic across Manifest/Collection/Canvas/Range), `.Metadata.cs`,
  `.ImageLikeResources.cs` (thumbnail/logo), `.LinkResources.cs` (rendering/homepage/seeAlso),
  `.Provider.cs` (Manifest/Collection-only per spec), `.Service.cs`, `.Helpers.cs` (language-map/
  label/description shared parsing). All still `internal`/`private static`; nothing in the public
  API surface moved.
- **`examples/IIIF.Manifest.Serializer.Net.Cookbook/CookbookCatalog.cs`** (1745 → 44 lines):
  redesigned around **Strategy + Registry** rather than split as one flat file, since a demo
  catalog is a natural fit for real polymorphism (unlike `IiifSerializer`, deliberately left a
  plain Facade - see the reasoning in this round's own retrospective below). New layout:
  - `IRecipeSet` (Strategy interface): `IEnumerable<ExampleDefinition> GetRecipes()`.
  - Nine `internal sealed class ...Recipes : IRecipeSet` files, one per thematic slice of the 78
    catalog entries (140-245 lines each): `FoundationRecipes` (0001-0011), `CanvasAndStructureRecipes`
    (0013-0026), `CollectionAndChoiceRecipes` (0027-0033), `MediaVariationRecipes` (0035-0045),
    `LinkingAndOperaRecipes` (0046-0103), `DescriptivePropertiesRecipes` (0117-0229),
    `ProviderAndTaggingRecipes` (0230-0283), `AnnotationCollectionRecipes` (0299-0346),
    `AdvancedCompositionRecipes` (0377-0599).
  - `RecipeBuilders.cs`: the shared construction helpers every recipe set calls into (`Id`,
    `CanvasId`, `NewManifest`/`NewCanvas` overloads, `PaintingImage`, `WithSupplementaryPage`, plus
    the `Base`/`ImageService3`/Gottingen-fixture constants). A handful of narrower helpers used by
    only one or two recipes within a single group (`NewVolumeManifest`, `AddUclaPlaybillCanvases`,
    `NewNewspaperIssue`) stayed private inside their owning recipe-set file instead, rather than
    being pulled into the shared class - they aren't shared beyond that group.
  - `CookbookCatalog.cs` itself is now a thin **Registry**: a fixed `IReadOnlyList<IRecipeSet>` of
    the nine strategies, aggregated via `RecipeSets.SelectMany(set => set.GetRecipes()).ToList()`.
  - The public `ExampleDefinition` record and `CookbookCatalog.GetAll(): IReadOnlyList<ExampleDefinition>`
    signature are unchanged - `ExampleCatalogTests.cs` needed no modification.

**Why `IiifSerializer` got a Facade split but not a Strategy rewrite**: it's security-critical,
heavily-tested production code (329 dependent tests) built around one closed set of IIIF resource
types the SDK itself defines - there's no varying "algorithm" to select between at runtime, so
introducing `IV3NodeWriter<T>`/`IV3NodeReader<T>`-style polymorphism there would be an abstraction
with no behavioral payoff, violating the "don't add abstractions beyond what the task requires"
principle. `CookbookCatalog.cs`, by contrast, is example/demo code where "which group of recipes to
build" genuinely is a set of interchangeable strategies, and a Registry aggregating them reads more
naturally than one giant switch or a single flat list.

Verification after each split: full solution build (`dotnet build` on the affected project) - 0
warnings, 0 errors both times; full test suite (`dotnet test`) - all 329 tests passing both times,
including the automatic per-catalog-entry round-trip test for all 78 `CookbookCatalog` entries from
Round 3's Milestone 10, confirming the Strategy/Registry rewrite preserved every recipe's output
byte-for-byte.

## Round 5: System.Text.Json interop for the 4 top-level document types

Scope: the whole model only ever worked correctly through **Newtonsoft.Json** - every custom
converter (`BaseNodeJsonConverter`, `ServiceJsonConverter`, `AnnotationTargetJsonConverter`,
`ObjectArrayJsonConverter`, `SelectorJsonConverter`, `ChoiceJsonConverter`,
`ContentStateTargetJsonConverter`, etc.) and `IiifSerializer`'s hand-rolled 3.0 writer are
Newtonsoft-specific. A developer who serializes/deserializes a `Manifest` (or `Collection`/
`AnnotationCollection`/`ContentState`) directly with **System.Text.Json** instead - explicitly, or
implicitly via ASP.NET Core's default request/response (de)serialization - got whatever
System.Text.Json's own reflection-based default produced against types that carry none of its
attributes: silently wrong output, not an error.

Considered and rejected: reimplementing every one of the ~15 custom Newtonsoft converters a second
time for System.Text.Json, so any individual type could be serialized standalone with either
library. Large surface area for no realistic benefit - nested types (`Canvas`, `Service`,
selectors, ...) are never serialized standalone in practice, only ever reached inside one of the 4
top-level documents' own JSON tree - and an ongoing burden of keeping two serializer backends in
sync as the model evolves.

**Decided**: a small **bridging** `System.Text.Json.Serialization.JsonConverter<T>` per top-level
document type (`src/IIIF.Manifest.Serializer.Net/SystemTextJson/`:
`ManifestSystemTextJsonConverter`, `CollectionSystemTextJsonConverter`,
`AnnotationCollectionSystemTextJsonConverter`, `ContentStateSystemTextJsonConverter`). Each just
parses the incoming `Utf8JsonReader`/writes to the `Utf8JsonWriter` via `JsonDocument`, and
delegates the actual read/write work entirely to this SDK's existing logic - `IiifSerializer.
Serialize`/`DeserializeManifest`/`DeserializeCollection`/`DeserializeAnnotationCollection` for the
first three, and `TrackableObject.Parse<ContentState>`/`ContentState.Serialize()` for the fourth
(Content State has no legacy shape, so it was never routed through `IiifSerializer`'s version
dispatch to begin with). Each converter is applied directly on its type via
`[System.Text.Json.Serialization.JsonConverter(typeof(...))]` - fully qualified at the attribute
site rather than via a `using` directive, since every one of these files already has an unqualified
`using Newtonsoft.Json;` and `JsonConverterAttribute` exists in both namespaces. This means
`System.Text.Json.JsonSerializer.Serialize(manifest)`/`Deserialize<Manifest>(json)` (and ASP.NET
Core defaulting to System.Text.Json) now produce/accept exactly the same JSON as calling
`IiifSerializer` directly, with zero configuration required from the consumer - there is exactly
one source of truth for how each document type reads and writes IIIF JSON, regardless of which
library a consumer's application uses elsewhere.

Required adding a `System.Text.Json` package reference to the core SDK project (previously
Newtonsoft-only); no other project needed changes.

Tests: `SystemTextJsonInteropTests.cs` (7 new - each of the 4 types' System.Text.Json output
matches `IiifSerializer`'s/`Serialize()`'s Newtonsoft output byte-for-byte via `JToken` structural
comparison, round-trip through System.Text.Json for all 4 types, plus a legacy-2.1-JSON-in →
System.Text.Json-`Deserialize<Manifest>` round trip confirming the read-side bridge also
auto-detects version correctly). Full suite: **336 tests, all passing**, 0 build warnings/errors
introduced.

## Round 6: harden version detection - Metadata API 1.0 and Presentation 4.0 RC tracking

Scope (`github.com/KiarashMinoo/IIIF.Manifest.Serializer.Net` issue #5, "SDK Phase 1"): the version
detector and `IiifPresentationVersion` enum only recognized 2.0/2.1/3.0 - anything else (a Metadata
API 1.0 "Shared Canvas" document, a draft Presentation 4.0 document, or a genuinely ambiguous/
invalid payload) fell into an undifferentiated `Unknown`, and there was no enum value to even
represent "detected, but not a version this SDK writes."

Researched directly against the live spec pages (not assumed) before making any enum/detector
change:

- **Presentation 2.0 vs 2.1 share one `@context`.** Confirmed via `iiif.io/api/presentation/2.1/`
  and a direct fetch of `iiif.io/api/presentation/{2.0,2.1}/context.json` (both 404 - only the
  unversioned `iiif.io/api/presentation/2/context.json` exists, HTTP 200). There is no version
  field or structural difference between a 2.0 and a 2.1 document either. This means `V2_0` was
  never actually reachable by the existing detector in practice - kept as an explicit enum value
  (for callers who want to *write* 2.0 specifically) and as a defensive fallback for a
  non-conformant `.../presentation/2.0/context.json` some tooling might emit, but real-world 2.x
  documents are - correctly and deliberately - always detected as `V2_1`.
- **Metadata API 1.0** (`@context: http://www.shared-canvas.org/ns/context.json`, confirmed via
  `iiif.io/api/metadata/1.0/`) is the "Shared Canvas" predecessor to Presentation 2.0 -
  structurally near-identical (`sc:Manifest`, `sequences`/`canvases`, `@id`/`@type`), so it's only
  reliably distinguished by this one context URL; without it, a Metadata 1.0 document is
  indistinguishable from - and correctly falls back to - `V2_1`.
- **Presentation 4.0** (`@context: http://iiif.io/api/presentation/4/context.json`, confirmed via
  `iiif.io/api/presentation/4.0/`) is still `4.0.0-draft` as of this research, not a stable spec.

Added `IiifPresentationVersion.Metadata_1_0` and `IiifPresentationVersion.V4_0_Rc` (inserted
between `Unknown` and `V2_0`/after `V3_0` respectively - safe, since every existing reference to
this enum in the codebase is by name, never by numeric/ordinal value, confirmed by grep before
reordering). `IiifPresentationVersionDetector` now recognizes both via `@context` (checked before
the existing 2.x/3.0 checks), and its `Detect(JToken)` overload gained an explicit doc comment
spelling out the full priority order (context → legacy 2.x structural signals → 3.0 structural
signals → `Unknown`) so a mixed/ambiguous document's outcome is documented, not just incidentally
deterministic.

`IiifSerializer.Serialize(Manifest/Collection, options)` already had an exhaustive `switch` with a
`_ => throw new NotSupportedException(...)` default arm from earlier work - so once the two new
enum values existed, requesting `Metadata_1_0` or `V4_0_Rc` as a *write* target immediately, correctly
threw `NotSupportedException` with zero additional code change (verified with tests, not assumed).
`DeserializeManifest`/`DeserializeCollection`'s default arm previously collapsed every non-2.x/3.0
outcome into the same generic `JsonSerializationException("Could not detect...")`, which became
misleading once the detector could genuinely detect (just not import) `Metadata_1_0`/`V4_0_Rc` - split
into `Unknown => JsonSerializationException` (unchanged, still means "couldn't tell") vs.
`_ => NotSupportedException($"Detected IIIF version '{version}', but this SDK does not import
it...")` (new, means "detected fine, but there's no importer for it yet") - a real, user-facing
improvement to error message accuracy, not just an internal detail.

Deliberately out of scope (per the issue): full Metadata 1.0 import normalization, full 4.0
conformance/writer support, and validation against external IIIF validators - this round is
detection/classification and clear-failure-mode only.

Tests: 16 new (`IiifPresentationVersionDetectorTests.cs` - Metadata 1.0 context, Presentation 4.0
context + context-array, the non-standard 2.0-only context defensive fallback, the real-world
2.0-collapses-to-2.1 case explicitly documented as such, a mixed-signal ambiguous payload, and
expanded non-object/invalid coverage: whitespace-only, `null`, `true`, a bare number, a bare
string; `IiifSerializerTests.cs`/`CollectionReshapeTests.cs` - explicit V2_0 write path produces
byte-identical output to V2_1, `NotSupportedException` for both new enum values on both `Serialize`
overloads, and the corresponding `DeserializeManifest` case). Full suite: **409 tests, all
passing**, 0 build warnings/errors introduced.

## Round 7: audit legacy-import normalization (SDK Phase 2)

Scope (issue #6, "SDK Phase 2"): verify that Presentation 2.0/2.1 legacy documents fully normalize
into the 3.0-native canonical model - `description`→`summary`, `attribution`→`requiredStatement`,
`license`→`rights`, `within`→`partOf`, `related`→`homepage`, `viewingHint`→`behavior`,
`sequences`/`canvases`/`images`→`items`/`AnnotationPage`/`Annotation` - and that unknown properties,
partial/minimal documents, and both round-trip directions (legacy→3.0, legacy→legacy) all behave
correctly and tolerantly.

Given how much of this was already built across Round 1's Milestones 1-8 (the entire "reshape
around 3.0-native storage" effort), this was primarily an **audit** - read every relevant model
file and existing test before assuming anything was missing, rather than reimplementing already-
correct behavior. Result: every mapping the issue asks for was already correctly implemented and
already had at least per-property test coverage (`BaseNodeReshapeTests.cs`,
`ManifestSequenceReshapeTests.cs`, `CanvasReshapeTests.cs`, `BehaviorLegacyLeakTests.cs`) - except
one real, concrete bug and several real test-coverage gaps:

- **Real bug**: `BaseNode.AddDescription`/`RemoveDescription` were the only legacy mutators in the
  whole `Description`/`Attribution`/`License`/`Within`/`Related` family *not* tagged
  `[Obsolete(error: true)]` - simply missed during Milestone 8's "consistency sweep" (confirmed:
  the sweep's own reflection test, `BaseNodeReshapeTests.LegacyMutators_Should_
  BeMarkedObsoleteAsCompileErrors`, never included them in its `[InlineData]` list either). Fixed,
  and the one existing call site (`NodeExtrasTests.cs`) was rewritten to set the modern `Summary`
  property and assert the legacy `Description` getter reflects it - the same pattern every other
  legacy-view test in this codebase already uses, rather than a `#pragma warning disable` escape
  hatch that would have been the only one of its kind.
- **Test gaps, not code gaps** (the underlying behavior already worked correctly for all of these -
  confirmed by writing the test and watching it pass on the first try, not by fixing anything):
  a direct `description`-JSON-in→`Summary` test (the other four legacy properties each already had
  one; `Description` didn't); the issue's own literal attribution/license and nested sequence/
  canvas/image acceptance examples, run end-to-end as their own tests rather than only exercised
  piecemeal across other files; explicit legacy→3.0 and legacy→legacy round-trip tests combining
  multiple mapped properties in one document; a generic (non-extension-specific) unknown-property
  preservation test through the legacy `JsonConvert` path; tolerant-parsing tests for a
  minimal-required-fields-only document and a canvas with no `images` at all; and a `viewingHint`→
  `behavior` write-time mapping test (the fallback itself - `WriteV3Behavior` falling back to
  `ViewingHint` when `Behavior` is empty - was already implemented, just untested in isolation).

**Known gap, found but explicitly deferred, not fixed**: `ServiceJsonConverter.
DetectAndDeserializeService`'s final fallback silently returns `null` (dropping the service
entirely) when a service's `@type`/`type` is unrecognized, its `profile` doesn't match any known
keyword, *and* it doesn't parse as a generic Image service either. This means "unknown service
types should be preserved when possible" (this issue's own wording) does not currently hold - a
genuinely unrecognized service is lost, not preserved. Fixing this properly needs a new
`IBaseService`-implementing fallback type that stores the raw service JSON verbatim (mirroring how
unknown *properties* are already preserved via `[JsonExtensionData]`/`ElementDescriptor`'s
`IsAdditional` flag) - a real new type with its own read/write/round-trip tests, materially larger
than the rest of this round's audit-and-testing scope, and not covered by any of the issue's own
concrete `Tests` section examples (only "unknown-property preservation," not "unknown service
preservation," is listed there). Left as a follow-up rather than attempted partially here.

Tests: 9 new (`LegacyImportNormalizationTests.cs`) plus 2 theory-list corrections in
`BaseNodeReshapeTests.cs` (`AddDescription`/`RemoveDescription` added to the obsolete-mutator
check; `Description`/`SetSummary`/`AddSummary`/`RemoveSummary` added to the not-obsolete checks).
Full suite: **424 unit tests + 8 architecture tests, all passing**, 0 build warnings/errors
introduced.

## Round 8: decorate obsolete members with IIIFVersionAttribute metadata

Scope (ad hoc request, no associated issue): every `[Obsolete]`-tagged member across the SDK -
27 legacy mutators (`Add*`/`Remove*`/`Set*`) spread across `Canvas`, `Collection`, `Manifest`,
`Structure`, and `BaseNode<TBaseNode>` - carried no `IIIFVersionAttribute`-derived attribute at all,
unlike their sibling legacy *getter* properties, which have consistently carried
`[PresentationAPI(minVersion, maxVersion, IsDeprecated = true, DeprecatedInVersion = "3.0",
ReplacedBy = "...")]` since Round 1. This meant reflection-based tooling (docs generation, API
diffing, IDE tooltips reading the attribute rather than the free-text `Obsolete` message) had no
structured way to see *what* a deprecated mutator was replaced by.

Fixed by adding `[PresentationAPI(...)]` to all 27 mutators, each one mirroring the exact
`MinVersion`/`MaxVersion`/`DeprecatedInVersion`/`ReplacedBy` values already present on its
corresponding legacy getter (e.g. `Canvas.AddImage` mirrors `Canvas.Images`'s
`[PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy =
"items")]`). `BaseNode.SetViewingHint` mirrors `BaseNode.ViewingHint`'s existing tag exactly, per
the reference-implementation pattern.

**Adjacent gap found and fixed in the same pass**: `Canvas.Audios` and `Canvas.Videos` - legacy
computed views structurally identical to `Canvas.Images` right next to them - had no
`IIIFVersionAttribute` at all (not even on the getter), unlike `Images`. Added the matching
`[PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy =
"items")]` to both.

Tests: 30 new (`LegacyMutators_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation` theories
added to `CanvasReshapeTests.cs`, `CollectionReshapeTests.cs`, `ManifestSequenceReshapeTests.cs`,
`StructureReshapeTests.cs`, `BaseNodeReshapeTests.cs`, mirroring each file's existing
`LegacyMutators_Should_BeMarkedObsoleteAsCompileErrors` theory list; plus
`LegacyGetters_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation` in
`CanvasReshapeTests.cs` covering `Images`/`Audios`/`Videos`). Full suite: **454 unit tests + 8
architecture tests, all passing**, 0 build warnings/errors introduced.

## Round 9: downgrade legacy mutators from compile-error to compile-warning

Scope (ad hoc request, no associated issue): reverse part of the Round 1-8 "compile-time wall"
design (§4/mandate item 4 in `CLAUDE.md`). Every legacy mutator (`AddImage`, `AddSequence`,
`SetLicense`, `AddAttribution`, ... - the same 26 members touched in Round 8, all of `BaseNode<T>`'s
family except `SetViewingHint`, which was already warning-level and untouched) was
`[Obsolete("...", error: true)]`, meaning any code - old or new - calling one of them failed to
compile. The user asked for this to stop: legacy mutators should keep working, not error, since
every one of them already internally forwards to the current 3.0-native API under the hood (e.g.
`Canvas.AddImage` builds an `Annotation` and routes it through the same `AddAnnotationCore` path
`AddAnnotation` uses; `Manifest.AddSequence` funnels through `ReplaceFromLegacySequences` into
`Items`).

Changed: dropped `error: true` from all 26 `[Obsolete(...)]` mutator attributes across `Canvas.cs`,
`Collection.cs`, `Manifest.cs`, `Structure.cs`, `Shared/BaseNode.cs` - they now behave exactly like
the pre-existing `SetViewingHint` (a compiler warning naming the replacement, not a build break).
The `[PresentationAPI(...)]` metadata added in Round 8 is untouched - `IsDeprecated`/
`DeprecatedInVersion`/`ReplacedBy` still document the same deprecation, just no longer enforced at
compile time. `CLAUDE.md`'s mandate section and "Conventions to follow" were updated to match (no
longer describes a compile-time wall as the end state).

Tests: the 5 `LegacyMutators_Should_BeMarkedObsoleteAsCompileErrors` theories (one per
`*ReshapeTests.cs` file, asserting `IsError == true`) were renamed to
`LegacyMutators_Should_BeMarkedObsoleteAsWarnings` and their assertions flipped to `IsError ==
false`. No other test changes were needed - the Round 8 `IIIFVersionAttribute`-coverage theories
assert on `[PresentationAPI]`, not `[Obsolete]`, so they were unaffected by the severity change.
Full suite: **454 unit tests + 8 architecture tests, all passing**, 0 build warnings/errors
introduced (no call site in this repo's own source/tests/examples/cookbook actually calls a legacy
mutator directly, so the warning doesn't surface anywhere in-tree - only external consumers will see
it).

## Round 10: versioned-writer audit (issue #7)

Scope (issue #7, "SDK Phase 3"): verify the 3.0/2.1/2.0 writers are deterministic and complete
against the issue's own acceptance criteria - default-to-3.0, explicit-version output shapes,
no cross-version property leakage, clear exceptions for unsupported versions, and documented
downgrade limitations for 3.0-only data.

Like Round 7, this was primarily an **audit**: the version-aware writer architecture (built across
Rounds 1-6) already satisfied 8 of the issue's 10 checklist items outright -
`Serialize(resource)` defaulting to 3.0, correct `id`/`type` vs `@id`/`@type` and property-name
shapes for both directions, `NotSupportedException` for `Metadata_1_0`/`V4_0_Rc`/any future enum
value on both `Serialize` overloads, and V2_0/V2_1 being intentionally wire-identical (confirmed
against the live spec: no version field, no structural difference between 2.0 and 2.1).

**One real bug found and fixed**: `BaseNode.ViewingHint`'s getter read only its own independent
storage, never falling back to `Behavior` - the reverse of the already-correct `WriteV3Behavior`
fallback (`viewingHint` → `behavior` when writing 3.0). This meant a `Manifest` built purely via
the 3.0-native `AddBehavior(...)` fluent API - never calling the obsolete `SetViewingHint` - lost
its behavior entirely when written as `V2_1`/`V2_0`: `viewingHint` was simply absent from the
output, a genuine, silent downgrade data-loss bug, and exactly the "behavior -> viewingHint where
safe" mapping the issue's own body calls out by name. Fixed by adding
`ComputeViewingHintFromBehavior()` to `BaseNode<TBaseNode>` (`Shared/BaseNode.cs`): the `ViewingHint`
getter now falls back to the first `Behavior` value that is also spec-valid as a `ViewingHint`
(`paged`, `continuous`, `individuals`, `facing-pages`, `non-paged`, `multi-part`) when its own
storage is empty. Behavior-only values (`unordered`, `sequence`, `auto-advance`, `hidden`, ...) are
intentionally left unconverted - per the issue's own "otherwise omit" rule - rather than picking an
incorrect value. This only ever *adds* a value where none existed before (falls back only when the
independent storage is null), so no existing test's expectations changed.

**Test gaps, not code gaps** (confirmed correct on first run, not fixed):
- No explicit `V2_0` test existed for `Collection` (only `Manifest` had one) -
  `IiifSerializer_Should_WriteTheSameLegacyShape_When_CollectionVersionIsV2_0` added.
- No test asserted that a legacy `V2_1` Manifest write excludes 3.0-only properties
  (`rights`/`requiredStatement`/`partOf`/`items`) - only the Canvas-level "no items" case was
  covered - `Serialize_Should_ExcludeV3OnlyProperties_FromLegacyV2Manifest` added.
- No negative test existed for `DeserializeCollection` with a detected-but-unimportable version
  (only `DeserializeManifest` had one) - `DeserializeCollection_Should_ThrowNotSupported_When_
  VersionIsDetectedButUnimportable` added.

**Documentation gap fixed**: added a "Downgrade limitations" subsection to `docs/README.md` (under
"Multi-version serialization") enumerating exactly which 3.0-only properties convert safely,
which are omitted, and why - satisfying the issue's "downgrade limitations are documented"
acceptance criterion, which had no prior dedicated section.

Tests: 7 new (`SerializeV2_Should_MapBehaviorToViewingHint_When_ValueIsSharedBetweenBoth`,
`SerializeV2_Should_OmitViewingHint_When_BehaviorHasNo2xEquivalent` in
`ObsoleteCompatibilityTests.cs`; `IiifSerializer_Should_WriteTheSameLegacyShape_When_
CollectionVersionIsV2_0`, `DeserializeCollection_Should_ThrowNotSupported_When_
VersionIsDetectedButUnimportable` in `CollectionReshapeTests.cs`;
`Serialize_Should_ExcludeV3OnlyProperties_FromLegacyV2Manifest` in `IiifSerializerTests.cs`). Full
suite: **459 unit tests + 8 architecture tests, all passing**, 0 build warnings/errors introduced.

## Round 11: auxiliary API surface audit (issue #8)

Scope (issue #8, "SDK Phase 4"): review and complete models/serializers/tests for the non-core
Presentation API families - Image API service descriptors, Auth (1.0 legacy + 2.0 flow), Content
Search, Change Discovery, and Content State - against the issue's acceptance criteria: a
documented coverage table per family, read/write tests per family, standalone-and-embedded both
working where applicable, unsupported/partial areas explicitly listed, and mixed-version scenarios
documented.

Audited all five families in parallel (one focused pass each, citing file:line for every claim).
Verdict: the models themselves were already thorough - every field the issue lists by name
(`profile`/`protocol`/`tiles`/`sizes`/... for Image; all 4 Auth 2.0 service types plus response
payloads; search/autocomplete descriptors plus the actual `SearchResponse`/`TermPageResponse`
result shapes, not just descriptors; `OrderedCollection`/`OrderedCollectionPage`/`Activity` with
actor/object/target/dataset/rights/paging; Content State's annotation shape, point selector, and
`iiif-content` codec) was already modeled and had at least one read/write test. What was
genuinely missing:

**One real code gap, fixed**: `Service` (the Image API descriptor) had `ToInfoJson()` to write a
standalone `info.json` document but no inverse - no way to *parse* one back into a `Service`. The
issue's own acceptance criteria explicitly requires "standalone payloads ... work" (not just write),
and its own example shows a raw `info.json` document as input. Added `Service.FromInfoJson(string)`
(`Properties/Services/Service.cs`): renames `id`/`type` to `@id`/`@type` (the inverse of
`ToInfoJson`'s rename) and delegates to the same `TrackableObject.Parse<Service>` path
`IiifSerializer` already trusts for embedded services, so both shapes stay backed by one
deserialization path rather than a second hand-rolled one.

**Test gaps, not code gaps** (all confirmed correct on first run):
- No test exercised an embedded service specifically on an `ImageResource` (only Manifest-level
  embedding was tested) or the issue's explicitly-named "mixed Presentation 3 + Image 2" scenario -
  `ServiceRoundTripTests.ImageService_Should_RoundTripWhenEmbeddedOnAnImageResource_
  ThroughIiifSerializer` added (a P3 Manifest/Canvas/Annotation whose `ImageResource` body carries
  an `ImageService2`-typed embedded service, round-tripped through `IiifSerializer`).
- No negative/malformed-payload test existed for **any** of the five families, despite the issue's
  own "Tests" section explicitly asking for them. Added one per family:
  `UnrecognizedServiceType_Should_BeSilentlyDropped_NotThrow` (the shared `ServiceJsonConverter`
  fallback - confirmed it already silently drops unrecognized service shapes rather than faulting
  the whole document, covering Image/Auth/Search/Discovery/ContentState at once since they share
  this converter); `AuthProbeService2_Parse_Should_Throw_When_JsonIsMalformed`;
  `SearchResponse_Parse_Should_Throw_When_JsonIsMalformed`;
  `DiscoveryCollectionPage_Parse_Should_Throw_When_JsonIsMalformed`;
  `ContentStateCodec_Decode_Should_ThrowFormatException_When_LengthIsInvalid` and
  `..._When_ContentIsGarbled`.
- `Activity.Type` is a plain string (no closed enum) but only `Update`/`Move`/`Create` had
  dedicated fixtures - added a `[Theory]` covering `Add`/`Remove`/`Delete` too
  (`Activity_Should_RoundTripAdditionalActivityTypes`).

**Documentation gap fixed**: added an "Auxiliary API coverage table" to `docs/README.md` (under
"Services") - one row per family covering descriptors, standalone-vs-embedded support, and
explicitly-out-of-scope notes. This is the one item genuinely missing across all five families
(none had a per-family coverage table before, only prose narrative scattered across milestones) -
satisfies the issue's "documented coverage table," "unsupported/partial areas... with reasons," and
"mixed-version scenarios are documented" acceptance criteria in one place. Search 1.0 legacy
response compatibility is recorded there as explicitly out of scope (structurally superseded by
2.0; `Profile` already accepts a 1.0-shaped profile URI as a plain string extension point with no
dedicated type needed), and Content State's lack of a region/`FragmentSelector` object is recorded
as a deliberate spec-accurate design (Milestone 21), not an omission.

Tests: 13 new across `ImageServiceCompletenessTests.cs` (3: `FromInfoJson` round-trip, the issue's
own literal info.json example, blank-input throw), `ServiceRoundTripTests.cs` (2: embedded-on-
ImageResource/mixed-version, unrecognized-service-silently-dropped), `ContentStateTests.cs` (2:
invalid-length and garbled-content decode), `SearchResponseTests.cs` (1), `DiscoveryServiceTests.cs`
(4: 3 additional activity types + 1 malformed-JSON), `Auth2ServiceTests.cs` (1). Full suite: **472
unit tests + 8 architecture tests, all passing**, 0 build warnings/errors introduced.

## Round 12: extension package hardening (issue #9)

Scope (issue #9, "SDK Phase 5"): make navPlace/Georeference/TextGranularity reliable, testable, and
safe - review model coverage, improve registration behavior (the issue specifically asks to "avoid
static-constructor-only surprises" and suggests explicit `Register()` APIs), ensure payloads
round-trip through core serializers, ensure unknown extension data is preserved, and update docs.

Audited all three extensions plus the shared registration architecture in parallel. Findings:

**Registration was already largely solved.** All three packages already expose an explicit,
idempotent `Register()` (`NavPlaceExtensions.Register()`, `GeoreferenceExtensions.Register()`,
`TextGranularityExtensions.Register()`), added in an earlier round - contrary to the issue's
premise that this still needed building. A static-constructor fallback remains only for `Feature`'s
`ResourceTypeRegistry` self-registration (the Feature-as-Annotation-body dispatch case), explicitly
documented in `Feature`'s own XML comment as a fallback, not the primary path. Added
`Register_Should_BeIdempotent_ForAllThreeExtensions` since nothing previously called `Register()`
anywhere in tests/examples.

**Two real bugs found and fixed**:

1. `IiifSerializer.Serialize(Manifest)`/`DeserializeManifest` - the SDK's primary, documented,
   default-3.0 entry point - **silently dropped every extension property** (`navPlace` on
   Manifest/Collection/Canvas/Range, `textGranularity` on Annotation). The hand-rolled V3
   writer/reader builds its `JObject` field-by-field rather than going through Newtonsoft's
   automatic property serialization, so it never reached the `[JsonExtensionData]` bridge that
   makes extension data survive a plain `JsonConvert.SerializeObject` call. This directly violated
   the issue's own "Extension data round-trips through IiifSerializer" acceptance criterion - every
   existing extension test round-tripped via bare `JsonConvert`/`TrackableObject.Parse`, never via
   `IiifSerializer` itself, so the gap had gone unnoticed. Fixed generically: added
   `WriteV3AdditionalProperties`/`ReadV3AdditionalProperty` helpers (`IiifSerializer.Helpers.cs`) -
   write sweeps every `IsAdditional`-flagged `ElementDescriptor` (safe by construction: only
   extension code ever sets that flag, so no core-modeled property can collide), while read is
   deliberately *not* a generic "any unrecognized key" sweep (that would risk miscategorizing a
   not-yet-hand-rolled core V3 property, e.g. `Manifest.ViewingDirection` is never read on the V3
   path today - a separate, pre-existing gap, left alone since it's out of this issue's scope) -
   instead it's targeted to the two literal keys the shipped extensions actually use
   (`"navPlace"` in `WriteV3NodeExtras`/`ReadV3NodeExtras`, `"textGranularity"` in
   `WriteV3Annotation`/`ReadV3Annotation`). Core cannot reference the extension assemblies (the
   dependency points the other way), so these two literal strings are necessarily hardcoded in
   core rather than referenced from `NavPlace.NavPlaceJName`/`TextGranularity.TextGranularityJName`.
   Georeference's `Transformation`/`ResourceCoords` attach to `NavPlace`/`FeatureProperties`
   themselves (not directly to a `BaseNode`), so they're unaffected by this gap and already
   round-tripped correctly - confirmed, not assumed, by tracing `TransformationExtensions`' actual
   constraint (`TrackableObject<TNode>`, not `BaseNode<TNode>`).
2. `TextGranularityExtensions` was constrained to `IBaseResource`, so `SetTextGranularity`/
   `TextGranularity` **could not be called on a real `Nodes.Contents.Annotation.Annotation` at
   all** (a compile error) - only on a standalone `BaseResource` tagged `ResourceType.Annotation`,
   a shape that never actually occurs on an Annotation embedded in a real Manifest/Canvas tree. The
   issue's own example (`"type": "Annotation", ..., "textGranularity": "word"`) shows the property
   directly on a real Annotation. Fixed by adding a second `extension(Annotation annotation)` block
   in `TextGranularityExtensions.cs` targeting the real type directly (no `ResourceType` check
   needed, since the C# type itself already guarantees "this is an Annotation") - the
   `IBaseResource`-constrained overload is kept for backward compatibility with existing callers of
   that shape.

**A third, adjacent bug fixed** (directly in scope - issue #9 explicitly asks to review
"polynomial ... transformations if supported", and this session is a hardening pass, not the
test-generation-only pass under which this bug was originally found and deliberately left unfixed -
see the Milestone/Round history in `PolynomialTransformationTests.cs`): `PolynomialTransformationOption.Order`
had no `[JsonProperty]` and no way to set it via any public API (bare `private set`, no constructor
parameter, no fluent setter), so it could never be given a non-zero value. Fixed by adding
`[JsonProperty("order")]` and a fluent `SetOrder(long)`, matching this codebase's established
pattern for every other settable property.

**Documentation fixed**: `docs/Extensions/TextGranularity/README.md` claimed the package "needs no
core SDK changes to work," which - after this round's fix - is now only true for *adding* the
package, not for round-tripping through `IiifSerializer`'s hand-rolled V3 path specifically;
corrected to describe both. Added a "Versioning note" to `docs/README.md`'s "Extension packages"
section documenting that each extension's `ProjectReference` to core becomes a version-pinned (not
floating-range) `PackageReference` once packed - previously undocumented anywhere, relative to the
issue's "remain compatible with core package versioning" acceptance criterion.

Tests: 11 new/changed. `ExtensionAttributeTests.cs`: `NavPlace_Should_RoundTripThroughIiifSerializer_
OnManifest`, `..._OnCanvas`, `NavPlace_Should_PreserveUnknownGeoJsonProperties_ThroughIiifSerializer`,
`TextGranularity_Should_RoundTripThroughIiifSerializer_OnAnnotationInsideManifest` (a `[Theory]`
covering all 6 granularity values, closing the "only Word was tested on the attachment path" gap),
`Register_Should_BeIdempotent_ForAllThreeExtensions`. `PolynomialTransformationTests.cs`:
`Options_Order_Should_RoundTripThroughJsonConvert` and `Options_SetOrder_Should_
BeSettableThroughFluentApi` replace the old `_KnownBug`-suffixed test now that the bug is fixed.
Full suite: **483 unit tests + 8 architecture tests, all passing**, 0 build warnings/errors
introduced.

## Round 13: cookbook coverage inventory (issue #10)

Scope (issue #10, "SDK Phase 6A"): turn official IIIF Cookbook coverage into a verifiable
compatibility suite - inventory every official recipe, classify implementation status, ensure
`CookbookCatalog.GetAll()` reaches everything, confirm recipes serve as regression tests (not just
sample code), confirm no live network dependency in normal test runs, and produce the coverage
matrix document the earlier Cookbook rounds never actually wrote.

**Verified, not assumed, 100% parity**: fetched the live recipe list directly
(`gh api repos/IIIF/cookbook-recipes/contents/recipe`) and diffed it against every `RecipeNNNN`
method name across `examples/IIIF.Manifest.Serializer.Net.Cookbook/*Recipes.cs` - the two lists
matched exactly (71 official recipes, excluding the 3 non-recipe entries `0000_template`/
`0231-transcript-meta-recipe`/`0466-link-for-loading-manifest` this catalog already correctly
excludes). Zero recipes missing, zero extras, zero Pending/Partial/Blocked classifications needed.

**Confirmed already-solid infrastructure** (no changes needed): `CookbookCatalog.GetAll()`
aggregates all nine `IRecipeSet` Strategy implementations and already reaches every one of the 78
catalog entries; `ExampleCatalogTests.Cookbook_examples_should_round_trip_through_IiifSerializer`
already uses every catalog entry as a `[Theory]`/`[MemberData]`-driven regression test (both
directions, both versions) - genuinely "recipes as regression tests," not merely runnable sample
code; a repo-wide grep for `HttpClient`/`WebRequest`/`GetAsync` across `tests/` and `examples/`
confirmed zero live network dependency anywhere.

**One real test gap found and closed**: the 3 Content State "sharing" recipes (0485, 0540's
second document, 0599) return a `ContentState`, not a `Manifest`/`Collection` - the generic
round-trip theory's `default` branch only plain-`JsonConvert`-serializes them, never actually
exercising `ContentStateCodec.Encode`/`Decode` (the `iiif-content` base64url string these recipes
exist to demonstrate). Issue #10's own "Tests" section calls this out by name
("Content State encode/decode test for sharing recipes"). Added
`ContentState_sharing_recipes_should_round_trip_through_ContentStateCodec` - a second
`[Theory]`/`[MemberData]` in `ExampleCatalogTests.cs` filtering `CookbookCatalog.GetAll()` to just
the `ContentState`-returning entries and round-tripping each through the actual codec.

**Deliverable created**: `docs/COOKBOOK_COVERAGE.md` - the coverage matrix this issue's acceptance
criteria requires and which did not exist in any form before this round (confirmed: no
`docs/COOKBOOK_COVERAGE.md`, no `examples/.../README.md`). One row per catalog entry (78 rows) with
official recipe URL, status, SDK class/method, test coverage, and notes, plus a table of the 3
intentionally-excluded non-recipe entries and a "how this was verified" section documenting the
`gh api` diff methodology so a future recipe addition can be checked the same way. Linked from
`docs/README.md`'s "Examples and Cookbook" section and "Documentation index".

Tests: 1 new `[Theory]` (3 cases) - `ContentState_sharing_recipes_should_round_trip_through_
ContentStateCodec` in `ExampleCatalogTests.cs`. Full suite: **486 unit tests + 8 architecture
tests, all passing**, 0 build warnings/errors introduced.

## Round 14: demo scenarios (issue #11)

Scope (issue #11, "SDK Phase 6B"): represent official IIIF demo patterns as local SDK
demo/example scenarios. `iiif.io/demos/` was already confirmed (per `CLAUDE.md`'s mandate, checked
in an earlier round) to be an external-links showcase page with no manifest content of its own -
consistent with that finding, issue #11's own example scenarios are explicitly "inspired by"
patterns (e.g. "Wellcome-style search integration"), not scraped content, so this round extended
`DemoCatalog` with locally-authored, deterministic fixtures rather than importing anything external.

Audited the existing 6 demos against issue #11's explicit category list (search/autocomplete
overlays, overlaid search results, deep zoom, paged books/newspapers, complex multi-canvas objects,
3D/audio/video/PDF references, collections/cross-navigation, education/storytelling annotations,
guided tours, map/navPlace, reunification) and found real gaps: no demo modeled actual overlaid
search results (only the service descriptors), no demo had non-image canvas content (audio/video/
PDF/3D), no demo represented a guided-tour Range structure, and the one demo labeled
"reunification" didn't actually preserve any cross-institution provenance data. Added 4 new demos
(`CreateOverlaidSearchResults`, `CreateStorytellingAnnotationExample`, `CreateGuidedTourExample`,
`CreateMixedMediaObjectExample`) and fixed the reunification demo in place
(`CreateReunificationExample`, formerly `CreateCollectionExample`) to embed two full nested
`Manifest` objects, each with its own `Provider`/`Homepage`/`SeeAlso` preserving a distinct source
institution - the actual pattern Biblissima-style reunification demonstrates.

**One real, more significant bug found while building the reunification/search demos and fixed**:
`IiifSerializer`'s hand-rolled V3 writer/reader never read or wrote the embedded/inline `service`
property (`BaseItem.Service`) on Manifest, Collection, Canvas, or Range/Structure themselves - only
on annotation-body content resources (`WriteV3Resource`) and Manifest's separate, 3.0-only
top-level `services` array (`WriteV3Manifest`). This silently dropped any service embedded directly
on one of these 4 top-level resource types when using `IiifSerializer.Serialize`/
`DeserializeManifest` - the SDK's primary, documented entry point - even though this is a
documented, explicitly-tested pattern via plain `JsonConvert`
(`ServiceRoundTripTests.SearchAndAutoCompleteService_Should_RoundTripThroughInlineServiceProperty`,
added in an earlier round). Same root cause and same fix shape as Round 12's extension-data gap,
but for core, spec-real service embedding rather than extension data. Fixed generically in
`WriteV3NodeExtras`/`ReadV3NodeExtras` (shared across all 4 `BaseNode`-derived types) using the
existing `WriteV3Service`/`ReadV3Service` helpers (context-preserving, matching the top-level
`Services` array's convention - not `WriteV3EmbeddedResourceService`, which deliberately strips
`@context` for the different case of a service embedded on a content-resource body).

Also fixed, while auditing the deep-zoom demo for accuracy: its Image API service declared a 2.x
`@context` but never called `.AsImageService2()`, so it actually wrote as `ImageService3` - an
internal inconsistency, not a core bug (the demo just hadn't been checked for self-consistency
before).

Created `docs/DEMO_COVERAGE.md` - the same kind of coverage matrix `COOKBOOK_COVERAGE.md` provides
for recipes, mapping each of the 10 demo scenarios to the real-world pattern it represents, plus a
table of issue #11 categories intentionally not given a separate demo (with reasons - e.g. no
native 3D IIIF body type exists as of Presentation 3.0, so 3D is represented via a `rendering` link
to an external viewer, the spec-correct mechanism for a format with no dedicated body type).

Tests: 9 new. `DemoCatalogTests.cs` (new file, 8 dedicated JSON-shape/builder tests - one per
demo pattern issue #11's "Tests" section names by category: search service, overlaid search
results, image service, annotation targeting, Range/TOC, mixed-media canvases, collection/
reference links, navPlace). `ServiceRoundTripTests.InlineService_Should_RoundTripThroughIiifSerializer_
OnManifestCollectionCanvasAndRange` (1) locks in the embedded-service fix across all 4 resource
types. Full suite: **498 unit tests + 8 architecture tests, all passing**, 0 build warnings/errors
introduced.

## Round 15: upstream standards & ecosystem coverage matrix (issue #12)

Scope (issue #12, "SDK Research" - explicitly research/planning only, no runtime changes):
build an evidence-based coverage matrix comparing this SDK against the live IIIF standards, the
`IIIF` GitHub organization, `awesome-iiif`, and the official validators, so future coverage
decisions don't rely on memory or the current code shape alone.

Researched via `gh api` (GitHub API) and direct `raw.githubusercontent.com` fetches - no scraping,
no live dependency introduced into the SDK's own build/test/CI process:
- **`awesome-iiif`'s actual structure**: its files are lowercase (`readme.md`/`implementations.md`/
  `curation.md`/`reading-viewing.md`), not `README.md` - confirmed by listing the repo contents
  directly rather than assuming the conventional filename. Categorized every relevant section
  (Standards, Presentation/Image API Libraries, Validators, Manifest Tools, Image Servers,
  Exhibition/Guided-Viewing Tools, Content Search, Authentication, plus several institutional/
  deployment categories confirmed out of scope for a serializer library).
- **No .NET/C# Presentation-manifest-building library is listed in `awesome-iiif` at all** - the
  only .NET entry anywhere in the document is `TremendousIIIF`, a .NET Image API *server*, not a
  manifest library. This SDK appears to fill a genuinely unlisted ecosystem gap - noted as a future
  ecosystem-visibility opportunity (submitting a PR to `awesome-iiif`), not a code gap.
- **Presentation API validator can be used for fully offline CI/release checks**: fetched its
  `action.yml` and `README.md` directly - `validate-dir` operates on local files/directories only,
  no network required, version auto-detected from `@context` or explicit. A concrete, positive
  finding for a *future* release-hardening issue to act on (issue #12 itself explicitly excludes
  "enabling validator CI directly").
- **Image API validator cannot be used the same way**: fetched its README directly (note: the
  actual content lives in the extensionless `README` file - `README.rst` is a stub) - it validates
  a *live, running* Image API server's actual HTTP responses (tile/region/size requests), not a
  static `info.json` file in isolation. Using it would require building a new mock Image API HTTP
  server backed by this SDK's `Service` model purely to satisfy the validator - a distinct future
  feature, not a simple CI wiring task, and explicitly noted as such rather than glossed over.
- **Per-API-family status re-verified against live spec pages** (not re-derived from memory): every
  row in the matrix cites the specific earlier round that verified it, cross-checked against the
  official spec URL for that area. No area was found to be genuinely `Missing`; the SDK's actual
  coverage matches what `CLAUDE.md`'s "Current state" section already claims, confirmed rather than
  assumed.

Created `docs/IIIF_UPSTREAM_COVERAGE_MATRIX.md` (the deliverable issue #12 asks for) and linked it
from both `docs/README.md`'s Documentation index and `docs/SDK_FIRST_VERSION_IMPLEMENTATION_GUIDE.md`
§7 (a pointer added alongside the existing coverage-matrix table there, not a rewrite of that
hand-authored planning document).

No code changes - this round is documentation/research only, per issue #12's own scope. No new
tests; full suite unchanged at 499 unit tests + 8 architecture tests.

## Round 16: validation layer and release hardening (issue #13)

Scope (issue #13, "SDK Phase 7"): prepare the SDK for stable public consumption - add an opt-in
validation layer separate from parsing (parsing itself must stay tolerant), prove the NuGet
packages actually work when consumed from a clean project (not just as in-repo project
references), and document a release checklist covering tests/packaging/license/docs/public-API
compatibility/validator evaluation.

**New validation layer** (`src/IIIF.Manifest.Serializer.Net/Validation/`): `IiifValidator` with
`ValidateManifest`/`ValidateCollection`/`ValidateJson` entry points, `IiifValidationResult`
(`IsValid` + `Errors`), `IiifValidationError` (`RuleId`/`Severity`/`Message`/`Path`),
`IiifValidationSeverity` (Info/Warning/Error), and `IiifValidationOptions`
(`Version`/`Strict`) - matching the shapes issue #13's own examples sketch almost exactly. Nothing
in `IiifSerializer` calls this implicitly; parsing behavior is completely unchanged. Rule set (a
foundational baseline, not an exhaustive spec validator - documented as such in `docs/README.md`):
- Required `label` on Manifest/Collection (`Error`).
- `Canvas` must have height+width, duration, or both (`Error`) - covers both visual and
  audio-only/time-based canvases correctly.
- `behavior` values checked against the **actual Presentation API 3.0 §5.4.3 table** (fetched via
  WebFetch rather than assumed from memory - Layout/Temporal/Collection/Range/Canvas behaviors each
  have a real, spec-defined valid-resource-type list, e.g. `facing-pages`/`non-paged` are
  Canvas-only, `multi-part`/`together` are Collection-only) (`Warning`).
- `requiredStatement` completeness - both `label` and `value` non-empty when present (`Error`).
- **Version-aware**: targeting `V2_0`/`V2_1` warns about the same 3.0-only, no-legacy-equivalent
  properties `docs/README.md`'s "Downgrade limitations" table (Round 10) already documents
  (`placeholderCanvas`, `start`, top-level `services`) - the validator and the docs table now stay
  in sync by construction, cross-referencing each other.
- `Strict` mode: an additional well-formed-URI check on `rights`, too noisy for default use against
  tolerantly-imported real-world data but useful when authoring new documents.

Needed an `IsExternalInit` polyfill (`Validation/IsExternalInit.cs`) since the core project targets
netstandard2.1, which has no built-in support for C# 9 `record`/`init` without it - a well-known,
standard workaround, not a real runtime type.

**Package smoke test** (`scripts/smoke-test-packages.ps1`): packs all 4 packages, installs them
into a throwaway clean console app via `--source` (deliberately never a `ProjectReference`, so this
proves the *published artifacts* work, not just the in-repo project graph), and confirms a Manifest
with a navPlace extension actually builds/serializes/round-trips. Verified by actually running it
end-to-end during this round (twice - the first run caught two real script bugs, both fixed before
landing): a PowerShell `-notmatch` array-vs-scalar gotcha in the output-matching check, and a
version-resolution bug where a naive `<id>.*.nupkg` glob for the core package would also match
every extension package's `.nupkg` (all four share the same name prefix) - fixed by deriving each
package's version from its own `.nupkg` filename via an anchored regex instead of a wildcard
filter. Wired into `.github/workflows/publish-nuget.yml`'s `pack` job, running immediately after
packing and before the packages are uploaded/published - a failure there blocks the whole release,
satisfying "CI or release workflow validates packages before publishing."

**Release checklist** (`docs/RELEASE_CHECKLIST.md`): tests/quality gates, packaging, license,
documentation re-verification (pointing back at each coverage doc's own "how this was verified"
section), and two decisions made explicit rather than silently deferred:
- **Public API compatibility**: evaluated adopting `Microsoft.CodeAnalysis.PublicApiAnalyzers` (MIT
  licensed, satisfies `CLAUDE.md`'s licensing-discipline rule) but deferred it - it requires
  committing an initial baseline covering the SDK's entire existing public surface across 4
  packages, a substantial one-time cost distinct from this round's scope. Documented as a future
  consideration, with the interim manual-check approach (no removing/breaking a public member
  unless it was already `[Obsolete(error: true)]` in the prior release) written down instead of
  left implicit.
- **Official validators**: reused Round 15's research rather than re-deriving it - the Presentation
  API validator (offline-capable, `validate-dir`) is flagged as a genuine, concrete future
  release-hardening follow-up (running the Cookbook catalog's output through it), explicitly not
  implemented here since issue #13 itself excludes "enabling validator CI directly." The Image API
  validator remains not usable for this SDK without a new mock-server test harness, also
  explicitly deferred rather than glossed over.

Tests: 16 new (`IiifValidatorTests.cs`) covering every rule above plus the explicit
parser-vs-validator split issue #13 asks for (a minimal legacy document missing a label
deserializes without throwing, then the validator reports the missing label). Full suite: **515
unit tests + 8 architecture tests, all passing**, 0 build warnings/errors introduced. The package
smoke test itself is a script, not a unit test (per its own nature - it packs artifacts and runs a
separate throwaway console app), verified by direct execution rather than only code review.

## Round 17: EF Core-style object-graph change tracking and changed-only manifest output (issue #23)

Scope (issue #23): give the entire SDK object graph EF Core-style change tracking - property-level
change notification, collection mutation tracking, parent-to-child change propagation,
`ClearChanges()`/`AcceptChanges()`, and changed-only manifest/delta-envelope output for time-scale/
delta persistence scenarios - all without taking an EF Core dependency.

**Design decision - pull-based, not the issue's suggested push/`AttachParent` design**: rather than
an explicit `IIiifObjectGraphTrackable.Parent`/`AttachParent`/`DetachParent` wiring scheme (which
needs correct subscribe/unsubscribe bookkeeping at every single mutation call site across the whole
SDK to avoid silently missing changes), `HasChanges`/`GetChanges()`/`ClearChanges()` walk the object
graph fresh on every call, reusing the Original/Modified value pair every property already
maintains via the existing `ElementDescriptors` mechanism on `TrackableObject<T>`. This correctly
detects a child mutated through a reference held outside its parent (e.g.
`manifest.AddItem(canvas); manifest.ClearChanges(); canvas.SetHeight(2000);` still makes
`manifest.HasChanges` true) with zero changes needed to any existing `Add*`/`Set*` method. Full
rationale in the new `docs/CHANGE_TRACKING.md`.

**New `IIIF.Manifests.Serializer.ChangeTracking` namespace**: `IIiifChangeTrackable`
(`HasChanges`/`GetChanges()`/`ClearChanges()`/`AcceptChanges()` - deliberately not extending
`INotifyPropertyChanging`/`INotifyPropertyChanged`, since `TrackableObject<T>` already implements
those separately from earlier work), `IiifChangeEntry` (`Path`/`Kind`/`PropertyName`/
`OriginalValue`/`CurrentValue`/`ChangedAtUtc`), `IiifChangeKind` (`Added`/`Modified`/`Removed`/
`CollectionItemAdded`/`CollectionItemRemoved`/`CollectionCleared`/`ChildChanged` - only `Modified`/
`CollectionItemAdded`/`CollectionItemRemoved` are actually synthesized by this implementation; the
rest exist for shape-parity with the issue's own suggestion and future extension, documented as an
explicit gap rather than left unexplained), and `IiifChangeSet` (`RootId`/`RootType`/
`CreatedAtUtc`/`Changes`/`ChangedManifest` - a plain constructor-based class, not `required init`
properties, since netstandard2.1 needs `RequiredMemberAttribute`/`CompilerFeatureRequiredAttribute`
polyfills this repo doesn't carry yet, beyond just the existing `IsExternalInit` one).

**Core implementation** (`Shared/Trackable/TrackableObject.ChangeTracking.cs`, a new partial-class
file on the existing shared base - this alone gives every derived type in the SDK, roughly 180
model types, change tracking for free): a structural-vs-value split for collection-valued
properties (anything whose element type implements the existing `IBaseItem` marker - `Items`,
`Structures`, etc. - is diffed by reference for `CollectionItemAdded`/`CollectionItemRemoved`;
everything else, e.g. `Label`/`Metadata`, is reported as one wholesale `Modified` entry), recursive
parent/child path-prefixing (a changed `Canvas` at `Items[0]` surfaces as `Items[0].Height` on the
owning `Manifest`, no synthetic wrapper entry), and cycle protection via a reference-equality
`HashSet<object>` threaded through every recursive call (a custom `ReferenceEqualityComparer`, since
`System.Collections.Generic.ReferenceEqualityComparer` isn't available on netstandard2.1). Cross-
closed-generic-type dispatch (`TrackableObject<Canvas>` and `TrackableObject<Manifest>` are distinct
runtime types despite sharing a template, so a `private` method on one can't be called from the
other during recursion) solved via an internal `IChangeTrackingCoreAccess` interface with explicit
implementations forwarding to the real private `*Core` methods.

**Two related bugs found and fixed, both via actually running the tests this round's own examples
implied, not assumed correct from the initial design** - both are instances of the same root cause:
`ElementDescriptor`'s Original/Modified pair treats a property's *first-ever* assignment as
"establishing the baseline" (correct for constructor-time properties), which under-reports a
property that was never touched *before* an explicit `ClearChanges()` call, then set for the first
time *after* it (e.g. `Items` on a fresh `Manifest`, or an optional property like `Rights` never set
in the constructor). Fixed generically with two new fields on `TrackableObject<T>`:
`_collectionBaselines` (snapshots collection contents at clear time, used as the diffing baseline
for `CollectionItemAdded`/`CollectionItemRemoved` instead of `ElementDescriptor.OriginalValue`) and
`_keysAtLastClear` (snapshots which `ElementDescriptors` keys existed at clear time; any key set for
the first time afterward is reported `Modified` with `OriginalValue = null`, even though its own
`ElementDescriptor.IsModified` says otherwise).

**Changed-only output** (`Nodes/Manifest.ChangeTracking.cs`, `Manifest`-only so far - the only
document type the issue's own examples exercise): `GetChangedManifest()` reconstructs a best-effort,
valid partial `Manifest` (`id` always kept; `rights`/`requiredStatement`/`summary` if changed; only
`Items` canvases that are new or internally changed) - explicitly never represents removals, since a
valid IIIF Manifest has no "this used to be here" concept. `GetChangeSet()` wraps the complete
`GetChanges()` list (including removals) alongside the same `ChangedManifest`, resolving that gap.
`IiifSerializer.SerializeChangedOnly(manifest, options)` is a named convenience equivalent to
`Serialize(manifest.GetChangedManifest(), options)`.

**Deserialization starts clean by design**: `IiifSerializer.DeserializeManifest`/
`DeserializeCollection`/`DeserializeAnnotationCollection`, and the shared
`TrackableObject.Parse<T>`/`TryParse<T>` (covering `SearchResponse`/`ContentState`/
`DiscoveryCollectionPage` and other document types built directly on the shared base), each call
`ClearChanges()` on the freshly-deserialized result before returning it - a document just loaded
from storage has no pending edits yet.

**Also added** (needed to satisfy this issue's own canonical worked examples, which reference
`canvas.SetHeight(2000)`): genuine public `Canvas.SetHeight(int)`/`SetWidth(int)` fluent setters -
`Canvas` previously only exposed `private set` on `Height`/`Width` (plus an unrelated, already-
documented buggy `SetHeight(this T, JToken)` extension-method helper from an earlier round, which
takes a `JToken` and doesn't apply here).

New documentation: [`docs/CHANGE_TRACKING.md`](CHANGE_TRACKING.md) covers the lifecycle, the
pull-based design rationale, parent/child propagation, the structural-vs-value collection split,
changed-only output for delta/time-scale storage (with the removals caveat spelled out), and this
feature's limitations/non-goals - linked from `docs/README.md`'s Documentation index.

Tests: 20 new (`ChangeTrackingTests.cs`) covering property changes, `PropertyChanging`/
`PropertyChanged` event firing, equivalent-value no-ops, collection add/remove, child-to-parent
bubbling (through a collection and through a direct scalar change), `ClearChanges`/`AcceptChanges`
recursion, deserialize-starts-clean, cycle-safety with a doubly-referenced child, extension-object
(navPlace) bubbling, all three `GetChangedManifest()` scenarios (top-level property, changed child
canvas, newly-added canvas), `GetChangeSet()` capturing a removal `GetChangedManifest()` can't
represent, `SerializeChangedOnly` parity with the manual two-step equivalent, and full-serialize
being unaffected by tracking state. Full suite: **535 unit tests + 8 architecture tests, all
passing**, 0 build warnings/errors introduced, zero regressions to the pre-existing 515+8.

## Status: all 24 (rounds 1-2) + 10 (round 3) milestones complete, plus the round 4 structural refactor, round 5 System.Text.Json interop, round 6 version-detection hardening, round 7 legacy-import normalization audit, round 8 obsolete-member IIIFVersionAttribute decoration, round 9 legacy-mutator severity downgrade (error to warning), round 10 versioned-writer audit with the behavior-to-viewingHint downgrade fix, round 11 auxiliary API surface audit with the Image API info.json read gap fixed, round 12 extension package hardening with the extension-data-dropped-by-IiifSerializer bug fixed, round 13 cookbook coverage inventory confirming 100% official recipe parity with a new coverage matrix, round 14 demo scenarios with the embedded-service-dropped-by-IiifSerializer bug fixed, round 15 upstream standards/ecosystem coverage matrix confirming no missing API-family coverage, round 16 validation layer plus release-hardening (package smoke test, release checklist), and round 17 EF Core-style object-graph change tracking plus changed-only manifest/delta output.
