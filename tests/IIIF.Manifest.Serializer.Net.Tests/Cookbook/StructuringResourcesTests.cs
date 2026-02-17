using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Nodes.StructureNode;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "Structuring Resources" category.
    /// https://iiif.io/api/cookbook/recipe/code/#structuring-resources
    ///
    /// Recipes covered:
    ///   - 0024: Table of Contents for Book Chapters (structures/ranges)
    ///   - Nested ranges and startCanvas
    ///   - Mixed canvases and ranges in structures
    /// </summary>
    public class StructuringResourcesTests
    {
        #region Recipe 0024: Table of Contents (Serialization)

        [Fact]
        public void Recipe0024_TOC_ShouldSerialize_StructuresWithCanvases()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/manifest.json",
                new Label("Simple Manifest - Book")
            );
            manifest.SetViewingDirection(ViewingDirection.Ltr);
            manifest.SetViewingHint(ViewingHint.Paged);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0009-book-1/sequence/normal");

            for (int i = 1; i <= 5; i++)
            {
                var canvas = new Canvas(
                    $"https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p{i}",
                    new Label($"Page {i}"),
                    1800, 1200
                );
                var res = new ImageResource(
                    $"https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-{24 + i}/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200);
                canvas.AddImage(new Image(
                    $"https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p{i:D4}-image",
                    res, canvas.Id
                ));
                sequence.AddCanvas(canvas);
            }
            manifest.AddSequence(sequence);

            // Table of Contents via structures
            var toc = new Structure(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/range/r0",
                new Label("Table of Contents")
            );
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p2");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p3");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p4");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p5");
            manifest.AddStructure(toc);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"structures\"");
            json.Should().Contain("\"canvases\"");
            json.Should().Contain("\"Table of Contents\"");
        }

        [Fact]
        public void Recipe0024_TOC_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/manifest.json",
                new Label("Simple Manifest - Book")
            );

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0009-book-1/sequence/normal");
            for (int i = 1; i <= 3; i++)
            {
                var canvas = new Canvas(
                    $"https://example.org/canvas/p{i}",
                    new Label($"Page {i}"),
                    1800, 1200
                );
                var res = new ImageResource($"https://example.org/img{i}.jpg", "image/jpeg")
                    .SetHeight(1800).SetWidth(1200);
                canvas.AddImage(new Image($"https://example.org/anno/{i}", res, canvas.Id));
                sequence.AddCanvas(canvas);
            }
            manifest.AddSequence(sequence);

            var toc = new Structure("https://example.org/range/toc", new Label("Table of Contents"));
            toc.AddCanvas("https://example.org/canvas/p1");
            toc.AddCanvas("https://example.org/canvas/p2");
            toc.AddCanvas("https://example.org/canvas/p3");
            manifest.AddStructure(toc);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Structures.Should().ContainSingle();
            deserialized.Structures.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(3);
        }

        #endregion

        #region Recipe 0024: Table of Contents (Deserialization)

        [Fact]
        public void Recipe0024_TOC_ShouldDeserializeFromJson()
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
                ""@id"": ""https://example.org/img1.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1800,
                ""width"": 1200
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1""
            }
          ]
        }
      ]
    }
  ],
  ""structures"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/0009-book-1/range/r0"",
      ""@type"": ""sc:Range"",
      ""label"": ""Table of Contents"",
      ""canvases"": [
        ""https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1""
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.Structures.Should().ContainSingle();
            var structure = manifest.Structures.Should().ContainSingle().Subject;
            structure.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0009-book-1/range/r0");
            structure.Label.Should().ContainSingle().Which.Value.Should().Be("Table of Contents");
            structure.Canvases.Should().ContainSingle();
        }

        #endregion

        #region Nested Ranges (Serialization & Deserialization)

        [Fact]
        public void NestedRanges_ShouldSerializeAndDeserialize()
        {
            var part1 = new Structure("https://example.org/range/part1");
            part1.AddLabel(new Label("Part I: Introduction"));
            part1.AddRange("https://example.org/range/chapter1");
            part1.AddRange("https://example.org/range/chapter2");

            var json = JsonConvert.SerializeObject(part1, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Structure>(json);

            deserialized.Ranges.Should().HaveCount(2);
            deserialized.Label.Should().ContainSingle()
                .Which.Value.Should().Be("Part I: Introduction");
        }

        [Fact]
        public void NestedRanges_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@id"": ""https://example.org/range/part1"",
  ""@type"": ""sc:Range"",
  ""label"": ""Part I: Introduction"",
  ""ranges"": [
    ""https://example.org/range/chapter1"",
    ""https://example.org/range/chapter2""
  ],
  ""canvases"": [
    ""https://example.org/canvas/toc""
  ]
}";

            var structure = JsonConvert.DeserializeObject<Structure>(json);

            structure.Should().NotBeNull();
            structure.Ranges.Should().HaveCount(2);
            structure.Canvases.Should().ContainSingle();
        }

        #endregion

        #region StartCanvas in Structure (Serialization & Deserialization)

        [Fact]
        public void StructureStartCanvas_ShouldRoundTrip()
        {
            var structure = new Structure("https://example.org/range/intro");
            structure.AddLabel(new Label("Introduction"));
            structure.AddCanvas("https://example.org/canvas/1");
            structure.AddCanvas("https://example.org/canvas/2");
            structure.SetStartCanvas("https://example.org/canvas/1");

            var json = JsonConvert.SerializeObject(structure, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Structure>(json);

            deserialized.StartCanvas.Should().Be("https://example.org/canvas/1");
            deserialized.Canvases.Should().HaveCount(2);
        }

        [Fact]
        public void StructureStartCanvas_ShouldDeserializeFromJson()
        {
            var json = @"{
  ""@id"": ""https://example.org/range/intro"",
  ""@type"": ""sc:Range"",
  ""label"": ""Introduction"",
  ""startCanvas"": ""https://example.org/canvas/1"",
  ""canvases"": [
    ""https://example.org/canvas/1"",
    ""https://example.org/canvas/2""
  ]
}";

            var structure = JsonConvert.DeserializeObject<Structure>(json);

            structure.StartCanvas.Should().Be("https://example.org/canvas/1");
            structure.Canvases.Should().HaveCount(2);
        }

        #endregion

        #region Full Book with Structure Deserialization

        [Fact]
        public void FullBookWithStructure_ShouldDeserializeComplexManifest()
        {
            var json = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/manifest/book-with-toc"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Book with Table of Contents"",
  ""viewingDirection"": ""left-to-right"",
  ""viewingHint"": ""paged"",
  ""sequences"": [
    {
      ""@id"": ""https://example.org/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://example.org/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Front Cover"",
          ""height"": 2000,
          ""width"": 1500,
          ""images"": [
            {
              ""@id"": ""https://example.org/annotation/p0001"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/image/cover.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 2000,
                ""width"": 1500
              },
              ""on"": ""https://example.org/canvas/p1""
            }
          ]
        },
        {
          ""@id"": ""https://example.org/canvas/p2"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Chapter 1"",
          ""height"": 2000,
          ""width"": 1500,
          ""images"": [
            {
              ""@id"": ""https://example.org/annotation/p0002"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/image/ch1.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 2000,
                ""width"": 1500
              },
              ""on"": ""https://example.org/canvas/p2""
            }
          ]
        }
      ]
    }
  ],
  ""structures"": [
    {
      ""@id"": ""https://example.org/range/toc"",
      ""@type"": ""sc:Range"",
      ""label"": ""Table of Contents"",
      ""ranges"": [
        ""https://example.org/range/chapter1""
      ],
      ""canvases"": [
        ""https://example.org/canvas/p1""
      ]
    }
  ]
}";

            var manifest = JsonConvert.DeserializeObject<Manifest>(json);

            manifest.Should().NotBeNull();
            manifest.ViewingDirection.Value.Should().Be("left-to-right");
            manifest.ViewingHint.Value.Should().Be("paged");
            manifest.Sequences.Should().ContainSingle()
                .Which.Canvases.Should().HaveCount(2);
            manifest.Structures.Should().ContainSingle();

            var toc = manifest.Structures.Should().ContainSingle().Subject;
            toc.Ranges.Should().ContainSingle();
            toc.Canvases.Should().ContainSingle();
        }

        #endregion
    }
}
