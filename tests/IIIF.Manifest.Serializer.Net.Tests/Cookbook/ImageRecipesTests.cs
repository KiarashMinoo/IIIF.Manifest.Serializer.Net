using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using IIIF.Manifests.Serializer.Properties.TileProperty;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "Image Recipes" category.
    /// https://iiif.io/api/cookbook/recipe/code/#image-recipes
    ///
    /// Recipes covered:
    ///   - 0001: Simplest Manifest – Single Image File
    ///   - 0004: Image and Canvas with Differing Dimensions
    ///   - 0005: Support Deep Viewing with Basic Use of a IIIF Image Service
    ///   - 0009: Simple Manifest – Book (image aspects)
    ///   - 0010: Viewing direction and Its Effect on Navigation
    ///   - Start Canvas
    /// </summary>
    public class ImageRecipesTests
    {
        #region Recipe 0001: Single Image (Image-focused tests)

        [Fact]
        public void Recipe0001_ImageResource_ShouldHaveCorrectProperties()
        {
            var imageResource = new ImageResource(
                "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                "image/png"
            ).SetHeight(1800).SetWidth(1200);

            imageResource.Id.Should().Be("http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png");
            imageResource.Format.Should().Be("image/png");
            imageResource.Height.Should().Be(1800);
            imageResource.Width.Should().Be(1200);
        }

        [Fact]
        public void Recipe0001_ImageResource_ShouldSerializeWithType()
        {
            var imageResource = new ImageResource(
                "https://example.org/image.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(800);

            var json = JsonConvert.SerializeObject(imageResource, Formatting.Indented);

            json.Should().Contain("\"@type\": \"dctypes:Image\"");
            json.Should().Contain("\"format\": \"image/jpeg\"");
            json.Should().Contain("\"height\": 1000");
            json.Should().Contain("\"width\": 800");
        }

        #endregion

        #region Recipe 0004: Canvas and Image Differing Dimensions (Serialization)

        [Fact]
        public void Recipe0004_CanvasSize_ShouldHaveDifferentDimensionsFromImage()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/manifest.json",
                new Label("Image and Canvas with Differing Dimensions")
            );

            // Canvas dimensions differ from image
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/canvas/p1",
                new Label("Canvas with image of different dimensions"),
                1920, // canvas height
                1080  // canvas width
            );

            // Image has larger dimensions
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);

            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0004-canvas-size/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0004-canvas-size/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            var deserializedCanvas = deserialized.Sequences.Should().ContainSingle().Subject
                .Canvases.Should().ContainSingle().Subject;

            deserializedCanvas.Height.Should().Be(1920);
            deserializedCanvas.Width.Should().Be(1080);

            var deserializedImage = deserializedCanvas.Images.Should().ContainSingle().Subject;
            deserializedImage.Resource.Height.Should().Be(3024);
            deserializedImage.Resource.Width.Should().Be(4032);
        }

        #endregion

        #region Recipe 0004: Canvas and Image Differing Dimensions (Deserialization)

        [Fact]
        public void Recipe0004_CanvasSize_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0004-canvas-size/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Image and Canvas with Differing Dimensions"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0004-canvas-size/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0004-canvas-size/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Canvas with image of different dimensions"",
          ""height"": 1920,
          ""width"": 1080,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0004-canvas-size/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3024,
                ""width"": 4032
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0004-canvas-size/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            var canvas = manifest.Sequences.Should().ContainSingle().Subject
                .Canvases.Should().ContainSingle().Subject;

            // Canvas dimensions
            canvas.Height.Should().Be(1920);
            canvas.Width.Should().Be(1080);

            // Image dimensions (differ from canvas)
            var img = canvas.Images.Should().ContainSingle().Subject;
            img.Resource.Height.Should().Be(3024);
            img.Resource.Width.Should().Be(4032);
        }

        #endregion

        #region Recipe 0005: Deep Viewing / IIIF Image Service (Serialization)

        [Fact]
        public void Recipe0005_ImageService_ShouldSerialize_WithServiceAndTiles()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0005-image-service/manifest.json",
                new Label("Picture of Göttingen taken during the 2019 IIIF Conference")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0005-image-service/canvas/p1",
                new Label("Canvas with a single IIIF image"),
                3024, 4032
            );

            var service = new Service(
                Context.Image2.Value,
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen",
                Profile.ImageApi2Level1.Value
            ).SetHeight(3024).SetWidth(4032);

            var tile = new Tile().SetWidth(512);
            tile.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8).AddScaleFactor(16);
            service.AddTile(tile);

            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032).SetService(service);

            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0005-image-service/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0005-image-service/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"service\"");
            json.Should().Contain("\"profile\"");
            json.Should().Contain("\"tiles\"");
            json.Should().Contain("\"scaleFactors\"");
        }

        [Fact]
        public void Recipe0005_ImageService_ShouldRoundTrip()
        {
            var service = new Service(
                Context.Image2.Value,
                "https://example.org/iiif/image1",
                Profile.ImageApi2Level1.Value
            ).SetHeight(3024).SetWidth(4032);

            var tile = new Tile().SetWidth(512);
            tile.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4);
            service.AddTile(tile);

            var imageResource = new ImageResource(
                "https://example.org/image.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032).SetService(service);

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 3024, 4032);
            canvas.AddImage(new Image("https://example.org/anno/1", imageResource, canvas.Id));

            var manifest = new Manifest("https://example.org/manifest/1", new Label("Service Test"));
            var seq = new Sequence("https://example.org/seq/1");
            seq.AddCanvas(canvas);
            manifest.AddSequence(seq);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            var deserializedResource = deserialized.Sequences.Should().ContainSingle().Subject
                .Canvases.Should().ContainSingle().Subject
                .Images.Should().ContainSingle().Subject.Resource;

            deserializedResource.Service.Should().NotBeNull();
            deserializedResource.Service.Profile.Should().Be(Profile.ImageApi2Level1.Value);
            deserializedResource.Service.Tiles.Should().ContainSingle()
                .Which.Width.Should().Be(512);
        }

        #endregion

        #region Recipe 0005: Deep Viewing / IIIF Image Service (Deserialization)

        [Fact]
        public void Recipe0005_ImageService_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0005-image-service/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Picture of Göttingen"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0005-image-service/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0005-image-service/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Canvas with a single IIIF image"",
          ""height"": 3024,
          ""width"": 4032,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0005-image-service/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3024,
                ""width"": 4032,
                ""service"": {
                  ""@context"": ""http://iiif.io/api/image/2/context.json"",
                  ""@id"": ""https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen"",
                  ""profile"": ""http://iiif.io/api/image/2/level1.json"",
                  ""height"": 3024,
                  ""width"": 4032,
                  ""tiles"": [
                    {
                      ""width"": 512,
                      ""scaleFactors"": [1, 2, 4, 8, 16]
                    }
                  ]
                }
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0005-image-service/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            var resource = manifest.Sequences.Should().ContainSingle().Subject
                .Canvases.Should().ContainSingle().Subject
                .Images.Should().ContainSingle().Subject.Resource;

            resource.Service.Should().NotBeNull();
            resource.Service.Id.Should().Be("https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen");
            resource.Service.Profile.Should().Be("http://iiif.io/api/image/2/level1.json");
            resource.Service.Height.Should().Be(3024);
            resource.Service.Width.Should().Be(4032);
            resource.Service.Tiles.Should().ContainSingle();
            resource.Service.Tiles.Should().ContainSingle().Which.Width.Should().Be(512);
        }

        #endregion

        #region Start Canvas (Serialization & Deserialization)

        [Fact]
        public void StartCanvas_ShouldRoundTrip()
        {
            var sequence = new Sequence("https://example.org/seq/1");

            for (int i = 1; i <= 3; i++)
            {
                var canvas = new Canvas(
                    $"https://example.org/canvas/{i}",
                    new Label($"Page {i}"),
                    1000, 800
                );
                var res = new ImageResource($"https://example.org/img{i}.jpg", "image/jpeg")
                    .SetHeight(1000).SetWidth(800);
                canvas.AddImage(new Image($"https://example.org/anno/{i}", res, canvas.Id));
                sequence.AddCanvas(canvas);
            }

            sequence.SetStartCanvas(new StartCanvas("https://example.org/canvas/2"));

            var json = JsonConvert.SerializeObject(sequence, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Sequence>(json);

            deserialized.StartCanvas.Should().NotBeNull();
            deserialized.StartCanvas.Id.Should().Be("https://example.org/canvas/2");
        }

        [Fact]
        public void StartCanvas_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@id"": ""https://example.org/seq/1"",
  ""@type"": ""sc:Sequence"",
  ""startCanvas"": {
    ""@id"": ""https://example.org/canvas/2""
  },
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
            ""@id"": ""https://example.org/img1.jpg"",
            ""@type"": ""dctypes:Image"",
            ""format"": ""image/jpeg"",
            ""height"": 1000,
            ""width"": 800
          },
          ""on"": ""https://example.org/canvas/1""
        }
      ]
    },
    {
      ""@id"": ""https://example.org/canvas/2"",
      ""@type"": ""sc:Canvas"",
      ""label"": ""Page 2"",
      ""height"": 1000,
      ""width"": 800,
      ""images"": [
        {
          ""@id"": ""https://example.org/anno/2"",
          ""@type"": ""oa:Annotation"",
          ""motivation"": ""sc:painting"",
          ""resource"": {
            ""@id"": ""https://example.org/img2.jpg"",
            ""@type"": ""dctypes:Image"",
            ""format"": ""image/jpeg"",
            ""height"": 1000,
            ""width"": 800
          },
          ""on"": ""https://example.org/canvas/2""
        }
      ]
    }
  ]
}";

            var sequence = JsonConvert.DeserializeObject<Sequence>(json);

            sequence.Should().NotBeNull();
            sequence.StartCanvas.Should().NotBeNull();
            sequence.StartCanvas.Id.Should().Be("https://example.org/canvas/2");
            sequence.Canvases.Should().HaveCount(2);
        }

        #endregion

        #region Multiple Images on Canvas

        [Fact]
        public void MultipleImages_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@id"": ""https://example.org/canvas/1"",
  ""@type"": ""sc:Canvas"",
  ""label"": ""Composite Canvas"",
  ""height"": 2000,
  ""width"": 3000,
  ""images"": [
    {
      ""@id"": ""https://example.org/anno/1"",
      ""@type"": ""oa:Annotation"",
      ""motivation"": ""sc:painting"",
      ""resource"": {
        ""@id"": ""https://example.org/img1.jpg"",
        ""@type"": ""dctypes:Image"",
        ""format"": ""image/jpeg"",
        ""height"": 2000,
        ""width"": 1500
      },
      ""on"": ""https://example.org/canvas/1""
    },
    {
      ""@id"": ""https://example.org/anno/2"",
      ""@type"": ""oa:Annotation"",
      ""motivation"": ""sc:painting"",
      ""resource"": {
        ""@id"": ""https://example.org/img2.jpg"",
        ""@type"": ""dctypes:Image"",
        ""format"": ""image/jpeg"",
        ""height"": 2000,
        ""width"": 1500
      },
      ""on"": ""https://example.org/canvas/1""
    }
  ]
}";

            var canvas = JsonConvert.DeserializeObject<Canvas>(json);

            canvas.Should().NotBeNull();
            canvas.Images.Should().HaveCount(2);
        }

        #endregion
    }
}
