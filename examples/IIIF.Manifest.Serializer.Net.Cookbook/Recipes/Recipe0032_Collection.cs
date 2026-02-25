using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using Newtonsoft.Json;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0032: Simple Collection
    /// https://iiif.io/api/cookbook/recipe/0032-collection/
    ///
    /// Demonstrates how to organize multiple manifests into a Collection.
    /// Collections are top-level resources that group related manifests together,
    /// such as a named digital collection, search results, or items sharing metadata.
    /// 
    /// v3 → v2 mapping:
    ///   - v3 items array → v2 manifests array (manifest references)
    ///   - v3 items with Collections → v2 collections array (nested collections)
    ///   - Both versions support metadata, thumbnail, and other descriptive properties
    /// </summary>
    public static class Recipe0032_Collection
    {
        public static Collection CreateCollection()
        {
            var collection = new Collection(
                "https://iiif.io/api/cookbook/recipe/0032-collection/collection.json",
                new Label("Simple Collection Example")
            );

            // Add manifest references
            // Note: In v2, collections contain @id references to manifests, not full manifests
            collection.AddManifest("https://iiif.io/api/cookbook/recipe/0032-collection/manifest-01.json");
            collection.AddManifest("https://iiif.io/api/cookbook/recipe/0032-collection/manifest-02.json");

            return collection;
        }

        public static IIIFManifest CreateManifest1()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest-01.json",
                new Label("The Gulf Stream")
            );

            // Add metadata
            manifest.AddMetadata(new Metadata("Artist", "Winslow Homer (1836–1910)"));
            manifest.AddMetadata(new Metadata("Date", "1899"));

            // Create canvas
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest/1/canvas/p1",
                new Label("The Gulf Stream"),
                3540,  // height
                5886   // width
            );

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Winslow_Homer_-_The_Gulf_Stream_-_Metropolitan_Museum_of_Art/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3540)
            .SetWidth(5886);

            // Add IIIF Image Service
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Winslow_Homer_-_The_Gulf_Stream_-_Metropolitan_Museum_of_Art",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            // Create painting annotation
            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest/1/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Add to sequence
            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest/1/sequence/normal"
            );
            sequence.AddCanvas(canvas);

            manifest.AddSequence(sequence);

            return manifest;
        }

        public static IIIFManifest CreateManifest2()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest-02.json",
                new Label("Northeaster")
            );

            // Add metadata
            manifest.AddMetadata(new Metadata("Artist", "Winslow Homer (1836–1910)"));
            manifest.AddMetadata(new Metadata("Date", "1895"));

            // Create canvas
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest/2/canvas/p1",
                new Label("Northeaster"),
                2572,  // height
                3764   // width
            );

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Northeaster_by_Winslow_Homer_1895/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(2572)
            .SetWidth(3764);

            // Add IIIF Image Service
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Northeaster_by_Winslow_Homer_1895",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            // Create painting annotation
            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest/2/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Add to sequence
            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0032-collection/manifest/2/sequence/normal"
            );
            sequence.AddCanvas(canvas);

            manifest.AddSequence(sequence);

            return manifest;
        }

        public static string ToJsonCollection()
        {
            var collection = CreateCollection();
            return JsonConvert.SerializeObject(collection, Formatting.Indented);
        }

        public static string ToJsonManifest1()
        {
            var manifest = CreateManifest1();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }

        public static string ToJsonManifest2()
        {
            var manifest = CreateManifest2();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
