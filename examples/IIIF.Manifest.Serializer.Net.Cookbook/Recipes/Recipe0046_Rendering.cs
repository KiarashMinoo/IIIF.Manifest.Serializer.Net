using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;
using Newtonsoft.Json;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0046: Providing Alternative Representations
    /// https://iiif.io/api/cookbook/recipe/0046-rendering/
    ///
    /// Demonstrates the 'rendering' property to offer alternative formats of the resource,
    /// such as a PDF version for offline reading. This is different from:
    /// - homepage: points to a web page ABOUT the object
    /// - seeAlso: provides machine-readable structured metadata
    /// - accompanyingCanvas: presents complementary IIIF content simultaneously
    /// 
    /// v3 → v2 mapping:
    ///   - v3 rendering with label language map → v2 rendering with label as string
    ///   - v3 viewingDirection → v2 viewingDirection (same property)
    /// </summary>
    [PresentationAPI("2.0", Notes = "Rendering property available in both Presentation API 2.x and 3.0. Provides alternative format representations.")]
    public static class Recipe0046_Rendering
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0046-rendering/manifest.json",
                new Label("Alternative Representations Through Rendering")
            );

            // Add description
            manifest.AddDescription(new Description(
                "Playbill for \"Akiba gongen kaisen-banashi,\" \"Futatsu chōchō kuruwa nikki\" " +
                "and \"Godairiki koi no fūjime\" performed at the Chikugo Theater in Osaka from the " +
                "fifth month of Kaei 2 (May, 1849); main actors: Gadō Kataoka II, Ebizō Ichikawa VI, " +
                "Kitō Sawamura II, Daigorō Mimasu IV and Karoku Nakamura I; on front cover: producer " +
                "Mominosuke Ichikawa's crest."
            ));

            // Set viewing direction for right-to-left reading
            manifest.SetViewingDirection(ViewingDirection.Rtl);

            // Add rendering property for PDF version
            var pdfRendering = new Rendering(
                "https://fixtures.iiif.io/other/UCLA/kabuki_ezukushi_rtl.pdf",
                "PDF version"
            ).SetFormat("application/pdf");

            manifest.AddRendering(pdfRendering);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0046-rendering/sequence/normal"
            );

            // Add 5 canvases for the complete playbill
            var canvasData = new[]
            {
                ("p1", "front cover", 4823, 3497, "001"),
                ("p2", "pages 1–2", 4804, 6062, "002"),
                ("p3", "pages 3–4", 4776, 6127, "003"),
                ("p4", "pages 5–6", 4751, 6124, "004"),
                ("p5", "back cover", 4808, 3510, "005")
            };

            foreach (var (id, label, height, width, imageNum) in canvasData)
            {
                var canvas = CreateCanvas(id, label, height, width, imageNum);
                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);

            return manifest;
        }

        private static Canvas CreateCanvas(
            string canvasId,
            string label,
            int height,
            int width,
            string imageNumber)
        {
            var canvas = new Canvas(
                $"https://iiif.io/api/cookbook/recipe/0046-rendering/canvas/{canvasId}",
                new Label(label),
                height,
                width
            );

            var imageId = $"4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_{imageNumber}";

            var imageResource = new ImageResource(
                $"https://iiif.io/api/image/3.0/example/reference/{imageId}/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(height)
            .SetWidth(width);

            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                $"https://iiif.io/api/image/3.0/example/reference/{imageId}",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            var image = new Image(
                $"https://iiif.io/api/cookbook/recipe/0046-rendering/annotation/p000{imageNumber}-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            return canvas;
        }

        public static string ToJson()
        {
            var manifest = Create();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
