using System;
using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Service;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Net.Cookbook.Recipes
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
                .SetType(ResourceType.AuthLogoutService2.Value)
                .SetLabel("Logout");

            // Create access token service
            var tokenService = new AuthService2("https://authentication.example.org/auth2/token")
                .SetType(ResourceType.AuthAccessTokenService2.Value)
                .AddService(logoutService);

            // Create access service (active pattern)
            var accessService = new AuthService2(
                "https://authentication.example.org/auth2/access",
                "active" // profile for active authentication
            )
            .SetType(ResourceType.AuthAccessService2.Value)
            .SetLabel("Login to Access Content")
            .SetHeading("Authentication Required")
            .SetNote("Please log in with your institutional credentials to access this content.")
            .SetConfirmLabel("Login")
            .AddService(tokenService);

            // Create probe service (entry point for auth flow)
            var probeService = new AuthService2("https://authentication.example.org/auth2/probe")
                .SetType(ResourceType.AuthProbeService2.Value)
                .AddService(accessService);

            // Create image service with auth 2.0
            var imageService = new Service(
                "https://iiif.example.org/image/auth2-image",
                Profile.ImageApi2Level1.Value
            )
            .SetHeight(3200)
            .SetWidth(2400)
            .SetService(probeService);

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.example.org/image/auth2-image/full/full/0/default.jpg",
                "https://iiif.example.org/image/auth2-image"
            )
            .SetFormat(ImageFormat.Jpeg)
            .SetHeight(3200)
            .SetWidth(2400)
            .SetService(imageService);

            // Create canvas with image
            var canvas = new Canvas(
                "https://example.org/iiif/document/canvas/p1",
                "Page 1",
                3200,
                2400
            );

            var image = new Image(
                "https://example.org/iiif/document/annotation/p1-image",
                canvas.Id
            )
            .SetResource(imageResource);

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/iiif/document/sequence/normal");
            sequence.AddCanvas(canvas);

            // Create manifest
            var manifest = new Manifest(
                "https://example.org/iiif/document/manifest",
                "Document with Auth 2.0"
            );

            manifest.AddLabel("en", "Secure Document - Auth 2.0");
            manifest.SetDescription("A document using IIIF Authentication API 2.0 with probe service pattern.");
            manifest.AddSequence(sequence);

            // Serialize to JSON
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
