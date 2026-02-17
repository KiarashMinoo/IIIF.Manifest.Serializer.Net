using IIIF.Manifests.Serializer.Examples.Cookbook;
using IIIF.Manifests.Serializer.Net.Cookbook.Recipes;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;

Console.WriteLine("=== IIIF Cookbook Recipes (Presentation API 2.0) ===");
Console.WriteLine("Categorized per https://iiif.io/api/cookbook/recipe/code/");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// BASIC RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║               BASIC RECIPES                           ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Cookbook 0001 — Simplest Manifest - Single Image File
Console.WriteLine("--- [Basic] Recipe 0001: Simplest Manifest - Single Image ---");
Console.WriteLine(Recipe001_SimpleImage.ToJson());
Console.WriteLine();

// Cookbook 0003 — Simplest Manifest - Video
Console.WriteLine("--- [Basic] Recipe 0003: Simplest Manifest - Video ---");
Console.WriteLine(Recipe003_Video.ToJson());
Console.WriteLine();

// Cookbook 0009 — Simple Manifest - Book
Console.WriteLine("--- [Basic] Recipe 0009: Simple Manifest - Book ---");
Console.WriteLine(Recipe003_BookWithTOC.ToJson());
Console.WriteLine();

// Cookbook 0006 — Internationalization and Multi-language Values
Console.WriteLine("--- [Basic] Recipe 0006: Internationalization and Multi-language Values ---");
Console.WriteLine(Recipe005_MultiLanguage.ToJson());
Console.WriteLine();

// Cookbook 0002 — Simplest Manifest - Audio
Console.WriteLine("--- [Basic] Recipe 0002: Simplest Manifest - Audio ---");
Console.WriteLine(Recipe002_Audio.ToJson());
Console.WriteLine();

