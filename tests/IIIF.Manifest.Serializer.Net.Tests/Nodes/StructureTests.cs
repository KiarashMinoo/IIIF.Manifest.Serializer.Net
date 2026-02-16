using IIIF.Manifests.Serializer.Nodes.Structure;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Nodes
{
    public class StructureTests
    {
        [Fact]
        public void Structure_ShouldSerializeWithCanvases()
        {
            // Arrange
            var structure = new Structure("https://example.org/range/chapter1");
            structure.AddLabel(new Label("Chapter 1"));
            structure.AddCanvas("https://example.org/canvas/1");
            structure.AddCanvas("https://example.org/canvas/2");
            structure.AddCanvas("https://example.org/canvas/3");

            // Act
            var json = JsonConvert.SerializeObject(structure, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Structure>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Canvases.Should().HaveCount(3);
            deserialized.Label.Should().ContainSingle()
                .Which.Value.Should().Be("Chapter 1");
        }

        [Fact]
        public void Structure_ShouldSupportNestedRanges()
        {
            // Arrange
            var part1 = new Structure("https://example.org/range/part1");
            part1.AddLabel(new Label("Part I"));
            part1.AddRange("https://example.org/range/chapter1");
            part1.AddRange("https://example.org/range/chapter2");

            // Act
            var json = JsonConvert.SerializeObject(part1, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Structure>(json);

            // Assert
            deserialized.Ranges.Should().HaveCount(2);
        }

        [Fact]
        public void Structure_ShouldSupportStartCanvas()
        {
            // Arrange
            var structure = new Structure("https://example.org/range/intro");
            structure.AddLabel(new Label("Introduction"));
            structure.AddCanvas("https://example.org/canvas/1");
            structure.AddCanvas("https://example.org/canvas/2");
            structure.SetStartCanvas("https://example.org/canvas/1");

            // Act
            var json = JsonConvert.SerializeObject(structure, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Structure>(json);

            // Assert
            deserialized.StartCanvas.Should().Be("https://example.org/canvas/1");
        }

        [Fact]
        public void Structure_ShouldSupportMixedCanvasesAndRanges()
        {
            // Arrange
            var toc = new Structure("https://example.org/range/toc");
            toc.AddLabel(new Label("Table of Contents"));
            toc.AddCanvas("https://example.org/canvas/toc");
            toc.AddRange("https://example.org/range/chapter1");
            toc.AddRange("https://example.org/range/chapter2");

            // Act
            var json = JsonConvert.SerializeObject(toc, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Structure>(json);

            // Assert
            deserialized.Canvases.Should().ContainSingle();
            deserialized.Ranges.Should().HaveCount(2);
        }
    }
}

