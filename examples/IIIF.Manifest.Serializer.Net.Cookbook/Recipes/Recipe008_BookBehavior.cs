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
    /// IIIF Cookbook Recipe 0011: Book behavior (continuous vs individuals)
    /// https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/
    ///
    /// Demonstrates different viewer behaviors via viewingHint.
    /// v3 "behavior": ["paged"] / ["continuous"] / ["individuals"]
    /// → v2 "viewingHint": "paged" / "continuous" / "individuals".
    /// </summary>
    public static class Recipe008_BookBehavior
    {
        /// <summary>
        /// Paged book: pages displayed two-up in book-reader mode.
        /// </summary>
        public static IIIFManifest CreatePaged()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/manifest-paged.json",
                new Label("Book with Paged Behavior")
            );

            manifest.AddBehavior(Behavior.Paged);
            manifest.SetViewingDirection(ViewingDirection.Ltr);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/sequence/paged"
            );

            for (int i = 1; i <= 6; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/canvas/p{i}",
                    new Label(i == 1 ? "Front Cover" : i == 6 ? "Back Cover" : $"Page {i - 1}"),
                    1800, 1200
                );

                var imageResource = new ImageResource(
                    $"https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                    "image/jpeg"
                )
                .SetHeight(1800)
                .SetWidth(1200);

                var image = new Image(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/annotation/p{i:D4}-image",
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
        /// Continuous scroll: pages displayed sequentially without page-turn UI.
        /// </summary>
        public static IIIFManifest CreateContinuous()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/manifest-continuous.json",
                new Label("Scroll with Continuous Behavior")
            );

            manifest.AddBehavior(Behavior.Continuous);
            manifest.SetViewingDirection(ViewingDirection.Ttb);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/sequence/continuous"
            );

            for (int i = 1; i <= 4; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/canvas/scroll{i}",
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
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/annotation/scroll{i}-image",
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
        /// Individuals: each canvas displayed independently (gallery mode).
        /// </summary>
        public static IIIFManifest CreateIndividuals()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/manifest-individuals.json",
                new Label("Gallery with Individuals Behavior")
            );

            manifest.AddBehavior(Behavior.Individuals);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/sequence/individuals"
            );

            for (int i = 1; i <= 3; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/canvas/item{i}",
                    new Label($"Item {i}"),
                    2000, 1500
                );

                var imageResource = new ImageResource(
                    $"https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                    "image/jpeg"
                )
                .SetHeight(2000)
                .SetWidth(1500);

                var image = new Image(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/annotation/item{i}-image",
                    imageResource,
                    canvas.Id
                );

                canvas.AddImage(image);
                sequence.AddCanvas(canvas);
            }

            manifest.AddSequence(sequence);
            return manifest;
        }

        public static string ToJsonPaged()
        {
            return JsonConvert.SerializeObject(CreatePaged(), Formatting.Indented);
        }

        public static string ToJsonContinuous()
        {
            return JsonConvert.SerializeObject(CreateContinuous(), Formatting.Indented);
        }

        public static string ToJsonIndividuals()
        {
            return JsonConvert.SerializeObject(CreateIndividuals(), Formatting.Indented);
        }
    }
}
