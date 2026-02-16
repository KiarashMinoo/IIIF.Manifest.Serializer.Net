using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Nodes
{
    public class CanvasTests
    {
        [Fact]
        public void Canvas_ShouldRequireIdLabelAndDimensions()
        {
            // Arrange
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Test Canvas"),
                1000,
                800
            );

            // Act
            var json = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://example.org/canvas/1");
            deserialized.Label.Should().ContainSingle();
            deserialized.Height.Should().Be(1000);
            deserialized.Width.Should().Be(800);
        }

        [Fact]
        public void Canvas_ShouldContainImages()
        {
            // Arrange
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Image Canvas"),
                2000,
                1500
            );

            var resource = new ImageResource(
                "https://example.org/image.jpg",
                "image/jpeg"
            )
            .SetHeight(2000)
            .SetWidth(1500);

            var image = new Image(
                "https://example.org/anno/1",
                resource,
                canvas.Id
            );

            canvas.AddImage(image);

            // Act
            var json = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(json);

            // Assert
            deserialized.Images.Should().ContainSingle();
            deserialized.Images.Should().ContainSingle()
                .Which.Resource.Id.Should().Be("https://example.org/image.jpg");
        }

        [Fact]
        public void Canvas_ShouldSupportMultipleImages()
        {
            // Arrange - recto/verso case
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Folio 1"),
                2000,
                1500
            );

            var rectoResource = new ImageResource("https://example.org/recto.jpg", "image/jpeg")
                .SetHeight(2000).SetWidth(1500);
            var versoResource = new ImageResource("https://example.org/verso.jpg", "image/jpeg")
                .SetHeight(2000).SetWidth(1500);

            canvas.AddImage(new Image("https://example.org/anno/recto", rectoResource, canvas.Id));
            canvas.AddImage(new Image("https://example.org/anno/verso", versoResource, canvas.Id));

            // Act
            var json = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(json);

            // Assert
            deserialized.Images.Should().HaveCount(2);
        }

        [Fact]
        public void Canvas_ShouldSupportThumbnail()
        {
            // Arrange
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Page with Thumbnail"),
                1000,
                800
            );

            canvas.SetThumbnail(new Thumbnail("https://example.org/thumb.jpg"));

            // Act
            var json = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(json);

            // Assert
            deserialized.Thumbnail.Should().NotBeNull();
            deserialized.Thumbnail.Id.Should().Be("https://example.org/thumb.jpg");
        }
    }
}

