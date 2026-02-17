using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.DescriptionProperty;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples.Examples
{
    /// <summary>
    /// Example: Creating a simple single-image manifest
    /// </summary>
    public static class SingleImageExample
    {
        public static void Run()
        {
            Console.WriteLine("=== Single Image Manifest Example ===\n");

            // Create the manifest
            var manifest = new Manifest(
                "https://example.org/manifest/single-image",
                new Label("Single Page Document")
            );

            // Add metadata
            manifest.SetMetadata("Title", "Example Document")
                    .SetMetadata("Author", "John Doe")
                    .SetMetadata("Date", "2024")
                    .AddAttribution(new Attribution("Example Institution"))
                    .AddDescription(new Description("A simple single-page document example."));

            // Create a canvas with dimensions
            var canvas = new Canvas(
                "https://example.org/canvas/p1",
                new Label("Page 1"),
                1000,
                800
            );

            // Add a thumbnail to the canvas
            canvas.SetThumbnail(new Thumbnail("https://example.org/thumbs/p1.jpg"));

            // Create the image resource
            var imageResource = new ImageResource(
                "https://example.org/images/page1.jpg",
                "image/jpeg"
            )
            .SetHeight(1000)
            .SetWidth(800);

            // Create the painting annotation that links the image to the canvas
            var image = new Image(
                "https://example.org/annotations/p1-image",
                imageResource,
                canvas.Id
            );

            // Assemble the hierarchy
            canvas.AddImage(image);

            var sequence = new Sequence("https://example.org/sequence/normal");
            sequence.AddCanvas(canvas);

            manifest.AddSequence(sequence);

            // Serialize to JSON
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            
            Console.WriteLine(json);
            Console.WriteLine("\n=== End of Single Image Example ===\n");
        }
    }
}

