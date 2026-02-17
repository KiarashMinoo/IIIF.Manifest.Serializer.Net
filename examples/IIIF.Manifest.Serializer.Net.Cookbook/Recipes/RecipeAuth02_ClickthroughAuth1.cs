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
                "https://iiif.example.org/image/restricted-image",
                Profile.ImageApi2Level1.Value
            )
            .SetHeight(2000)
            .SetWidth(1500)
            .SetService(clickthroughService);

            // Create image resource
            var imageResource = new ImageResource(
                "https://iiif.example.org/image/restricted-image/full/full/0/default.jpg",
                "https://iiif.example.org/image/restricted-image"
            )
            .SetFormat(ImageFormat.Jpeg)
            .SetHeight(2000)
            .SetWidth(1500)
            .SetService(imageService);

            // Create canvas with image
            var canvas = new Canvas(
                "https://example.org/iiif/manuscript/canvas/p1",
                "Folio 1r",
                2000,
                1500
            );

            var image = new Image(
                "https://example.org/iiif/manuscript/annotation/p1-image",
                canvas.Id
            )
            .SetResource(imageResource);

            canvas.AddImage(image);

            // Create sequence
            var sequence = new Sequence("https://example.org/iiif/manuscript/sequence/normal");
            sequence.AddCanvas(canvas);

            // Create manifest
            var manifest = new Manifest(
                "https://example.org/iiif/manuscript/manifest",
                "Manuscript with Terms Agreement"
            );

            manifest.AddLabel("en", "Restricted Manuscript");
            manifest.SetDescription("A manuscript that requires acceptance of terms before viewing.");
            manifest.AddSequence(sequence);

            // Serialize to JSON
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
