using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace IIIF.Manifests.Serializer.Tests.Cookbook
{
    /// <summary>
    /// Tests for IIIF Cookbook "Linking &amp; Metadata Recipes" category.
    /// https://iiif.io/api/cookbook/recipe/code/
    ///
    /// Recipes covered:
    ///   - 0029: Metadata on any Resource
    ///   - 0046: Providing Alternative Representations (Rendering)
    ///   - 0047: Linking to Web Page of an Object (Homepage)
    ///   - 0053: Linking to Structured Metadata (SeeAlso)
    ///   - 0117: Image Thumbnail for Manifest
    /// </summary>
    [PresentationAPI("2.0", Notes = "Tests verify v2 serialization for linking and metadata properties that are compatible with both API 2.x and 3.0.")]
    public class LinkingMetadataRecipesTests
    {
        #region Recipe 0029: Metadata on any Resource

        [Fact]
        public void Recipe0029_MetadataAnywhere_ShouldSerialize_WithManifestAndCanvasMetadata()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/manifest.json",
                new Label("John Dee performing an experiment before Queen Elizabeth I.")
            );

            // Manifest-level metadata
            manifest.AddMetadata(new Metadata("Creator", "Glindoni, Henry Gillard, 1852-1913"));
            manifest.AddMetadata(new Metadata("Date", "1800-1899"));
            manifest.AddMetadata(new Metadata("Physical Description", "1 painting : oil on canvas ; canvas 152 x 244.4 cm"));

            manifest.AddAttribution(new Attribution("Wellcome Collection. Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)"));

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/canvas/p1",
                new Label("Painting under natural light"),
                1271, 2000
            );

            // Canvas-level metadata
            canvas.AddMetadata(new Metadata("Description", "The scene is the house at Mortlake of Dr John Dee (1527-1608)."));

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"metadata\"");
            json.Should().Contain("Creator");
            json.Should().Contain("Glindoni, Henry Gillard");
            json.Should().Contain("Date");
            json.Should().Contain("Physical Description");
            json.Should().Contain("attribution");
            json.Should().Contain("Wellcome Collection");
        }

        [Fact]
       public void Recipe0029_MetadataAnywhere_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0029-metadata-anywhere/manifest.json",
                new Label("John Dee performing an experiment")
            );

            manifest.AddMetadata(new Metadata("Creator", "Glindoni, Henry Gillard"));
            manifest.AddMetadata(new Metadata("Date", "1800-1899"));

            var canvas = new Canvas("https://example.org/canvas/p1", new Label("Canvas 1"), 1000, 800);
            canvas.AddMetadata(new Metadata("Description", "Canvas-specific description"));

            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Metadata.Should().HaveCount(2);
            deserialized.Label.First().Value.Should().Contain("John Dee");
            
            var firstCanvas = deserialized.Sequences.First().Canvases.First();
            firstCanvas.Metadata.Should().HaveCount(1);
            firstCanvas.Metadata.First().Label.Should().Be("Description");
        }

        #endregion

        #region Recipe 0046: Providing Alternative Representations (Rendering)

        [Fact]
        public void Recipe0046_Rendering_ShouldSerialize_WithPdfRendering()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0046-rendering/manifest.json",
                new Label("Alternative Representations Through Rendering")
            );

            var pdfRendering = new Rendering(
                "https://fixtures.iiif.io/other/UCLA/kabuki_ezukushi_rtl.pdf",
                "PDF version"
            ).SetFormat("application/pdf");

            manifest.AddRendering(pdfRendering);
            manifest.SetViewingDirection(ViewingDirection.Rtl);

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"rendering\"");
            json.Should().Contain("PDF version");
            json.Should().Contain("application/pdf");
            json.Should().Contain("kabuki_ezukushi_rtl.pdf");
            json.Should().Contain("\"viewingDirection\": \"right-to-left\"");
        }

        [Fact]
        public void Recipe0046_Rendering_ShouldRoundTrip()
        {
            var manifest = new Manifest(
                "https://example.org/manifest",
                new Label("Manifest with Rendering")
            );

            var rendering = new Rendering("https://example.org/document.pdf", "PDF version")
                .SetFormat("application/pdf");
            manifest.AddRendering(rendering);

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Rendering.Should().HaveCount(1);
            var firstRendering = deserialized.Rendering.First();
            firstRendering.Id.Should().Be("https://example.org/document.pdf");
            firstRendering.Format.Should().Be("application/pdf");
        }

        [Fact]
        public void Recipe0046_Rendering_ShouldSupport_MultipleRenderings()
        {
            var manifest = new Manifest("https://example.org/manifest", new Label("Multi-format Document"));

            manifest.AddRendering(new Rendering("https://example.org/doc.pdf", "PDF").SetFormat("application/pdf"));
            manifest.AddRendering(new Rendering("https://example.org/doc.epub", "ePub").SetFormat("application/epub+zip"));
            manifest.AddRendering(new Rendering("https://example.org/doc.txt", "Plain Text").SetFormat("text/plain"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("application/pdf");
            json.Should().Contain("application/epub+zip");
            json.Should().Contain("text/plain");
            json.Should().Contain("PDF");
            json.Should().Contain("ePub");
            json.Should().Contain("Plain Text");
        }

        #endregion

        #region Recipe 0047: Linking to Web Page (Homepage)

        [Fact]
        public void Recipe0047_Homepage_ShouldSerialize_WithInstitutionalLink()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0047-homepage/manifest.json",
                new Label("Laocöon")
            );

            var homepage = new Homepage(
                "https://www.getty.edu/art/collection/object/103RQQ",
                "Home page at the Getty Museum Collection"
            ).SetFormat("text/html");

            manifest.AddHomepage(homepage);

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Front"), 3000, 2315);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"homepage\"");
            json.Should().Contain("www.getty.edu");
            json.Should().Contain("Home page at the Getty Museum Collection");
            json.Should().Contain("text/html");
        }

        [Fact]
        public void Recipe0047_Homepage_ShouldRoundTrip()
        {
            var manifest = new Manifest("https://example.org/manifest", new Label("Object"));

            var homepage = new Homepage("https://example.org/object/123", "Object page")
                .SetFormat("text/html");
            manifest.AddHomepage(homepage);

            var canvas = new Canvas("https://example.org/canvas/1", new Label("View"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Homepage.Should().HaveCount(1);
            var firstHomepage = deserialized.Homepage.First();
            firstHomepage.Id.Should().Be("https://example.org/object/123");
            firstHomepage.Label.Should().Be("Object page");
            firstHomepage.Format.Should().Be("text/html");
        }

        [Fact]
        public void Recipe0047_Homepage_ShouldSupport_MultipleHomepages()
        {
            var manifest = new Manifest("https://example.org/manifest", new Label("Multi-institution Object"));

            manifest.AddHomepage(new Homepage("https://museum1.org/object/1", "Museum 1 page").SetFormat("text/html"));
            manifest.AddHomepage(new Homepage("https://museum2.org/item/1", "Museum 2 page").SetFormat("text/html"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Image"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Homepage.Should().HaveCount(2);
        }

        #endregion

        #region Recipe 0053: Linking to Structured Metadata (SeeAlso)

        [Fact]
        public void Recipe0053_SeeAlso_ShouldSerialize_WithMultipleMetadataFormats()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0053-seeAlso/manifest.json",
                new Label("Picture of Göttingen")
            );

            // Add multiple seeAlso entries for different formats
            manifest.AddSeeAlso(new SeeAlso("https://example.org/metadata/mods.xml").SetFormat("application/mods+xml"));
            manifest.AddSeeAlso(new SeeAlso("https://example.org/metadata/dc.xml").SetFormat("application/xml"));
            manifest.AddSeeAlso(new SeeAlso("https://example.org/metadata/marc.xml").SetFormat("application/marcxml+xml"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Photo"), 3024, 4032);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"seeAlso\"");
            json.Should().Contain("application/mods+xml");
            json.Should().Contain("application/xml");
            json.Should().Contain("application/marcxml+xml");
            json.Should().Contain("mods.xml");
            json.Should().Contain("dc.xml");
            json.Should().Contain("marc.xml");
        }

        [Fact]
        public void Recipe0053_SeeAlso_ShouldRoundTrip()
        {
            var manifest = new Manifest("https://example.org/manifest", new Label("Document"));

            manifest.AddSeeAlso(new SeeAlso("https://example.org/metadata.xml").SetFormat("application/xml"));
            manifest.AddSeeAlso(new SeeAlso("https://example.org/metadata.json").SetFormat("application/json"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.SeeAlso.Should().HaveCount(2);
            
            var xmlSeeAlso = deserialized.SeeAlso.First(s => s.Format == "application/xml");
            xmlSeeAlso.Id.Should().Be("https://example.org/metadata.xml");

            var jsonSeeAlso = deserialized.SeeAlso.First(s => s.Format == "application/json");
            jsonSeeAlso.Id.Should().Be("https://example.org/metadata.json");
        }

        #endregion

        #region Recipe 0117: Image Thumbnail for Manifest

        [Fact]
        public void Recipe0117_ThumbnailManifest_ShouldSerialize_WithThumbnail()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/0117-add-image-thumbnail/manifest.json",
                new Label("Playbill Cover with Manifest Thumbnail")
            );

            var thumbnail = new Thumbnail(
                "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla/full/max/0/default.jpg"
            );
            manifest.SetThumbnail(thumbnail);

            var canvas = new Canvas("https://example.org/canvas/p0", new Label("front cover"), 5312, 4520);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("\"thumbnail\"");
            json.Should().Contain("4f92cceb12dd53b52433425ce44308c7");
        }

        [Fact]
        public void Recipe0117_ThumbnailManifest_ShouldRoundTrip()
        {
            var manifest = new Manifest("https://example.org/manifest", new Label("Book with Thumbnail"));

            manifest.SetThumbnail(new Thumbnail("https://example.org/thumb.jpg"));
            manifest.AddDescription(new Description("A book with a representative thumbnail image"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page"), 1000, 800);
            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);

            deserialized.Should().NotBeNull();
            deserialized.Thumbnail.Should().NotBeNull();
            deserialized.Thumbnail.Id.Should().Be("https://example.org/thumb.jpg");
        }

        [Fact]
        public void Recipe0117_ThumbnailManifest_CanvasShouldAlsoHaveThumbnail()
        {
            var manifest = new Manifest("https://example.org/manifest", new Label("Book"));

            manifest.SetThumbnail(new Thumbnail("https://example.org/manifest-thumb.jpg"));

            var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800);
            canvas.SetThumbnail(new Thumbnail("https://example.org/canvas-thumb.jpg"));

            var sequence = new Sequence("https://example.org/seq1");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            json.Should().Contain("manifest-thumb.jpg");
            json.Should().Contain("canvas-thumb.jpg");

            var deserialized = JsonConvert.DeserializeObject<Manifest>(json);
            deserialized.Thumbnail.Id.Should().Be("https://example.org/manifest-thumb.jpg");
            deserialized.Sequences.First().Canvases.First().Thumbnail.Id.Should().Be("https://example.org/canvas-thumb.jpg");
        }

        #endregion
    }
}
