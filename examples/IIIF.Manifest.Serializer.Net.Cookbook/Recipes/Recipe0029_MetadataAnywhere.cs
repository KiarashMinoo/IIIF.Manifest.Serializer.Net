using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using Newtonsoft.Json;
using IIIFDescription = IIIF.Manifests.Serializer.Properties.Description;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0029: Metadata on any Resource
    /// https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/
    ///
    /// Demonstrates how to provide descriptive metadata at multiple levels:
    /// - Manifest-level metadata for overall resource information
    /// - Canvas-level metadata for view-specific details
    /// 
    /// v3 → v2 mapping:
    ///   - v3 language maps {"en": ["value"]} → v2 plain strings "value"
    ///   - v3 requiredStatement → v2 attribution
    ///   - Metadata structure remains compatible between versions
    /// </summary>
    [PresentationAPI("2.0", Notes = "Metadata property available in both Presentation API 2.x and 3.0. In v3 uses language maps, in v2 uses plain strings.")]
    public static class Recipe0029_MetadataAnywhere
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/manifest.json",
                new Label("John Dee performing an experiment before Queen Elizabeth I.")
            );

            // Add manifest-level metadata
            manifest.AddMetadata(new Metadata(
                "Creator",
                "Glindoni, Henry Gillard, 1852-1913"
            ));

            manifest.AddMetadata(new Metadata(
                "Date",
                "1800-1899"
            ));

            manifest.AddMetadata(new Metadata(
                "Physical Description",
                "1 painting : oil on canvas ; canvas 152 x 244.4 cm"
            ));

            manifest.AddMetadata(new Metadata(
                "Reference",
                "Wellcome Library no. 47369i"
            ));

            // Add attribution (v2 equivalent of v3 requiredStatement)
            manifest.AddAttribution(new Attribution(
                "Wellcome Collection. Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)"
            ));

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/sequence/normal"
            );

            // Canvas 1: Painting under natural light
            var canvas1 = CreateCanvasWithMetadata(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/canvas/p1",
                "Painting under natural light",
                1271,
                2000,
                "The scene is the house at Mortlake of Dr John Dee (1527-1608). At the court of Queen Elizabeth I, " +
                "Dee was revered for the range of his scientific knowledge, which embraced the fields of mathematics, " +
                "navigation, geography, alchemy/chemistry, medicine and optics. In the painting he is showing the effect " +
                "of combining two elements, either to cause combustion or to extinguish it. Behind him is his assistant " +
                "Edward Kelly, wearing a long skullcap to conceal the fact that his ears had been cropped as a punishment for forgery.",
                "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural/full/max/0/default.jpg",
                "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural",
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/annotation/p0001-image"
            );

            sequence.AddCanvas(canvas1);

            // Canvas 2: X-ray view of painting
            var canvas2 = CreateCanvasWithMetadata(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/canvas/p2",
                "X-ray view of painting",
                1271,
                2000,
                "The painting originally showed Dee standing in a circle of skulls on the floor, stretching from the floor area " +
                "in front of the Queen (on the left) to the floor near Edward Kelly (on the right). The skulls were at an early stage " +
                "painted over, but have since become visible. Another pentimento is visible in the tapestry on the right: shelves " +
                "containing monstrous animals are visible behind it. The pentimenti were clarified when the painting was X-rayed in 2015.",
                "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray/full/max/0/default.jpg",
                "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray",
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/annotation/p0002-image"
            );

            sequence.AddCanvas(canvas2);

            manifest.AddSequence(sequence);

            return manifest;
        }

        private static Canvas CreateCanvasWithMetadata(
            string canvasId,
            string canvasLabel,
            int height,
            int width,
            string description,
            string imageId,
            string serviceId,
            string annotationId)
        {
            var canvas = new Canvas(canvasId, new Label(canvasLabel), height, width);

            // Add canvas-specific metadata
            canvas.AddMetadata(new Metadata("Description", description));

            // Create image with service
            var imageResource = new ImageResource(imageId, "image/jpeg")
                .SetHeight(height)
                .SetWidth(width);

            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                serviceId,
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            var image = new Image(annotationId, imageResource, canvas.Id);
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
