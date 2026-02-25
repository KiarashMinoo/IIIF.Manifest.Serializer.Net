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
    /// IIIF Cookbook Recipe 0047: Linking to Web Page of an Object
    /// https://iiif.io/api/cookbook/recipe/0047-homepage/
    ///
    /// Demonstrates the 'homepage' property to provide a link to the institutional
    /// web page about the object. This is the landing page where users can find more
    /// information about the cultural heritage object.
    /// 
    /// Different from:
    /// - rendering: provides alternative format of the SAME resource (PDF, ePub, etc.)
    /// - seeAlso: provides machine-readable structured metadata
    /// - related: general related links
    /// 
    /// v3 → v2 mapping:
    ///   - v3 homepage with language/format → v2 homepage with label/format
    ///   - v3 label language map → v2 label as string
    /// </summary>
    public static class Recipe0047_Homepage
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0047-homepage/manifest.json",
                new Label("Laocöon")
            );

            // Add homepage linking to the Getty Museum catalog entry
            var homepage = new Homepage(
                "https://www.getty.edu/art/collection/object/103RQQ",
                "Home page at the Getty Museum Collection"
            ).SetFormat("text/html");

            manifest.AddHomepage(homepage);

            // Create canvas with image
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0047-homepage/canvas/1",
                new Label("Front"),
                3000,  // height
                2315   // width
            );

            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon/full/!500,500/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3000)
            .SetWidth(2315);

            // Add IIIF Image Service
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0047-homepage/canvas/1/page/1/annotation/1",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Add to sequence
            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0047-homepage/sequence/normal"
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
