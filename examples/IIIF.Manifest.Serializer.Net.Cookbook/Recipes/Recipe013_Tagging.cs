using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Content.OtherContent;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0013: Tagging with Text
    /// https://iiif.io/api/cookbook/recipe/0013-tagging/
    ///
    /// Demonstrates how to add external annotation lists to canvases
    /// for tagging and other annotations.
    /// </summary>
    public static class Recipe013_Tagging
    {
        public static string ToJson()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/manifest.json",
                new Label("Tagging with Text")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/canvas/p1",
                new Label("Canvas with Tags"),
                1000, 1000
            );

            // Add the main image
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(1000);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/annotation/p0001-image",
                imageResource,
                canvas.Id
            );
            canvas.AddImage(image);

            // Add external annotation list for tagging
            var annotationList = new OtherContent(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/list/p1"
            );
            canvas.AddOtherContent(annotationList);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0013-tagging/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}