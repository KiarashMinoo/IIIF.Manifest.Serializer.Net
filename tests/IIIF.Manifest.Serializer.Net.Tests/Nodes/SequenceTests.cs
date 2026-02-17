using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;

namespace IIIF.Manifests.Serializer.Tests.Nodes
{
    public class SequenceTests
    {
        [Fact]
        public void Sequence_ShouldSerializeWithCanvases()
        {
            // Arrange
            var sequence = new Sequence("https://example.org/seq/1");

            for (int i = 1; i <= 3; i++)
            {
                var canvas = CreateTestCanvas(i);
                sequence.AddCanvas(canvas);
            }

            // Act
            var json = JsonConvert.SerializeObject(sequence, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Sequence>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Canvases.Should().HaveCount(3);
        }

        [Fact]
        public void Sequence_ShouldSupportStartCanvas()
        {
            // Arrange
            var sequence = new Sequence("https://example.org/seq/1");
            var canvas1 = CreateTestCanvas(1);
            var canvas2 = CreateTestCanvas(2);

            sequence.AddCanvas(canvas1);
            sequence.AddCanvas(canvas2);
            sequence.SetStartCanvas(new StartCanvas("https://example.org/canvas/2"));

            // Act
            var json = JsonConvert.SerializeObject(sequence, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Sequence>(json);

            // Assert
            deserialized.StartCanvas.Should().NotBeNull();
            deserialized.StartCanvas.Id.Should().Be("https://example.org/canvas/2");
        }

        [Fact]
        public void Sequence_ShouldSupportViewingDirection()
        {
            // Arrange
            var sequence = new Sequence("https://example.org/seq/1");
            sequence.SetViewingDirection(ViewingDirection.Btt);

            var canvas = CreateTestCanvas(1);
            sequence.AddCanvas(canvas);

            // Act
            var json = JsonConvert.SerializeObject(sequence, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Sequence>(json);

            // Assert
            deserialized.ViewingDirection.Value.Should().Be("bottom-to-top");
        }

        private Canvas CreateTestCanvas(int index)
        {
            var canvas = new Canvas(
                $"https://example.org/canvas/{index}",
                new Label($"Page {index}"),
                1000,
                800
            );

            var resource = new ImageResource(
                $"https://example.org/image{index}.jpg",
                "image/jpeg"
            )
            .SetHeight(1000)
            .SetWidth(800);

            var image = new Image(
                $"https://example.org/anno/{index}",
                resource,
                canvas.Id
            );

            canvas.AddImage(image);
            return canvas;
        }
    }
}

