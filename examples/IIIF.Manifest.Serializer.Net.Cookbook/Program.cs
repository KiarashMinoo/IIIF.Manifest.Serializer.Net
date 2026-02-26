using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Recipe 0001: Simplest Manifest - Single Image File
            // https://iiif.io/api/cookbook/recipe/0001-mvm-image/

            var manifest = Recipe0001_SimplestManifest();

            var json = manifest.Serialize();

            Console.WriteLine("Recipe 0001: Simplest Manifest - Single Image File");
            Console.WriteLine("=".PadRight(60, '='));
            Console.WriteLine(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
            }
        }
    }

    /// <summary>
    /// Creates the simplest viable manifest for image content - IIIF Presentation API 2.0 version.
    /// This is the v2 equivalent of https://iiif.io/api/cookbook/recipe/0001-mvm-image/
    /// </summary>
    public static Manifest Recipe0001_SimplestManifest()
    {
        // Create the image resource with dimensions and format
        var imageResource = new ImageResource(
                id: "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                format: "image/png"
            )
            .SetHeight(1800)
            .SetWidth(1200);

        // Create the canvas with label and dimensions
        var canvas = new Canvas(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
            label: new Label("p. 1"),
            height: 1800,
            width: 1200
        );

        // Create the image annotation (painting motivation) targeting the canvas
        var image = new Image(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
            resource: imageResource,
            on: canvas.Id
        );

        // Add the image to the canvas
        canvas.AddImage(image);

        // Create a sequence and add the canvas
        var sequence = new Sequence(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/s0"
        );
        sequence.AddCanvas(canvas);

        // Create the manifest with label
        var manifest = new Manifest(
            id: "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
            label: new Label("Single Image Example")
        );
        manifest.AddSequence(sequence);

        manifest.AddContext("TestContext");

        return manifest;
    }
}