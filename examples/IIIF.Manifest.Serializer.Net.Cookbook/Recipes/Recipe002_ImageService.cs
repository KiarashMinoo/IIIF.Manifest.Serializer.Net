using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using IIIF.Manifests.Serializer.Properties.TileProperty;
using Newtonsoft.Json;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0005: Support Deep Viewing with Basic Use of a IIIF Image Service
    /// https://iiif.io/api/cookbook/recipe/0005-image-service/
    ///
    /// Demonstrates deep-zoom capabilities with a IIIF Image API service.
    /// v3 service type ImageService3 → v2 service with Image API 2 context/profile.
    /// </summary>
    public static class Recipe002_ImageService
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0005-image-service/manifest.json",
                new Label("Picture of Göttingen taken during the 2019 IIIF Conference")
            );

            // Canvas with dimensions matching the image
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0005-image-service/canvas/p1",
                new Label("Canvas with a single IIIF image"),
                3024, // height
                4032  // width
            );

            // IIIF Image API 2 Service (v3 ImageService3 mapped to v2 Image API 2 context)
            var service = new Service(
                Context.Image2.Value,
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen",
                Profile.ImageApi2Level1.Value
            )
            .SetHeight(3024)
            .SetWidth(4032);

            // Add tile information for deep zoom
            var tile = new Tile().SetWidth(512);
            tile.AddScaleFactor(1)
                .AddScaleFactor(2)
                .AddScaleFactor(4)
                .AddScaleFactor(8)
                .AddScaleFactor(16);
            service.AddTile(tile);

            // Image resource with service for deep zoom
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3024)
            .SetWidth(4032)
            .SetService(service);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0005-image-service/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0005-image-service/sequence/normal");
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

