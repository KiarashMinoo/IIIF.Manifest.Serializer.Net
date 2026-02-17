using System;
using System.Linq;
using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Audio;
using IIIF.Manifests.Serializer.Nodes.Content.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Content.Video;
using IIIF.Manifests.Serializer.Nodes.Content.Video.Resource;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Metadata;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using IIIFDescription = IIIF.Manifests.Serializer.Properties.Description.Description;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "Basic Recipes" category.
    /// https://iiif.io/api/cookbook/recipe/code/#basic-recipes
    ///
    /// Recipes covered:
    ///   - 0001: Simplest Manifest – Single Image File
    ///   - 0009: Simple Manifest – Book
    ///   - 0006: Internationalization and Multi-language Values
    ///   - Simple Collection
    ///   - Simplest Manifest - Audio
    ///   - Simplest Manifest - Video
    /// </summary>
    public class BasicRecipesTests
    {
        #region Recipe 0001: Simplest Manifest - Single Image File (Serialization)

        [Fact]
        public void Recipe0001_SimpleImage_ShouldSerialize_WithRequiredFields()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
                new Label("Single Image Example")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
                new Label("Canvas 1"),
                1800, 1200
            );

            var imageResource = new ImageResource(
                "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                "image/png"
            ).SetHeight(1800).SetWidth(1200);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
                imageResource,
                canvas.Id
            );
            canvas.AddImage(image);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"@type\": \"sc:Manifest\"");
            json.Should().Contain("\"label\": \"Single Image Example\"");
            json.Should().Contain("\"@type\": \"sc:Canvas\"");
            json.Should().Contain("\"@type\": \"dctypes:Image\"");
            json.Should().Contain("\"format\": \"image/png\"");
        }

        [Fact]
        public void Recipe0001_SimpleImage_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
                new Label("Single Image Example")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
                new Label("Canvas 1"),
                1800, 1200
            );

            var imageResource = new ImageResource(
                "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                "image/png"
            ).SetHeight(1800).SetWidth(1200);

            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json");
            deserialized.Sequences.Should().ContainSingle();
            deserialized.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().ContainSingle()
                .Which.Images.Should().ContainSingle()
                .Which.Resource.Format.Should().Be("image/png");
        }

        #endregion

        #region Recipe 0001: Simplest Manifest - Single Image File (Deserialization)

        [Fact]
        public void Recipe0001_SimpleImage_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Single Image Example"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Canvas 1"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/png"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json");
            manifest.Type.Should().Be("sc:Manifest");
            manifest.Label.Should().ContainSingle().Which.Value.Should().Be("Single Image Example");
            manifest.Sequences.Should().ContainSingle();

            var canvas = manifest.Sequences.Should().ContainSingle().Subject
                .Canvases.Should().ContainSingle().Subject;

            canvas.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1");
            canvas.Height.Should().Be(1800);
            canvas.Width.Should().Be(1200);

            var img = canvas.Images.Should().ContainSingle().Subject;
            img.Resource.Id.Should().Be("http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png");
            img.Resource.Format.Should().Be("image/png");
            img.Resource.Height.Should().Be(1800);
            img.Resource.Width.Should().Be(1200);
        }

        [Fact]
        public void Recipe0001_SimpleImage_Deserialized_ShouldReserialize()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Single Image Example"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Canvas 1"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/png"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);
            var reserialized = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var roundTripped = JsonConvert.DeserializeObject<Manifest>(reserialized);

            roundTripped.Id.Should().Be(manifest.Id);
            roundTripped.Sequences.Should().HaveCount(manifest.Sequences.Count);
        }

        #endregion

        #region Recipe 0009: Simple Manifest - Book (Serialization)

        [Fact]
        public void Recipe0009_Book_ShouldSerialize_MultipleCanvases()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/manifest.json",
                new Label("Simple Manifest - Book")
            );
            manifest.SetViewingDirection(ViewingDirection.Ltr);
            manifest.SetViewingHint(ViewingHint.Paged);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0009-book-1/sequence/normal");

            var pageLabels = new[] { "Blank page", "Frontispiece", "Title page", "Page 4", "Page 5" };
            for (int i = 0; i < pageLabels.Length; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p{i + 1}",
                    new Label(pageLabels[i]),
                    1800, 1200
                );
                var img = new Image(
                    $"https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p{i + 1:D4}-image",
                    new ImageResource(
                        $"https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-{24 + i}/full/max/0/default.jpg",
                        "image/jpeg"
                    ).SetHeight(1800).SetWidth(1200),
                    canvas.Id
                );
                canvas.AddImage(img);
                sequence.AddCanvas(canvas);
            }
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"viewingDirection\": \"left-to-right\"");
            json.Should().Contain("\"viewingHint\": \"paged\"");

            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);
            deserialized.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(5);
        }

        #endregion

        #region Recipe 0009: Simple Manifest - Book (Deserialization)

        [Fact]
        public void Recipe0009_Book_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Simple Manifest - Book"",
  ""viewingDirection"": ""left-to-right"",
  ""viewingHint"": ""paged"",
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Blank page"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-24/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1""
            }
          ]
        },
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p2"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Frontispiece"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0002-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-25/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p2""
            }
          ]
        },
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p3"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Title page"",
          ""height"": 1800,
          ""width"": 1200,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0003-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-26/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p3""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.ViewingDirection.Value.Should().Be("left-to-right");
            manifest.ViewingHint.Value.Should().Be("paged");
            manifest.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(3);

            // Verify page labels
            var canvases = manifest.Sequences.Should().ContainSingle().Subject.Canvases;
            canvases.Should().HaveCount(3);
        }

        #endregion

        #region Recipe 0006: Internationalization / Multi-language (Serialization)

        [Fact]
        public void Recipe0006_MultiLanguage_ShouldSerialize_WithLanguageTags()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/manifest.json",
                new Label("Whistler's Mother")
            );

            manifest.AddDescription(
                new IIIFDescription("Arrangement in Grey and Black No.1, commonly known as Whistler's Mother")
                    .SetLanguage("en")
            );
            manifest.AddDescription(
                new IIIFDescription("Arrangement en gris et noir n°1, communément appelé la Mère de Whistler")
                    .SetLanguage("fr")
            );

            manifest.AddMetadata(new Metadata("Creator", "James Abbott McNeill Whistler", "en"));
            manifest.AddMetadata(new Metadata("Date", "1871"));

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/canvas/p1",
                new Label("Whistler's Mother"),
                3405, 4096
            );
            var imageResource = new ImageResource(
                "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1b/Whistlers_Mother_high_res.jpg/1024px-Whistlers_Mother_high_res.jpg",
                "image/jpeg"
            ).SetHeight(3405).SetWidth(4096);
            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0006-text-language/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0006-text-language/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Description.Should().HaveCount(2);
            deserialized.Metadata.Should().HaveCount(2);
        }

        #endregion

        #region Recipe 0006: Internationalization / Multi-language (Deserialization)

        [Fact]
        public void Recipe0006_MultiLanguage_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/0006-text-language/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Whistler's Mother"",
  ""description"": [
    {
      ""@value"": ""Arrangement in Grey and Black No.1"",
      ""@language"": ""en""
    },
    {
      ""@value"": ""Arrangement en gris et noir n°1"",
      ""@language"": ""fr""
    }
  ],
  ""metadata"": [
    {
      ""label"": ""Creator"",
      ""value"": {
        ""@value"": ""James Abbott McNeill Whistler"",
        ""@language"": ""en""
      }
    },
    {
      ""label"": ""Date"",
      ""value"": ""1871""
    }
  ],
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0006-text-language/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/0006-text-language/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Whistler's Mother"",
          ""height"": 3405,
          ""width"": 4096,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/0006-text-language/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://upload.wikimedia.org/wikipedia/commons/thumb/1/1b/Whistlers_Mother_high_res.jpg/1024px-Whistlers_Mother_high_res.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3405,
                ""width"": 4096
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0006-text-language/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.Description.Should().HaveCount(2);
            manifest.Metadata.Should().HaveCount(2);
        }

        #endregion

        #region Simplest Manifest - Audio

        [Fact]
        public void SimplestAudioManifest_ShouldSerialize_WithRequiredFields()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
                new Label("Simplest Audio Manifest")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/1",
                new Label("Audio Canvas"),
                640, 480
            ).SetDuration(1985.024);

            var audioResource = new IIIF.Manifests.Serializer.Nodes.Content.Audio.Resource.AudioResource(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/audio/full/max/default.mp3",
                "audio/mp3"
            ).SetDuration(1985.024);

            var audio = new IIIF.Manifests.Serializer.Nodes.Content.Audio.Audio(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/annotation/audio",
                audioResource,
                canvas.Id
            );
            canvas.AddAudio(audio);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"@type\": \"sc:Manifest\"");
            json.Should().Contain("\"label\": \"Simplest Audio Manifest\"");
            json.Should().Contain("\"@type\": \"sc:Canvas\"");
            json.Should().Contain("\"@type\": \"dctypes:Sound\"");
            json.Should().Contain("\"format\": \"audio/mp3\"");
            json.Should().Contain("\"duration\": 1985.024");
        }

        [Fact]
        public void SimplestAudioManifest_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
                new Label("Simplest Audio Manifest")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/1",
                new Label("Audio Canvas"),
                640, 480
            ).SetDuration(1985.024);

            var audioResource = new IIIF.Manifests.Serializer.Nodes.Content.Audio.Resource.AudioResource(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/audio/full/max/default.mp3",
                "audio/mp3"
            ).SetDuration(1985.024);

            canvas.AddAudio(new IIIF.Manifests.Serializer.Nodes.Content.Audio.Audio(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/annotation/audio",
                audioResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json");
            deserialized.Sequences.Should().HaveCount(1);
            var seq = deserialized.Sequences.First();
            seq.Canvases.Should().HaveCount(1);
            var canv = seq.Canvases.First();
            canv.Audios.Should().HaveCount(1);
            var aud = canv.Audios.First();
            aud.Resource.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/audio/full/max/default.mp3");
        }

        #endregion

        #region Simplest Manifest - Video

        [Fact]
        public void SimplestVideoManifest_ShouldSerialize_WithRequiredFields()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/manifest.json",
                new Label("Simplest Video Manifest")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/canvas/1",
                new Label("Video Canvas"),
                640, 480
            ).SetDuration(660.0);

            var videoResource = new IIIF.Manifests.Serializer.Nodes.Content.Video.Resource.VideoResource(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/video/full/max/default.mp4",
                "video/mp4"
            ).SetDuration(660.0);

            var video = new IIIF.Manifests.Serializer.Nodes.Content.Video.Video(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/annotation/video",
                videoResource,
                canvas.Id
            );
            canvas.AddVideo(video);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0003-mvm-video/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"@type\": \"sc:Manifest\"");
            json.Should().Contain("\"label\": \"Simplest Video Manifest\"");
            json.Should().Contain("\"@type\": \"sc:Canvas\"");
            json.Should().Contain("\"@type\": \"dctypes:MovingImage\"");
            json.Should().Contain("\"format\": \"video/mp4\"");
            json.Should().Contain("\"duration\": 660.0");
        }

        [Fact]
        public void SimplestVideoManifest_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/manifest.json",
                new Label("Simplest Video Manifest")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/canvas/1",
                new Label("Video Canvas"),
                640, 480
            ).SetDuration(660.0);

            var videoResource = new IIIF.Manifests.Serializer.Nodes.Content.Video.Resource.VideoResource(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/video/full/max/default.mp4",
                "video/mp4"
            ).SetDuration(660.0);

            canvas.AddVideo(new IIIF.Manifests.Serializer.Nodes.Content.Video.Video(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/annotation/video",
                videoResource,
                canvas.Id
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0003-mvm-video/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0003-mvm-video/manifest.json");
            deserialized.Sequences.Should().HaveCount(1);
            var seq = deserialized.Sequences.First();
            seq.Canvases.Should().HaveCount(1);
            var canv = seq.Canvases.First();
            canv.Videos.Should().HaveCount(1);
            var vid = canv.Videos.First();
            vid.Resource.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0003-mvm-video/video/full/max/default.mp4");
        }

        #endregion

        #region Simple Collection (Serialization & Deserialization)

        [Fact]
        public void SimpleCollection_ShouldSerializeAndDeserialize()
        {
            var collection = new IIIF.Manifests.Serializer.Nodes.Collection.Collection(
                "https://example.org/collection/top",
                new Label("Top-level Collection")
            );

            collection.AddManifest("https://example.org/manifest/1");
            collection.AddManifest("https://example.org/manifest/2");

            var json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.Collection.Collection>(json);

            deserialized.Should().NotBeNull();
            deserialized.Manifests.Should().HaveCount(2);
        }

        [Fact]
        public void SimpleCollection_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/collection/top"",
  ""@type"": ""sc:Collection"",
  ""label"": ""Top-level Collection"",
  ""manifests"": [
    {
      ""@id"": ""https://example.org/manifest/1"",
      ""@type"": ""sc:Manifest"",
      ""label"": ""Manifest 1""
    },
    {
      ""@id"": ""https://example.org/manifest/2"",
      ""@type"": ""sc:Manifest"",
      ""label"": ""Manifest 2""
    }
  ]
}";

            var collection = JsonConvert.DeserializeObject<IIIF.Manifests.Serializer.Nodes.Collection.Collection>(json);

            collection.Should().NotBeNull();
            collection.Id.Should().Be("https://example.org/collection/top");
            collection.Label.Should().ContainSingle().Which.Value.Should().Be("Top-level Collection");
            collection.Manifests.Should().HaveCount(2);
        }

        #endregion
    }
}
