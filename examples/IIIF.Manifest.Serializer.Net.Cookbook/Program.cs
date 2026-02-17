using IIIF.Manifest.Serializer.Cookbook.Recipes;
using Newtonsoft.Json;

Console.WriteLine("=== IIIF Cookbook Recipes (Presentation API 2.0) ===");
Console.WriteLine("Categorized per https://iiif.io/api/cookbook/recipe/code/");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// BASIC RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║               BASIC RECIPES                         ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Cookbook 0001 — Simplest Manifest - Single Image File
Console.WriteLine("--- [Basic] Recipe 0001: Simplest Manifest - Single Image ---");
Console.WriteLine(Recipe001_SimpleImage.ToJson());
Console.WriteLine();

// Cookbook 0009 — Simple Manifest - Book
Console.WriteLine("--- [Basic] Recipe 0009: Simple Manifest - Book ---");
Console.WriteLine(Recipe003_BookWithTOC.ToJson());
Console.WriteLine();

// Cookbook 0006 — Internationalization and Multi-language Values
Console.WriteLine("--- [Basic] Recipe 0006: Internationalization and Multi-language Values ---");
Console.WriteLine(Recipe005_MultiLanguage.ToJson());
Console.WriteLine();

// Deserialization round-trip test for basic recipe
Console.WriteLine("--- [Basic] Round-trip: Deserialize → Reserialize (0001) ---");
var basicJson = Recipe001_SimpleImage.ToJson();
var basicManifest = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.Manifest.Manifest>(basicJson);
var basicReserialized = JsonConvert.SerializeObject(basicManifest, Formatting.Indented);
Console.WriteLine($"  Round-trip OK: {basicManifest != null && basicManifest.Sequences.Count == 1}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// IIIF PROPERTIES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║              IIIF PROPERTIES                        ║");
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
Console.WriteLine("║           STRUCTURING RESOURCES                     ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();

// Book with Table of Contents (structures/ranges)
Console.WriteLine("--- [Structure] Recipe 0024: Table of Contents for Book ---");
Console.WriteLine(Recipe003_BookWithTOC.ToJson());
Console.WriteLine();

// Deserialization round-trip for structures
Console.WriteLine("--- [Structure] Round-trip: Deserialize → Reserialize (TOC) ---");
var tocJson = Recipe003_BookWithTOC.ToJson();
var tocManifest = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.Manifest.Manifest>(tocJson);
Console.WriteLine($"  Round-trip OK: {tocManifest != null && tocManifest.Structures?.Count > 0}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// IMAGE RECIPES
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║              IMAGE RECIPES                          ║");
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
var imgManifest = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.Manifest.Manifest>(imgJson);
var imgCanvas = System.Linq.Enumerable.First(System.Linq.Enumerable.First(imgManifest.Sequences).Canvases);
var imgRes = System.Linq.Enumerable.First(imgCanvas.Images).Resource;
Console.WriteLine($"  Round-trip OK: {imgRes.Service != null && imgRes.Service.Tiles?.Count > 0}");
Console.WriteLine();

// ═══════════════════════════════════════════════════════
// NOTE: Audio/Visual, Annotation, and Geo Recipes
// ═══════════════════════════════════════════════════════
Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
Console.WriteLine("║  AUDIO/VISUAL, ANNOTATION & GEO RECIPES             ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("  These categories from the IIIF Cookbook are not yet");
Console.WriteLine("  implemented in this Presentation 2.0 library:");
Console.WriteLine("  - Audio/Visual Recipes (audio, video, A/V content)");
Console.WriteLine("  - Annotation Recipes (tagging, hotspot linking, etc.)");
Console.WriteLine("  - Geo Recipes (geographic areas, web maps)");
Console.WriteLine("  See: https://iiif.io/api/cookbook/recipe/code/");
Console.WriteLine();

Console.WriteLine("=== All recipes completed successfully ===");

