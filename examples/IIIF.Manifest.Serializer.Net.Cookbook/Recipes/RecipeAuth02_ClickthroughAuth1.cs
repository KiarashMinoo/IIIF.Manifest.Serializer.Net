using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.DescriptionProperty;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// Auth Recipe 2: IIIF Authentication API 1.0 - Clickthrough Pattern
    /// Demonstrates how to add auth services for content requiring terms acceptance.
    /// </summary>
    public static class RecipeAuth02_ClickthroughAuth1
    {
        public static string ToJson()
        {
            // Create token service
            var tokenService = new AuthService1(
                "https://authentication.example.org/clickthrough/token",
                Profile.AuthToken.Value
            );

            // Create clickthrough service
            var clickthroughService = new AuthService1(
                "https://authentication.example.org/clickthrough",
                Profile.AuthClickthrough.Value
            )
            .SetLabel("Terms of Use")
            .SetHeader("Restricted Material")
            .SetDescription("This content is restricted. By clicking Accept, you agree to the terms of use.")
            .SetConfirmLabel("Accept Terms")
            .SetFailureHeader("Terms Not Accepted")
            .SetFailureDescription("You must accept the terms of use to view this content.")
            .AddService(tokenService);

            // Create image service with authentication
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.example.org/image/restricted-image",
                Profile.ImageApi2Level1.Value
            )
            .SetHeight(2000)
            .SetWidth(1500)
            .SetService(clickthroughService);

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.example.org/image/restricted-image/full/full/0/default.jpg",
                ImageFormat.Jpg.Value
            )
            .SetHeight(2000)
            .SetWidth(1500)
            .SetService(imageService);

            // Create canvas with image
            var canvas = new Canvas(
                "https://example.org/iiif/manuscript/canvas/p1",
                new Label("Folio 1r"),
                2000,
                1500
            );

            var image = new Image(
                "https://example.org/iiif/manuscript/annotation/p1-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/iiif/manuscript/sequence/normal");
            sequence.AddCanvas(canvas);

            // Create manifest
            var manifest = new Manifest(
                "https://example.org/iiif/manuscript/manifest",
                new Label("Manuscript with Terms Agreement")
            );

            manifest.AddDescription(new Description("A manuscript that requires acceptance of terms before viewing."));
            manifest.AddSequence(sequence);

            // Serialize to JSON
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
