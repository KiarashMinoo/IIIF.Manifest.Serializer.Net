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
    /// IIIF Cookbook Recipe 0010: Viewing Direction and Its Effect on Navigation
    /// https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/
    ///
    /// Demonstrates right-to-left and top-to-bottom viewing directions.
    /// v3 "viewingDirection" → v2 "viewingDirection" (values are identical).
    /// </summary>
    public static class Recipe007_ViewingDirection
    {
        /// <summary>
        /// Creates a right-to-left manifest (e.g., Hebrew/Arabic book).
        /// </summary>
        public static IIIFManifest CreateRtl()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/manifest-rtl.json",
                new Label("Book with Right-to-Left Viewing Direction")
            );

            manifest.SetViewingDirection(ViewingDirection.Rtl);
            manifest.AddBehavior(Behavior.Paged);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/sequence/rtl"
            );

            for (int i = 1; i <= 4; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/p{i}",
                    new Label($"Page {i}"),
                    1800, 1200
                );

                var imageResource = new ImageResource(
                    $"https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                    "image/jpeg"
                )
                .SetHeight(1800)
                .SetWidth(1200);

                var image = new Image(
                    $"https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/annotation/p{i:D4}-image",
                    imageResource,
                    canvas.Id
                );

                canvas.AddImage(image);
                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);
            return manifest;
        }

        /// <summary>
        /// Creates a top-to-bottom manifest (e.g., scroll).
        /// </summary>
        public static IIIFManifest CreateTtb()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/manifest-ttb.json",
                new Label("Scroll with Top-to-Bottom Viewing Direction")
            );

            manifest.SetViewingDirection(ViewingDirection.Ttb);
            manifest.AddBehavior(Behavior.Continuous);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/sequence/ttb"
            );

            for (int i = 1; i <= 3; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/scroll{i}",
                    new Label($"Section {i}"),
                    3000, 1200
                );

                var imageResource = new ImageResource(
                    $"https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                    "image/jpeg"
                )
                .SetHeight(3000)
                .SetWidth(1200);

                var image = new Image(
                    $"https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/annotation/scroll{i}-image",
                    imageResource,
                    canvas.Id
                );

                canvas.AddImage(image);
                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);
            return manifest;
        }

        public static string ToJsonRtl()
        {
            return JsonConvert.SerializeObject(CreateRtl(), Formatting.Indented);
        }

        public static string ToJsonTtb()
        {
            return JsonConvert.SerializeObject(CreateTtb(), Formatting.Indented);
        }
    }
}
