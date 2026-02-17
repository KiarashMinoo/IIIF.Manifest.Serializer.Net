using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Nodes.StructureNode;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples.Examples
{
    /// <summary>
    /// Example: Creating a manifest with hierarchical structure (table of contents)
    /// </summary>
    public static class StructuredManifestExample
    {
        public static void Run()
        {
            Console.WriteLine("=== Manifest with Hierarchical Structure Example ===\n");

            // Create the manifest
            var manifest = new Manifest(
                "https://example.org/manifest/anthology",
                new Label("Poetry Anthology")
            );

            manifest.SetMetadata("Title", "Complete Poems Collection")
                    .SetMetadata("Editor", "Jane Smith")
                    .SetMetadata("Publisher", "Example Press")
                    .SetMetadata("Date", "2024")
                    .AddAttribution(new Attribution("Digital Library"));

            // Create sequence with all pages
            var sequence = new Sequence("https://example.org/sequence/normal");

            // Add 20 pages
            for (int i = 1; i <= 20; i++)
            {
                var canvas = CreateCanvas(i);
                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);

            // Create hierarchical structure for navigation
            
            // Front matter
            var frontMatter = new Structure("https://example.org/range/front")
                .AddLabel(new Label("Front Matter"))
                .AddCanvas("https://example.org/canvas/p1")  // Title page
                .AddCanvas("https://example.org/canvas/p2"); // Table of contents

            // Part 1: Nature Poems
            var part1 = new Structure("https://example.org/range/part1")
                .AddLabel(new Label("Part I: Nature"))
                .SetStartCanvas("https://example.org/canvas/p3");

            var poem1 = new Structure("https://example.org/range/poem1")
                .AddLabel(new Label("The Forest"))
                .AddCanvas("https://example.org/canvas/p3")
                .AddCanvas("https://example.org/canvas/p4")
                .AddCanvas("https://example.org/canvas/p5");

            var poem2 = new Structure("https://example.org/range/poem2")
                .AddLabel(new Label("Mountain Spring"))
                .AddCanvas("https://example.org/canvas/p6")
                .AddCanvas("https://example.org/canvas/p7");

            part1.AddRange(poem1.Id).AddRange(poem2.Id);

            // Part 2: Urban Life
            var part2 = new Structure("https://example.org/range/part2")
                .AddLabel(new Label("Part II: Urban Life"))
                .SetStartCanvas("https://example.org/canvas/p8");

            var poem3 = new Structure("https://example.org/range/poem3")
                .AddLabel(new Label("City Streets"))
                .AddCanvas("https://example.org/canvas/p8")
                .AddCanvas("https://example.org/canvas/p9")
                .AddCanvas("https://example.org/canvas/p10");

            var poem4 = new Structure("https://example.org/range/poem4")
                .AddLabel(new Label("Night Cafe"))
                .AddCanvas("https://example.org/canvas/p11")
                .AddCanvas("https://example.org/canvas/p12");

            part2.AddRange(poem3.Id).AddRange(poem4.Id);

            // Add all structures to manifest
            manifest.AddStructure(frontMatter)
                    .AddStructure(part1)
                    .AddStructure(poem1)
                    .AddStructure(poem2)
                    .AddStructure(part2)
                    .AddStructure(poem3)
                    .AddStructure(poem4);

            // Serialize to JSON
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            
            Console.WriteLine(json);
            Console.WriteLine("\n=== End of Structured Manifest Example ===\n");
        }

        private static Canvas CreateCanvas(int pageNumber)
        {
            var canvas = new Canvas(
                $"https://example.org/canvas/p{pageNumber}",
                new Label($"Page {pageNumber}"),
                1200,
                900
            );

            var imageResource = new ImageResource(
                $"https://example.org/images/p{pageNumber}.jpg",
                "image/jpeg"
            )
            .SetHeight(1200)
            .SetWidth(900);

            var image = new Image(
                $"https://example.org/annotations/p{pageNumber}-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);
            return canvas;
        }
    }
}

