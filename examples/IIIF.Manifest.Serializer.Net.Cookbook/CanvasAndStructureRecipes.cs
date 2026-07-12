using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0013-0026: canvas-level features (placeholder/accompanying canvases, deep-linking
///     start, A/V transcription rendering, supplementary annotations - HTML, tagging, hotspot linking)
///     and the first Range/Structure-based tables of contents.
/// </summary>
internal sealed class CanvasAndStructureRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0013: Placeholder Canvas", Recipe0013),
            new("Recipe 0014: Accompanying Canvas", Recipe0014),
            new("Recipe 0015: Deep Linking with Start", Recipe0015),
            new("Recipe 0017: Transcription of Audio/Video Content", Recipe0017),
            new("Recipe 0019: HTML in Annotations", Recipe0019),
            new("Recipe 0021: Tagging", Recipe0021),
            new("Recipe 0022: Linking with a Hotspot", Recipe0022),
            new("Recipe 0024: Table of Contents for Book", Recipe0024),
            new("Recipe 0026: Table of Contents for Opera", Recipe0026)
        ];
    }

    // ---- 0013-placeholderCanvas ------------------------------------------------------------------

    private static Manifest Recipe0013()
    {
        var manifest = NewManifest("0013-placeholderCanvas", "Video recording of Donizetti's _The Elixer of Love_");
        var canvas = NewCanvas("0013-placeholderCanvas", "donizetti", null, 360, 640).SetDuration(7278.466);

        var placeholder = NewCanvas("0013-placeholderCanvas", "donizetti/placeholder", null, 360, 640);
        placeholder.AddAnnotation(PaintingImage(placeholder, "0013-placeholderCanvas", "donizetti/placeholder/1-image",
            "https://fixtures.iiif.io/video/indiana/donizetti-elixir/act1-thumbnail.png", "image/png", 360, 640, null));
        canvas.SetPlaceholderCanvas(placeholder);

        canvas.AddAnnotation(new Annotation(Id("0013-placeholderCanvas", "donizetti/1-video"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low.mp4", "video/mp4")
                .SetHeight(360).SetWidth(640).SetDuration(7278.466),
            canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0014-accompanyingcanvas -----------------------------------------------------------------

    private static Manifest Recipe0014()
    {
        var manifest = NewManifest("0014-accompanyingcanvas", "Partial audio recording of Gustav Mahler's _Symphony No. 3_");
        var canvas = NewCanvas("0014-accompanyingcanvas", "p1", "Gustav Mahler, Symphony No. 3, CD 1", null, null).SetDuration(1985.024);
        canvas.AddAnnotation(new Annotation(Id("0014-accompanyingcanvas", "canvas/page/annotation/segment1-audio"),
            new AudioResource("https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4", "video/mp4").SetDuration(1985.024),
            canvas.Id));

        var accompanying = NewCanvas("0014-accompanyingcanvas", "accompanying", "First page of score for Gustav Mahler, Symphony No. 3", 998, 772);
        accompanying.AddAnnotation(PaintingImage(accompanying, "0014-accompanyingcanvas", "accompanying/annotation/image",
            "https://iiif.io/api/image/3.0/example/reference/4b45bba3ea612ee46f5371ce84dbcd89-mahler-0/full/,998/0/default.jpg", "image/jpeg", 998, 772,
            "https://iiif.io/api/image/3.0/example/reference/4b45bba3ea612ee46f5371ce84dbcd89-mahler-0"));
        canvas.SetAccompanyingCanvas(new AccompanyingCanvas(accompanying.Id));

        return manifest.AddItem(canvas).AddItem(accompanying);
    }

    // ---- 0015-start -----------------------------------------------------------------------------

    private static Manifest Recipe0015()
    {
        var manifest = NewManifest("0015-start", "Video of a 30-minute digital clock");
        manifest.SetRights(new Rights("http://creativecommons.org/licenses/by/3.0/"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description("DrLex1. <a href=\"https://creativecommons.org/licenses/by/3.0\">CC BY 3.0</a>").SetLanguage("en")));

        var canvas = NewCanvas("0015-start", "segment1", null, null, null).SetDuration(1801.055);
        canvas.AddAnnotation(new Annotation(Id("0015-start", "annotation/segment1-video"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/30-minute-clock/medium/30-minute-clock.mp4", "video/mp4").SetDuration(1801.055),
            canvas.Id));

        manifest.SetStart(new AnnotationTarget(canvas.Id, "Canvas").SetSpecificResourceId(Id("0015-start", "canvas-start/segment1")).SetSelector(PointSelector.ForTemporalPoint(120.5)));
        return manifest.AddItem(canvas);
    }

    // ---- 0017-transcription-av -------------------------------------------------------------------

    private static Manifest Recipe0017()
    {
        var manifest = NewManifest("0017-transcription-av", "Volleyball for Boys");
        var canvas = NewCanvas("0017-transcription-av", "canvas", null, 1080, 1920).SetDuration(662.037);
        canvas.AddAnnotation(new Annotation(Id("0017-transcription-av", "canvas/page/annotation"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/volleyball/high/volleyball-for-boys.mp4", "video/mp4")
                .SetHeight(1080).SetWidth(1920).SetDuration(662.037),
            canvas.Id));
        canvas.AddRendering(new Rendering("https://fixtures.iiif.io/video/indiana/volleyball/volleyball.txt", "Transcript").SetFormat("text/plain"));
        return manifest.AddItem(canvas);
    }

    // ---- 0019-html-in-annotations ----------------------------------------------------------------

    private static Manifest Recipe0019()
    {
        var manifest = NewManifest("0019-html-in-annotations", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0019-html-in-annotations", "canvas-1", null, 3024, 4032, "canvas-1");
        canvas.AddAnnotation(PaintingImage(canvas, "0019-html-in-annotations", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, true));

        var comment = new Annotation(Id("0019-html-in-annotations", "canvas-1/annopage-2/anno-1"),
                new TextualBody(
                        "<p>Göttinger Marktplatz mit <a href='https://de.wikipedia.org/wiki/G%C3%A4nseliesel-Brunnen_(G%C3%B6ttingen)'>Gänseliesel Brunnen <img src='https://en.wikipedia.org/static/images/project-logos/enwiki.png' alt='Wikipedia logo'></a></p>")
                    .SetLanguage("de").SetFormat("text/html"),
                canvas.Id)
            .SetMotivation("commenting");
        var page = new AnnotationPage(Id("0019-html-in-annotations", "canvas-1/annopage-2"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, comment);
    }

    // ---- 0021-tagging ---------------------------------------------------------------------------

    private static Manifest Recipe0021()
    {
        var manifest = NewManifest("0021-tagging", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0021-tagging", "p1", null, 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0021-tagging", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));

        var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(265, 661, 1260, 1239));
        var tag = new Annotation(Id("0021-tagging", "annotation/p0002-tag"),
                new TextualBody("Gänseliesel-Brunnen").SetLanguage("de").SetFormat("text/plain"), target)
            .SetMotivation("tagging");
        var page = new AnnotationPage(Id("0021-tagging", "page/p2/1"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, tag);
    }

    // ---- 0022-linking-with-a-hotspot --------------------------------------------------------------

    private static Manifest Recipe0022()
    {
        var manifest = NewManifest("0022-linking-with-a-hotspot", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas1 = NewCanvas("0022-linking-with-a-hotspot", "p1", null, 3024, 4032);
        canvas1.AddAnnotation(PaintingImage(canvas1, "0022-linking-with-a-hotspot", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));

        var canvas2 = NewCanvas("0022-linking-with-a-hotspot", "p2", null, 4032, 3024);
        var fountainImage = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-fountain/full/max/0/default.jpg";
        var fountainService = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-fountain";
        canvas2.AddAnnotation(PaintingImage(canvas2, "0022-linking-with-a-hotspot", "p0002-image", fountainImage, "image/jpeg", 4032, 3024, fountainService));

        var linkTarget = new AnnotationTarget(canvas1.Id).SetSelector(FragmentSelector.ForRegion(265, 661, 1260, 1239));
        var linkedCanvasRef = new SpecificResource(new BaseResource(canvas2.Id, "Canvas"));
        var link = new Annotation(Id("0022-linking-with-a-hotspot", "annotation/p0002-link"),
                new TextualBody("A link to a close up of Gänseliesel-Brunnen fountain.").SetLanguage("de").SetFormat("text/plain"), linkTarget)
            .AddBody(linkedCanvasRef)
            .SetMotivation("linking");
        var page = new AnnotationPage(Id("0022-linking-with-a-hotspot", "page/p1/2"));
        canvas1.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas1, page, link).AddItem(canvas2);
    }

    // ---- 0024-book-4-toc ------------------------------------------------------------------------

    private static Manifest Recipe0024()
    {
        var manifest = NewManifest("0024-book-4-toc", "Ethiopic Ms 10");
        var folios = new[] { ("1", "f. 1r", 2504, 1768), ("2", "f. 1v", 2512, 1792), ("3", "f. 2r", 2456, 1792), ("4", "f. 2v", 2440, 1760), ("5", "f. 3r", 2416, 1776), ("6", "f. 3v", 2416, 1776) };
        for (var i = 0; i < folios.Length; i++)
        {
            var (n, label, height, width) = folios[i];
            var canvas = NewCanvas("0024-book-4-toc", $"p{n}", label, height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/d3bbf5397c6df6b894c5991195c912ab-{n}-21198-zz001hbmd9_master/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0024-book-4-toc", $"p000{n}-image", imageId, "image/jpeg", height, width,
                $"https://iiif.io/api/image/3.0/example/reference/d3bbf5397c6df6b894c5991195c912ab-{n}-21198-zz001hbmd9_master"));
            manifest.AddItem(canvas);
        }

        var toc = new Structure(Id("0024-book-4-toc", "range/r0"), new Label("Table of Contents"));
        var arede = new Structure(Id("0024-book-4-toc", "range/r2"), new Label("Arede'et", "gez"));
        arede.AddItem(new Structure(Id("0024-book-4-toc", "range/r2/1"), new Label("Monday")).AddCanvasReference(CanvasId("0024-book-4-toc", "p3")).AddCanvasReference(CanvasId("0024-book-4-toc", "p4")));
        arede.AddItem(new Structure(Id("0024-book-4-toc", "range/r2/2"), new Label("Tuesday")).AddCanvasReference(CanvasId("0024-book-4-toc", "p5")).AddCanvasReference(CanvasId("0024-book-4-toc", "p6")));
        toc.AddItem(new Structure(Id("0024-book-4-toc", "range/r1"), new Label("Tabiba Tabiban", "gez")).AddCanvasReference(CanvasId("0024-book-4-toc", "p1")).AddCanvasReference(CanvasId("0024-book-4-toc", "p2")));
        toc.AddItem(arede);
        return manifest.AddStructure(toc);
    }

    // ---- 0026-toc-opera -------------------------------------------------------------------------

    private static Manifest Recipe0026()
    {
        var manifest = NewManifest("0026-toc-opera", new Label("The Elixir of Love"), new Label("L'Elisir D'Amore", "it"));
        var canvas = NewCanvas("0026-toc-opera", "1", null, 1080, 1920).SetDuration(7278.422);
        canvas.AddAnnotation(new Annotation(Id("0026-toc-opera", "canvas/1/annotation_page/1/annotation/1"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low.mp4", "video/mp4")
                .SetHeight(1080).SetWidth(1920).SetDuration(7278.422),
            canvas.Id));
        manifest.AddItem(canvas);

        var root = new Structure(Id("0026-toc-opera", "range/1"), new Label("Gaetano Donizetti, L'Elisir D'Amore", "it"));
        var atto1 = new Structure(Id("0026-toc-opera", "range/2"), new Label("Atto Primo", "it"));
        atto1.AddItem(new Structure(Id("0026-toc-opera", "range/3"), new Label("Preludio e Coro d'introduzione", "it")).AddCanvasReference($"{canvas.Id}#t=0,302.05"));
        atto1.AddItem(new Structure(Id("0026-toc-opera", "range/4"), new Label("Remainder of Atto Primo")).AddCanvasReference($"{canvas.Id}#t=302.05,3971.24"));
        root.AddItem(atto1);
        root.AddItem(new Structure(Id("0026-toc-opera", "range/5"), new Label("Atto Secondo", "it")).AddCanvasReference($"{canvas.Id}#t=3971.24"));
        return manifest.AddStructure(root);
    }
}