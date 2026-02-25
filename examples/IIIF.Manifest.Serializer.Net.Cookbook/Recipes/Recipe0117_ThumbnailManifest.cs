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
    /// IIIF Cookbook Recipe 0117: Image Thumbnail for Manifest
    /// https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/
    ///
    /// Demonstrates how to add a thumbnail to a Manifest to provide a representative image
    /// for discovery interfaces and catalogs. The thumbnail can be different from the first canvas.
    /// 
    /// v3 → v2 mapping:
    ///   - v3 thumbnail array → v2 thumbnail property
    ///   - v3 ImageService3 → v2 IIIF Image API 2.0 service
    /// </summary>
    public static class Recipe0117_ThumbnailManifest
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/manifest.json",
                new Label("Playbill Cover with Manifest Thumbnail")
            );

            // Add description (v3 summary → v2 description)
            manifest.AddDescription(new Description(
                "Cover of playbill for \"Akiba gongen kaisen-banashi,\" \"Futatsu chōchō kuruwa nikki\" " +
                "and \"Godairiki koi no fūjime\" performed at the Chikugo Theater in Osaka from the " +
                "fifth month of Kaei 2 (May, 1849); main actors: Gadō Kataoka II, Ebizō Ichikawa VI, " +
                "Kitō Sawamura II, Daigorō Mimasu IV and Karoku Nakamura I; on front cover: producer " +
                "Mominosuke Ichikawa's crest."
            ));

            // Set manifest-level thumbnail
            // In v2, thumbnail is a simple URI or object with @id
            // The v3 cookbook example includes full service details
            var thumbnailService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001",
                "http://iiif.io/api/image/2/level1.json"
            );

            // Add tile configuration for deep zoom
            thumbnailService.AddTile(new Tile()
                .SetWidth(512)
                .AddScaleFactor(1)
                .AddScaleFactor(2)
                .AddScaleFactor(4)
                .AddScaleFactor(8));

            var thumbnail = new Thumbnail(
                "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001/full/max/0/default.jpg"
            );

            manifest.SetThumbnail(thumbnail);

            // Create the canvas with the actual content (which includes color bar in this example)
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/canvas/p0",
                new Label("front cover with color bar"),
                5312,  // height
                4520   // width
            );

            // Create image resource with IIIF Image Service
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001_full/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(5312)
            .SetWidth(4520);

            // Add image service to the resource
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001_full",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            // Create annotation linking image to canvas
            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/annotation/p0000-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Add canvas to sequence
            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/sequence/normal"
            );
            sequence.AddCanvas(canvas);

            manifest.AddSequence(sequence);

            return manifest;
        }

        public static string ToJson()
        {
            var manifest = Create();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
