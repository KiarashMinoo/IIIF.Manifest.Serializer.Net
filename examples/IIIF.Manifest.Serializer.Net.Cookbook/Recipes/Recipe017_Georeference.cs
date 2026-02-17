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
    /// IIIF Cookbook Recipe: Georeferenced Map with Georeference Extension
    /// Demonstrates how to add georeferencing information to map canvases using the georeference extension.
    ///
    /// The georeference extension allows associating geographic coordinate systems with image pixels,
    /// enabling accurate geographic positioning and measurement in IIIF viewers.
    /// </summary>
    public static class Recipe017_Georeference
    {
        public static IIIFManifest Create()
        {
            // Create the manifest
            var manifest = new IIIFManifest(
                "https://example.org/manifests/georeference-example.json",
                new Label("Georeferenced Historical Map")
            );

            // Create the canvas for a georeferenced map
            var canvas = new Canvas(
                "https://example.org/canvases/georef-map1",
                new Label("Georeferenced Map of Göttingen (1890)"),
                4000, // height
                3200  // width
            );

            // Add georeferencing information using georeference extension
            // Ground control points relating image pixels to geographic coordinates
            var gcps = new[]
            {
                new GroundControlPoint(200, 300, 9.920, 51.550),    // Top-left corner
                new GroundControlPoint(3000, 300, 9.960, 51.550),   // Top-right corner
                new GroundControlPoint(3000, 3700, 9.960, 51.510),  // Bottom-right corner
                new GroundControlPoint(200, 3700, 9.920, 51.510)    // Bottom-left corner
            };

            // Define transformation (polynomial of order 1 = affine transformation)
            var transformation = new Transformation("polynomial", new { order = 1 });

            var georeference = new Georeference("Georeference")
                .SetCrs("EPSG:4326")  // WGS84 latitude/longitude
                .SetGcps(gcps)
                .SetTransformation(transformation);

            canvas.SetGeoreference(georeference);

            // Create the image resource for the map
            var imageResource = new ImageResource(
                "https://example.org/images/goettingen-1890-georef.jpg",
                "image/jpeg"
            )
            .SetHeight(4000)
            .SetWidth(3200);

            // Create the painting annotation
            var image = new Image(
                "https://example.org/annotations/georef-map1-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/sequences/georef-sequence");
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