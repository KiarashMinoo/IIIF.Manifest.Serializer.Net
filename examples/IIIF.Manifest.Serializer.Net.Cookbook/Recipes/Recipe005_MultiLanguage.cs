using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest.Manifest;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Metadata;
using IIIFDescription = IIIF.Manifests.Serializer.Properties.Description.Description;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0006: Internationalization and Multi-language Values
    /// https://iiif.io/api/cookbook/recipe/0006-text-language/
    ///
    /// Demonstrates multilingual metadata and descriptions.
    /// v3 uses language maps ({ "en": [...], "fr": [...] }); v2 uses
    /// @value/@language objects in metadata values.
    /// </summary>
    public static class Recipe005_MultiLanguage
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/manifest.json",
                new Label("Whistler's Mother")
            );

            // Multi-language descriptions (v2 pattern: Description with @language)
            manifest.AddDescription(
                new IIIFDescription("Arrangement in Grey and Black No.1, commonly known as Whistler's Mother")
                    .SetLanguage("en")
            );
            manifest.AddDescription(
                new IIIFDescription("Arrangement en gris et noir n°1, communément appelé la Mère de Whistler")
                    .SetLanguage("fr")
            );

            // Multi-language metadata
            manifest.AddMetadata(new Metadata("Creator", "James Abbott McNeill Whistler", "en"));
            manifest.AddMetadata(new Metadata("Date", "1871"));

            // Canvas with the painting
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/canvas/p1",
                new Label("Whistler's Mother"),
                3405, // height
                4096  // width
            );

            var imageResource = new ImageResource(
                "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1b/Whistlers_Mother_high_res.jpg/1024px-Whistlers_Mother_high_res.jpg",
                "image/jpeg"
            )
            .SetHeight(3405)
            .SetWidth(4096);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/sequence/normal"
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
