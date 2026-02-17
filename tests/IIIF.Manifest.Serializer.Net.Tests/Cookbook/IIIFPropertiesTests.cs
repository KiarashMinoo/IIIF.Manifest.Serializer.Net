using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "IIIF Properties" category.
    /// https://iiif.io/api/cookbook/recipe/code/#iiif-properties
    ///
    /// Recipes covered:
    ///   - 0008: Rights statement (rights, requiredStatement) → v2 license + attribution
    ///   - 0010: Viewing direction (viewingDirection)
    ///   - 0011: Book behavior variations (continuous, individuals) → v2 viewingHint
    ///   - navDate
    ///   - thumbnail
    ///   - rendering
    /// </summary>
    public class IIIFPropertiesTests
    {
        #region Recipe 0008: Rights Statement (Serialization)

        [Fact]
        public void Recipe0008_Rights_ShouldSerialize_LicenseAndAttribution()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0008-rights/manifest.json",
                new Label("Picture of Göttingen taken during the 2019 IIIF Conference")
            );

            manifest.SetLicense(new License("http://creativecommons.org/licenses/by-sa/3.0/"));
            manifest.AddAttribution(
                new Attribution("Götttingen, Lower Saxony, Germany. Taken by the IIIF community during the 2019 IIIF Conference.")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0008-rights/canvas/p1",
                new Label("Canvas with rights statement"),
                3024, 4032
            );
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);
            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0008-rights/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0008-rights/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"license\": \"http://creativecommons.org/licenses/by-sa/3.0/\"");
            json.Should().Contain("attribution");
        }

        [Fact]
        public void Recipe0008_Rights_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0008-rights/manifest.json",
                new Label("Rights Test")
            );
            manifest.SetLicense(new License("http://creativecommons.org/licenses/by-sa/3.0/"));
            manifest.AddAttribution(new Attribution("Example Attribution"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
            var res = new ImageResource("https://example.org/img.jpg", "image/jpeg").SetHeight(1000).SetWidth(800);
            canvas.AddImage(new Image("https://example.org/anno/1", res, canvas.Id));
            var seq = new Sequence("https://example.org/seq/1");
            seq.AddCanvas(canvas);
            manifest.AddSequence(seq);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.License.Value.Should().Be("http://creativecommons.org/licenses/by-sa/3.0/");
            deserialized.Attribution.Should().ContainSingle()
                .Which.Value.Should().Be("Example Attribution");
        }

        #endregion

        #region Recipe 0008: Rights Statement (Deserialization)

        [Fact]
        public void Recipe0008_Rights_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0008-rights/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Picture of Göttingen"",
  ""license"": ""http://creativecommons.org/licenses/by-sa/3.0/"",
  ""attribution"": ""Göttingen, Lower Saxony, Germany"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0008-rights/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0008-rights/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Canvas with rights"",
          ""height"": 3024,
          ""width"": 4032,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0008-rights/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3024,
                ""width"": 4032
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0008-rights/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.License.Value.Should().Be("http://creativecommons.org/licenses/by-sa/3.0/");
            manifest.Attribution.Should().ContainSingle()
                .Which.Value.Should().Be("Göttingen, Lower Saxony, Germany");
        }

        #endregion

        #region Recipe 0010: Viewing Direction (Serialization)

        [Theory]
        [InlineData("right-to-left")]
        [InlineData("left-to-right")]
        [InlineData("top-to-bottom")]
        [InlineData("bottom-to-top")]
        public void Recipe0010_ViewingDirection_ShouldSerialize(string direction)
        {
            var manifest = new Manifest(
                "https://example.org/manifest/1",
                new Label("Viewing Direction Test")
            );
            manifest.SetViewingDirection(new ViewingDirection(direction));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
            var res = new ImageResource("https://example.org/img.jpg", "image/jpeg").SetHeight(1000).SetWidth(800);
            canvas.AddImage(new Image("https://example.org/anno/1", res, canvas.Id));
            var seq = new Sequence("https://example.org/seq/1");
            seq.AddCanvas(canvas);
            manifest.AddSequence(seq);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain($"\"viewingDirection\": \"{direction}\"");
        }

        [Fact]
        public void Recipe0010_RtlBook_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/manifest-rtl.json",
                new Label("Book with Right-to-Left Viewing Direction")
            );
            manifest.SetViewingDirection(ViewingDirection.Rtl);
            manifest.SetViewingHint(ViewingHint.Paged);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/sequence/rtl");
            for (int i = 1; i <= 4; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/p{i}",
                    new Label($"Page {i}"),
                    1800, 1200
                );
                var res = new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200);
                canvas.AddImage(new Image(
                    $"https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/annotation/p{i:D4}-image",
                    res, canvas.Id
                ));
                sequence.AddCanvas(canvas);
            }
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.ViewingDirection.Value.Should().Be("right-to-left");
            deserialized.ViewingHint.Value.Should().Be("paged");
            deserialized.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(4);
        }

        #endregion

        #region Recipe 0010: Viewing Direction (Deserialization)

        [Fact]
        public void Recipe0010_ViewingDirection_ShouldDeserializeRtlFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/manifest-rtl.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Book with Right-to-Left Viewing Direction"",
  ""viewingDirection"": ""right-to-left"",
  ""viewingHint"": ""paged"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/sequence/rtl"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Page 1"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/p1""
            }
          ]
        },
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/p2"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Page 2"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/annotation/p0002-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0010-book-2-viewing-direction/canvas/p2""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.ViewingDirection.Value.Should().Be("right-to-left");
            manifest.ViewingHint.Value.Should().Be("paged");
            manifest.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(2);
        }

        [Fact]
        public void Recipe0010_ViewingDirection_ShouldDeserializeTtbFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/manifest/ttb"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Scroll with Top-to-Bottom Viewing Direction"",
  ""viewingDirection"": ""top-to-bottom"",
  ""viewingHint"": ""continuous"",
  ""sequences"": [
    {
      ""@id"": ""https://example.org/seq/ttb"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://example.org/canvas/scroll1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Section 1"",
          ""height"": 3000,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://example.org/anno/scroll1-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/image.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3000,
                ""width"": 1200
              },
              ""on"": ""https://example.org/canvas/scroll1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.ViewingDirection.Value.Should().Be("top-to-bottom");
            manifest.ViewingHint.Value.Should().Be("continuous");
        }

        #endregion

        #region Recipe 0011: Book Behavior Variations (Serialization)

        [Theory]
        [InlineData("paged")]
        [InlineData("continuous")]
        [InlineData("individuals")]
        public void Recipe0011_BookBehavior_ShouldSerialize(string hintValue)
        {
            var manifest = new Manifest(
                "https://example.org/manifest/behavior",
                new Label($"Manifest with {hintValue} behavior")
            );
            manifest.SetViewingHint(new ViewingHint(hintValue));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
            var res = new ImageResource("https://example.org/img.jpg", "image/jpeg")
                .SetHeight(1000).SetWidth(800);
            canvas.AddImage(new Image("https://example.org/anno/1", res, canvas.Id));
            var seq = new Sequence("https://example.org/seq/1");
            seq.AddCanvas(canvas);
            manifest.AddSequence(seq);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            json.Should().Contain($"\"viewingHint\": \"{hintValue}\"");
        }

        [Fact]
        public void Recipe0011_PagedBehavior_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/manifest-paged.json",
                new Label("Book with Paged Behavior")
            );
            manifest.SetViewingHint(ViewingHint.Paged);
            manifest.SetViewingDirection(ViewingDirection.Ltr);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/sequence/paged");
            for (int i = 1; i <= 6; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/canvas/p{i}",
                    new Label(i == 1 ? "Front Cover" : i == 6 ? "Back Cover" : $"Page {i - 1}"),
                    1800, 1200
                );
                var res = new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200);
                canvas.AddImage(new Image(
                    $"https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/annotation/p{i:D4}-image",
                    res, canvas.Id
                ));
                sequence.AddCanvas(canvas);
            }
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.ViewingHint.Value.Should().Be("paged");
            deserialized.ViewingDirection.Value.Should().Be("left-to-right");
            deserialized.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(6);
        }

        #endregion

        #region Recipe 0011: Book Behavior Variations (Deserialization)

        [Fact]
        public void Recipe0011_BookBehavior_ShouldDeserializePagedFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/manifest-paged.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Book with Paged Behavior"",
  ""viewingHint"": ""paged"",
  ""viewingDirection"": ""left-to-right"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/sequence/paged"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Front Cover"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/img.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0011-book-3-behavior/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.ViewingHint.Value.Should().Be("paged");
            manifest.ViewingDirection.Value.Should().Be("left-to-right");
        }

        [Fact]
        public void Recipe0011_BookBehavior_ShouldDeserializeContinuousFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/manifest/continuous"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Scroll with Continuous Behavior"",
  ""viewingHint"": ""continuous"",
  ""viewingDirection"": ""top-to-bottom"",
  ""sequences"": [
    {
      ""@id"": ""https://example.org/seq/continuous"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://example.org/canvas/scroll1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Section 1"",
          ""height"": 3000,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://example.org/anno/scroll1-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/image.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3000,
                ""width"": 1200
              },
              ""on"": ""https://example.org/canvas/scroll1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.ViewingHint.Value.Should().Be("continuous");
            manifest.ViewingDirection.Value.Should().Be("top-to-bottom");
        }

        #endregion

        #region NavDate (Deserialization)

        [Fact]
        public void NavDate_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/manifest/historical"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Historical Document"",
  ""navDate"": ""1856-01-01T00:00:00Z"",
  ""sequences"": [
    {
      ""@id"": ""https://example.org/seq/1"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://example.org/canvas/1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Page 1"",
          ""height"": 1000,
          ""width"": 800,
          ""images"": [
            {
              ""@id"": ""https://example.org/anno/1"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/img.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1000,
                ""width"": 800
              },
              ""on"": ""https://example.org/canvas/1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.NavDate.Should().NotBeNull();
            manifest.NavDate.Value.Year.Should().Be(1856);
        }

        #endregion

        #region Thumbnail (Serialization & Deserialization)

        [Fact]
        public void Thumbnail_ShouldRoundTrip()
        {
            var canvas = new Canvas(
                "https://example.org/canvas/1",
                new Label("Page with Thumbnail"),
                1000, 800
            );
            canvas.SetThumbnail(new Thumbnail("https://example.org/thumb.jpg"));

            var res = new ImageResource("https://example.org/img.jpg", "image/jpeg")
                .SetHeight(1000).SetWidth(800);
            canvas.AddImage(new Image("https://example.org/anno/1", res, canvas.Id));

            var json = JsonConvert.SerializeObject(canvas, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Canvas>(json);

            deserialized.Thumbnail.Should().NotBeNull();
            deserialized.Thumbnail.Id.Should().Be("https://example.org/thumb.jpg");
        }

        [Fact]
        public void Thumbnail_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@id"": ""https://example.org/canvas/1"",
  ""@type"": ""sc:Canvas"",
  ""label"": ""Page with Thumbnail"",
  ""height"": 1000,
  ""width"": 800,
  ""thumbnail"": {
    ""@id"": ""https://example.org/thumb.jpg""
  },
  ""images"": [
    {
      ""@id"": ""https://example.org/anno/1"",
      ""@type"": ""oa:Annotation"",
      ""motivation"": ""sc:painting"",
      ""resource"": {
        ""@id"": ""https://example.org/img.jpg"",
        ""@type"": ""dctypes:Image"",
        ""format"": ""image/jpeg"",
        ""height"": 1000,
        ""width"": 800
      },
      ""on"": ""https://example.org/canvas/1""
    }
  ]
}";

            var canvas = JsonConvert.DeserializeObject<Canvas>(json);

            canvas.Thumbnail.Should().NotBeNull();
            canvas.Thumbnail.Id.Should().Be("https://example.org/thumb.jpg");
        }

        #endregion
    }
}
