using System.Linq;
using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Content.OtherContent;
using IIIF.Manifests.Serializer.Nodes.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "Annotation Recipes" category.
    /// https://iiif.io/api/cookbook/recipe/code/#annotation-recipes
    ///
    /// Recipes covered:
    ///   - 0013: Tagging with Text
    ///   - 0014: Non-Rectangular Selection
    /// </summary>
    public class AnnotationRecipesTests
    {
        #region Recipe 0013: Tagging with Text (Serialization)

        [Fact]
        public void Recipe0013_Tagging_ShouldSerialize_WithOtherContent()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/manifest.json",
                new Label("Tagging with Text")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/canvas/p1",
                new Label("Canvas with Tags"),
                1000, 1000
            );

            // Add the main image
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(1000);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/annotation/p0001-image",
                imageResource,
                canvas.Id
            );
            canvas.AddImage(image);

            // Add external annotation list for tagging
            var annotationList = new OtherContent(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/list/p1"
            );
            canvas.AddOtherContent(annotationList);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0013-tagging/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"@type\": \"sc:Manifest\"");
            json.Should().Contain("\"label\": \"Tagging with Text\"");
            json.Should().Contain("\"@type\": \"sc:Canvas\"");
            json.Should().Contain("\"otherContent\":");
            json.Should().Contain("\"@type\": \"sc:AnnotationList\"");
        }

        [Fact]
        public void Recipe0013_Tagging_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/manifest.json",
                new Label("Tagging with Text")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/canvas/p1",
                new Label("Canvas with Tags"),
                1000, 1000
            );

            // Add the main image
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(1000);

            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            // Add external annotation list for tagging
            canvas.AddOtherContent(new OtherContent(
                "https://iiif.io/api/cookbook/recipe/0013-tagging/list/p1"
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0013-tagging/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0013-tagging/manifest.json");
            deserialized.Sequences.Should().HaveCount(1);
            var seq = deserialized.Sequences.First();
            seq.Canvases.Should().HaveCount(1);
            var canv = seq.Canvases.First();
            canv.OtherContents.Should().HaveCount(1);
            var otherContent = canv.OtherContents.First();
            otherContent.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0013-tagging/list/p1");
        }

        #endregion

        #region Recipe 0014: Non-Rectangular Selection (Serialization)

        [Fact]
        public void Recipe0014_NonRectangularSelection_ShouldSerialize_WithOtherContent()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/manifest.json",
                new Label("Non-Rectangular Selection with External Annotations")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/canvas/p1",
                new Label("Canvas with Non-Rectangular Annotations"),
                3024, 4032
            );

            // Add the main image
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/annotation/p0001-image",
                imageResource,
                canvas.Id
            );
            canvas.AddImage(image);

            // Add external annotation list for non-rectangular selections
            var annotationList = new OtherContent(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/list/p1"
            );
            canvas.AddOtherContent(annotationList);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"@type\": \"sc:Manifest\"");
            json.Should().Contain("\"label\": \"Non-Rectangular Selection with External Annotations\"");
            json.Should().Contain("\"@type\": \"sc:Canvas\"");
            json.Should().Contain("\"otherContent\":");
            json.Should().Contain("\"@type\": \"sc:AnnotationList\"");
        }

        [Fact]
        public void Recipe0014_NonRectangularSelection_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/manifest.json",
                new Label("Non-Rectangular Selection with External Annotations")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/canvas/p1",
                new Label("Canvas with Non-Rectangular Annotations"),
                3024, 4032
            );

            // Add the main image
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);

            canvas.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/annotation/p0001-image",
                imageResource,
                canvas.Id
            ));

            // Add external annotation list for non-rectangular selections
            canvas.AddOtherContent(new OtherContent(
                "https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/list/p1"
            ));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/manifest.json");
            deserialized.Sequences.Should().HaveCount(1);
            var seq = deserialized.Sequences.First();
            seq.Canvases.Should().HaveCount(1);
            var canv = seq.Canvases.First();
            canv.OtherContents.Should().HaveCount(1);
            var otherContent = canv.OtherContents.First();
            otherContent.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0014-non-rectangular-selection/list/p1");
        }
        }

        #endregion
    }