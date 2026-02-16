using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Collection;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Description;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples.Examples
{
    /// <summary>
    /// Example: Creating a Collection with multiple manifests and viewing hints
    /// </summary>
    public static class CollectionExample
    {
        public static void Run()
        {
            Console.WriteLine("=== Collection with Multiple Manifests Example ===\n");

            // Create the parent collection
            var collection = new Collection(
                "https://example.org/collection/library",
                new Label("Digital Library Collection")
            );

            collection.AddMetadata(new Properties.Metadata.Metadata("Institution", "Example University"))
                      .AddMetadata(new Properties.Metadata.Metadata("Curator", "Dr. Jane Smith"))
                      .AddDescription(new Description("A curated collection of historical manuscripts and documents."))
                      .AddAttribution(new Attribution("Example University Library"))
                      .SetViewingHint(ViewingHint.MultiPart)
                      .SetTotal(25); // Total items, only showing some

            // Create subcollection for manuscripts
            var manuscriptsCollection = new Collection(
                "https://example.org/collection/manuscripts",
                new Label("Medieval Manuscripts")
            );
            manuscriptsCollection.SetViewingHint(ViewingHint.Paged);
            
            // Add manifest references to the manuscripts collection
            manuscriptsCollection.AddManifest("https://example.org/manifest/ms-001");
            manuscriptsCollection.AddManifest("https://example.org/manifest/ms-002");
            manuscriptsCollection.AddManifest("https://example.org/manifest/ms-003");

            // Create subcollection for maps
            var mapsCollection = new Collection(
                "https://example.org/collection/maps",
                new Label("Historical Maps")
            );
            mapsCollection.SetViewingHint(ViewingHint.Individuals);
            
            mapsCollection.AddManifest("https://example.org/manifest/map-001");
            mapsCollection.AddManifest("https://example.org/manifest/map-002");

            // Create an actual embedded manifest example
            var sampleManifest = CreateSampleManifest();
            
            // Add subcollections to parent
            collection.AddCollection(manuscriptsCollection);
            collection.AddCollection(mapsCollection);

            // Serialize to JSON
            var json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            
            Console.WriteLine("Collection JSON:");
            Console.WriteLine(json);
            
            Console.WriteLine("\n\n=== Sample Manifest with Viewing Hints ===\n");
            var manifestJson = JsonConvert.SerializeObject(sampleManifest, Formatting.Indented);
            Console.WriteLine(manifestJson);

            Console.WriteLine("\n=== End of Collection Example ===\n");
        }

        private static Manifest CreateSampleManifest()
        {
            var manifest = new Manifest(
                "https://example.org/manifest/book-paged",
                new Label("Paged Book Example")
            );

            manifest.SetViewingHint(ViewingHint.Paged)
                    .SetViewingDirection(ViewingDirection.Ltr)
                    .SetMetadata("Title", "Example Book")
                    .SetMetadata("Date", "1650");

            var sequence = new Sequence("https://example.org/sequence/s1");

            // First canvas - cover page marked as "top"
            var coverCanvas = new Canvas(
                "https://example.org/canvas/cover",
                new Label("Front Cover"),
                1000,
                800
            );
            coverCanvas.SetViewingHint(ViewingHint.Top);

            var coverResource = new ImageResource(
                "https://example.org/images/cover.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(800);

            coverCanvas.AddImage(new Image(
                "https://example.org/anno/cover",
                coverResource,
                coverCanvas.Id
            ));

            sequence.AddCanvas(coverCanvas);

            // Regular pages with facing-pages hint
            for (int i = 1; i <= 4; i++)
            {
                var canvas = new Canvas(
                    $"https://example.org/canvas/p{i}",
                    new Label($"Page {i}"),
                    1000,
                    800
                );

                if (i % 2 == 1 && i < 4) // Odd pages that have a facing page
                {
                    canvas.SetViewingHint(ViewingHint.FacingPages);
                }

                var resource = new ImageResource(
                    $"https://example.org/images/page{i}.jpg",
                    "image/jpeg"
                ).SetHeight(1000).SetWidth(800);

                canvas.AddImage(new Image(
                    $"https://example.org/anno/p{i}",
                    resource,
                    canvas.Id
                ));

                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);
            return manifest;
        }
    }
}

