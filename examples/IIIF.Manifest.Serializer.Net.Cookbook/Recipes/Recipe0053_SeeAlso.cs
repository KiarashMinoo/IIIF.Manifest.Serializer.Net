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
    /// IIIF Cookbook Recipe 0053: Linking to Structured Metadata
    /// https://iiif.io/api/cookbook/recipe/0053-seeAlso/
    ///
    /// Demonstrates the 'seeAlso' property to link to machine-readable structured
    /// metadata in formats like MODS, Dublin Core, BIBFRAME, or other schemas.
    /// This enables discovery, harvesting, and faceting by aggregators.
    /// 
    /// Different from:
    /// - metadata: for human-readable label/value pairs in the manifest itself
    /// - homepage: links to human-readable web page about the object
    /// - rendering: provides alternative representations of the resource
    /// 
    /// v3 → v2 mapping:
    ///   - v3 seeAlso with type/format/profile → v2 seeAlso with @type/format/profile
    ///   - Both versions support multiple seeAlso entries
    /// </summary>
    public static class Recipe0053_SeeAlso
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0053-seeAlso/manifest.json",
                new Label("Picture of Göttingen")
            );

            // Add multiple seeAlso entries for different metadata formats

            // MODS metadata
            var modsSeeAlso = new SeeAlso(
                "https://example.org/metadata/mods/123456.xml"
            )
            .SetFormat("application/mods+xml");

            manifest.AddSeeAlso(modsSeeAlso);

            // Dublin Core metadata
            var dcSeeAlso = new SeeAlso(
                "https://example.org/metadata/dc/123456.xml"
            )
            .SetFormat("application/xml");

            manifest.AddSeeAlso(dcSeeAlso);

            // MARC XML metadata
            var marcSeeAlso = new SeeAlso(
                "https://example.org/metadata/marc/123456.xml"
            )
            .SetFormat("application/marcxml+xml");

            manifest.AddSeeAlso(marcSeeAlso);

            // Add description  
            manifest.AddDescription(new Description(
                "This manifest demonstrates linking to external structured metadata sources " +
                "that provide detailed cataloging information in standard formats."
            ));

            // Create canvas with image
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0053-seeAlso/canvas/p1",
                new Label("Photograph"),
                3024,  // height
                4032   // width
            );

            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            )
            .SetHeight(3024)
            .SetWidth(4032);

            // Add IIIF Image Service
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0053-seeAlso/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Add to sequence
            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0053-seeAlso/sequence/normal"
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
