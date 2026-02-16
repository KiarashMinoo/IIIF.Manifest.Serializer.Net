using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class ViewingHintTests
    {
        [Theory]
        [InlineData("paged")]
        [InlineData("continuous")]
        [InlineData("individuals")]
        [InlineData("facing-pages")]
        [InlineData("non-paged")]
        [InlineData("top")]
        [InlineData("multi-part")]
        public void ViewingHint_ShouldSerializeCorrectly(string hintValue)
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Test")
            );
            manifest.SetViewingHint(new ViewingHint(hintValue));

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            // Assert
            json.Should().Contain($"\"viewingHint\": \"{hintValue}\"");
        }

        [Theory]
        [InlineData("paged")]
        [InlineData("continuous")]
        [InlineData("individuals")]
        [InlineData("facing-pages")]
        [InlineData("non-paged")]
        [InlineData("top")]
        [InlineData("multi-part")]
        public void ViewingHint_ShouldDeserializeCorrectly(string hintValue)
        {
            // Arrange
            var json = $@"{{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/manifest/1"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Test"",
  ""viewingHint"": ""{hintValue}"",
  ""sequences"": [
    {{
      ""@id"": ""https://example.org/seq/1"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {{
          ""@id"": ""https://example.org/canvas/1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Page 1"",
          ""height"": 1000,
          ""width"": 800,
          ""images"": [
            {{
              ""@id"": ""https://example.org/anno/1"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {{
                ""@id"": ""https://example.org/img.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1000,
                ""width"": 800
              }},
              ""on"": ""https://example.org/canvas/1""
            }}
          ]
        }}
      ]
    }}
  ]
}}";

            // Act
            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            manifest.ViewingHint.Should().NotBeNull();
            manifest.ViewingHint.Value.Should().Be(hintValue);
        }

        [Fact]
        public void ViewingHint_StaticProperties_ShouldWorkCorrectly()
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/book",
                new Label("Book with Paged Hint")
            );
            manifest.SetViewingHint(ViewingHint.Paged);

            var sequence = new Sequence("https://example.org/seq/1");
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Cover"),
                1000,
                800
            );
            canvas.SetViewingHint(ViewingHint.Top);

            var resource = new ImageResource("https://example.org/image.jpg", "image/jpeg")
                .SetHeight(1000).SetWidth(800);
            var image = new Image("https://example.org/anno/1", resource, canvas.Id);

            canvas.AddImage(image);
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            // Assert
            deserialized.ViewingHint.Should().NotBeNull();
            deserialized.ViewingHint.Value.Should().Be("paged");
            deserialized.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().ContainSingle()
                .Which.ViewingHint.Value.Should().Be("top");
        }

        [Fact]
        public void ViewingHint_Null_ShouldNotSerialize()
        {
            // Arrange
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Test")
            );
            // Don't set viewing hint - should be null

            var sequence = new Sequence("https://example.org/seq/1");
            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
            var resource = new ImageResource("https://example.org/image.jpg", "image/jpeg")
                .SetHeight(1000).SetWidth(800);
            canvas.AddImage(new Image("https://example.org/anno/1", resource, canvas.Id));
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            // Act
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            // Assert
            json.Should().NotContain("viewingHint");
        }
    }
}

