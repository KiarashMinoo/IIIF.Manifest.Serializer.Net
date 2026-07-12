# IIIF Cookbook Coverage Matrix

Tracks every recipe in [`github.com/IIIF/cookbook-recipes`](https://github.com/IIIF/cookbook-recipes)
against this SDK's `examples/IIIF.Manifest.Serializer.Net.Cookbook` implementation. See issue #10
("SDK Phase 6A") and [`SDK_VERSIONING_GUIDE.md`, Round 13](SDK_VERSIONING_GUIDE.md#round-13-cookbook-coverage-inventory-issue-10)
for how this inventory was produced and verified.

## Summary

- **71 of 71 official recipes implemented** (100% parity, confirmed against the live
  `IIIF/cookbook-recipes` repo's `recipe/` folder listing as of this writing) - **78 catalog
  entries** in `CookbookCatalog.GetAll()`, since 7 recipes have more than one manifest/document
  worth modeling (0010, 0011, 0040, 0232, 0306, 0309, 0540 each produce two entries).
- **3 recipes intentionally excluded**, not counted above because they carry no manifest JSON of
  their own: `0000_template` (the repo's own authoring template, not a recipe), `0231-transcript-meta-recipe`
  (a meta/index page describing the transcript-pattern recipes, not itself a document),
  `0466-link-for-loading-manifest` (a prose explainer for how a Content State link loads a
  manifest, no JSON payload).
- **Every implemented recipe is reachable from `CookbookCatalog.GetAll()`** (the Registry
  aggregates all nine `IRecipeSet` Strategy implementations - see `docs/README.md`'s "Examples and
  Cookbook" section) and is used as a **regression test**, not just sample code: every catalog
  entry round-trips through `IiifSerializer` (`ExampleCatalogTests.Cookbook_examples_should_
  round_trip_through_IiifSerializer`, one per catalog entry via `[Theory]`/`[MemberData]`). The 3
  Content State ("sharing") recipes additionally round-trip through `ContentStateCodec.Encode`/
  `Decode` specifically (`ExampleCatalogTests.ContentState_sharing_recipes_should_round_trip_
  through_ContentStateCodec`).
- **No recipe is Pending, Partially implemented, or Blocked.** If a future official recipe is
  added upstream that needs substantial new SDK model support to implement, open a follow-up issue
  rather than a partial/best-effort implementation, per this issue's own instruction.
- Normal test/CI runs have **no live network dependency** - every recipe is a hand-authored local
  C# fixture; nothing in `tests/` or `examples/` calls out to `iiif.io` or any other live URL.

## Coverage table

| Recipe | Official URL | Status | SDK class / method | Tests | Notes |
| --- | --- | --- | --- | --- | --- |
| 0001 Simplest Manifest - Image | [recipe/0001-mvm-image](https://iiif.io/api/cookbook/recipe/0001-mvm-image/) | Implemented | `FoundationRecipes.Recipe0001` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0002 Simplest Manifest - Audio | [recipe/0002-mvm-audio](https://iiif.io/api/cookbook/recipe/0002-mvm-audio/) | Implemented | `FoundationRecipes.Recipe0002` | Builder, v3 shape, v2.1 writer, round-trip | Also a hand-picked demo (`Recipe0002SimplestAudioExample.cs`) |
| 0003 Simplest Manifest - Video | [recipe/0003-mvm-video](https://iiif.io/api/cookbook/recipe/0003-mvm-video/) | Implemented | `FoundationRecipes.Recipe0003` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0004 Canvas Size and Image Size Differ | [recipe/0004-canvas-size](https://iiif.io/api/cookbook/recipe/0004-canvas-size/) | Implemented | `FoundationRecipes.Recipe0004` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0005 Support IIIF Image Deep Zoom | [recipe/0005-image-service](https://iiif.io/api/cookbook/recipe/0005-image-service/) | Implemented | `FoundationRecipes.Recipe0005` | Builder, v3 shape, v2.1 writer, round-trip | Image API service descriptor |
| 0006 Simplest Manifest with Multiple Languages | [recipe/0006-text-language](https://iiif.io/api/cookbook/recipe/0006-text-language/) | Implemented | `FoundationRecipes.Recipe0006` | Builder, v3 shape, v2.1 writer, round-trip | Language maps |
| 0007 String Formats | [recipe/0007-string-formats](https://iiif.io/api/cookbook/recipe/0007-string-formats/) | Implemented | `FoundationRecipes.Recipe0007` | Builder, v3 shape, v2.1 writer, round-trip | HTML in descriptive properties |
| 0008 Rights Statement | [recipe/0008-rights](https://iiif.io/api/cookbook/recipe/0008-rights/) | Implemented | `FoundationRecipes.Recipe0008` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0009 Simplest Book | [recipe/0009-book-1](https://iiif.io/api/cookbook/recipe/0009-book-1/) | Implemented | `FoundationRecipes.Recipe0009` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0010 Book, Right-to-Left | [recipe/0010-book-2-viewing-direction](https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/) | Implemented | `FoundationRecipes.Recipe0010Rtl` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0010 Diary, Top-to-Bottom | [recipe/0010-book-2-viewing-direction](https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/) | Implemented | `FoundationRecipes.Recipe0010Ttb` | Builder, v3 shape, v2.1 writer, round-trip | Same recipe, second document |
| 0011 Continuous Scroll Behavior | [recipe/0011-book-3-behavior](https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/) | Implemented | `FoundationRecipes.Recipe0011Continuous` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0011 Individuals Behavior | [recipe/0011-book-3-behavior](https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/) | Implemented | `FoundationRecipes.Recipe0011Individuals` | Builder, v3 shape, v2.1 writer, round-trip | Same recipe, second document |
| 0013 Placeholder Canvas | [recipe/0013-placeholderCanvas](https://iiif.io/api/cookbook/recipe/0013-placeholderCanvas/) | Implemented | `CanvasAndStructureRecipes.Recipe0013` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0014 Accompanying Canvas | [recipe/0014-accompanyingcanvas](https://iiif.io/api/cookbook/recipe/0014-accompanyingcanvas/) | Implemented | `CanvasAndStructureRecipes.Recipe0014` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0015 Deep Linking with Start | [recipe/0015-start](https://iiif.io/api/cookbook/recipe/0015-start/) | Implemented | `CanvasAndStructureRecipes.Recipe0015` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0017 Transcription of Audio/Video | [recipe/0017-transcription-av](https://iiif.io/api/cookbook/recipe/0017-transcription-av/) | Implemented | `CanvasAndStructureRecipes.Recipe0017` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0019 HTML in Annotations | [recipe/0019-html-in-annotations](https://iiif.io/api/cookbook/recipe/0019-html-in-annotations/) | Implemented | `CanvasAndStructureRecipes.Recipe0019` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0021 Tagging | [recipe/0021-tagging](https://iiif.io/api/cookbook/recipe/0021-tagging/) | Implemented | `CanvasAndStructureRecipes.Recipe0021` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0022 Linking with a Hotspot | [recipe/0022-linking-with-a-hotspot](https://iiif.io/api/cookbook/recipe/0022-linking-with-a-hotspot/) | Implemented | `CanvasAndStructureRecipes.Recipe0022` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0024 Table of Contents for Book | [recipe/0024-book-4-toc](https://iiif.io/api/cookbook/recipe/0024-book-4-toc/) | Implemented | `CanvasAndStructureRecipes.Recipe0024` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0026 Table of Contents for Opera | [recipe/0026-toc-opera](https://iiif.io/api/cookbook/recipe/0026-toc-opera/) | Implemented | `CanvasAndStructureRecipes.Recipe0026` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0027 Alternative Page Order | [recipe/0027-alternative-page-order](https://iiif.io/api/cookbook/recipe/0027-alternative-page-order/) | Implemented | `CollectionAndChoiceRecipes.Recipe0027` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0029 Metadata Anywhere | [recipe/0029-metadata-anywhere](https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/) | Implemented | `CollectionAndChoiceRecipes.Recipe0029` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0030 Multi-Volume Work | [recipe/0030-multi-volume](https://iiif.io/api/cookbook/recipe/0030-multi-volume/) | Implemented | `CollectionAndChoiceRecipes.Recipe0030` | Builder, v3 shape, v2.1 writer, round-trip | Returns a `Collection` |
| 0031 Bound Multi-Volume Work | [recipe/0031-bound-multivolume](https://iiif.io/api/cookbook/recipe/0031-bound-multivolume/) | Implemented | `CollectionAndChoiceRecipes.Recipe0031` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0032 Simple Collection | [recipe/0032-collection](https://iiif.io/api/cookbook/recipe/0032-collection/) | Implemented | `CollectionAndChoiceRecipes.Recipe0032` | Builder, v3 shape, v2.1 writer, round-trip | Returns a `Collection` |
| 0033 Image Choice | [recipe/0033-choice](https://iiif.io/api/cookbook/recipe/0033-choice/) | Implemented | `CollectionAndChoiceRecipes.Recipe0033` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0035 Foldouts | [recipe/0035-foldouts](https://iiif.io/api/cookbook/recipe/0035-foldouts/) | Implemented | `MediaVariationRecipes.Recipe0035` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0036 Composition From Multiple Images | [recipe/0036-composition-from-multiple-images](https://iiif.io/api/cookbook/recipe/0036-composition-from-multiple-images/) | Implemented | `MediaVariationRecipes.Recipe0036` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0040 Image Rotation (Service) | [recipe/0040-image-rotation-service](https://iiif.io/api/cookbook/recipe/0040-image-rotation-service/) | Implemented | `MediaVariationRecipes.Recipe0040Service` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0040 Image Rotation (CSS) | [recipe/0040-image-rotation-service](https://iiif.io/api/cookbook/recipe/0040-image-rotation-service/) | Implemented | `MediaVariationRecipes.Recipe0040Css` | Builder, v3 shape, v2.1 writer, round-trip | Same recipe, second document |
| 0045 Simple Annotation with CSS | [recipe/0045-css](https://iiif.io/api/cookbook/recipe/0045-css/) | Implemented | `MediaVariationRecipes.Recipe0045` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0046 Rendering | [recipe/0046-rendering](https://iiif.io/api/cookbook/recipe/0046-rendering/) | Implemented | `LinkingAndOperaRecipes.Recipe0046` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0047 Linking (Homepage) | [recipe/0047-homepage](https://iiif.io/api/cookbook/recipe/0047-homepage/) | Implemented | `LinkingAndOperaRecipes.Recipe0047` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0053 Linking (SeeAlso) | [recipe/0053-seeAlso](https://iiif.io/api/cookbook/recipe/0053-seeAlso/) | Implemented | `LinkingAndOperaRecipes.Recipe0053` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0057 Presentation API 2.1 vs 3.0 | [recipe/0057-publishing-v2-and-v3](https://iiif.io/api/cookbook/recipe/0057-publishing-v2-and-v3/) | Implemented | `LinkingAndOperaRecipes.Recipe0057` | Builder, v3 shape, v2.1 writer, round-trip | Directly exercises this SDK's core multi-version feature |
| 0064 Book/Opera - One Canvas | [recipe/0064-opera-one-canvas](https://iiif.io/api/cookbook/recipe/0064-opera-one-canvas/) | Implemented | `LinkingAndOperaRecipes.Recipe0064` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0065 Book/Opera - Multiple Canvases | [recipe/0065-opera-multiple-canvases](https://iiif.io/api/cookbook/recipe/0065-opera-multiple-canvases/) | Implemented | `LinkingAndOperaRecipes.Recipe0065` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0068 Newspaper | [recipe/0068-newspaper](https://iiif.io/api/cookbook/recipe/0068-newspaper/) | Implemented | `LinkingAndOperaRecipes.Recipe0068` | Builder, v3 shape, v2.1 writer, round-trip | Real-world object recipe |
| 0074 Multiple Language Captions | [recipe/0074-multiple-language-captions](https://iiif.io/api/cookbook/recipe/0074-multiple-language-captions/) | Implemented | `LinkingAndOperaRecipes.Recipe0074` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0103 Simple Annotation on Audio | [recipe/0103-poetry-reading-annotations](https://iiif.io/api/cookbook/recipe/0103-poetry-reading-annotations/) | Implemented | `LinkingAndOperaRecipes.Recipe0103` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0117 Add a Thumbnail to a Manifest | [recipe/0117-add-image-thumbnail](https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/) | Implemented | `DescriptivePropertiesRecipes.Recipe0117` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0118 Multiple Values in Metadata | [recipe/0118-multivalue](https://iiif.io/api/cookbook/recipe/0118-multivalue/) | Implemented | `DescriptivePropertiesRecipes.Recipe0118` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0135 Annotating a Point in a Canvas | [recipe/0135-annotating-point-in-canvas](https://iiif.io/api/cookbook/recipe/0135-annotating-point-in-canvas/) | Implemented | `DescriptivePropertiesRecipes.Recipe0135` | Builder, v3 shape, v2.1 writer, round-trip | Point selector |
| 0139 Geolocate a Canvas Fragment | [recipe/0139-geolocate-canvas-fragment](https://iiif.io/api/cookbook/recipe/0139-geolocate-canvas-fragment/) | Implemented | `DescriptivePropertiesRecipes.Recipe0139` | Builder, v3 shape, v2.1 writer, round-trip | navPlace extension, Feature-as-Annotation-body |
| 0154 Simple navPlace Extension | [recipe/0154-geo-extension](https://iiif.io/api/cookbook/recipe/0154-geo-extension/) | Implemented | `DescriptivePropertiesRecipes.Recipe0154` | Builder, v3 shape, v2.1 writer, round-trip | navPlace extension |
| 0202 Manifest Start Canvas | [recipe/0202-start-canvas](https://iiif.io/api/cookbook/recipe/0202-start-canvas/) | Implemented | `DescriptivePropertiesRecipes.Recipe0202` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0219 Using a Caption File | [recipe/0219-using-caption-file](https://iiif.io/api/cookbook/recipe/0219-using-caption-file/) | Implemented | `DescriptivePropertiesRecipes.Recipe0219` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0229 Table of Contents with Behavior | [recipe/0229-behavior-ranges](https://iiif.io/api/cookbook/recipe/0229-behavior-ranges/) | Implemented | `DescriptivePropertiesRecipes.Recipe0229` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0230 NavDate on Collection/Manifest | [recipe/0230-navdate](https://iiif.io/api/cookbook/recipe/0230-navdate/) | Implemented | `ProviderAndTaggingRecipes.Recipe0230` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0232 Thumbnail on a Canvas (A/V) | [recipe/0232-image-thumbnail-canvas](https://iiif.io/api/cookbook/recipe/0232-image-thumbnail-canvas/) | Implemented | `ProviderAndTaggingRecipes.Recipe0232Av` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0232 Thumbnail on a Canvas (Image) | [recipe/0232-image-thumbnail-canvas](https://iiif.io/api/cookbook/recipe/0232-image-thumbnail-canvas/) | Implemented | `ProviderAndTaggingRecipes.Recipe0232Image` | Builder, v3 shape, v2.1 writer, round-trip | Same recipe, second document |
| 0234 Provider | [recipe/0234-provider](https://iiif.io/api/cookbook/recipe/0234-provider/) | Implemented | `ProviderAndTaggingRecipes.Recipe0234` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0240 navPlace on Canvases | [recipe/0240-navPlace-on-canvases](https://iiif.io/api/cookbook/recipe/0240-navPlace-on-canvases/) | Implemented | `ProviderAndTaggingRecipes.Recipe0240` | Builder, v3 shape, v2.1 writer, round-trip | navPlace extension, canvas-level |
| 0258 Tagging an External Resource | [recipe/0258-tagging-external-resource](https://iiif.io/api/cookbook/recipe/0258-tagging-external-resource/) | Implemented | `ProviderAndTaggingRecipes.Recipe0258` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0261 Non-Rectangular Commenting | [recipe/0261-non-rectangular-commenting](https://iiif.io/api/cookbook/recipe/0261-non-rectangular-commenting/) | Implemented | `ProviderAndTaggingRecipes.Recipe0261` | Builder, v3 shape, v2.1 writer, round-trip | SVG selector |
| 0266 Annotating the Whole Canvas | [recipe/0266-full-canvas-annotation](https://iiif.io/api/cookbook/recipe/0266-full-canvas-annotation/) | Implemented | `ProviderAndTaggingRecipes.Recipe0266` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0269 Embedded or Referenced Annotations | [recipe/0269-embedded-or-referenced-annotations](https://iiif.io/api/cookbook/recipe/0269-embedded-or-referenced-annotations/) | Implemented | `ProviderAndTaggingRecipes.Recipe0269` | Builder, v3 shape, v2.1 writer, round-trip | External AnnotationPage reference form |
| 0283 Missing Image | [recipe/0283-missing-image](https://iiif.io/api/cookbook/recipe/0283-missing-image/) | Implemented | `ProviderAndTaggingRecipes.Recipe0283` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0299 Cropping an Image (Region) | [recipe/0299-region](https://iiif.io/api/cookbook/recipe/0299-region/) | Implemented | `AnnotationCollectionRecipes.Recipe0299` | Builder, v3 shape, v2.1 writer, round-trip | Fragment/region selector |
| 0306 Linking Annotations to Manifests | [recipe/0306-linking-annotations-to-manifests](https://iiif.io/api/cookbook/recipe/0306-linking-annotations-to-manifests/) | Implemented | `AnnotationCollectionRecipes.Recipe0306` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0306 Referenced AnnotationPage | [recipe/0306-linking-annotations-to-manifests](https://iiif.io/api/cookbook/recipe/0306-linking-annotations-to-manifests/) | Implemented | `AnnotationCollectionRecipes.Recipe0306AnnotationPage` | Builder, v3 shape, v2.1 writer, round-trip | Same recipe, second document |
| 0309 Annotation Collection | [recipe/0309-annotation-collection](https://iiif.io/api/cookbook/recipe/0309-annotation-collection/) | Implemented | `AnnotationCollectionRecipes.Recipe0309` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0309 Standalone AnnotationCollection | [recipe/0309-annotation-collection](https://iiif.io/api/cookbook/recipe/0309-annotation-collection/) | Implemented | `AnnotationCollectionRecipes.Recipe0309Collection` | Builder, round-trip (falls to plain-JSON check - `AnnotationCollection` has no 2.x equivalent) | Returns a standalone `AnnotationCollection` document |
| 0318 navPlace and navDate on a Collection | [recipe/0318-navPlace-navDate](https://iiif.io/api/cookbook/recipe/0318-navPlace-navDate/) | Implemented | `AnnotationCollectionRecipes.Recipe0318` | Builder, v3 shape, v2.1 writer, round-trip | Returns a `Collection`; navPlace extension |
| 0326 Annotating a Choice of Images | [recipe/0326-annotating-image-layer](https://iiif.io/api/cookbook/recipe/0326-annotating-image-layer/) | Implemented | `AnnotationCollectionRecipes.Recipe0326` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0346 Multilingual Annotation Body | [recipe/0346-multilingual-annotation-body](https://iiif.io/api/cookbook/recipe/0346-multilingual-annotation-body/) | Implemented | `AnnotationCollectionRecipes.Recipe0346` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0377 Image in Annotation (Multiple Bodies) | [recipe/0377-image-in-annotation](https://iiif.io/api/cookbook/recipe/0377-image-in-annotation/) | Implemented | `AdvancedCompositionRecipes.Recipe0377` | Builder, v3 shape, v2.1 writer, round-trip | Multi-body Annotation |
| 0434 Choice of Audio/Video Formats | [recipe/0434-choice-av](https://iiif.io/api/cookbook/recipe/0434-choice-av/) | Implemented | `AdvancedCompositionRecipes.Recipe0434` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0464 Reusing Another Manifest's Canvas | [recipe/0464-reuse-manifest](https://iiif.io/api/cookbook/recipe/0464-reuse-manifest/) | Implemented | `AdvancedCompositionRecipes.Recipe0464` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0485 Content State - Canvas Region | [recipe/0485-contentstate-canvas-region](https://iiif.io/api/cookbook/recipe/0485-contentstate-canvas-region/) | Implemented | `AdvancedCompositionRecipes.Recipe0485` | Builder, **ContentStateCodec encode/decode round-trip** | Returns a `ContentState`, not a Manifest - "sharing" recipe |
| 0489 Multimedia Canvas | [recipe/0489-multimedia-canvas](https://iiif.io/api/cookbook/recipe/0489-multimedia-canvas/) | Implemented | `AdvancedCompositionRecipes.Recipe0489` | Builder, v3 shape, v2.1 writer, round-trip | Mixed media types on one Canvas |
| 0540 Content State - Opening Multiple Canvases | [recipe/0540-link-for-opening-multiple-canvases](https://iiif.io/api/cookbook/recipe/0540-link-for-opening-multiple-canvases/) | Implemented | `AdvancedCompositionRecipes.Recipe0540` | Builder, v3 shape, v2.1 writer, round-trip | The linked-to Manifest |
| 0540 Content State Document | [recipe/0540-link-for-opening-multiple-canvases](https://iiif.io/api/cookbook/recipe/0540-link-for-opening-multiple-canvases/) | Implemented | `AdvancedCompositionRecipes.Recipe0540ContentState` | Builder, **ContentStateCodec encode/decode round-trip** | Returns a `ContentState` - "sharing" recipe |
| 0560 Resources on a Timeline | [recipe/0560-resources-on-a-timeline](https://iiif.io/api/cookbook/recipe/0560-resources-on-a-timeline/) | Implemented | `AdvancedCompositionRecipes.Recipe0560` | Builder, v3 shape, v2.1 writer, round-trip | Time-based canvas composition |
| 0561 Visible Text Painted on an Image | [recipe/0561-text-on-image](https://iiif.io/api/cookbook/recipe/0561-text-on-image/) | Implemented | `AdvancedCompositionRecipes.Recipe0561` | Builder, v3 shape, v2.1 writer, round-trip | |
| 0599 Content State - Drag and Drop | [recipe/0599-drag-and-drop](https://iiif.io/api/cookbook/recipe/0599-drag-and-drop/) | Implemented | `AdvancedCompositionRecipes.Recipe0599` | Builder, **ContentStateCodec encode/decode round-trip** | Returns a `ContentState` - "sharing" recipe |

## Intentionally excluded (not recipes)

| Folder | Why excluded |
| --- | --- |
| [`0000_template`](https://github.com/IIIF/cookbook-recipes/tree/main/recipe/0000_template) | The repository's own authoring template for new recipes - not a real recipe. |
| [`0231-transcript-meta-recipe`](https://github.com/IIIF/cookbook-recipes/tree/main/recipe/0231-transcript-meta-recipe) | A meta/index page describing the transcript-pattern recipe group; no manifest JSON of its own. |
| [`0466-link-for-loading-manifest`](https://github.com/IIIF/cookbook-recipes/tree/main/recipe/0466-link-for-loading-manifest) | A prose explainer for how a Content State link loads a Manifest; no JSON payload to model. |

## How this inventory was verified

The official recipe list was fetched directly from the live repository
(`gh api repos/IIIF/cookbook-recipes/contents/recipe`) and diffed against every `RecipeNNNN` method
name across `examples/IIIF.Manifest.Serializer.Net.Cookbook/*Recipes.cs` - the two lists matched
exactly, with zero recipes missing and zero extras. This file should be re-verified the same way
whenever a new recipe is added upstream, and updated in the same change that adds the new
`RecipeNNNN` implementation.
