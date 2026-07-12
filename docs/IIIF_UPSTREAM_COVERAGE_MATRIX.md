# IIIF Upstream Standards & Ecosystem Coverage Matrix

A research/planning document (issue #12, "SDK Research") comparing this SDK's implementation
against the official IIIF standards, the `IIIF` GitHub organization, `awesome-iiif`, and the
official validators - so future coverage decisions are evidence-based, not memory-based. No
runtime code changed to produce this document (out of scope per issue #12).

**Retrieved**: 2026-07-12, via `gh api` (GitHub API, no scraping) against `github.com/IIIF/*` and
`raw.githubusercontent.com/IIIF/awesome-iiif`. Re-run the same queries (see "How this was
researched" at the end) to refresh if this document goes stale - do not treat it as a permanent
snapshot.

## Status classifications used below

`Supported` · `Mostly supported` · `Partial` · `Missing` · `Research needed` · `Not applicable` ·
`Future tracking`

## 1. Official API families vs. current SDK support

| API family | Official reference | Current SDK status | Missing work | Follow-up |
| --- | --- | --- | --- | --- |
| Presentation API 3.0 (Manifest/Collection/Canvas/Range/Annotation/AnnotationCollection) | [iiif.io/api/presentation/3.0](https://iiif.io/api/presentation/3.0/) | **Supported** | None identified this pass. Default read/write target; 486+ tests including full Cookbook round-trip coverage (`COOKBOOK_COVERAGE.md`). | - |
| Presentation API 2.1 legacy read/write | [iiif.io/api/presentation/2.1](https://iiif.io/api/presentation/2.1/) | **Supported** | None identified. Confirmed wire-identical to 2.0 per spec (`IiifPresentationVersion.V2_0`'s own doc remarks); every Cookbook recipe has a v2.1-writer test. | - |
| Presentation API 2.0 legacy read/write | [iiif.io/api/presentation/2.0](https://iiif.io/api/presentation/2.0/) | **Supported** | None. Confirmed no structural difference from 2.1 exists in the live spec (Round 6/10 research) - `V2_0` is a real, tested write target, not merely detected. | - |
| Metadata API 1.0 ("Shared Canvas") legacy import | Predecessor to Presentation 2.0, no longer a maintained standalone spec | **Not applicable** (by design) | Detected for classification only (`IiifPresentationVersion.Metadata_1_0`); this SDK deliberately does not import/export it as a first-class model - it predates and is structurally near-identical to Presentation 2.x, and a real Metadata 1.0 document without an explicit `@context` already detects and imports correctly as `V2_1`. Documented rationale: `SDK_VERSIONING_GUIDE.md` Round 6. | - |
| Image API 3.0 service descriptor + standalone info.json | [iiif.io/api/image/3.0](https://iiif.io/api/image/3.0/) | **Supported** | None. `Service.ToInfoJson()`/`FromInfoJson()` (Round 11) cover both directions; all spec fields modeled (`profile`/`protocol`/`tiles`/`sizes`/`preferredFormats`/`extraFormats`/`extraQualities`/`extraFeatures`/`maxWidth`/`maxHeight`/`maxArea`/`rights`). | - |
| Image API 2.1 service descriptor | [iiif.io/api/image/2.1](https://iiif.io/api/image/2.1/) | **Supported** | None. `Service.AsImageService2()` toggle; embedded-on-resource and mixed-Presentation-3+Image-2 scenarios tested (Round 11). | - |
| Authorization Flow API 2.0 (probe/access/token/logout) | [iiif.io/api/auth/2.0](https://iiif.io/api/auth/2.0/) | **Supported** | None. All 4 real service types plus response payloads modeled and tested (Round 11 audit). | - |
| Auth 1.0 legacy service | [iiif.io/api/auth/1.0](https://iiif.io/api/auth/1.0/) (legacy reference only, superseded) | **Supported** | None for the legacy shape itself. `AuthService1` modeled/tested. | - |
| Content Search API 2.0 (descriptors + response shapes) | [iiif.io/api/search/2.0](https://iiif.io/api/search/2.0/) | **Supported** | None. `SearchService`/`AutoCompleteService` descriptors, `SearchResponse`/`TermPageResponse` result shapes (not just descriptors) - a common gap in other SDKs, closed here in an earlier milestone. | - |
| Content Search API 1.0 legacy compatibility | [iiif.io/api/search/1.0](https://iiif.io/api/search/1.0/) (legacy reference only) | **Not applicable** (by design) | Search 1.0 predates and is structurally superseded by 2.0; this SDK's `Profile` property already accepts a 1.0-shaped profile URI as a plain string extension point without a dedicated legacy response type. Documented rationale: Round 11 audit, `docs/README.md`'s Auxiliary API coverage table. | - |
| Change Discovery API 1.0 | [iiif.io/api/discovery/1.0](https://iiif.io/api/discovery/1.0/) | **Supported** | None. `OrderedCollection`/`OrderedCollectionPage`/`Activity` with actor/object/target/dataset/rights/paging all modeled and tested (Round 11). | - |
| Content State API 1.0 | [iiif.io/api/content-state/1.0](https://iiif.io/api/content-state/1.0/) | **Supported** | None for the modeled surface. `ContentState`, `ContentStateTarget`, `ContentStatePointSelector`, `iiif-content` base64url codec (`ContentStateCodec`), 3 Cookbook "sharing" recipes exercise it end-to-end (Round 13). Region targeting is deliberately a Media Fragment suffix on the target `id`, not a selector object - confirmed no such selector form exists in Content State 1.0 (Milestone 21). | - |
| navPlace extension | [iiif.io/api/extension/navplace](https://iiif.io/api/extension/navplace/) | **Supported** | None. `Feature`/`Geometry`/`FeatureCollection` (`NavPlace` itself), all 7 GeoJSON geometry types, Manifest/Collection/Canvas/Range attach points, explicit `Register()` (Round 12). | - |
| Georeference extension | [iiif.io/api/extension/georef](https://iiif.io/api/extension/georef/) | **Mostly supported** | `PolynomialTransformationOption.Order` fixed in Round 12 (was previously unsettable via any public API). No other gaps found in the Round 12 audit (georeferencing annotation, SVG selector, target, resource coordinates, thin-plate-spline all modeled/tested). | - |
| Text Granularity extension | [iiif.io/api/extension/text-granularity](https://iiif.io/api/extension/text-granularity/) | **Supported** | None remaining. Fixed in Round 12: could not previously be attached to a real `Annotation` (only a standalone `IBaseResource` stand-in), and was silently dropped by `IiifSerializer`'s hand-rolled V3 Annotation writer/reader. | - |
| Presentation API 4.0 (RC/draft) | No stable public URL yet - draft header "4.0.0-draft" as of this writing | **Future tracking** | Detected (`IiifPresentationVersion.V4_0_Rc`) for classification only; explicitly **not** a write target - `Serialize`/`DeserializeManifest` both throw `NotSupportedException` rather than silently treating draft output as stable (Round 6). Re-audit when 4.0 reaches Recommendation/stable status. | Reopen this matrix's row, not a new issue, when 4.0 stabilizes. |
| 3D resources | No stable IIIF Presentation API body type as of 3.0 | **Not applicable** (by design, current spec) | No native 3D body/resource type exists in Presentation 3.0 to model. This SDK represents a 3D reference the only spec-correct way available - a `rendering` link to an external viewer (Round 14 demo). Re-audit if/when a 3D IIIF extension reaches the Registry of Extensions. | Track via [iiif.io/api/extension/](https://iiif.io/api/extension/) periodically, per `CLAUDE.md`'s mandate item 5. |

## 2. `awesome-iiif` ecosystem categories relevant to this SDK

Retrieved from `github.com/IIIF/awesome-iiif`'s `readme.md`/`implementations.md` (the repo's actual
filenames - note lowercase, not `README.md`).

| Category | Relevance to this SDK | Notable entries | Action |
| --- | --- | --- | --- |
| **Standards** | Direct - this is the spec index this SDK targets | Links to Image/Presentation/Search/Auth/Discovery/Content State APIs + the 3 extensions | Already the basis of §1 above; no new links found beyond what this SDK already tracks. |
| **Presentation API Libraries** | Direct competitor/peer-library category | `iiif-prezi`/`iiif-prezi3` (Python), `jiiify-presentation` (Java), Manifesto/Manifold (JS ecosystem), `pyIIIFpres` (Python), `O'Sullivan` (Ruby), `Swiiift` (Swift) | **No .NET/C# Presentation-manifest-building library is listed anywhere in `awesome-iiif`.** This SDK appears to fill a real, currently-unlisted gap in the ecosystem for .NET. Consider submitting a PR to `awesome-iiif` adding this SDK once it has a public release - genuinely useful, low-effort ecosystem visibility, but a separate action from this research ticket (not runtime work). |
| **Image API Libraries** | Adjacent - this SDK only models the Image API *service descriptor*, not URL template building/parsing | `iiif` (Python), `iiif-tiler` (Java), `image-iiif` (PHP), `piffle` (Python), `libvips` | Not applicable - this SDK is a manifest/service-descriptor serializer, not an Image API URL builder. No overlap to close. |
| **Validators** | Direct - see §3 below | Presentation API validator, Image API validator, Hyperion, Tripoli | Assessed in detail in §3. |
| **Presentation Manifest Tools** | Adjacent - manifest editors/converters, not libraries | `biiif`, Digirati Manifest Editor, `pdiiif`, `demetsiiify` | Not applicable - out of scope (this SDK is a library, not an authoring tool), but useful prior art to check for edge-case JSON shapes if a future recipe/demo needs one. |
| **Image Servers** | Adjacent, one .NET entry exists | `TremendousIIIF` - "A .NET C# IIIF Image API 2.1 server" | Confirms .NET IIIF tooling exists for the *server* side; this SDK is complementary (a consumer/producer of the documents such a server would host), not overlapping. |
| **Exhibition and Guided Viewing Tools** | Informed Round 14's demo scenarios | Storytelling/curated-tour platforms | Already reflected in `DEMO_COVERAGE.md`'s "guided annotation tour" and "education/storytelling" demos - no further action. |
| **Content Search API** | Adjacent - indexing/search backends, not the response shape this SDK models | Search indexing tools/applications | Not applicable - this SDK models the wire format, not a search backend implementation. |
| **Authentication** | Adjacent - Auth Flow implementations (servers), not this SDK's descriptor models | Auth server implementations | Not applicable - same reasoning as Image Servers. |
| Tutorials, Videos/Slides, Community, Hosting, CMS Integration, Wiki Integration, Newspapers, STEM, Experiments, Collection Management Systems | No direct SDK-coverage relevance | - | Reviewed and confirmed out of scope for a serializer library - these are institutional/deployment concerns, not spec-coverage gaps. |

## 3. Validator CI/release-integration feasibility

| Validator | Repo | How it runs | Network required? | CI feasibility for this SDK |
| --- | --- | --- | --- | --- |
| **Presentation API validator** | [`IIIF/presentation-validator`](https://github.com/IIIF/presentation-validator) | CLI (`iiif-validator validate`/`validate-dir`), local web service, **and an official GitHub Action** (`action.yml`, `IIIF/presentation-validator@main`) | **No** - `validate-dir` operates on local files/directories only; version can be explicit (`--version 2.1`/`3.0`) or auto-detected from `@context` | **Feasible, no fixture-server needed.** This SDK could write each Cookbook recipe's serialized JSON to a temp directory (both v3 and v2.1 output) and run the official Action/CLI against it entirely offline. Concrete, actionable follow-up for a future release-hardening pass - explicitly out of scope for issue #12 itself per its own "Out of scope" list. |
| **Image API validator** | [`IIIF/image-validator`](https://github.com/IIIF/image-validator) | CLI (`iiif-validate.py -s <server> -p <prefix> -i <image-id>`), or a running validator web service | **Yes** - it validates a *live, running Image API server's actual HTTP responses* (tile requests, `info.json`, error codes), not a static `info.json` file in isolation | **Not directly feasible for this SDK today.** This SDK only serializes/deserializes the `info.json` *descriptor* (a `Service` object) - it does not run an Image API server that could respond to real tile/region/size requests. Using this validator would require standing up a fake/mock Image API HTTP server backed by this SDK's `Service` model purely to satisfy the validator's live-request expectations - a nontrivial new component, not a simple CI wiring change. If ever pursued, that's a distinct future feature (a minimal test-only Image API server), not a validator-integration task alone. |

## 4. Fixture sources safe to vendor/snapshot locally

Per issue #12's ask to identify deterministic, offline-safe fixture sources (distinct from live
validator integration above):

| Source | Safe to vendor? | Notes |
| --- | --- | --- |
| `github.com/IIIF/cookbook-recipes` recipe JSON | **Yes, already effectively done** | This SDK doesn't literally vendor the upstream JSON files, but hand-reconstructs each recipe as a C# builder (`examples/IIIF.Manifest.Serializer.Net.Cookbook`), verified against the live recipe list via `gh api` (Round 13) rather than a network fetch at test time - achieves the same determinism goal without a live dependency or a stale vendored-copy staleness risk. |
| Presentation API 3.0/2.1 `@context` documents (`iiif.io/api/presentation/3/context.json`, `.../2/context.json`) | **Not currently vendored, low priority** | This SDK writes these as plain string literals (confirmed throughout `IiifSerializer.*.cs`); it never fetches or validates against the live JSON-LD context document at runtime, so there's no live dependency to eliminate. Vendoring would only matter if a future JSON-LD context-expansion feature were added - not currently planned. |
| Image API 3.0/2.1 `@context` documents | **Not currently vendored, same reasoning as above** | Same - written as literals, no live dependency. |
| Presentation validator's own `schema/` directory (JSON Schema files, per its repo listing) | **Research needed if validator integration is pursued** | Not evaluated in depth this pass (out of scope - validator *integration* itself is explicitly deferred to a future release-hardening issue per §3). Worth a closer look at that time: vendoring the validator's JSON Schemas directly (rather than depending on the validator package/Action) could let this SDK assert schema-conformance in unit tests with zero external tooling dependency at all. |

## 5. Summary of actionable gaps found by this research pass

Everything below already has either a fix already shipped in an earlier round of this session, or
an explicit, documented rationale - consistent with this document's own acceptance criteria (a
follow-up issue number OR explicit rationale, not a mandatory new issue for every row):

1. **Georeference `PolynomialTransformationOption.Order`** - was unsettable via any public API;
   fixed in Round 12 (`SDK_VERSIONING_GUIDE.md`).
2. **Text Granularity on a real `Annotation`** - was impossible to attach at all (wrong generic
   constraint) and silently dropped by `IiifSerializer`; fixed in Round 12.
3. **navPlace/extension data dropped by `IiifSerializer`'s hand-rolled V3 writer** - fixed in
   Round 12.
4. **Embedded `service` property dropped by `IiifSerializer` on Manifest/Collection/Canvas/Range**
   - found and fixed in Round 14 while building demo scenarios for issue #11.
5. **Image API validator cannot be used for this SDK's fixture tests** without a new mock-server
   component (§3) - noted as a distinct future feature, not filed as an issue by this research
   ticket per its own "out of scope: enabling validator CI directly."
6. **Presentation API validator CAN be used for offline CI/release checks** (§3) - a concrete,
   positive finding for a future release-hardening issue to act on; not implemented here per issue
   #12's own scope boundary.
7. **No .NET Presentation-manifest library is listed in `awesome-iiif`** - an ecosystem-visibility
   opportunity (submitting this SDK once publicly released), not a code gap.

No area was found to be `Missing` outright; the SDK's actual coverage, as verified against live
upstream sources rather than assumed from memory, matches what `CLAUDE.md`'s "Current state"
section already claims.

## How this was researched

- Official API family status (§1): cross-referenced against this repo's own `SDK_VERSIONING_GUIDE.md`
  round history (each claim above cites the specific round that verified it) and the official spec
  pages linked in each row - not re-derived from memory alone.
- `awesome-iiif` categories (§2): fetched directly via `gh api repos/IIIF/awesome-iiif/contents`
  (confirms the repo's actual file names: `readme.md`, `implementations.md`, `curation.md`,
  `reading-viewing.md` - lowercase, not `README.md`) and `raw.githubusercontent.com` fetches of
  `readme.md`/`implementations.md`.
- Validator feasibility (§3): fetched `presentation-validator`'s `action.yml` and `README.md`, and
  `image-validator`'s `README` (note: no extension - its `README.rst` is a stub) directly via `gh
  api repos/IIIF/<repo>/contents/<file>` + base64 decode, to read actual usage instructions rather
  than assume behavior from the repo name alone.
- All lookups used `gh api` (authenticated GitHub API) or direct `raw.githubusercontent.com`
  fetches at research time - **no network dependency was introduced into this SDK's normal
  build/test/CI process**, satisfying issue #12's own constraint.
