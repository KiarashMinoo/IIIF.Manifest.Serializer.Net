using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Description;
using IIIF.Manifests.Serializer.Properties.Service;
using IIIF.Manifests.Serializer.Properties.Tile;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples.Examples
{
    /// <summary>
    /// Example: Creating a multi-page book manifest with IIIF Image Service for deep zoom
    /// </summary>
    public static class BookManifestExample
    {
        public static void Run()
        {
            Console.WriteLine("=== Book Manifest with Deep Zoom Example ===\n");

            // Create the manifest for a book
            var manifest = new Manifest(
                "https://example.org/manifest/book-001",
                new Label("16th Century Manuscript")
            );

            // Add metadata
            manifest.SetMetadata("Title", "Historia Naturalis")
                    .SetMetadata("Author", "Anonymous")
                    .SetMetadata("Date", "1580")
                    .SetMetadata("Language", "Latin")
                    .AddAttribution(new Attribution("National Archive"))
                    .AddDescription(new Description("A beautifully illuminated natural history manuscript."))
                    .SetLicense(new License("https://creativecommons.org/licenses/by/4.0/"))
                    .SetThumbnail(new Thumbnail("https://example.org/thumbs/book-001.jpg"));

            // Set viewing direction for right-to-left reading
            manifest.SetViewingDirection(ViewingDirection.Rtl);

            // Create sequence
            var sequence = new Sequence("https://example.org/sequence/normal");

            // Add multiple pages
            for (int i = 1; i <= 10; i++)
            {
                var canvas = new Canvas(
                    $"https://example.org/canvas/p{i}",
                    new Label($"Page {i}"),
                    2000,
                    1500
                );

                // Add thumbnail for navigation
                canvas.SetThumbnail(new Thumbnail($"https://example.org/thumbs/p{i}.jpg"));

                // Create image resource
                var imageResource = new ImageResource(
                    $"https://example.org/iiif/p{i}/full/full/0/default.jpg",
                    "image/jpeg"
                )
                .SetHeight(2000)
                .SetWidth(1500);

                // Add IIIF Image Service for deep zoom
                var service = new Service(
                    "http://iiif.io/api/image/2/context.json",
                    $"https://example.org/iiif/p{i}",
                    "http://iiif.io/api/image/2/level2.json"
                )
                .SetHeight(2000)
                .SetWidth(1500);

                var tile1 = new Tile().SetWidth(512);
                tile1.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8).AddScaleFactor(16);
                
                var tile2 = new Tile().SetWidth(1024);
                tile2.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8);
                
                service.AddTile(tile1).AddTile(tile2);

                imageResource.SetService(service);

                // Create painting annotation
                var image = new Image(
                    $"https://example.org/annotations/p{i}-image",
                    imageResource,
                    canvas.Id
                );

                canvas.AddImage(image);
                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);

            // Serialize to JSON
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            
            Console.WriteLine(json);
            Console.WriteLine("\n=== End of Book Manifest Example ===\n");
        }
    }
}

