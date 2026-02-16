using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest.Manifest;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0004: Image and Canvas with Differing Dimensions
    /// https://iiif.io/api/cookbook/recipe/0004-canvas-size/
    ///
    /// Demonstrates that a Canvas can have different dimensions from its image.
    /// The Canvas is the display surface; the image is scaled to fit.
    /// Canvas: 1080×1920, Image: 360×640.
    /// </summary>
    public static class Recipe004_CanvasSize
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/manifest.json",
                new Label("Image and Canvas with Differing Dimensions")
            );

            // Canvas dimensions differ from image dimensions
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/canvas/p1",
                new Label("Canvas with image of different dimensions"),
                1920, // canvas height
                1080  // canvas width
            );

            // Image has smaller dimensions than canvas — viewer scales it
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3024)
            .SetWidth(4032);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/sequence/normal"
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
