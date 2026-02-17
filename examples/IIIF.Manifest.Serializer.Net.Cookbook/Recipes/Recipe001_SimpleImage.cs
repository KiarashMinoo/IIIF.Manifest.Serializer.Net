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
    /// IIIF Cookbook Recipe 0001: Simplest Manifest - Single Image File
    /// https://iiif.io/api/cookbook/recipe/0001-mvm-image/
    ///
    /// The simplest viable manifest for image content.
    /// Mapped from IIIF Presentation 3.0 to 2.0 equivalents:
    ///   v3 items → v2 sequences/canvases/images
    ///   v3 body → v2 resource
    ///   v3 target → v2 on
    /// </summary>
    public static class Recipe001_SimpleImage
    {
        public static IIIFManifest Create()
        {
            // Create the manifest (v2: @type = sc:Manifest)
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
                new Label("Single Image Example")
            );

            // Create the canvas with dimensions matching the image
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
                new Label("Canvas 1"),
                1800, // height
                1200  // width
            );

            // Create the image resource (v2: @type = dctypes:Image)
            var imageResource = new ImageResource(
                "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                "image/png"
            )
            .SetHeight(1800)
            .SetWidth(1200);

            // Create the painting annotation (v2: @type = oa:Annotation, motivation = sc:painting)
            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Create sequence (v2 concept, replaced by items in v3)
            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/normal");
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