// Deserialization round-trip test for basic recipe
Console.WriteLine("--- [Basic] Round-trip: Deserialize → Reserialize (0001) ---");
var basicJson = Recipe001_SimpleImage.ToJson();
var basicManifest = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest>(basicJson);
var basicReserialized = JsonConvert.SerializeObject(basicManifest, Formatting.Indented);
Console.WriteLine($"  Round-trip OK: {basicManifest != null && basicManifest.Sequences.Count == 1}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// IIIF PROPERTIES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║              IIIF PROPERTIES                          ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Cookbook 0008 — Rights Statement
Console.WriteLine("--- [Properties] Recipe 0008: Rights Statement ---");
Console.WriteLine(Recipe006_Rights.ToJson());
Console.WriteLine();

// Cookbook 0010 — Viewing Direction
Console.WriteLine("--- [Properties] Recipe 0010a: Right-to-Left Viewing Direction ---");
Console.WriteLine(Recipe007_ViewingDirection.ToJsonRtl());
Console.WriteLine();
Console.WriteLine("--- [Properties] Recipe 0010b: Top-to-Bottom Viewing Direction ---");
Console.WriteLine(Recipe007_ViewingDirection.ToJsonTtb());
Console.WriteLine();

// Cookbook 0011 — Book Behavior Variations
Console.WriteLine("--- [Properties] Recipe 0011a: Paged Behavior ---");
Console.WriteLine(Recipe008_BookBehavior.ToJsonPaged());
Console.WriteLine();
Console.WriteLine("--- [Properties] Recipe 0011b: Continuous Behavior ---");
Console.WriteLine(Recipe008_BookBehavior.ToJsonContinuous());
Console.WriteLine();
Console.WriteLine("--- [Properties] Recipe 0011c: Individuals Behavior ---");
Console.WriteLine(Recipe008_BookBehavior.ToJsonIndividuals());
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// STRUCTURING RESOURCES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║           STRUCTURING RESOURCES                       ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Book with Table of Contents (structures/ranges)
Console.WriteLine("--- [Structure] Recipe 0024: Table of Contents for Book ---");
Console.WriteLine(Recipe003_BookWithTOC.ToJson());
Console.WriteLine();

// Deserialization round-trip for structures
Console.WriteLine("--- [Structure] Round-trip: Deserialize → Reserialize (TOC) ---");
var tocJson = Recipe003_BookWithTOC.ToJson();
var tocManifest = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest>(tocJson);
Console.WriteLine($"  Round-trip OK: {tocManifest != null && tocManifest.Structures?.Count > 0}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// IMAGE RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║              IMAGE RECIPES                            ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Cookbook 0001 — Single Image (also in basic)
Console.WriteLine("--- [Image] Recipe 0001: Simplest Manifest - Single Image ---");
Console.WriteLine(Recipe001_SimpleImage.ToJson());
Console.WriteLine();

// Cookbook 0004 — Canvas and Image Differing Dimensions
Console.WriteLine("--- [Image] Recipe 0004: Image and Canvas with Differing Dimensions ---");
Console.WriteLine(Recipe004_CanvasSize.ToJson());
Console.WriteLine();

// Cookbook 0005 — Deep Viewing / IIIF Image Service
Console.WriteLine("--- [Image] Recipe 0005: Support Deep Viewing with IIIF Image Service ---");
Console.WriteLine(Recipe002_ImageService.ToJson());
Console.WriteLine();

// Deserialization round-trip for image service
Console.WriteLine("--- [Image] Round-trip: Deserialize → Reserialize (Image Service) ---");
var imgJson = Recipe002_ImageService.ToJson();
var imgManifest = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest>(imgJson);
var imgCanvas = System.Linq.Enumerable.First(System.Linq.Enumerable.First(imgManifest.Sequences).Canvases);
var imgRes = System.Linq.Enumerable.First(imgCanvas.Images).Resource;
Console.WriteLine($"  Round-trip OK: {imgRes.Service is Service { Tiles.Count: > 0 }}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// ANNOTATION RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║              ANNOTATION RECIPES                       ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Cookbook 0013 — Tagging with Text
Console.WriteLine("--- [Annotation] Recipe 0013: Tagging with Text ---");
Console.WriteLine(Recipe013_Tagging.ToJson());
Console.WriteLine();

// Cookbook 0014 — Non-Rectangular Selection
Console.WriteLine("--- [Annotation] Recipe 0014: Non-Rectangular Selection ---");
Console.WriteLine(Recipe014_NonRectangularSelection.ToJson());
Console.WriteLine();

// Cookbook 0015 — Choice of Different Versions
Console.WriteLine("--- [Annotation] Recipe 0015: Choice of Different Versions ---");
Console.WriteLine(Recipe015_ChoiceOfVersions.ToJson());
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// AUTHENTICATION RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║            AUTHENTICATION RECIPES                     ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Auth Recipe 1 — IIIF Auth 1.0 Login Pattern
Console.WriteLine("--- [Authentication] Auth 1: Login Authentication Pattern (Auth 1.0) ---");
Console.WriteLine(RecipeAuth01_LoginAuth1.ToJson());
Console.WriteLine();

// Auth Recipe 2 — IIIF Auth 1.0 Clickthrough Pattern
Console.WriteLine("--- [Authentication] Auth 2: Clickthrough Authentication Pattern (Auth 1.0) ---");
Console.WriteLine(RecipeAuth02_ClickthroughAuth1.ToJson());
Console.WriteLine();

// Auth Recipe 3 — IIIF Auth 2.0 Active Pattern
Console.WriteLine("--- [Authentication] Auth 3: Active Authentication Pattern (Auth 2.0) ---");
Console.WriteLine(RecipeAuth03_ActiveAuth2.ToJson());
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// SEARCH & DISCOVERY RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║         SEARCH & DISCOVERY RECIPES                    ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Search Recipe 1 — IIIF Content Search API 2.0
Console.WriteLine("--- [Search] Recipe Search 1: Content Search API 2.0 ---");
Console.WriteLine(RecipeSearch01_ContentSearch.ToJson());
Console.WriteLine();

// Discovery Recipe 1 — IIIF Change Discovery API 1.0
Console.WriteLine("--- [Discovery] Recipe Discovery 1: Change Discovery API 1.0 ---");
Console.WriteLine(RecipeDiscovery01_ChangeDiscovery.ToJson());
Console.WriteLine();

// State Recipe 1 — IIIF Content State API 1.0
Console.WriteLine("--- [State] Recipe State 1: Content State API 1.0 ---");
Console.WriteLine(RecipeState01_ContentState.ToJson());
Console.WriteLine();
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║  AUDIO/VISUAL, ANNOTATION & GEO RECIPES               ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("  These categories from the IIIF Cookbook are not yet");
Console.WriteLine("  fully implemented in this Presentation 2.0 library:");
Console.WriteLine("  - Audio/Visual Recipes (basic audio/video implemented; complex A/V content pending)");
Console.WriteLine("  - Annotation Recipes (Recipes 0013 tagging, 0014 non-rectangular selection, and 0015 choice of versions implemented; complex inline annotations pending)");
Console.WriteLine("  - Geo Recipes (geographic areas, web maps)");
Console.WriteLine("  See: https://iiif.io/api/cookbook/recipe/code/");
Console.WriteLine();

Console.WriteLine("=== All recipes completed successfully ===");

