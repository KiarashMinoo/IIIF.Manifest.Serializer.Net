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
    /// IIIF Cookbook Recipe: Geographic Location with navPlace Extension
    /// Demonstrates how to add geographic location information to canvases using the navPlace extension.
    ///
    /// The navPlace extension allows associating geographic coordinates with IIIF resources,
    /// enabling geographic navigation and location-based features in IIIF viewers.
    /// </summary>
    public static class Recipe016_NavPlace
    {
        public static IIIFManifest Create()
        {
            // Create the manifest
            var manifest = new IIIFManifest(
                "https://example.org/manifests/navplace-example.json",
                new Label("Map with Geographic Location")
            );

            // Create the canvas for a map
            var canvas = new Canvas(
                "https://example.org/canvases/map1",
                new Label("Historical Map of Göttingen"),
                3000, // height
                2500  // width
            );

            // Add geographic location using navPlace extension
            // This represents the center point of Göttingen, Germany
            var navPlace = NavPlace.FromPoint(9.938, 51.533, "Göttingen, Germany");
            canvas.SetNavPlace(navPlace);

            // Create the image resource for the map
            var imageResource = new ImageResource(
                "https://example.org/images/goettingen-map.jpg",
                "image/jpeg"
            )
            .SetHeight(3000)
            .SetWidth(2500);

            // Create the painting annotation
            var image = new Image(
                "https://example.org/annotations/map1-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/sequences/map-sequence");
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