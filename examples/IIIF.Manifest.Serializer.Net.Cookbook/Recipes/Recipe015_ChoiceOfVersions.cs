using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest.Manifest;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0015: Choice of Different Versions
    /// Adapted for Presentation API 2.0 using multiple sequences
    ///
    /// Demonstrates providing different versions of content using multiple sequences.
    /// In v2, choices can be represented through multiple sequences where users
    /// can choose between different versions of the same content.
    /// </summary>
    public static class Recipe015_ChoiceOfVersions
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/manifest.json",
                new Label("Choice of Different Versions")
            );

            // Sequence 1: High resolution version
            var sequence1 = new Sequence("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/high-res");
            sequence1.AddLabel(new Label("High Resolution Version"));

            var canvas1 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/canvas/p1-high",
                new Label("High Resolution Image"),
                3024, 4032
            );

            var imageResource1 = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);

            var image1 = new Image(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/annotation/p0001-image-high",
                imageResource1,
                canvas1.Id
            );

            canvas1.AddImage(image1);
            sequence1.AddCanvas(canvas1);
            manifest.AddSequence(sequence1);

            // Sequence 2: Low resolution version
            var sequence2 = new Sequence("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/low-res");
            sequence2.AddLabel(new Label("Low Resolution Version"));

            var canvas2 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/canvas/p1-low",
                new Label("Low Resolution Image"),
                1000, 1000
            );

            var imageResource2 = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(1000);

            var image2 = new Image(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/annotation/p0001-image-low",
                imageResource2,
                canvas2.Id
            );

            canvas2.AddImage(image2);
            sequence2.AddCanvas(canvas2);
            manifest.AddSequence(sequence2);

            return manifest;
        }

        public static string ToJson()
        {
            return JsonConvert.SerializeObject(Create(), Formatting.Indented);
        }
    }
}