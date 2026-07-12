using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0001-0011: the simplest single-canvas image/audio/video manifests, plus the
///     language/rights/format basics and the "book" viewing-direction and paging-behavior variants.
/// </summary>
internal sealed class FoundationRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0001: Simplest Manifest - Single Image", Recipe0001),
            new("Recipe 0002: Simplest Manifest - Single Audio", Recipe0002),
            new("Recipe 0003: Simplest Manifest - Single Video", Recipe0003),
            new("Recipe 0004: Canvas Size and Image Size Differ", Recipe0004),
            new("Recipe 0005: Support IIIF Image Deep Zoom", Recipe0005),
            new("Recipe 0006: Simplest Manifest with Multiple Language Values", Recipe0006),
            new("Recipe 0007: String Formats (HTML in Descriptive Properties)", Recipe0007),
            new("Recipe 0008: Rights Statement", Recipe0008),
            new("Recipe 0009: Simplest Book", Recipe0009),
            new("Recipe 0010: Book with Right-to-Left Viewing Direction", Recipe0010Rtl),
            new("Recipe 0010: Diary with Top-to-Bottom Viewing Direction", Recipe0010Ttb),
            new("Recipe 0011: Continuous Scroll Behavior", Recipe0011Continuous),
            new("Recipe 0011: Individuals Behavior", Recipe0011Individuals)
        ];
    }

    // ---- 0001-mvm-image -----------------------------------------------------------------------

    private static Manifest Recipe0001()
    {
        var manifest = NewManifest("0001-mvm-image", "Single Image Example");
        var canvas = NewCanvas("0001-mvm-image", "p1", null, 1800, 1200);
        canvas.AddAnnotation(PaintingImage(canvas, "0001-mvm-image", "p0001-image",
            "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png", "image/png", 1800, 1200, null));
        return manifest.AddItem(canvas);
    }

    // ---- 0002-mvm-audio ------------------------------------------------------------------------

    private static Manifest Recipe0002()
    {
        var manifest = NewManifest("0002-mvm-audio", "Simplest Audio Example 1");
        var canvas = NewCanvas("0002-mvm-audio", "canvas", null, null, null).SetDuration(1985.024);
        canvas.AddAnnotation(new Annotation(Id("0002-mvm-audio", "canvas/page/annotation"),
            new AudioResource("https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4", "audio/mp4").SetDuration(1985.024),
            canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0003-mvm-video ------------------------------------------------------------------------

    private static Manifest Recipe0003()
    {
        var manifest = NewManifest("0003-mvm-video", "Video Example 3");
        var canvas = NewCanvas("0003-mvm-video", "canvas", null, 360, 480).SetDuration(572.034);
        canvas.AddAnnotation(new Annotation(Id("0003-mvm-video", "canvas/page/annotation"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/lunchroom_manners/high/lunchroom_manners_1024kb.mp4", "video/mp4")
                .SetHeight(360).SetWidth(480).SetDuration(572.034),
            canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0004-canvas-size ----------------------------------------------------------------------

    private static Manifest Recipe0004()
    {
        var manifest = NewManifest("0004-canvas-size", "Still image from an opera performance at Indiana University");
        var canvas = NewCanvas("0004-canvas-size", "p1", null, 1080, 1920);
        canvas.AddAnnotation(PaintingImage(canvas, "0004-canvas-size", "p0001-image",
            "https://fixtures.iiif.io/video/indiana/donizetti-elixir/act1-thumbnail.png", "image/png", 360, 640, null));
        return manifest.AddItem(canvas);
    }

    // ---- 0005-image-service --------------------------------------------------------------------

    private static Manifest Recipe0005()
    {
        var manifest = NewManifest("0005-image-service", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0005-image-service", "p1", "Canvas with a single IIIF image", 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0005-image-service", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));
        return manifest.AddItem(canvas);
    }

    // ---- 0006-text-language --------------------------------------------------------------------

    private static Manifest Recipe0006()
    {
        var manifest = NewManifest("0006-text-language", new Label("Whistler's Mother"), new Label("La Mère de Whistler", "fr"));
        manifest.AddMetadata(new Metadata("Creator", "Whistler, James Abbott McNeill"));
        manifest.AddMetadata(new Metadata("Subject", "McNeill Anna Matilda, mother of Whistler (1804-1881)", "en")
            .AddValue("McNeill Anna Matilda, mère de Whistler (1804-1881)", "fr"));
        manifest.SetSummary([
            new Description("Arrangement in Grey and Black No. 1, also called Portrait of the Artist's Mother.").SetLanguage("en"),
            new Description("Arrangement en gris et noir n°1, also called Portrait de la mère de l'artiste.").SetLanguage("fr")
        ]);
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Held By"), new Description("Musée d'Orsay, Paris, France")));

        var canvas = NewCanvas("0006-text-language", "p1", null, 991, 1114);
        var imageId = "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Whistlers_Mother/full/max/0/default.jpg";
        canvas.AddAnnotation(PaintingImage(canvas, "0006-text-language", "p0001-image", imageId, "image/jpeg", 991, 1114,
            "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Whistlers_Mother"));
        return manifest.AddItem(canvas);
    }

    // ---- 0007-string-formats -------------------------------------------------------------------

    private static Manifest Recipe0007()
    {
        var manifest = NewManifest("0007-string-formats", "Picture of Göttingen taken during the 2019 IIIF Conference");
        manifest.AddSummary(new Description("<p>Picture taken by the <a href=\"https://github.com/glenrobson\">IIIF Technical Coordinator</a></p>").SetLanguage("en"));
        manifest.AddMetadata(new Metadata("Author", "<span><a href='https://github.com/glenrobson'>Glen Robson</a></span>"));
        manifest.SetRights(new Rights("http://creativecommons.org/licenses/by-sa/3.0/"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description("<span>Glen Robson, IIIF Technical Coordinator. <a href=\"https://creativecommons.org/licenses/by-sa/3.0\">CC BY-SA 3.0</a> <img src=\"https://licensebuttons.net/l/by-sa/3.0/88x31.png\"/></span>").SetLanguage("en")));

        var canvas = NewCanvas("0007-string-formats", "p1", null, 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0007-string-formats", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));
        return manifest.AddItem(canvas);
    }

    // ---- 0008-rights ---------------------------------------------------------------------------

    private static Manifest Recipe0008()
    {
        var manifest = NewManifest("0008-rights", "Picture of Göttingen taken during the 2019 IIIF Conference");
        manifest.AddSummary(new Description("<p>Picture taken by the <a href=\"https://github.com/glenrobson\">IIIF Technical Coordinator</a></p>").SetLanguage("en"));
        manifest.SetRights(new Rights("http://creativecommons.org/licenses/by-sa/3.0/"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description(
                    "<span>Glen Robson, IIIF Technical Coordinator. <a href=\"https://creativecommons.org/licenses/by-sa/3.0\">CC BY-SA 3.0</a> <a href=\"https://creativecommons.org/licenses/by-sa/3.0\" title=\"CC BY-SA 3.0\"><img src=\"https://licensebuttons.net/l/by-sa/3.0/88x31.png\"/></a></span>")
                .SetLanguage("en")));

        var canvas = NewCanvas("0008-rights", "p1", null, 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0008-rights", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));
        return manifest.AddItem(canvas);
    }

    // ---- 0009-book-1 ---------------------------------------------------------------------------

    private static Manifest Recipe0009()
    {
        var manifest = NewManifest("0009-book-1", "Simple Manifest - Book").AddBehavior(new Behavior("paged"));
        var pages = new[] { ("f18", "Blank page", 4613, 3204), ("f19", "Frontispiece", 4612, 3186), ("f20", "Title page", 4613, 3204), ("f21", "Blank page", 4578, 3174), ("f22", "Bookplate", 4632, 3198) };
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label, height, width) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0009-book-1", $"p{n}", label, height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e9f3c1e674f-gallica_ark_12148_bpt6k1526005v_{suffix}/full/max/0/default.jpg";
            var serviceId = $"https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e9f3c1e674f-gallica_ark_12148_bpt6k1526005v_{suffix}";
            canvas.AddAnnotation(PaintingImage(canvas, "0009-book-1", $"p000{n}-image", imageId, "image/jpeg", height, width, serviceId));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    // ---- 0010-book-2-viewing-direction -----------------------------------------------------------

    private static Manifest Recipe0010Rtl()
    {
        var manifest = NewManifest("0010-book-2-viewing-direction", "Book with Right-to-Left Viewing Direction")
            .SetViewingDirection(ViewingDirection.Rtl);
        manifest.AddSummary(new Description("A Japanese playbill.").SetLanguage("en"));
        var pages = new[] { ("001", "front cover", 4823, 3497), ("002", "pages 1–2", 4804, 6062), ("003", "pages 3–4", 4776, 6127), ("004", "pages 5–6", 4751, 6124), ("005", "back cover", 4808, 3510) };
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label, height, width) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0010-book-2-viewing-direction", $"p{n}", label, height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_{suffix}/full/max/0/default.jpg";
            var serviceId = $"https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_{suffix}";
            canvas.AddAnnotation(PaintingImage(canvas, "0010-book-2-viewing-direction", $"p000{n}-image", imageId, "image/jpeg", height, width, serviceId));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    private static Manifest Recipe0010Ttb()
    {
        var manifest = NewManifest("0010-book-2-viewing-direction", "Diary with Top-to-Bottom Viewing Direction")
            .SetViewingDirection(ViewingDirection.Ttb);
        manifest.AddSummary(new Description("A travel diary.").SetLanguage("en"));
        var pages = new[] { ("02", 3152, 2251), ("03", 3135, 2268), ("04", 3135, 2274), ("05", 3135, 2268) };
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, height, width) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0010-book-2-viewing-direction", $"v{n}", $"image {n}", height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/9ee11092dfd2782634f5e8e2c87c16d5-uclamss_1841_diary_07_0{suffix}/full/max/0/default.jpg";
            var serviceId = $"https://iiif.io/api/image/3.0/example/reference/9ee11092dfd2782634f5e8e2c87c16d5-uclamss_1841_diary_07_0{suffix}";
            canvas.AddAnnotation(PaintingImage(canvas, "0010-book-2-viewing-direction", $"v000{n}-image", imageId, "image/jpeg", height, width, serviceId));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    // ---- 0011-book-3-behavior -------------------------------------------------------------------

    private static Manifest Recipe0011Continuous()
    {
        var manifest = NewManifest("0011-book-3-behavior", new Label("Ms. 21 Māzemurā Dāwit, Asmat", "gez"))
            .AddBehavior(new Behavior("continuous"));
        var sections = new[] { ("d9", 1592, 11368), ("ft", 1536, 11608), ("gb", 1504, 10576), ("hv", 1464, 2488) };
        for (var i = 0; i < sections.Length; i++)
        {
            var (suffix, height, width) = sections[i];
            var n = i + 1;
            var canvas = NewCanvas("0011-book-3-behavior", $"s{n}", $"Section {n} [Recto]", height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/8c169124171e6b2253b698a22a938f07-21198-zz001hbm{suffix}/full/max/0/default.jpg";
            var serviceId = $"https://iiif.io/api/image/3.0/example/reference/8c169124171e6b2253b698a22a938f07-21198-zz001hbm{suffix}";
            canvas.AddAnnotation(PaintingImage(canvas, "0011-book-3-behavior", $"s000{n}-image", imageId, "image/jpeg", height, width, serviceId));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    private static Manifest Recipe0011Individuals()
    {
        var manifest = NewManifest("0011-book-3-behavior", new Label("[Conoximent de las orines] Ihesus, Ihesus.", "ca"))
            .AddBehavior(new Behavior("individuals"));
        var labels = new[] { "inside cover; 1r", "2v, 3r", "3v, 4r", "4v, 5r" };
        var suffixes = new[] { "840", "882", "8b3", "8d4" };
        for (var i = 0; i < labels.Length; i++)
        {
            var n = i + 1;
            var canvas = NewCanvas("0011-book-3-behavior", $"v{n}", labels[i], 2250, 3375);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/85a96c630f077e6ac6cb984f1b752bbf-{i}-21198-zz00022{suffixes[i]}-1-master/full/max/0/default.jpg";
            var serviceId = $"https://iiif.io/api/image/3.0/example/reference/85a96c630f077e6ac6cb984f1b752bbf-{i}-21198-zz00022{suffixes[i]}-1-master";
            canvas.AddAnnotation(PaintingImage(canvas, "0011-book-3-behavior", $"v000{n}-image", imageId, "image/jpeg", 2250, 3375, serviceId));
            manifest.AddItem(canvas);
        }

        return manifest;
    }
}