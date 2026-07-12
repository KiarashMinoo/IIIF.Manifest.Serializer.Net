# IIIF Demo Scenario Coverage

Maps `examples/IIIF.Manifest.Serializer.Net.Examples`' `DemoCatalog` entries to the real-world IIIF
usage patterns [`iiif.io/demos/`](https://iiif.io/demos/) showcases, and to issue #11's own
explicit category list. See [`SDK_VERSIONING_GUIDE.md`, Round 14](SDK_VERSIONING_GUIDE.md#round-14-demo-scenarios-issue-11)
for how this catalog was extended and verified.

## Why these are "inspired by," not scraped

`iiif.io/demos/` was already confirmed in an earlier round (see `CLAUDE.md`'s mandate) to be an
external-links showcase page - it links out to third-party viewer deployments (Wellcome
Collection, Nationalmuseum, Stanford, Biblissima, Harvard, and others), not a source of manifest
JSON of its own. Every `DemoCatalog` entry is therefore a **locally-authored, deterministic
fixture** built to exercise the real SDK feature combination each linked institution's demo is
known for - never a live fetch, snapshot, or scrape of any external site. This keeps demo tests
fully offline (confirmed: no `HttpClient`/`WebRequest`/`GetAsync` anywhere in `tests/` or
`examples/`).

## Coverage table

| Demo scenario | `DemoCatalog` method | Represents | SDK features exercised | Tests |
| --- | --- | --- | --- | --- |
| Wellcome-style search and access manifest | `CreateSearchManifest` | Search/autocomplete overlay descriptors + a protected-content Auth 2.0 flow, as seen on Wellcome Collection's viewer | `SearchService`, `AutoCompleteService`, `ContentStateService`, `AuthProbeService2`/`AuthAccessService2`/`AuthAccessTokenService2`/`AuthLogoutService2` chain, all embedded inline on a Manifest | Round-trip (`ExampleCatalogTests`), JSON-shape (`DemoCatalogTests.SearchDemo_...`) |
| Wellcome-style overlaid search results | `CreateOverlaidSearchResults` | The actual search *response* a query returns - hit-highlighted regions overlaid on canvases, distinct from the descriptor-only scenario above | `SearchResponse`, `SearchHitAnnotation`, `SearchHitTarget`, `SearchTextQuoteSelector`, paging via `SearchAnnotationCollectionRef` | Round-trip, JSON-shape (`DemoCatalogTests.OverlaidSearchResults_...`) |
| Nationalmuseum-style deep zoom manifest | `CreateDeepZoomManifest` | Image API 2.x deep-zoom viewer (OpenSeadragon-style) integration | `Service.AsImageService2()`, image dimensions, `Provider`/`Homepage`/`Thumbnail` | Round-trip, JSON-shape (`DemoCatalogTests.DeepZoomDemo_...`) |
| Stanford-style paged book manifest | `CreatePagedBookManifest` | A multi-page paged-book/newspaper reading experience | `ViewingDirection`, `Behavior.Paged`, `Start` (deep link to a specific page), multiple canvases | Round-trip (`ExampleCatalogTests`) |
| Biblissima-style reunification of a separated object | `CreateReunificationExample` | Separated-object reunification: a manuscript physically split across two institutions, browsable as one virtual whole while each part still credits its actual holding institution | `Collection` embedding full nested `Manifest` objects (not bare id references), each with its own `Provider`/`Homepage`/`SeeAlso`; `Behavior.MultiPart` | Round-trip, JSON-shape (`DemoCatalogTests.ReunificationDemo_...`) - also confirms Collection item references never leak a nested Manifest's provenance data inline (spec-correct: a viewer must fetch each Manifest separately to see its own provider) |
| Harvard-style canvas annotation example | `CreateAnnotationExample` | Legacy (2.x-only) `AnnotationList`/`Layer` support - no 3.0 equivalent exists, so this is deliberately the one demo that stays on the legacy-only model | `AnnotationList`, `SegmentResource` | Round-trip (`ExampleCatalogTests`, falls to the plain-JSON branch since `AnnotationList` has no `IiifSerializer` V3 path by design) |
| Education/storytelling annotation tour | `CreateStorytellingAnnotationExample` | Museum-education "closer look" pattern: multiple rich, multilingual commenting/describing Annotations targeting specific regions of one Canvas, distinct from the *structural* guided tour below | `Annotation` with `commenting`/`describing` motivations, `FragmentSelector`-targeted regions, multilingual `TextualBody` | Round-trip, JSON-shape (`DemoCatalogTests.StorytellingDemo_...`) |
| Guided annotation tour (Range-based) | `CreateGuidedTourExample` | A curated "highlights tour" through a subset of canvases in a deliberate order - the *structural* wayfinding pattern (as opposed to the annotation *content* pattern above) | `Structure`/Range with `AddCanvasReference`, ordered subset of a Manifest's canvases | Round-trip, JSON-shape (`DemoCatalogTests.GuidedTourDemo_...`) |
| Mixed-media multi-canvas object (audio/video/PDF) | `CreateMixedMediaObjectExample` | A complex, real-world multi-format object: image + audio guide + video walkthrough on separate canvases, plus a PDF companion document and an external 3D model viewer linked via `rendering` | `AudioResource`, `VideoResource` (native canvas-painting body types), `Rendering` (the spec-correct way to reference PDF/3D-mesh formats that have no Presentation API body type of their own) | Round-trip, JSON-shape (`DemoCatalogTests.MixedMediaDemo_...`) |
| Map-based place example | `CreateMapExample` | A navPlace/geospatial "locate this object in place" pattern | `NavPlace`, `Feature`, `Geometry` (`Point`) | Round-trip, JSON-shape (`DemoCatalogTests.MapDemo_...`) |

## Categories from issue #11 not given a separate demo

| Category | Status | Reason |
| --- | --- | --- |
| Complex multi-canvas objects | Covered by existing demos | `CreatePagedBookManifest` (4 canvases) and `CreateMixedMediaObjectExample` (3 canvases, mixed types) already exercise this; a dedicated demo would duplicate them without representing a materially different real-world scenario, which issue #11 itself says to avoid. |
| Collections and cross-object navigation | Covered | `CreateReunificationExample`'s `Collection` embeds full nested `Manifest` objects for exactly this purpose. |
| 3D resource references | Partially covered, by design | IIIF Presentation API has no native 3D body/resource type as of 3.0 - `CreateMixedMediaObjectExample` links an external 3D viewer via `rendering` (`model/gltf-binary`), the spec-correct mechanism for a format with no dedicated body type. A true 3D IIIF extension (e.g. a future 3D Presentation extension) is not yet part of any stable spec this SDK targets, so no further model support is applicable today. |

## How this was verified

Every demo is round-tripped through `IiifSerializer` (or plain `JsonConvert` for the one
legacy-only `AnnotationList` demo) by `ExampleCatalogTests.Demo_examples_should_round_trip_
through_IiifSerializer`, and each of the important patterns above additionally has a dedicated
JSON-shape assertion in `DemoCatalogTests.cs`, per issue #11's own "Tests" section. A repo-wide
grep for `HttpClient`/`WebRequest`/`GetAsync` confirms zero live network dependency in normal test
runs.
