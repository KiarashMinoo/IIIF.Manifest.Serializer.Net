using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Nodes
{
    public class ManifestTests
    {
        [Fact]
        public void Manifest_ShouldSerializeAndDeserialize_WithSingleCanvas()
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Test Manifest")
            );

            var sequence = new Sequence("https://example.org/seq/1");
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Page 1"),
                1000,
                800
            );

            var resource = new ImageResource(
                "https://example.org/image1.jpg",
                "image/jpeg"
            )
            .SetHeight(1000)
            .SetWidth(800);

            var image = new Image(
                "https://example.org/anno/1",
                resource,
                canvas.Id
            );

            canvas.AddImage(image);
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://example.org/manifest/1");
            deserialized.Sequences.Should().HaveCount(1);
            deserialized.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(1);
        }

        [Fact]
        public void Manifest_ShouldIncludeMetadata_WhenSet()
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Metadata Test")
            );

            manifest.SetMetadata("Author", "Jane Doe")
                    .SetMetadata("Date", "1850")
                    .AddAttribution(new Attribution("Example Archive"));

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            deserialized.Metadata.Should().HaveCount(2);
            deserialized.Attribution.Should().ContainSingle()
                .Which.Value.Should().Be("Example Archive");
        }

        [Fact]
        public void Manifest_ShouldSupportViewingDirection()
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Right-to-Left Book")
            ).SetViewingDirection(ViewingDirection.Rtl);

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            deserialized.ViewingDirection.Value.Should().Be("right-to-left");
        }

        [Fact]
        public void Manifest_ShouldSupportNavDate()
        {
            // Arrange
            var navDate = new DateTime(1850, 6, 15);
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Historical Document")
            ).SetNavDate(navDate);

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            deserialized.NavDate.Should().Be(navDate);
        }

        [Fact]
        public void Manifest_ShouldSupportMultipleLabels()
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("English Title")
            );
            manifest.AddLabel(new Label("French Title"));

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            deserialized.Label.Should().HaveCount(2);
        }
    }
}

