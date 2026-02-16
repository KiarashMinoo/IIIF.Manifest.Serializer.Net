using IIIF.Manifest.Serializer.Cookbook.Recipes;

Console.WriteLine("=== IIIF Cookbook Recipes (Presentation API 2.0) ===");
Console.WriteLine();

// Recipe 001 — Simple Image (cookbook 0001)
Console.WriteLine("--- Recipe 001: Simplest Manifest - Single Image ---");
Console.WriteLine(Recipe001_SimpleImage.ToJson());
Console.WriteLine();

// Recipe 002 — Image with IIIF Image Service (cookbook 0005)
Console.WriteLine("--- Recipe 002: Support Deep Viewing with IIIF Image Service ---");
Console.WriteLine(Recipe002_ImageService.ToJson());
Console.WriteLine();

// Recipe 003 — Book with Table of Contents (cookbook 0009)
Console.WriteLine("--- Recipe 003: Simple Manifest - Book ---");
Console.WriteLine(Recipe003_BookWithTOC.ToJson());
Console.WriteLine();

// Recipe 004 — Canvas with Differing Dimensions (cookbook 0004)
Console.WriteLine("--- Recipe 004: Image and Canvas with Differing Dimensions ---");
Console.WriteLine(Recipe004_CanvasSize.ToJson());
Console.WriteLine();

// Recipe 005 — Multi-language / Internationalization (cookbook 0006)
Console.WriteLine("--- Recipe 005: Internationalization and Multi-language Values ---");
Console.WriteLine(Recipe005_MultiLanguage.ToJson());
Console.WriteLine();

// Recipe 006 — Rights Statement (cookbook 0008)
Console.WriteLine("--- Recipe 006: Rights Statement ---");
Console.WriteLine(Recipe006_Rights.ToJson());
Console.WriteLine();

// Recipe 007 — Viewing Direction (cookbook 0010)
Console.WriteLine("--- Recipe 007a: Right-to-Left Viewing Direction ---");
Console.WriteLine(Recipe007_ViewingDirection.ToJsonRtl());
Console.WriteLine();
Console.WriteLine("--- Recipe 007b: Top-to-Bottom Viewing Direction ---");
Console.WriteLine(Recipe007_ViewingDirection.ToJsonTtb());
Console.WriteLine();

// Recipe 008 — Book Behavior (cookbook 0011)
Console.WriteLine("--- Recipe 008a: Paged Behavior ---");
Console.WriteLine(Recipe008_BookBehavior.ToJsonPaged());
Console.WriteLine();
Console.WriteLine("--- Recipe 008b: Continuous Behavior ---");
Console.WriteLine(Recipe008_BookBehavior.ToJsonContinuous());
Console.WriteLine();
Console.WriteLine("--- Recipe 008c: Individuals Behavior ---");
Console.WriteLine(Recipe008_BookBehavior.ToJsonIndividuals());
Console.WriteLine();

Console.WriteLine("=== All recipes completed successfully ===");
