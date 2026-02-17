using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.ContentNode.OtherContent;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0014: Non-Rectangular Selection
    /// Adapted for Presentation API 2.0 using external annotation lists
    ///
    /// Demonstrates non-rectangular annotation targets using external annotation lists
    /// with fragment selectors. In v2, this is achieved through otherContent referencing
    /// annotation lists that contain annotations with selectors.
    /// </summary>
    public static class Recipe014_NonRectangularSelection
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/manifest.json",
                new Label("Non-Rectangular Selection with External Annotations")
            );

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/sequence/normal");

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/canvas/p1",
                new Label("Canvas with Non-Rectangular Annotations"),
                3024, // height
                4032  // width
            );

            // Add the main image
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3024)
            .SetWidth(4032);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Add external annotation list for non-rectangular selections
            // In v2, this would reference an external annotation list containing
            // annotations with selectors for non-rectangular regions
            var otherContent = new OtherContent(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/list/p1"
            );
            canvas.AddOtherContent(otherContent);

            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            return manifest;
        }

        public static string ToJson()
        {
            return JsonConvert.SerializeObject(Create(), Formatting.Indented);
        }
    }
}
