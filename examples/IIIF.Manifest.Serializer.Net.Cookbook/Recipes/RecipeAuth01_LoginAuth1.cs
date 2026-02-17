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
    /// Auth Recipe 1: IIIF Authentication API 1.0 - Login Pattern
    /// Demonstrates how to add auth services to an image that requires login.
    /// </summary>
    public static class RecipeAuth01_LoginAuth1
    {
        public static string ToJson()
        {
            // Create token service (provides access token after login)
            var tokenService = new AuthService1(
                "https://authentication.example.org/token",
                Profile.AuthToken.Value
            );

            // Create logout service
            var logoutService = new AuthService1(
                "https://authentication.example.org/logout",
                Profile.AuthLogout.Value
            )
            .SetLabel("Logout from Example Institution");

            // Create login service with nested token and logout services
            var loginService = new AuthService1(
                "https://authentication.example.org/login",
                Profile.AuthLogin.Value
            )
            .SetLabel("Login to Example Institution")
            .SetHeader("Please Log In")
            .SetDescription("This content requires you to log in with your Example Institution credentials.")
            .SetConfirmLabel("Login")
            .SetFailureHeader("Authentication Failed")
            .SetFailureDescription("Unable to authenticate. Please check your credentials and try again.")
            .AddService(tokenService)
            .AddService(logoutService);

            // Create image service with authentication
            var imageService = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://iiif.example.org/image/secure-image",
                Profile.ImageApi2Level1.Value
            )
            .SetHeight(3000)
            .SetWidth(2000)
            .SetService(loginService); // Attach auth service to image service

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.example.org/image/secure-image/full/full/0/default.jpg",
                ImageFormat.Jpg.Value
            )
            .SetHeight(3000)
            .SetWidth(2000)
            .SetService(imageService);

            // Create canvas with image
            var canvas = new Canvas(
                "https://example.org/iiif/book1/canvas/p1",
                new Label("Page 1"),
                3000,
                2000
            );

            var image = new Image(
                "https://example.org/iiif/book1/annotation/p1-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/iiif/book1/sequence/normal");
            sequence.AddCanvas(canvas);

            // Create manifest
            var manifest = new Manifest(
                "https://example.org/iiif/book1/manifest",
                new Label("Book with Login Authentication")
            );

            manifest.AddDescription(new Description("A book that requires login authentication to view images."));
            manifest.AddSequence(sequence);

            // Serialize to JSON
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
