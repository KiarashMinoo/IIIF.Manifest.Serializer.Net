using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0008: Rights Statement
    /// https://iiif.io/api/cookbook/recipe/0008-rights/
    ///
    /// Demonstrates attaching a rights/license statement and
    /// attribution/requiredStatement to a manifest.
    /// v3 "rights" + "requiredStatement" → v2 "license" + "attribution".
    /// </summary>
    public static class Recipe006_Rights
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0008-rights/manifest.json",
                new Label("Picture of Göttingen taken during the 2019 IIIF Conference")
            );

            // v2 "license" maps to v3 "rights"
            manifest.SetLicense(
                new License("http://creativecommons.org/licenses/by-sa/3.0/")
            );

            // v2 "attribution" maps to v3 "requiredStatement"
            manifest.AddAttribution(
                new Attribution("Götttingen, Lower Saxony, Germany. Taken by the IIIF community during the 2019 IIIF Conference.")
            );

            // Canvas with photo
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0008-rights/canvas/p1",
                new Label("Canvas with rights statement"),
                3024, // height
                4032  // width
            );

            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3024)
            .SetWidth(4032);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0008-rights/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0008-rights/sequence/normal"
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
