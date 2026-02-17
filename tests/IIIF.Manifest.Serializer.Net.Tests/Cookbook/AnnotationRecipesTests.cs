using System.Linq;
using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.ContentNode.OtherContent;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "Annotation Recipes" category.
    /// https://iiif.io/api/cookbook/recipe/code/#annotation-recipes
    ///
    /// Recipes covered:
    ///   - 0013: Tagging with Text
    ///   - 0014: Non-Rectangular Selection
    ///   - 0015: Choice of Different Versions
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

        #endregion

        #region Recipe 0015: Choice of Different Versions (Serialization)

        [Fact]
        public void Recipe0015_ChoiceOfVersions_ShouldSerialize_WithMultipleSequences()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/manifest.json",
                new Label("Choice of Different Versions")
            );

            // Sequence 1: High resolution version
            var sequence1 = new Sequence("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/high-res");
            sequence1.AddLabel(new Label("High Resolution Version"));

            var canvas1 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/canvas/p1-high",
                new Label("High Resolution Image"),
                3024, 4032
            );

            var imageResource1 = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);

            var image1 = new Image(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/annotation/p0001-image-high",
                imageResource1,
                canvas1.Id
            );

            canvas1.AddImage(image1);
            sequence1.AddCanvas(canvas1);
            manifest.AddSequence(sequence1);

            // Sequence 2: Low resolution version
            var sequence2 = new Sequence("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/low-res");
            sequence2.AddLabel(new Label("Low Resolution Version"));

            var canvas2 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/canvas/p1-low",
                new Label("Low Resolution Image"),
                1000, 1000
            );

            var imageResource2 = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(1000);

            var image2 = new Image(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/annotation/p0001-image-low",
                imageResource2,
                canvas2.Id
            );

            canvas2.AddImage(image2);
            sequence2.AddCanvas(canvas2);
            manifest.AddSequence(sequence2);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"@type\": \"sc:Manifest\"");
            json.Should().Contain("\"label\": \"Choice of Different Versions\"");
            json.Should().Contain("\"sequences\":");
            json.Should().Contain("\"@type\": \"sc:Sequence\"");
            json.Should().Contain("\"label\": \"High Resolution Version\"");
            json.Should().Contain("\"label\": \"Low Resolution Version\"");
            json.Should().Contain("\"@type\": \"sc:Canvas\"");
            json.Should().Contain("\"height\": 3024");
            json.Should().Contain("\"width\": 4032");
            json.Should().Contain("\"height\": 1000");
            json.Should().Contain("\"width\": 1000");
        }

        [Fact]
        public void Recipe0015_ChoiceOfVersions_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/manifest.json",
                new Label("Choice of Different Versions")
            );

            // Sequence 1: High resolution version
            var sequence1 = new Sequence("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/high-res");
            sequence1.AddLabel(new Label("High Resolution Version"));

            var canvas1 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/canvas/p1-high",
                new Label("High Resolution Image"),
                3024, 4032
            );

            var imageResource1 = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3024).SetWidth(4032);

            canvas1.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/annotation/p0001-image-high",
                imageResource1,
                canvas1.Id
            ));

            sequence1.AddCanvas(canvas1);
            manifest.AddSequence(sequence1);

            // Sequence 2: Low resolution version
            var sequence2 = new Sequence("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/low-res");
            sequence2.AddLabel(new Label("Low Resolution Version"));

            var canvas2 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/canvas/p1-low",
                new Label("Low Resolution Image"),
                1000, 1000
            );

            var imageResource2 = new ImageResource(
                "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(1000).SetWidth(1000);

            canvas2.AddImage(new Image(
                "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/annotation/p0001-image-low",
                imageResource2,
                canvas2.Id
            ));

            sequence2.AddCanvas(canvas2);
            manifest.AddSequence(sequence2);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/manifest.json");
            deserialized.Sequences.Should().HaveCount(2);

            // Check first sequence (high res)
            var seq1 = deserialized.Sequences.First(s => s.Id == "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/high-res");
            seq1.Label.Should().ContainSingle().Which.Value.Should().Be("High Resolution Version");
            seq1.Canvases.Should().HaveCount(1);
            var canv1 = seq1.Canvases.First();
            canv1.Height.Should().Be(3024);
            canv1.Width.Should().Be(4032);

            // Check second sequence (low res)
            var seq2 = deserialized.Sequences.First(s => s.Id == "https://iiif.io/api/cookbook/recipe/0015-choice-of-versions/sequence/low-res");
            seq2.Label.Should().ContainSingle().Which.Value.Should().Be("Low Resolution Version");
            seq2.Canvases.Should().HaveCount(1);
            var canv2 = seq2.Canvases.First();
            canv2.Height.Should().Be(1000);
            canv2.Width.Should().Be(1000);
        }

        #endregion
    }
}