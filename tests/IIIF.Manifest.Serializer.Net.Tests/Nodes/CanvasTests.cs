using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using System.Linq;

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

        [Fact]
        public void Image_ShouldSupportTextGranularity()
        {
            // Arrange
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Text Canvas"),
                1000,
                800
            );

            var resource = new ImageResource(
                "https://example.org/image.jpg",
                "image/jpeg"
            )
            .SetHeight(1000)
            .SetWidth(800);

            var image = new Image(
                "https://example.org/anno/1",
                resource,
                canvas.Id
            )
            .SetTextGranularity(TextGranularity.Line);

            canvas.AddImage(image);

            // Act
            var json = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(json);

            // Assert
            deserialized.Images.Should().ContainSingle();
            var deserializedImage = deserialized.Images.Single();
            deserializedImage.TextGranularity.Should().NotBeNull();
            deserializedImage.TextGranularity.Value.Should().Be("line");
        }

        [Fact]
        public void Canvas_ShouldSupportNavPlace()
        {
            // Arrange
            var canvas = new Canvas(
                "http://example.com/canvas/1",
                new Label("Test Canvas"),
                1000,
                1000
            );
            var navPlace = NavPlace.FromPoint(9.938, 51.533, "Göttingen");
            canvas.SetNavPlace(navPlace);

            // Act
            var serialized = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(serialized);

            // Assert
            deserialized.NavPlace.Should().NotBeNull();
            deserialized.NavPlace.Features.Should().NotBeNull();
            deserialized.NavPlace.Features.Type.Should().Be("FeatureCollection");
            deserialized.NavPlace.Features.Features.Should().ContainSingle();
            var feature = deserialized.NavPlace.Features.Features.First();
            feature.Type.Should().Be("Feature");
            feature.Geometry.Type.Should().Be("Point");
            feature.Geometry.Coordinates.Should().BeOfType<Point>();
            var point = (Point)feature.Geometry.Coordinates;
            point.Coordinates.Should().Equal(new[] { 9.938, 51.533 });
            feature.Properties.Should().NotBeNull();
            feature.Properties.Label.Should().Be("Göttingen");
        }

        [Fact]
        public void Canvas_ShouldSupportGeoreference()
        {
            // Arrange
            var canvas = new Canvas(
                "http://example.com/canvas/2",
                new Label("Georeferenced Canvas"),
                2000,
                1500
            );

            var gcps = new[]
            {
                new GroundControlPoint(100, 200, 9.938, 51.533),  // Top-left
                new GroundControlPoint(1900, 200, 9.950, 51.533), // Top-right
                new GroundControlPoint(1900, 1300, 9.950, 51.520), // Bottom-right
                new GroundControlPoint(100, 1300, 9.938, 51.520)   // Bottom-left
            };

            var transformation = new Transformation("polynomial", new { order = 1 });
            var georeference = new Georeference("Georeference")
                .SetCrs("EPSG:4326")
                .SetGcps(gcps)
                .SetTransformation(transformation);

            canvas.SetGeoreference(georeference);

            // Act
            var serialized = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(serialized);

            // Assert
            deserialized.Georeference.Should().NotBeNull();
            deserialized.Georeference.Type.Should().Be("Georeference");
            deserialized.Georeference.Crs.Should().Be("EPSG:4326");
            deserialized.Georeference.Gcps.Should().NotBeNull();
            deserialized.Georeference.Gcps.Should().HaveCount(4);
            deserialized.Georeference.Transformation.Should().NotBeNull();
            deserialized.Georeference.Transformation.Type.Should().Be("polynomial");
        }
    }
}

