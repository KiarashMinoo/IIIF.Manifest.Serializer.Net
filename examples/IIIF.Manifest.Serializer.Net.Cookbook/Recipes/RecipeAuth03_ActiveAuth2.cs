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
    /// Auth Recipe 3: IIIF Authentication API 2.0 - Active Pattern
    /// Demonstrates Auth 2.0 with probe service and access service (active pattern).
    /// </summary>
    public static class RecipeAuth03_ActiveAuth2
    {
        public static string ToJson()
        {
            // Create logout service
            var logoutService = new AuthService2("https://authentication.example.org/auth2/logout")
                .SetLabel("Logout");

            // Create access token service
            var tokenService = new AuthService2("https://authentication.example.org/auth2/token")
                .AddService(logoutService);

            // Create access service (active pattern)
            var accessService = new AuthService2(
                "https://authentication.example.org/auth2/access",
                "active" // profile for active authentication
            )
            .SetLabel("Login to Access Content")
            .SetHeading("Authentication Required")
            .SetNote("Please log in with your institutional credentials to access this content.")
            .SetConfirmLabel("Login")
            .AddService(tokenService);

            // Create probe service (entry point for auth flow)
            var probeService = new AuthService2("https://authentication.example.org/auth2/probe")
                .AddService(accessService);

            // Create image service with auth 2.0
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.example.org/image/auth2-image",
                Profile.ImageApi2Level1.Value
            )
            .SetHeight(3200)
            .SetWidth(2400)
            .SetService(probeService);

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.example.org/image/auth2-image/full/full/0/default.jpg",
                ImageFormat.Jpg.Value
            )
            .SetHeight(3200)
            .SetWidth(2400)
            .SetService(imageService);

            // Create canvas with image
            var canvas = new Canvas(
                "https://example.org/iiif/document/canvas/p1",
                new Label("Page 1"),
                3200,
                2400
            );

            var image = new Image(
                "https://example.org/iiif/document/annotation/p1-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/iiif/document/sequence/normal");
            sequence.AddCanvas(canvas);

            // Create manifest
            var manifest = new Manifest(
                "https://example.org/iiif/document/manifest",
                new Label("Document with Auth 2.0")
            );

            manifest.AddDescription(new Description("A document using IIIF Authentication API 2.0 with probe service pattern."));
            manifest.AddSequence(sequence);

            // Serialize to JSON
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
