using System.Linq;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.ContentState;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

public sealed record ExampleDefinition(string Title, Func<object> Build)
{
    public void Run()
    {
        Console.WriteLine(Title);
        Console.WriteLine(new string('=', Title.Length));
        Console.WriteLine(JsonConvert.SerializeObject(Build(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }
}

/// <summary>
/// Faithful C# reconstructions of every real recipe in github.com/IIIF/cookbook-recipes (71
/// recipes; 0000_template/0231-transcript-meta-recipe/0466-link-for-loading-manifest are excluded
/// as non-recipes with no manifest JSON of their own). See SDK_VERSIONING_GUIDE.md for the
/// milestone history of the SDK features (Groups A-I) this catalog exercises.
/// </summary>
public static class CookbookCatalog
{
    public static IReadOnlyList<ExampleDefinition> GetAll() =>
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
        new("Recipe 0011: Individuals Behavior", Recipe0011Individuals),
        new("Recipe 0013: Placeholder Canvas", Recipe0013),
        new("Recipe 0014: Accompanying Canvas", Recipe0014),
        new("Recipe 0015: Deep Linking with Start", Recipe0015),
        new("Recipe 0017: Transcription of Audio/Video Content", Recipe0017),
        new("Recipe 0019: HTML in Annotations", Recipe0019),
        new("Recipe 0021: Tagging", Recipe0021),
        new("Recipe 0022: Linking with a Hotspot", Recipe0022),
        new("Recipe 0024: Table of Contents for Book", Recipe0024),
        new("Recipe 0026: Table of Contents for Opera", Recipe0026),
        new("Recipe 0027: Alternative Page Order", Recipe0027),
        new("Recipe 0029: Metadata Anywhere", Recipe0029),
        new("Recipe 0030: Multi-Volume Work (Collection)", Recipe0030),
        new("Recipe 0031: Bound Multi-Volume Work (Single Manifest)", Recipe0031),
        new("Recipe 0032: Simple Collection", Recipe0032),
        new("Recipe 0033: Image Choice", Recipe0033),
        new("Recipe 0035: Foldouts", Recipe0035),
        new("Recipe 0036: Composition From Multiple Images", Recipe0036),
        new("Recipe 0040: Image Rotation Service", Recipe0040Service),
        new("Recipe 0040: Image Rotation via CSS", Recipe0040Css),
        new("Recipe 0045: Simple Annotation with CSS Styling", Recipe0045),
        new("Recipe 0046: Rendering", Recipe0046),
        new("Recipe 0047: Linking to Structured Metadata (Homepage)", Recipe0047),
        new("Recipe 0053: Linking to Structured Metadata (SeeAlso)", Recipe0053),
        new("Recipe 0057: Presentation API 2.1 vs 3.0", Recipe0057),
        new("Recipe 0064: Book/Opera - One Canvas", Recipe0064),
        new("Recipe 0065: Book/Opera - Multiple Canvases", Recipe0065),
        new("Recipe 0068: Newspaper", Recipe0068),
        new("Recipe 0074: Multiple Language Captions", Recipe0074),
        new("Recipe 0103: Simple Annotation on Audio Segment", Recipe0103),
        new("Recipe 0117: Add a Thumbnail to a Manifest", Recipe0117),
        new("Recipe 0118: Multiple Values in Metadata", Recipe0118),
        new("Recipe 0135: Annotating a Point in a Canvas", Recipe0135),
        new("Recipe 0139: Geolocate a Canvas Fragment", Recipe0139),
        new("Recipe 0154: Simple navPlace Extension", Recipe0154),
        new("Recipe 0202: Manifest Start Canvas", Recipe0202),
        new("Recipe 0219: Using a Caption File", Recipe0219),
        new("Recipe 0229: Table of Contents with Behavior", Recipe0229),
        new("Recipe 0230: NavDate on Collection and Manifest", Recipe0230),
        new("Recipe 0232: Thumbnail on a Canvas (Video)", Recipe0232Av),
        new("Recipe 0232: Thumbnail on a Canvas (Image)", Recipe0232Image),
        new("Recipe 0234: Provider", Recipe0234),
        new("Recipe 0240: navPlace on Canvases", Recipe0240),
        new("Recipe 0258: Tagging an External Resource", Recipe0258),
        new("Recipe 0261: Non-Rectangular Commenting", Recipe0261),
        new("Recipe 0266: Annotating the Whole Canvas", Recipe0266),
        new("Recipe 0269: Embedded or Referenced Annotations", Recipe0269),
        new("Recipe 0283: Missing Image", Recipe0283),
        new("Recipe 0299: Cropping an Image (Region)", Recipe0299),
        new("Recipe 0306: Linking Annotations to Manifests", Recipe0306),
        new("Recipe 0306: Referenced AnnotationPage", Recipe0306AnnotationPage),
        new("Recipe 0309: Annotation Collection", Recipe0309),
        new("Recipe 0309: Standalone AnnotationCollection Document", Recipe0309Collection),
        new("Recipe 0318: navPlace and navDate on a Collection", Recipe0318),
        new("Recipe 0326: Annotating a Choice of Images", Recipe0326),
        new("Recipe 0346: Multilingual Annotation Body", Recipe0346),
        new("Recipe 0377: Image in Annotation (Multiple Bodies)", Recipe0377),
        new("Recipe 0434: Choice of Audio/Video Formats", Recipe0434),
        new("Recipe 0464: Reusing Another Manifest's Canvas", Recipe0464),
        new("Recipe 0485: Content State - Canvas Region", Recipe0485),
        new("Recipe 0489: Multimedia Canvas", Recipe0489),
        new("Recipe 0540: Content State - Opening Multiple Canvases", Recipe0540),
        new("Recipe 0540: Content State Document", Recipe0540ContentState),
        new("Recipe 0560: Resources on a Timeline", Recipe0560),
        new("Recipe 0561: Visible Text Painted on an Image", Recipe0561),
        new("Recipe 0599: Content State - Drag and Drop", Recipe0599)
    ];

    private const string Base = "https://iiif.io/api/cookbook/recipe";
    private const string ImageService3 = "http://iiif.io/api/image/3/context.json";

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
            new Description("<span>Glen Robson, IIIF Technical Coordinator. <a href=\"https://creativecommons.org/licenses/by-sa/3.0\">CC BY-SA 3.0</a> <a href=\"https://creativecommons.org/licenses/by-sa/3.0\" title=\"CC BY-SA 3.0\"><img src=\"https://licensebuttons.net/l/by-sa/3.0/88x31.png\"/></a></span>").SetLanguage("en")));

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
        canvas.SetAccompanyingCanvas(new Properties.AccompanyingCanvas(accompanying.Id));

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
        canvas.AddAnnotation(PaintingImage(canvas, "0019-html-in-annotations", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, idIsFull: true));

        var comment = new Annotation(Id("0019-html-in-annotations", "canvas-1/annopage-2/anno-1"),
                new TextualBody("<p>Göttinger Marktplatz mit <a href='https://de.wikipedia.org/wiki/G%C3%A4nseliesel-Brunnen_(G%C3%B6ttingen)'>Gänseliesel Brunnen <img src='https://en.wikipedia.org/static/images/project-logos/enwiki.png' alt='Wikipedia logo'></a></p>")
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

    // ---- 0027-alternative-page-order --------------------------------------------------------------

    private static Manifest Recipe0027()
    {
        var manifest = NewManifest("0027-alternative-page-order", "Alternative Page Sequences");
        var provider = new Provider("https://www.bibliotecabertoliana.it/", new Label("Biblioteca Civica Bertoliana"))
            .AddHomepage(new Homepage("https://www.bibliotecabertoliana.it/", "Biblioteca Civica Bertoliana Homepage").SetFormat("text/html"))
            .AddLogo(new Logo("https://www.bibliotecabertoliana.it/img/logo-bertoliana.png").SetFormat("image/png").SetHeight(346).SetWidth(89));
        manifest.AddProvider(provider);

        var pages = new[] { ("387", "171r (387)"), ("384", "171v (384)"), ("385", "172r (385)"), ("386", "172v [386]") };
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0027-alternative-page-order", $"p{n}", label, 2156, 3184);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/3ec31f43ce55cfcc076804c88c06aa43-CF-f0186r_aVISNK_{suffix}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0027-alternative-page-order", $"p000{n}-image", imageId, "image/jpeg", 2156, 3184,
                $"https://iiif.io/api/image/3.0/example/reference/3ec31f43ce55cfcc076804c88c06aa43-CF-f0186r_aVISNK_{suffix}"));
            manifest.AddItem(canvas);
        }

        var physical = new Structure(Id("0027-alternative-page-order", "range/r1"), new Label("Physical sequence")).AddBehavior(new Behavior("sequence"));
        for (var n = 1; n <= 4; n++)
        {
            physical.AddCanvasReference(CanvasId("0027-alternative-page-order", $"p{n}"));
        }

        var authorIntended = new Structure(Id("0027-alternative-page-order", "range/r2"), new Label("Author-intended sequence")).AddBehavior(new Behavior("sequence"));
        foreach (var n in new[] { 2, 3, 4, 1 })
        {
            authorIntended.AddCanvasReference(CanvasId("0027-alternative-page-order", $"p{n}"));
        }

        return manifest.AddStructure(physical).AddStructure(authorIntended);
    }

    // ---- 0029-metadata-anywhere ------------------------------------------------------------------

    private static Manifest Recipe0029()
    {
        var manifest = NewManifest("0029-metadata-anywhere", "John Dee performing an experiment before Queen Elizabeth I.");
        manifest.AddMetadata(new Metadata("Creator", "Glindoni, Henry Gillard, 1852-1913"));
        manifest.AddMetadata(new Metadata("Date", "1800-1899"));
        manifest.AddMetadata(new Metadata("Physical Description", "1 painting : oil on canvas ; canvas 152 x 244.4 cm"));
        manifest.AddMetadata(new Metadata("Reference", "Wellcome Library no. 47369i"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description("Wellcome Collection. Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)").SetLanguage("en")));

        var canvas1 = NewCanvas("0029-metadata-anywhere", "p1", "Painting under natural light", 1271, 2000);
        canvas1.AddMetadata(new Metadata("Description", "A scene of John Dee performing an experiment before Queen Elizabeth I, under natural light."));
        canvas1.AddAnnotation(PaintingImage(canvas1, "0029-metadata-anywhere", "p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural/full/max/0/default.jpg", "image/jpeg", 1271, 2000,
            "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural"));

        var canvas2 = NewCanvas("0029-metadata-anywhere", "p2", "X-ray view of painting", 1271, 2000);
        canvas2.AddMetadata(new Metadata("Description", "An X-ray of the painting, showing pentimenti."));
        canvas2.AddAnnotation(PaintingImage(canvas2, "0029-metadata-anywhere", "p0002-image",
            "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray/full/max/0/default.jpg", "image/jpeg", 1271, 2000,
            "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray"));

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    // ---- 0030-multi-volume (Collection) ------------------------------------------------------------

    private static Collection Recipe0030()
    {
        var collection = new Collection(Id("0030-multi-volume", "collection.json"), new Label("青楼絵本年中行事 [Seirō ehon nenjū gyōji]", "jp"))
            .AddBehavior(new Behavior("multi-part"));
        collection.AddItem(NewVolumeManifest("manifest_v1.json", "Seirō ehon nenjū gyōji : kan 1 | 青楼絵本年中行事 : 巻 1",
            "5b0b39c2bf5591d21d807f9aadb437fa-uclaeal_wahon_A06_bib1974505_vol01", [(1, 4301, 5730), (2, 7451, 5702), (3, 7451, 5702), (7, 7451, 5702), (8, 7451, 5702)]));
        collection.AddItem(NewVolumeManifest("manifest_v2.json", "Seirō ehon nenjū gyōji : kan 2 | 青楼絵本年中行事 : 巻 2",
            "ecbc73b7cd459faf609e54eb4305da1f-uclaeal_wahon_A06_bib1974505_vol02", [(1, 4114, 5745), (2, 7253, 5745), (3, 7253, 5745), (4, 7253, 5745), (5, 7253, 5745)]));
        return collection;
    }

    private static Manifest NewVolumeManifest(string file, string label, string imageHashBase, (int page, int height, int width)[] pages)
    {
        var manifest = new Manifest(Id("0030-multi-volume", file), new Label(label))
            .AddBehavior(new Behavior("individuals"))
            .SetViewingDirection(ViewingDirection.Rtl);
        var titles = new[] { "Front cover", "Page spread 1", "Page spread 2", "Page spread 3", "Page spread 4" };
        for (var i = 0; i < pages.Length; i++)
        {
            var (page, height, width) = pages[i];
            var canvas = new Canvas(Id("0030-multi-volume", $"{file}/canvas/p{i + 1}"), new Label(titles[i]), height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/{imageHashBase}_00{page}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0030-multi-volume", $"{file}/annotation/p000{i + 1}-image", imageId, "image/jpeg", height, width,
                $"https://iiif.io/api/image/3.0/example/reference/{imageHashBase}_00{page}"));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    // ---- 0031-bound-multivolume ------------------------------------------------------------------

    private static Manifest Recipe0031()
    {
        var manifest = NewManifest("0031-bound-multivolume", new Label("Gottesdienstliche Ceremonien, Oder H. Kirchen-Gebräuche Und Religions-Pflichten Der Christen", "de"));
        var pages = new[] { ("1_frontcover", "Front cover"), ("2_insidefrontcover", "Inside front cover"), ("3_titlepage1", "Vol. 1 title page"), ("4_titlepage1_verso", "Vol. 1 title page (verso)"), ("5_titlepage2", "Vol. 2 title page"), ("6_titlepage2_verso", "Vol. 2 title page (verso)") };
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0031-bound-multivolume", $"p{n}", label, 7230, 5428);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/15f769d62ca9a3a2deca390efed75d73-{suffix}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0031-bound-multivolume", $"p000{n}-image", imageId, "image/jpeg", 7230, 5428,
                $"https://iiif.io/api/image/3.0/example/reference/15f769d62ca9a3a2deca390efed75d73-{suffix}"));
            manifest.AddItem(canvas);
        }

        var root = new Structure(Id("0031-bound-multivolume", "range/r0"), new Label("Gottesdienstliche Ceremonien", "de"));
        root.AddItem(new Structure(Id("0031-bound-multivolume", "range/r1"), new Label("Front Matter")).AddCanvasReference(CanvasId("0031-bound-multivolume", "p1")).AddCanvasReference(CanvasId("0031-bound-multivolume", "p2")));
        root.AddItem(new Structure(Id("0031-bound-multivolume", "range/r2"), new Label("Erste Ausgabe", "de")).AddCanvasReference(CanvasId("0031-bound-multivolume", "p3")).AddCanvasReference(CanvasId("0031-bound-multivolume", "p4")));
        root.AddItem(new Structure(Id("0031-bound-multivolume", "range/r3"), new Label("Zweyte Ausgabe", "de")).AddCanvasReference(CanvasId("0031-bound-multivolume", "p5")).AddCanvasReference(CanvasId("0031-bound-multivolume", "p6")));
        return manifest.AddStructure(root);
    }

    // ---- 0032-collection ------------------------------------------------------------------------

    private static Collection Recipe0032()
    {
        var collection = new Collection(Id("0032-collection", "collection.json"), new Label("Simple Collection Example"));

        var m1 = new Manifest(Id("0032-collection", "manifest-01.json"), new Label("The Gulf Stream"));
        m1.AddMetadata(new Metadata("Artist", "Winslow Homer (1836–1910)"));
        m1.AddMetadata(new Metadata("Date", "1899"));
        var c1 = new Canvas(Id("0032-collection", "manifest/1/canvas/p1"), new Label("p1"), 3540, 5886);
        c1.AddAnnotation(PaintingImage(c1, "0032-collection", "manifest/1/annotation/p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Winslow_Homer_-_The_Gulf_Stream_-_Metropolitan_Museum_of_Art/full/max/0/default.jpg", "image/jpeg", 3540, 5886,
            "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Winslow_Homer_-_The_Gulf_Stream_-_Metropolitan_Museum_of_Art"));
        m1.AddItem(c1);

        var m2 = new Manifest(Id("0032-collection", "manifest-02.json"), new Label("Northeaster"));
        m2.AddMetadata(new Metadata("Artist", "Winslow Homer (1836–1910)"));
        m2.AddMetadata(new Metadata("Date", "1895"));
        var c2 = new Canvas(Id("0032-collection", "manifest/2/canvas/p1"), new Label("p1"), 2572, 3764);
        c2.AddAnnotation(PaintingImage(c2, "0032-collection", "manifest/2/annotation/p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Northeaster_by_Winslow_Homer_1895/full/max/0/default.jpg", "image/jpeg", 2572, 3764,
            "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Northeaster_by_Winslow_Homer_1895"));
        m2.AddItem(c2);

        collection.AddItem(m1);
        collection.AddItem(m2);
        return collection;
    }

    // ---- 0033-choice ----------------------------------------------------------------------------

    private static Manifest Recipe0033()
    {
        var manifest = NewManifest("0033-choice", "John Dee performing an experiment before Queen Elizabeth I.");
        var canvas = NewCanvas("0033-choice", "p1", null, 1271, 2000);
        var natural = new ImageResource("https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(1271).SetWidth(2000).SetLabel(new Label("Natural Light"))
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural", "level1"));
        var xray = new ImageResource("https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(1271).SetWidth(2000).SetLabel(new Label("X-Ray"))
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray", "level1"));
        canvas.AddAnnotation(new Annotation(Id("0033-choice", "annotation/p0001-image"), new Choice([natural, xray]), canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0035-foldouts --------------------------------------------------------------------------

    private static Manifest Recipe0035()
    {
        var manifest = NewManifest("0035-foldouts", "Outlines of geology being the substance of a course of lectures delivered in the Theatre of the Royal Institution in the year 1816")
            .AddBehavior(new Behavior("paged"));
        var pages = new (string suffix, string label, int height, int width, string? behavior)[]
        {
            ("1_frontcover", "Front cover", 4429, 2533, null),
            ("2_insidefrontcover", "Inside front cover", 4315, 2490, null),
            ("3_foldout-folded", "Foldout, folded", 4278, 2197, null),
            ("4_foldout", "Foldout, unfolded", 1968, 3688, "non-paged"),
            ("3_foldout-rotated", "Foldout, folded (recto)", 4278, 2197, null),
            ("5_titlepage", "Title page", 4315, 2490, null),
            ("6_titlepage-recto", "Back of title page", 4315, 2490, null),
            ("8_insidebackcover", "Inside back cover", 4315, 2490, null),
            ("9_backcover", "Back cover", 4315, 2490, null)
        };

        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label, height, width, behavior) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0035-foldouts", $"{n}", label, height, width);
            if (behavior is not null)
            {
                canvas.AddBehavior(new Behavior(behavior));
            }

            var imageId = $"https://iiif.io/api/image/3.0/example/reference/0a469c27256eda739d43124cc448a3ba-{suffix}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0035-foldouts", $"000{n}-image", imageId, "image/jpeg", height, width,
                $"https://iiif.io/api/image/3.0/example/reference/0a469c27256eda739d43124cc448a3ba-{suffix}"));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    // ---- 0036-composition-from-multiple-images ------------------------------------------------------

    private static Manifest Recipe0036()
    {
        var manifest = NewManifest("0036-composition-from-multiple-images", "Folio from Grandes Chroniques de France, ca. 1460");
        var canvas = NewCanvas("0036-composition-from-multiple-images", "p1", null, 5412, 7216, labelOverride: "f. 033v-034r [Chilpéric Ier tue Galswinthe, se remarie et est assassiné]", labelLanguage: "none");
        canvas.AddAnnotation(PaintingImage(canvas, "0036-composition-from-multiple-images", "p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/899da506920824588764bc12b10fc800-bnf_chateauroux/full/max/0/default.jpg", "image/jpeg", 5412, 7216,
            "https://iiif.io/api/image/3.0/example/reference/899da506920824588764bc12b10fc800-bnf_chateauroux"));

        var detail = new ImageResource("https://iiif.io/api/image/3.0/example/reference/899da506920824588764bc12b10fc800-bnf_chateauroux_miniature/full/max/0/default.jpg", "image/jpeg")
            .SetLabel(new Label("Miniature [Chilpéric Ier tue Galswinthe, se remarie et est assassiné]", "fr"))
            .SetHeight(2414).SetWidth(2138)
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/899da506920824588764bc12b10fc800-bnf_chateauroux_miniature", "level1"));
        var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(3949, 994, 1091, 1232));
        canvas.AddAnnotation(new Annotation(Id("0036-composition-from-multiple-images", "annotation/p0002-image"), detail, target));
        return manifest.AddItem(canvas);
    }

    // ---- 0040-image-rotation-service --------------------------------------------------------------

    private static Manifest Recipe0040Service()
    {
        var manifest = NewManifest("0040-image-rotation-service", new Label("[Conoximent de las orines] Ihesus, Ihesus.", "ca"));
        var canvas = NewCanvas("0040-image-rotation-service", "p1", "inside cover; 1r", 1523, 2105);
        var image = new ImageResource("https://iiif.io/api/image/3.0/example/reference/85a96c630f077e6ac6cb984f1b752bbf-0-21198-zz00022840-1-page1/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(2105).SetWidth(1523)
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/85a96c630f077e6ac6cb984f1b752bbf-0-21198-zz00022840-1-page1", "level1"));
        var body = new SpecificResource(image).SetId(Id("0040-image-rotation-service", "body/v0001-image")).SetSelector(new ImageApiSelector().SetRotation("90"));
        canvas.AddAnnotation(new Annotation(Id("0040-image-rotation-service", "annotation/v0001-image"), body, canvas.Id));
        return manifest.AddItem(canvas);
    }

    private static Manifest Recipe0040Css()
    {
        var manifest = NewManifest("0040-image-rotation-service", new Label("[Conoximent de las orines] Ihesus, Ihesus.", "ca"));
        var canvas = NewCanvas("0040-image-rotation-service", "p1", "inside cover; 1r", 1523, 2105);
        var image = new ImageResource("https://iiif.io/api/image/3.0/example/reference/85a96c630f077e6ac6cb984f1b752bbf-0-21198-zz00022840-1-page1/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(2105).SetWidth(1523)
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/85a96c630f077e6ac6cb984f1b752bbf-0-21198-zz00022840-1-page1", "level1"));
        var body = new SpecificResource(image).SetId(Id("0040-image-rotation-service", "body/sr1")).SetStyleClass("rotated");
        var annotation = new Annotation(Id("0040-image-rotation-service", "annotation/v0001-image"), body, canvas.Id)
            .SetStylesheet(".rotated { transform-origin: 761px 1344px; transform: rotate(90deg) translateY(-582px); }");
        canvas.AddAnnotation(annotation);
        return manifest.AddItem(canvas);
    }

    // ---- 0045-css -------------------------------------------------------------------------------

    private static Manifest Recipe0045()
    {
        var manifest = NewManifest("0045-css", new Label("Koto, chess, calligraphy, and painting"), new Label("琴棋書画図屏風", "ja"));
        var canvas = NewCanvas("0045-css", "p1", null, 3966, 8800);
        canvas.AddAnnotation(PaintingImage(canvas, "0045-css", "p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004/full/max/0/default.jpg", "image/jpeg", 3966, 8800,
            "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004"));

        var text1 = new TextualBody("<p>Three of the four pursuits of refined and noble men named in the screen's title are shown on this side of the screen.</p>").SetLanguage("en").SetFormat("text/html");
        var body1 = new SpecificResource(text1).SetId(Id("0045-css", "body/sr1")).SetStyleClass("author1note");
        var target1 = new AnnotationTarget(canvas.Id).SetStyleClass("author2highlight").SetSelector(FragmentSelector.ForRegion(700, 1250, 1850, 1150));
        var anno1 = new Annotation(Id("0045-css", "page/p2/anno-1"), body1, target1).SetMotivation("commenting").SetStylesheet(Id("0045-css", "style.css"));

        var text2 = new TextualBody("<p>The detail in the natural beauty of the setting could be seen as a contrast to the manufactured pursuits of noble men.</p>").SetLanguage("en").SetFormat("text/html");
        var body2 = new SpecificResource(text2).SetId(Id("0045-css", "body/sr2")).SetStyleClass("author2note");
        var target2 = new AnnotationTarget(canvas.Id).SetStyleClass("author2highlight").SetSelector(FragmentSelector.ForRegion(170, 160, 2200, 1000));
        var anno2 = new Annotation(Id("0045-css", "page/p2/anno-2"), body2, target2).SetMotivation("commenting").SetStylesheet(Id("0045-css", "style.css"));

        var page = new AnnotationPage(Id("0045-css", "page/p2/1"));
        canvas.AddAnnotationPageReference(page);
        page.AddItem(anno1);
        page.AddItem(anno2);
        return manifest.AddItem(canvas);
    }

    // ---- 0046-rendering -------------------------------------------------------------------------

    private static Manifest Recipe0046()
    {
        var manifest = NewManifest("0046-rendering", "Alternative Representations Through Rendering").SetViewingDirection(ViewingDirection.Rtl);
        manifest.AddSummary(new Description("Playbill for a Kabuki performance at the Chikugo Theater, Osaka, 1849.").SetLanguage("en"));
        manifest.AddRendering(new Rendering("https://fixtures.iiif.io/other/UCLA/kabuki_ezukushi_rtl.pdf", "PDF version").SetFormat("application/pdf"));
        AddUclaPlaybillCanvases(manifest, "0046-rendering");
        return manifest;
    }

    // ---- 0047-homepage --------------------------------------------------------------------------

    private static Manifest Recipe0047()
    {
        var manifest = NewManifest("0047-homepage", new Label("Laocöon", "none"));
        manifest.AddHomepage(new Homepage("https://www.getty.edu/art/collection/object/103RQQ", "Home page at the Getty Museum Collection").SetFormat("text/html"));
        var canvas = NewCanvas("0047-homepage", "1", "Front", 3000, 2315, labelLanguage: "none");
        canvas.AddAnnotation(PaintingImage(canvas, "0047-homepage", "canvas/1/page/1/annotation/1",
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon/full/!500,500/0/default.jpg", "image/jpeg", 3000, 2315,
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon", idIsFull: true));
        return manifest.AddItem(canvas);
    }

    // ---- 0053-seeAlso ---------------------------------------------------------------------------

    private static Manifest Recipe0053()
    {
        var manifest = NewManifest("0053-seeAlso", "Linking to Structured Metadata").SetViewingDirection(ViewingDirection.Rtl);
        manifest.AddSummary(new Description("Playbill for a Kabuki performance at the Chikugo Theater, Osaka, 1849.").SetLanguage("en"));
        manifest.AddSeeAlso(new SeeAlso("https://fixtures.iiif.io/other/UCLA/ezukushi_mods.xml").SetType("Dataset").SetFormat("text/xml").SetProfile("http://www.loc.gov/mods/v3").SetLabel("MODS metadata"));
        AddUclaPlaybillCanvases(manifest, "0053-seeAlso");
        return manifest;
    }

    private static void AddUclaPlaybillCanvases(Manifest manifest, string recipe)
    {
        var pages = new[] { ("001", "front cover", 4823, 3497), ("002", "pages 1–2", 4804, 6062), ("003", "pages 3–4", 4776, 6127), ("004", "pages 5–6", 4751, 6124), ("005", "back cover", 4808, 3510) };
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label, height, width) = pages[i];
            var n = i + 1;
            var canvas = new Canvas(Id(recipe, $"p{n}"), new Label(label), height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_{suffix}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, recipe, $"p000{n}-image", imageId, "image/jpeg", height, width,
                $"https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_{suffix}"));
            manifest.AddItem(canvas);
        }
    }

    // ---- 0057-publishing-v2-and-v3 ----------------------------------------------------------------

    private static Manifest Recipe0057()
    {
        // The SDK itself demonstrates the 2.1-vs-3.0 duality via IiifSerializer's version-aware
        // Serialize(manifest, options) - this single 3.0-native Manifest can be written as either.
        var manifest = new Manifest("https://iiif.io/api/cookbook/recipe/0057-publishing-v2-and-v3/manifest.json",
            new Label("IIIF Presentation Version 3 Minimum Viable Manifest"));
        var canvas = new Canvas(Id("0057-publishing-v2-and-v3", "canvas/p1"), new Label("Untitled"), 1800, 1200);
        canvas.AddAnnotation(new Annotation(Id("0057-publishing-v2-and-v3", "annotation/p0001-image"),
            new ImageResource("https://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png", "image/png").SetHeight(1800).SetWidth(1200),
            canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0064-opera-one-canvas / 0065-opera-multiple-canvases ------------------------------------

    private static Manifest Recipe0064()
    {
        var manifest = NewManifest("0064-opera-one-canvas", new Label("The Elixir of Love"), new Label("L'Elisir D'Amore", "it"));
        manifest.AddMetadata(new Metadata("Date Issued", "2019"));
        manifest.AddMetadata(new Metadata("Publisher", "Indiana University Jacobs School of Music"));

        var canvas = NewCanvas("0064-opera-one-canvas", "1", null, 1080, 1920).SetDuration(7278.422);
        canvas.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/video/indiana/donizetti-elixir/act1-thumbnail.png"));
        canvas.AddAnnotation(new Annotation(Id("0064-opera-one-canvas", "annotation/1"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_1.mp4", "video/mp4").SetHeight(1080).SetWidth(1920).SetDuration(3971.24),
            new AnnotationTarget($"{canvas.Id}#t=0,3971.24")));
        canvas.AddAnnotation(new Annotation(Id("0064-opera-one-canvas", "annotation/2"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_2.mp4", "video/mp4").SetHeight(1080).SetWidth(1920).SetDuration(3307.22),
            new AnnotationTarget($"{canvas.Id}#t=3971.24")));
        manifest.AddItem(canvas);

        var root = new Structure(Id("0064-opera-one-canvas", "range/1"), new Label("Gaetano Donizetti, L'Elisir D'Amore", "it"));
        var atto1 = new Structure(Id("0064-opera-one-canvas", "range/2"), new Label("Atto Primo", "it"));
        atto1.AddItem(new Structure(Id("0064-opera-one-canvas", "range/3"), new Label("Preludio e Coro d'introduzione", "it")).AddCanvasReference($"{canvas.Id}#t=0,302.05"));
        atto1.AddItem(new Structure(Id("0064-opera-one-canvas", "range/4"), new Label("Remainder of Atto Primo")).AddCanvasReference($"{canvas.Id}#t=302.05,3971.24"));
        root.AddItem(atto1);
        root.AddItem(new Structure(Id("0064-opera-one-canvas", "range/5"), new Label("Atto Secondo", "it")).AddCanvasReference($"{canvas.Id}#t=3971.24,7278.422"));
        return manifest.AddStructure(root);
    }

    private static Manifest Recipe0065()
    {
        var manifest = NewManifest("0065-opera-multiple-canvases", new Label("The Elixir of Love"), new Label("L'Elisir D'Amore", "it"));
        manifest.AddMetadata(new Metadata("Date Issued", "2019"));
        manifest.AddMetadata(new Metadata("Publisher", "Indiana University Jacobs School of Music"));

        var canvas1 = NewCanvas("0065-opera-multiple-canvases", "1", "Atto Primo", 1080, 1920).SetDuration(3971.24);
        canvas1.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/video/indiana/donizetti-elixir/act1-thumbnail.png"));
        canvas1.AddAnnotation(new Annotation(Id("0065-opera-multiple-canvases", "annotation/1"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_1.mp4", "video/mp4").SetHeight(1080).SetWidth(1920).SetDuration(3971.24), canvas1.Id));

        var canvas2 = NewCanvas("0065-opera-multiple-canvases", "2", "Atto Secondo", 1080, 1920).SetDuration(3307.22);
        canvas2.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/video/indiana/donizetti-elixir/act2-thumbnail.png"));
        canvas2.AddAnnotation(new Annotation(Id("0065-opera-multiple-canvases", "annotation/1"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_2.mp4", "video/mp4").SetHeight(1080).SetWidth(1920).SetDuration(3307.22), canvas2.Id));

        manifest.AddItem(canvas1).AddItem(canvas2);

        var root = new Structure(Id("0065-opera-multiple-canvases", "range/1"), new Label("Gaetano Donizetti, L'Elisir D'Amore", "it"));
        var atto1 = new Structure(Id("0065-opera-multiple-canvases", "range/2"), new Label("Atto Primo"));
        atto1.AddItem(new Structure(Id("0065-opera-multiple-canvases", "range/3"), new Label("Preludio e Coro d'introduzione", "it")).AddCanvasReference($"{canvas1.Id}#t=0,302.05"));
        atto1.AddItem(new Structure(Id("0065-opera-multiple-canvases", "range/4"), new Label("Remainder of Atto Primo")).AddCanvasReference($"{canvas1.Id}#t=302.05,3971.24"));
        root.AddItem(atto1);
        root.AddItem(new Structure(Id("0065-opera-multiple-canvases", "range/5"), new Label("Atto Secondo")).AddCanvasReference($"{canvas2.Id}#t=0,3307.22"));
        return manifest.AddStructure(root);
    }

    // ---- 0068-newspaper (Collection) --------------------------------------------------------------

    private static Collection Recipe0068()
    {
        var collection = new Collection(Id("0068-newspaper", "newspaper_title-collection.json"), new Label("Berliner Tageblatt", "de"));
        collection.AddMetadata(new Metadata("type", "Newspaper Title").AddValue("Serial"));
        collection.AddMetadata(new Metadata("language", "German"));
        collection.SetRights(new Rights("http://creativecommons.org/publicdomain/mark/1.0/"));
        collection.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description("<a href='https://www.europeana.eu/portal/record/9200355/BibliographicResource_3000096302605.html'>Berliner Tageblatt</a> - Staatsbibliothek zu Berlin.").SetLanguage("en")));
        collection.AddProvider(new Provider("https://www.europeana.eu/", new Label("Europeana"))
            .AddLogo(new Logo("https://style.europeana.eu/images/europeana-logo-default.png").SetFormat("image/png").SetHeight(310).SetWidth(100)));
        collection.AddSeeAlso(new SeeAlso("https://www.europeana.eu/api/v2/record/9200355/BibliographicResource_3000096302605.json-ld").SetType("Dataset").SetFormat("application/ld+json"));

        collection.AddItem(NewNewspaperIssue("newspaper_issue_1-manifest.json", "Berliner Tageblatt - 1925-02-16", "1925-02-16", "newspaper-p"));
        collection.AddItem(NewNewspaperIssue("newspaper_issue_2-manifest.json", "Berliner Tageblatt - 1925-03-13", "1925-03-13", "newspaper-issue2-p"));
        return collection;
    }

    private static Manifest NewNewspaperIssue(string file, string label, string navDate, string imageSuffixBase)
    {
        var manifest = new Manifest(Id("0068-newspaper", file), new Label(label, "de"))
            .SetNavDate(DateTime.Parse($"{navDate}T00:00:00Z").ToUniversalTime());
        manifest.AddPartOf(new PartOf(Id("0068-newspaper", "newspaper_title-collection.json"), "Collection"));
        manifest.AddMetadata(new Metadata("type", "Newspaper Issue").AddValue("Analytic serial"));
        manifest.SetRights(new Rights("http://creativecommons.org/publicdomain/mark/1.0/"));

        for (var n = 1; n <= 2; n++)
        {
            var canvas = new Canvas(Id("0068-newspaper", $"canvas/p{n}"), new Label($"p. {n}"), 5000, 3602);
            canvas.AddRendering(new Rendering(Id("0068-newspaper", $"{file}-alto_p{n}.xml"), "ALTO XML").SetFormat("application/xml"));
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-{imageSuffixBase}{n}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0068-newspaper", $"annotation_page_painting/ap{n}/{file}-p{n}", imageId, "image/jpeg", null, null,
                $"https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-{imageSuffixBase}{n}", idIsFull: true));

            var ocrPage = new AnnotationPage(Id("0068-newspaper", $"{file}-anno_p{n}.json"));
            var ocrTarget = new AnnotationTarget(canvas.Id, "Canvas").SetPartOf(Id("0068-newspaper", file), "Manifest").SetSelector(FragmentSelector.ForRegion(0, 376, 399, 53));
            ocrPage.AddItem(new Annotation(Id("0068-newspaper", $"{file}-anno_p{n}.json-1"),
                new TextualBody("I. 54. Jahrgang").SetLanguage("de").SetFormat("text/plain"), ocrTarget).SetMotivation("supplementing"));
            canvas.AddAnnotationPageReference(ocrPage);
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    // ---- 0074-multiple-language-captions -----------------------------------------------------------

    private static Manifest Recipe0074()
    {
        var manifest = NewManifest("0074-multiple-language-captions", new Label("For ladies. French models", "en"), new Label("Per voi signore. Modelli francesi", "it"));
        manifest.SetRights(new Rights("http://rightsstatements.org/vocab/InC/1.0/"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Rights"), new Description("All rights reserved Cinecittà Luce spa").SetLanguage("en")));

        var canvas = NewCanvas("0074-multiple-language-captions", "canvas", null, 384, 288).SetDuration(65.0);
        canvas.AddAnnotation(new Annotation(Id("0074-multiple-language-captions", "canvas/page/annotation"),
            new VideoResource("https://fixtures.iiif.io/video/europeana/Per_voi_signore_Modelli_francesi.mp4", "video/mp4").SetHeight(384).SetWidth(288).SetDuration(65.0),
            canvas.Id));

        var en = new TextualBody(Id("0074-multiple-language-captions", "Per_voi_signore_Modelli_francesi_en.vtt")).SetFormat("text/vtt").SetLanguage("en");
        var it = new TextualBody(Id("0074-multiple-language-captions", "Per_voi_signore_Modelli_francesi_it.vtt")).SetFormat("text/vtt").SetLanguage("it");
        var captions = new Annotation(Id("0074-multiple-language-captions", "subtitles_captions-files-vtt"), new Choice([en, it]), canvas.Id).SetMotivation("supplementing");
        var page = new AnnotationPage(Id("0074-multiple-language-captions", "anno/page/1"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, captions);
    }

    // ---- 0103-poetry-reading-annotations -----------------------------------------------------------

    private static Manifest Recipe0103()
    {
        var manifest = NewManifest("0103-poetry-reading-annotations", "Mediation and Lived Experience - Daphne Marlatt Performs With Her Younger Self");
        var canvas = NewCanvas("0103-poetry-reading-annotations", "1", null, null, null).SetDuration(707.81);
        canvas.AddAnnotation(new Annotation(Id("0103-poetry-reading-annotations", "canvas/1/painting/1"),
            new AudioResource("https://fixtures.iiif.io/audio/ubc/Performing-the-Archive-Leaf-leafs-by-Daphne-Marlatt.mp3", "audio/mpeg").SetDuration(707.81),
            canvas.Id));

        var target = new AnnotationTarget($"{canvas.Id}#t=702.0,705.0");
        var comment = new Annotation(Id("0103-poetry-reading-annotations", "canvas/1/page1/anno2"),
            new TextualBody("Soft laughter, rustling").SetFormat("text/plain"), target).SetMotivation("commenting");
        var page = new AnnotationPage(Id("0103-poetry-reading-annotations", "canvas/1/page1")).AddLabel(new Label("Environmental Noise"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, comment);
    }

    // ---- 0117-add-image-thumbnail -----------------------------------------------------------------

    private static Manifest Recipe0117()
    {
        var manifest = NewManifest("0117-add-image-thumbnail", "Playbill Cover with Manifest Thumbnail");
        manifest.AddSummary(new Description("Cover of playbill for a Kabuki performance at the Chikugo Theater, Osaka, 1849.").SetLanguage("en"));
        manifest.SetThumbnail(new Thumbnail("https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001/full/max/0/default.jpg")
            .SetFormat("image/jpeg").SetHeight(4823).SetWidth(3497)
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001", "level1")
                .SetHeight(4823).SetWidth(3497)));

        var canvas = NewCanvas("0117-add-image-thumbnail", "p0", "front cover with color bar", 5312, 4520);
        canvas.AddAnnotation(PaintingImage(canvas, "0117-add-image-thumbnail", "p0000-image",
            "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001_full/full/max/0/default.jpg", "image/jpeg", 5312, 4520,
            "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001_full"));
        return manifest.AddItem(canvas);
    }

    // ---- 0118-multivalue -------------------------------------------------------------------------

    private static Manifest Recipe0118()
    {
        var manifest = NewManifest("0118-multivalue", new Label("Arrangement en gris et noir no 1", "fr"));
        manifest.AddMetadata(new Metadata("Alternative titles", "Whistler's Mother", "en").AddValue("Arrangement in Grey and Black No. 1", "en")
            .AddValue("Portrait de la mère de l'artiste", "fr").AddValue("La Mère de Whistler", "fr"));
        manifest.AddSummary(new Description("A painting in oil on canvas created by the American-born painter James McNeill Whistler, in 1871.").SetLanguage("en"));

        var canvas = NewCanvas("0118-multivalue", "1", null, 991, 1114);
        canvas.AddAnnotation(new Annotation(Id("0118-multivalue", "canvas/1/page/1/annotation/1"),
            new ImageResource("https://upload.wikimedia.org/wikipedia/commons/thumb/1/1b/Whistlers_Mother_high_res.jpg/1114px-Whistlers_Mother_high_res.jpg", "image/jpeg"),
            canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0135-annotating-point-in-canvas -----------------------------------------------------------

    private static Manifest Recipe0135()
    {
        var manifest = NewManifest("0135-annotating-point-in-canvas", "Using a point selector for annotating a location on a map.");
        manifest.AddSummary(new Description("A map containing a point with an annotation of the location.").SetLanguage("en"));

        var canvas = NewCanvas("0135-annotating-point-in-canvas", "canvas.json", "Chesapeake and Ohio Canal Pamphlet", 7072, 5212, idIsFull: true);
        canvas.AddAnnotation(PaintingImage(canvas, "0135-annotating-point-in-canvas", "content.json",
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674/full/max/0/default.jpg", "image/jpeg", 7072, 5212,
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", idIsFull: true));

        var target = new AnnotationTarget(canvas.Id).SetSelector(PointSelector.ForSpatialPoint(3385, 1464));
        var tag = new Annotation(Id("0135-annotating-point-in-canvas", "annotation/p0002-tag"),
            new TextualBody("Town Creek Aqueduct").SetLanguage("en").SetFormat("text/plain"), target).SetMotivation("tagging");
        var page = new AnnotationPage(Id("0135-annotating-point-in-canvas", "page/p2/1"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, tag);
    }

    // ---- 0139-geolocate-canvas-fragment -------------------------------------------------------------

    private static Manifest Recipe0139()
    {
        var manifest = NewManifest("0139-geolocate-canvas-fragment", "Recipe Manifest for #139");
        manifest.AddSummary(new Description("A IIIF Presentation API 3.0 Manifest containing a GeoJSON-LD Web Annotation which targets a Canvas fragment.").SetLanguage("en"));

        var canvas = NewCanvas("0139-geolocate-canvas-fragment", "canvas.json", "Chesapeake and Ohio Canal Pamphlet", 7072, 5212, idIsFull: true);
        canvas.AddAnnotation(PaintingImage(canvas, "0139-geolocate-canvas-fragment", "content.json",
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674/full/max/0/default.jpg", "image/jpeg", 7072, 5212,
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", idIsFull: true));

        var feature = new Feature(Id("0139-geolocate-canvas-fragment", "geo.json"))
            .SetGeometry(new Geometry(GeometryType.Polygon)
                .AddCoordinate(new CoordinateItem(-77.019853, 38.913101))
                .AddCoordinate(new CoordinateItem(-77.110013, 38.843254))
                .AddCoordinate(new CoordinateItem(-77.284698, 38.997574))
                .AddCoordinate(new CoordinateItem(-77.188911, 39.062648)))
            .SetProperties(new FeatureProperties().AddLabel(new Label("Targeted Map from Chesapeake and Ohio Canal Pamphlet")));
        var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(920, 3600, 1510, 3000));
        var geoAnno = new Annotation(Id("0139-geolocate-canvas-fragment", "geoAnno.json"), feature, target).SetMotivation("tagging");
        var page = new AnnotationPage(Id("0139-geolocate-canvas-fragment", "supplementingPage.json"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, geoAnno);
    }

    // ---- 0154-geo-extension ---------------------------------------------------------------------

    private static Manifest Recipe0154()
    {
        var manifest = NewManifest("0154-geo-extension", new Label("Bronzo Laocoonte e i suoi figli", "it"));
        manifest.SetNavPlace(new NavPlace(Id("0154-geo-extension", "feature-collection/1"))
            .AddFeature(new Feature(Id("0154-geo-extension", "feature/1"))
                .SetProperties(new FeatureProperties().AddLabel(new Label("The Laocoön Bronze")).AddLabel(new Label("Bronzo Laocoonte e i suoi figli", "it")))
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-118.4745559, 34.0776376)))));

        var canvas = NewCanvas("0154-geo-extension", "1", "Front of Bronze", 3000, 2315);
        canvas.AddAnnotation(PaintingImage(canvas, "0154-geo-extension", "anno/1",
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon/full/max/0/default.jpg", "image/jpeg", 3000, 2315,
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon"));
        return manifest.AddItem(canvas);
    }

    // ---- 0202-start-canvas ----------------------------------------------------------------------

    private static Manifest Recipe0202()
    {
        var manifest = NewManifest("0202-start-canvas", "Multiple Related Images (Book, etc.)");
        var pages = new[] { ("f18", "Blank page", 4613, 3204), ("f19", "Frontispiece", 4612, 3186), ("f20", "Title page", 4613, 3204), ("f21", "Blank page", 4578, 3174), ("f22", "Bookplate", 4632, 3198) };
        Canvas? startCanvas = null;
        for (var i = 0; i < pages.Length; i++)
        {
            var (suffix, label, height, width) = pages[i];
            var n = i + 1;
            var canvas = NewCanvas("0202-start-canvas", $"p{n}", label, height, width);
            var imageId = $"https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e9f3c1e674f-gallica_ark_12148_bpt6k1526005v_{suffix}/full/max/0/default.jpg";
            canvas.AddAnnotation(PaintingImage(canvas, "0202-start-canvas", $"p000{n}-image", imageId, "image/jpeg", height, width,
                $"https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e9f3c1e674f-gallica_ark_12148_bpt6k1526005v_{suffix}"));
            manifest.AddItem(canvas);
            if (n == 2)
            {
                startCanvas = canvas;
            }
        }

        manifest.SetStart(new AnnotationTarget(startCanvas!.Id, "Canvas"));
        return manifest;
    }

    // ---- 0219-using-caption-file ------------------------------------------------------------------

    private static Manifest Recipe0219()
    {
        var manifest = NewManifest("0219-using-caption-file", "Lunchroom Manners");
        var canvas = NewCanvas("0219-using-caption-file", "canvas", null, 360, 480).SetDuration(572.034);
        canvas.AddAnnotation(new Annotation(Id("0219-using-caption-file", "canvas/page/annotation1"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/lunchroom_manners/high/lunchroom_manners_1024kb.mp4", "video/mp4").SetHeight(360).SetWidth(480).SetDuration(572.034),
            canvas.Id));

        var captions = new Annotation(Id("0219-using-caption-file", "canvas/page2/a1"),
            new TextualBody(Id("0219-using-caption-file", "lunchroom_manners.vtt")).SetFormat("text/vtt").SetLanguage("en"), canvas.Id).SetMotivation("supplementing");
        var page = new AnnotationPage(Id("0219-using-caption-file", "canvas/page2"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, captions);
    }

    // ---- 0229-behavior-ranges -------------------------------------------------------------------

    private static Manifest Recipe0229()
    {
        var manifest = NewManifest("0229-behavior-ranges", "Video navigation with thumbnails in a Range");
        var canvas = NewCanvas("0229-behavior-ranges", "1", null, 1080, 1920).SetDuration(3307.22);
        canvas.AddAnnotation(new Annotation(Id("0229-behavior-ranges", "canvas/1/annotation_page/1/annotation/2"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_2.mp4", "video/mp4").SetHeight(1080).SetWidth(1920).SetDuration(3307.22),
            canvas.Id));
        manifest.AddItem(canvas);

        var root = new Structure(Id("0229-behavior-ranges", "range/1"), new Label("Thumbnail Navigation")).AddBehavior(new Behavior("thumbnail-nav"));
        root.AddItem(new Structure(Id("0229-behavior-ranges", "range/1.1")).AddBehavior(new Behavior("no-nav")).AddCanvasReference($"{canvas.Id}#t=0,9"));

        var segments = new[] { (9, 305, "01"), (305, 610, "02"), (610, 915, "03"), (915, 1220, "04"), (1220, 1525, "05"), (1525, 1830, "06"), (1830, 2135, "07"), (2135, 2440, "08"), (2440, 2745, "09") };
        foreach (var (start, end, thumbSuffix) in segments)
        {
            var range = new Structure(Id("0229-behavior-ranges", $"range/{thumbSuffix.TrimStart('0')}"), new Label($"{start}s – {end}s"));
            range.AddCanvasReference($"{canvas.Id}#t={start},{end}");
            range.SetThumbnail(new Thumbnail($"https://fixtures.iiif.io/video/indiana/donizetti-elixir/thumbnails/thumb-nav-{thumbSuffix}.png").SetFormat("image/png").SetHeight(1266).SetWidth(2250));
            root.AddItem(range);
        }

        var last = new Structure(Id("0229-behavior-ranges", "range/11"), new Label("2745s – end"));
        last.AddCanvasReference($"{canvas.Id}#t=2745,3307.22");
        last.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/video/indiana/donizetti-elixir/thumbnails/thumb-nav-10.png").SetFormat("image/png").SetHeight(1266).SetWidth(2250));
        root.AddItem(last);

        return manifest.AddStructure(root);
    }

    // ---- 0230-navdate (Collection) ----------------------------------------------------------------

    private static Collection Recipe0230()
    {
        var collection = new Collection(Id("0230-navdate", "navdate-collection.json"), new Label("Chesapeake and Ohio Canal map and guide pamphlets"));
        collection.SetThumbnail(new Thumbnail("https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674/full/max/0/default.jpg")
            .SetFormat("image/jpeg").SetHeight(300).SetWidth(221)
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", "level1")));

        var m1986 = new Manifest(Id("0230-navdate", "navdate_map_2-manifest.json"), new Label("1986 Chesapeake and Ohio Canal, Washington, D.C., Maryland, West Virginia, official map and guide"))
            .SetNavDate(DateTime.Parse("1986-01-01T00:00:00Z").ToUniversalTime());
        var c1986 = new Canvas(Id("0230-navdate", "navdate_map_2-manifest.json/canvas/p1"), new Label("1986 Map, recto and verso, with a date of publication"), 1765, 1286);
        c1986.AddAnnotation(PaintingImage(c1986, "0230-navdate", "navdate_map_2-manifest.json/annotation/p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-87691274-1986/full/max/0/default.jpg", "image/jpeg", 1765, 1286,
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-87691274-1986", idIsFull: true));
        m1986.AddItem(c1986);

        var m1987 = new Manifest(Id("0230-navdate", "navdate_map_1-manifest.json"), new Label("1987 Chesapeake and Ohio Canal, Washington, D.C., Maryland, West Virginia, official map and guide"))
            .SetNavDate(DateTime.Parse("1987-01-01T00:00:00Z").ToUniversalTime());
        var c1987 = new Canvas(Id("0230-navdate", "navdate_map_1-manifest.json/canvas/p1"), new Label("1987 Map, recto and verso, with a date of publication"), 7072, 5212);
        c1987.AddAnnotation(PaintingImage(c1987, "0230-navdate", "navdate_map_1-manifest.json/annotation/p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674/full/max/0/default.jpg", "image/jpeg", 7072, 5212,
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", idIsFull: true));
        m1987.AddItem(c1987);

        collection.AddItem(m1986);
        collection.AddItem(m1987);
        return collection;
    }

    // ---- 0232-image-thumbnail-canvas -------------------------------------------------------------

    private static Manifest Recipe0232Av()
    {
        var manifest = NewManifest("0232-image-thumbnail-canvas", "Video recording of Donizetti's _The Elixir of Love_", "manifest-av.json");
        var canvas1 = NewCanvas("0232-image-thumbnail-canvas", "donizetti/1", "The Elixir of Love, Act 1", 360, 640).SetDuration(3971.243);
        canvas1.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/video/indiana/donizetti-elixir/act1-thumbnail.png").SetFormat("image/png").SetHeight(360).SetWidth(640));
        canvas1.AddAnnotation(new Annotation(Id("0232-image-thumbnail-canvas", "donizetti/1/1-video"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_1.mp4", "video/mp4").SetHeight(360).SetWidth(640).SetDuration(3971.243), canvas1.Id));

        var canvas2 = NewCanvas("0232-image-thumbnail-canvas", "donizetti/2", "The Elixir of Love, Act 2", 360, 640).SetDuration(3307.224);
        canvas2.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/video/indiana/donizetti-elixir/act2-thumbnail.png").SetFormat("image/png").SetHeight(360).SetWidth(640));
        canvas2.AddAnnotation(new Annotation(Id("0232-image-thumbnail-canvas", "donizetti/2/1-video"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/donizetti-elixir/vae0637_accessH264_low_act_2.mp4", "video/mp4").SetHeight(360).SetWidth(640).SetDuration(3307.224), canvas2.Id));

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    private static Manifest Recipe0232Image()
    {
        var manifest = NewManifest("0232-image-thumbnail-canvas", "Gänseliesel-Brunnen, Göttingen", "manifest-image.json");
        var canvas1 = NewCanvas("0232-image-thumbnail-canvas", "p1", "Photo of the Gänseliesel-Brunnen taken at the 2019 IIIF Conference", 3024, 4032);
        canvas1.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/other/level0/Glen/photos/gottingen/full/max/0/default.jpg").SetFormat("image/jpeg").SetHeight(189).SetWidth(252)
            .AddService(new Service(ImageService3, "https://fixtures.iiif.io/other/level0/Glen/photos/gottingen", "level0")));
        canvas1.AddAnnotation(PaintingImage(canvas1, "0232-image-thumbnail-canvas", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));

        var canvas2 = NewCanvas("0232-image-thumbnail-canvas", "p2", "Gänseliesel-Brunnen at Night", 4032, 3024);
        canvas2.SetThumbnail(new Thumbnail("https://fixtures.iiif.io/other/level0/Glen/photos/fountain/full/max/0/default.jpg").SetFormat("image/jpeg").SetHeight(252).SetWidth(189)
            .AddService(new Service(ImageService3, "https://fixtures.iiif.io/other/level0/Glen/photos/fountain", "level0")));
        var fountainImage = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-fountain/full/max/0/default.jpg";
        canvas2.AddAnnotation(PaintingImage(canvas2, "0232-image-thumbnail-canvas", "p0002-image", fountainImage, "image/jpeg", 4032, 3024,
            "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-fountain"));

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    // ---- 0234-provider --------------------------------------------------------------------------

    private static Manifest Recipe0234()
    {
        var manifest = NewManifest("0234-provider", "Playbill Cover");
        manifest.AddSummary(new Description("Cover of a Kabuki playbill, Chikugo Theater, Osaka, 1849.").SetLanguage("en"));
        var provider = new Provider("https://id.loc.gov/authorities/n79055331", new Label("UCLA Library"))
            .AddHomepage(new Homepage("https://digital.library.ucla.edu/", "UCLA Library Digital Collections").SetFormat("text/html"))
            .AddLogo(new Logo("https://iiif.library.ucla.edu/iiif/2/UCLA-Library-Logo-double-line-2/full/max/0/default.png")
                .AddService(new Service(ImageService3, "https://iiif.library.ucla.edu/iiif/2/UCLA-Library-Logo-double-line-2", "level2").SetHeight(502).SetWidth(1200)))
            .AddSeeAlso(new SeeAlso("https://id.loc.gov/authorities/names/n79055331.madsxml.xml").SetType("Dataset")
                .SetLabel("US Library of Congress data about the UCLA Library").SetFormat("application/xml").SetProfile("http://www.loc.gov/mads/v2"));
        manifest.AddProvider(provider);

        var canvas = NewCanvas("0234-provider", "p0", "front cover with color bar", 5312, 4520);
        canvas.AddAnnotation(PaintingImage(canvas, "0234-provider", "p0000-image",
            "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001_full/full/max/0/default.jpg", "image/jpeg", 5312, 4520,
            "https://iiif.io/api/image/3.0/example/reference/4f92cceb12dd53b52433425ce44308c7-ucla_bib1987273_no001_rs_001_full"));
        return manifest.AddItem(canvas);
    }

    // ---- 0240-navPlace-on-canvases ----------------------------------------------------------------

    private static Manifest Recipe0240()
    {
        var manifest = NewManifest("0240-navPlace-on-canvases", "Laocöon, geolocated sculpture and painting.");

        var canvas1 = NewCanvas("0240-navPlace-on-canvases", "1", "Front of Bronze", 3000, 2315);
        canvas1.SetNavPlace(new NavPlace(Id("0240-navPlace-on-canvases", "feature-collection/1"))
            .AddFeature(new Feature(Id("0240-navPlace-on-canvases", "feature/1"))
                .SetProperties(new FeatureProperties().AddLabel(new Label("Current Location of the Laocoön Bronze")).AddLabel(new Label("Ubicazione attuale del Bronzo Laocoonte e i suoi figli", "it")))
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-118.4745559, 34.0776376)))));
        canvas1.AddAnnotation(PaintingImage(canvas1, "0240-navPlace-on-canvases", "anno/1",
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon/full/max/0/default.jpg", "image/jpeg", 3000, 2315,
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon"));

        var canvas2 = NewCanvas("0240-navPlace-on-canvases", "2", "Painting", 3259, 4096);
        canvas2.SetNavPlace(new NavPlace(Id("0240-navPlace-on-canvases", "feature-collection/2"))
            .AddFeature(new Feature(Id("0240-navPlace-on-canvases", "feature/2"))
                .SetProperties(new FeatureProperties().AddLabel(new Label("Current Location of Painting")))
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-77.0199025, 38.8920717)))));
        canvas2.AddAnnotation(PaintingImage(canvas2, "0240-navPlace-on-canvases", "anno/2",
            "https://iiif.io/api/image/3.0/example/reference/58763298b61c2a99f78ff94d8364c639-laocoon_1946_18_1/full/max/0/default.jpg", "image/jpeg", 3259, 4096,
            "https://iiif.io/api/image/3.0/example/reference/58763298b61c2a99f78ff94d8364c639-laocoon_1946_18_1"));

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    // ---- 0258-tagging-external-resource -------------------------------------------------------------

    private static Manifest Recipe0258()
    {
        var manifest = NewManifest("0258-tagging-external-resource", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0258-tagging-external-resource", "p1", null, 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0258-tagging-external-resource", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));

        var wikidata = new SpecificResource(new BaseResource("http://www.wikidata.org/entity/Q18624915", "Dataset"));
        var target = new AnnotationTarget($"{canvas.Id}#xywh=749,1054,338,460");
        var tag = new Annotation(Id("0258-tagging-external-resource", "annotation/anno/p0002-wikidata"), wikidata, target)
            .AddBody(new TextualBody("Gänseliesel-Brunnen").SetFormat("text/plain").SetLanguage("de"))
            .SetMotivation("tagging");
        var page = new AnnotationPage(Id("0258-tagging-external-resource", "page/p2/1"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, tag);
    }

    // ---- 0261-non-rectangular-commenting ------------------------------------------------------------

    private static Manifest Recipe0261()
    {
        var manifest = NewManifest("0261-non-rectangular-commenting", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0261-non-rectangular-commenting", "p1", null, 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0261-non-rectangular-commenting", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));

        const string svg = "<svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink'><g><path d='M270,1900 L1530,1900 L1530,1610 L1315,1300 L1200,986 L904,661 L600,986 L500,1300 L270,1630 L270,1900' /></g></svg>";
        var target = new AnnotationTarget(canvas.Id).SetSelector(new SvgSelector(svg));
        var tag = new Annotation(Id("0261-non-rectangular-commenting", "annotation/p0002-svg"),
            new TextualBody("Gänseliesel-Brunnen").SetLanguage("de").SetFormat("text/plain"), target).SetMotivation("tagging");
        var page = new AnnotationPage(Id("0261-non-rectangular-commenting", "page/p2/1"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, tag);
    }

    // ---- 0266-full-canvas-annotation ---------------------------------------------------------------

    private static Manifest Recipe0266()
    {
        var manifest = NewManifest("0266-full-canvas-annotation", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0266-full-canvas-annotation", "canvas-1", null, 3024, 4032, "canvas-1");
        canvas.AddAnnotation(PaintingImage(canvas, "0266-full-canvas-annotation", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, idIsFull: true));

        var comment = new Annotation(Id("0266-full-canvas-annotation", "canvas-1/annopage-2/anno-1"),
            new TextualBody("Göttinger Marktplatz mit Gänseliesel Brunnen").SetLanguage("de").SetFormat("text/plain"), canvas.Id).SetMotivation("commenting");
        var page = new AnnotationPage(Id("0266-full-canvas-annotation", "canvas-1/annopage-2"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, comment);
    }

    // ---- 0269-embedded-or-referenced-annotations ------------------------------------------------------

    private static Manifest Recipe0269()
    {
        var manifest = NewManifest("0269-embedded-or-referenced-annotations", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0269-embedded-or-referenced-annotations", "canvas-1", null, 3024, 4032, "canvas-1");
        canvas.AddAnnotation(PaintingImage(canvas, "0269-embedded-or-referenced-annotations", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, idIsFull: true));

        // Referenced (external) form: only a stub {id,type} on the canvas, unlike 0266's embedded form.
        canvas.AddAnnotationPageReference(new AnnotationPage(Id("0269-embedded-or-referenced-annotations", "annotationpage.json")));
        return manifest.AddItem(canvas);
    }

    // ---- 0283-missing-image ----------------------------------------------------------------------

    private static Manifest Recipe0283()
    {
        var manifest = NewManifest("0283-missing-image", "Ethiopic Ms 10");
        var pages = new[] { ("1", "f. 1r", 2504, 1768, true), ("2", "f. 1v — MISSING", 2504, 1768, false), ("3", "f. 2r", 2456, 1792, true), ("4", "f. 2v", 2440, 1760, true) };
        foreach (var (n, label, height, width, hasImage) in pages)
        {
            var canvas = NewCanvas("0283-missing-image", $"p{n}", label, height, width);
            if (hasImage)
            {
                var imageId = $"https://iiif.io/api/image/3.0/example/reference/d3bbf5397c6df6b894c5991195c912ab-{n}-21198-zz001d8m41_774608_master/full/max/0/default.jpg";
                canvas.AddAnnotation(PaintingImage(canvas, "0283-missing-image", $"p000{n}-image", imageId, "image/jpeg", height, width,
                    $"https://iiif.io/api/image/3.0/example/reference/d3bbf5397c6df6b894c5991195c912ab-{n}-21198-zz001d8m41_774608_master"));
            }
            else
            {
                canvas.AddMetadata(new Metadata("Description", "Image unavailable or does not exist"));
            }

            manifest.AddItem(canvas);
        }

        return manifest;
    }

    // ---- 0299-region ----------------------------------------------------------------------------

    private static Manifest Recipe0299()
    {
        var manifest = NewManifest("0299-region", "Berliner Tageblatt article, 'Ein neuer Sicherungsplan?'");
        var canvas = NewCanvas("0299-region", "p1", null, 2080, 1768);

        var source = new ImageResource("https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p2/full/max/0/default.jpg", "image/jpeg")
            .SetHeight(4999).SetWidth(3536)
            .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p2", "level1"));
        var body = new SpecificResource(source).SetId(Id("0299-region", "body/b1")).SetSelector(new ImageApiSelector().SetRegion(1768, 2423, 1768, 2080));
        canvas.AddAnnotation(new Annotation(Id("0299-region", "annotation/p0001-image"), body, canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0306-linking-annotations-to-manifests -------------------------------------------------------

    private static Manifest Recipe0306()
    {
        var manifest = NewManifest("0306-linking-annotations-to-manifests", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0306-linking-annotations-to-manifests", "canvas-1", null, 3024, 4032, "canvas-1");
        canvas.AddAnnotation(PaintingImage(canvas, "0306-linking-annotations-to-manifests", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, idIsFull: true));

        // The recipe's annotationpage.json is a separate, externally-referenced resource (see
        // Recipe0306AnnotationPage) - the canvas itself only carries the {id,type} stub.
        canvas.AddAnnotationPageReference(new AnnotationPage(Id("0306-linking-annotations-to-manifests", "annotationpage.json")));
        return manifest.AddItem(canvas);
    }

    private static AnnotationPage Recipe0306AnnotationPage()
    {
        var target = new AnnotationTarget(Id("0306-linking-annotations-to-manifests", "canvas-1"), "Canvas")
            .SetPartOf(Id("0306-linking-annotations-to-manifests", "manifest.json"), "Manifest")
            .SetSelector(FragmentSelector.ForRegion(300, 800, 1200, 1200));
        var comment = new Annotation(Id("0306-linking-annotations-to-manifests", "canvas-1/annopage-2/anno-1"),
            new TextualBody("Der Gänseliesel-Brunnen").SetLanguage("de").SetFormat("text/plain"), target).SetMotivation("commenting");
        return new AnnotationPage(Id("0306-linking-annotations-to-manifests", "annotationpage.json")).AddItem(comment);
    }

    // ---- 0309-annotation-collection ---------------------------------------------------------------

    private static Manifest Recipe0309()
    {
        var manifest = NewManifest("0309-annotation-collection", new Label("Berliner Tageblatt - 1925-02-16", "de"));
        manifest.SetRights(new Rights("http://creativecommons.org/publicdomain/mark/1.0/"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description("<a href='https://www.europeana.eu/portal/record/9200355/BibliographicResource_3000096302605.html'>Berliner Tageblatt</a> - Staatsbibliothek zu Berlin.").SetLanguage("en")));

        var collectionId = Id("0309-annotation-collection", "anno_coll.json");
        var p1Id = Id("0309-annotation-collection", "anno_p1.json");
        var p2Id = Id("0309-annotation-collection", "anno_p2.json");

        var canvas1 = new Canvas(Id("0309-annotation-collection", "canvas/p1"), new Label("p. 1", "none"), 5000, 3602);
        canvas1.AddAnnotation(PaintingImage(canvas1, "0309-annotation-collection", "p1",
            "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p1/full/max/0/default.jpg", "image/jpeg", null, null,
            "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p1"));
        var page1 = new AnnotationPage(p1Id).AddPartOf(new Properties.PartOf(collectionId, "AnnotationCollection")).SetNext(p2Id);
        canvas1.AddAnnotationPageReference(page1);

        var canvas2 = new Canvas(Id("0309-annotation-collection", "canvas/p2"), new Label("p. 2", "none"), 5000, 3602);
        canvas2.AddAnnotation(PaintingImage(canvas2, "0309-annotation-collection", "p2",
            "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p2/full/max/0/default.jpg", "image/jpeg", null, null,
            "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p2"));
        var page2 = new AnnotationPage(p2Id).AddPartOf(new Properties.PartOf(collectionId, "AnnotationCollection")).SetPrev(p1Id);
        canvas2.AddAnnotationPageReference(page2);

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    private static AnnotationCollection Recipe0309Collection() =>
        new AnnotationCollection(Id("0309-annotation-collection", "anno_coll.json"), new Label("Newspaper layout markup"))
            .SetTotal(8)
            .SetFirst(Id("0309-annotation-collection", "anno_p1.json"))
            .SetLast(Id("0309-annotation-collection", "anno_p2.json"));

    // ---- 0318-navPlace-navDate (Collection) ---------------------------------------------------------

    private static Collection Recipe0318()
    {
        var collection = new Collection(Id("0318-navPlace-navDate", "collection.json"), new Label("NavPlace and NavDate Collection"));
        collection.AddSummary(new Description("A collection of items related to Rome.").SetLanguage("en"));
        collection.SetRequiredStatement(new RequiredStatement(new Label("Attribution"), new Description("Objects from the Yale Center for British Art").SetLanguage("en")));

        var items = new[]
        {
            ("manifest-1.json", "Castel Sant'Angelo, Rome", "1776-01-01", 1, -118.4745559, 34.0776376, 1516, 2048, "Castel_Sant_Angelo_Rome"),
            ("manifest-2.json", "The Colosseum", "1776-01-01", 2, 12.492222, 41.890278, 1529, 2048, "The_Colosseum"),
            ("manifest-3.json", "The Arch of Titus from the Forum, Rome, ca. 1725", "1725-01-01", 3, 12.488585, 41.890717, 2875, 2048, "The_Arch_of_Titus_from_the_Forum_Rome"),
            ("manifest-4.json", "The Temple of Vesta, Rome, 1849", "1849-01-01", 4, 12.4862, 41.8917, 2875, 2048, "The_Temple_of_Vesta_Rome"),
            ("manifest-5.json", "A View of Trajan's Forum, Rome, 1821", "1821-01-01", 5, 12.485869, 41.895419, 3005, 2048, "A_View_of_Trajans_Forum_Rome")
        };

        foreach (var (file, label, navDate, n, lon, lat, height, width, slug) in items)
        {
            var m = new Manifest(Id("0318-navPlace-navDate", file), new Label(label)).SetNavDate(DateTime.Parse($"{navDate}T00:00:00Z").ToUniversalTime());
            m.SetNavPlace(new NavPlace(Id("0318-navPlace-navDate", $"feature-collection/{n}"))
                .AddFeature(new Feature(Id("0318-navPlace-navDate", $"feature/{n}"))
                    .SetProperties(new FeatureProperties().AddLabel(new Label(label)))
                    .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(lon, lat)))));
            var canvas = new Canvas(Id("0318-navPlace-navDate", $"canvas/{n}"), new Label(label), height, width);
            canvas.AddAnnotation(PaintingImage(canvas, "0318-navPlace-navDate", $"anno/{n}",
                $"https://iiif.io/api/image/3.0/example/reference/71b9228e087f15c75b628214cd9f647d-{slug}/full/max/0/default.jpg", "image/jpeg", height, width,
                $"https://iiif.io/api/image/3.0/example/reference/71b9228e087f15c75b628214cd9f647d-{slug}"));
            m.AddItem(canvas);
            collection.AddItem(m);
        }

        return collection;
    }

    // ---- 0326-annotating-image-layer --------------------------------------------------------------

    private static Manifest Recipe0326()
    {
        var manifest = NewManifest("0326-annotating-image-layer", "Choice Example with layer specific annotation");
        var canvas = NewCanvas("0326-annotating-image-layer", "p1", null, 1271, 2000);

        var naturalId = "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-natural";
        var natural = new ImageResource($"{naturalId}/full/max/0/default.jpg", "image/jpeg").SetHeight(1271).SetWidth(2000).SetLabel(new Label("Natural Light"))
            .AddService(new Service(ImageService3, naturalId, "level1"));

        var xrayId = "https://iiif.io/api/image/3.0/example/reference/421e65be2ce95439b3ad6ef1f2ab87a9-dee-xray";
        var xray = new ImageResource($"{xrayId}/full/2000,1271/0/default.jpg", "image/jpeg").SetHeight(1271).SetWidth(2000).SetLabel(new Label("X-Ray"))
            .AddService(new Service(ImageService3, xrayId, "level1"));

        canvas.AddAnnotation(new Annotation(Id("0326-annotating-image-layer", "annotation/p0001-image"), new Choice([natural, xray]), canvas.Id));

        // The recipe's own "annotations" property lives on the X-ray Choice item itself (a rare
        // per-layer annotation target, not a Canvas-level one) - targeting the image resource
        // directly (not the Canvas) via an ImageApiSelector region+size.
        var layerTarget = new AnnotationTarget(xrayId).SetSelector(new ImageApiSelector().SetRegion(810, 900, 260, 370).SetSize("2000,1271"));
        var layerTag = new Annotation(Id("0326-annotating-image-layer", "annotation/p0002-tag"),
            new TextualBody("A group of skulls.").SetLanguage("en").SetFormat("text/plain"), layerTarget).SetMotivation("tagging");
        var layerPage = new AnnotationPage(Id("0326-annotating-image-layer", "page/p2/1"));
        layerPage.AddItem(layerTag);
        canvas.AddAnnotationPageReference(layerPage);

        return manifest.AddItem(canvas);
    }

    // ---- 0346-multilingual-annotation-body -----------------------------------------------------------

    private static Manifest Recipe0346()
    {
        var manifest = NewManifest("0346-multilingual-annotation-body", new Label("Koto, chess, calligraphy, and painting"), new Label("琴棋書画図屏風", "ja"));
        var canvas = NewCanvas("0346-multilingual-annotation-body", "p1", null, 3966, 8800);
        canvas.AddAnnotation(PaintingImage(canvas, "0346-multilingual-annotation-body", "p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004/full/max/0/default.jpg", "image/jpeg", 3966, 8800,
            "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004"));

        var en = new TextualBody("Koto with a cover being carried").SetLanguage("en").SetFormat("text/plain");
        var ja = new TextualBody("袋に収められた琴").SetLanguage("ja").SetFormat("text/plain");
        var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(1650, 1200, 925, 1250));
        var comment = new Annotation(Id("0346-multilingual-annotation-body", "annotation/p0001-comment"), new Choice([en, ja]), target).SetMotivation("commenting");
        var page = new AnnotationPage(Id("0346-multilingual-annotation-body", "page/p2/1"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, comment);
    }

    // ---- 0377-image-in-annotation -----------------------------------------------------------------

    private static Manifest Recipe0377()
    {
        var manifest = NewManifest("0377-image-in-annotation", "Picture of Göttingen taken during the 2019 IIIF Conference");
        var canvas = NewCanvas("0377-image-in-annotation", "canvas-1", null, 3024, 4032, "canvas-1");
        canvas.AddAnnotation(PaintingImage(canvas, "0377-image-in-annotation", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, idIsFull: true));

        var fountainDetail = new ImageResource("https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-fountain/full/300,/0/default.jpg", "image/jpeg");
        var text = new TextualBody("Night picture of the Gänseliesel fountain in Göttingen taken during the 2019 IIIF Conference").SetLanguage("en");
        var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(138, 550, 1477, 1710));
        var comment = new Annotation(Id("0377-image-in-annotation", "canvas-1/annopage-2/anno-1"), fountainDetail, target).AddBody(text).SetMotivation("commenting");
        var page = new AnnotationPage(Id("0377-image-in-annotation", "canvas-1/annopage-2"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, comment);
    }

    // ---- 0434-choice-av -------------------------------------------------------------------------

    private static Manifest Recipe0434()
    {
        var manifest = NewManifest("0434-choice-av", "Excerpt from Egbe Iyawo");
        manifest.AddSummary(new Description("Excerpt from a performance of Egbe Iyawo recorded in Kabba Division, Kwara State.").SetLanguage("en"));
        manifest.SetRights(new Rights("http://creativecommons.org/publicdomain/zero/1.0/"));

        var canvas = NewCanvas("0434-choice-av", "1", null, null, null).SetDuration(16.0);
        var formats = new[] { ("m4a", "audio/alac", "ALAC"), ("mp3", "audio/mpeg", "MP3"), ("flac", "audio/flac", "FLAC"), ("ogg", "audio/ogg", "OGG Vorbis OGG"), ("mpeg", "audio/mpeg", "MPEG2"), ("wav", "audio/wav", "WAV") };
        var items = formats.Select(f => (IBaseResource)new AudioResource($"https://fixtures.iiif.io/audio/ucla/egbe-iyawo-ucla.{f.Item1}", f.Item2)
            .SetDuration(16.0).SetLabel(new Label(f.Item3))).ToList();
        canvas.AddAnnotation(new Annotation(Id("0434-choice-av", "canvas/1/annotation_page/1/annotation/1"), new Choice(items), canvas.Id));
        return manifest.AddItem(canvas);
    }

    // ---- 0464-reuse-manifest ---------------------------------------------------------------------

    private static Manifest Recipe0464()
    {
        var manifest = NewManifest("0464-reuse-manifest", "Picture of Göttingen taken during the 2019 IIIF Conference, reused and annotated.");
        manifest.SetRights(new Rights("http://creativecommons.org/licenses/by-sa/3.0/"));
        manifest.SetRequiredStatement(new RequiredStatement(new Label("Attribution"),
            new Description("Glen Robson, IIIF Technical Coordinator. CC BY-SA 3.0").SetLanguage("en")));
        manifest.AddMetadata(new Metadata("Source Manifest", "Reuse of Source Manifest with added Annotation, see https://iiif.io/api/cookbook/recipe/0008-rights/manifest.json"));

        // Reuses the ORIGINAL 0008-rights canvas/annotation ids verbatim to signal it's the same resource.
        var canvas = new Canvas(Id("0008-rights", "canvas/p1"), new Label("Untitled"), 3024, 4032);
        canvas.AddAnnotation(PaintingImage(canvas, "0008-rights", "p0001-image", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId));

        var comment = new Annotation(Id("0464-reuse-manifest", "canvas-1/annopage-2/anno-1"),
            new TextualBody("Göttinger Marktplatz mit Gänseliesel Brunnen").SetLanguage("de").SetFormat("text/plain"), canvas.Id).SetMotivation("commenting");
        var page = new AnnotationPage(Id("0464-reuse-manifest", "canvas-1/annopage-2"));
        canvas.AddAnnotationPageReference(page);
        return WithSupplementaryPage(manifest, canvas, page, comment);
    }

    // ---- 0485-contentstate-canvas-region (Content State) ------------------------------------------

    private static ContentState Recipe0485()
    {
        var target = new ContentStateTarget(Id("0009-book-1", "canvas/p2") + "#xywh=1528,3024,344,408", "Canvas")
            .SetPartOf(Id("0009-book-1", "manifest.json"), "Manifest");
        return new ContentState(target).SetId(Id("0485-contentstate-canvas-region", "annotation.json"));
    }

    // ---- 0489-multimedia-canvas -------------------------------------------------------------------

    private static Manifest Recipe0489()
    {
        var manifest = NewManifest("0489-multimedia-canvas", "Multimedia Canvas");
        var canvas = NewCanvas("0489-multimedia-canvas", "canvas", null, 31722, 70399).SetDuration(180.0);

        canvas.AddAnnotation(new Annotation(Id("0489-multimedia-canvas", "annotation/p0001-image"),
            new ImageResource("https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004/full/max/0/default.jpg", "image/jpeg")
                .SetHeight(31722).SetWidth(70399)
                .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004", "level1")),
            new AnnotationTarget($"{canvas.Id}#t=11,42")));

        canvas.AddAnnotation(new Annotation(Id("0489-multimedia-canvas", "annotation/p0002-video"),
            new VideoResource("https://fixtures.iiif.io/video/indiana/30-minute-clock/medium/30-minute-clock.mp4", "video/mp4").SetHeight(360).SetWidth(640).SetDuration(1801.055),
            new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(1000, 500, 5000, 6000))));

        var texts = new (string id, int x, int y, int w, int h, double t0, double t1, string value)[]
        {
            ("p0004-text", 30200, 10200, 15000, 5000, 0, 1, "<p style='font-size:2000px'>Press Play</p>"),
            ("p0005-text", 20220, 5000, 30000, 5000, 1, 11, "<p style='font-size:1500px'>In 10 seconds, this text will be replaced...</p>"),
            ("p0006-text", 27000, 10200, 25000, 5000, 42, 180, "<p style='font-size:2000px'>Close your browser</p>")
        };
        foreach (var (id, x, y, w, h, t0, t1, value) in texts)
        {
            var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(x, y, w, h));
            canvas.AddAnnotation(new Annotation(Id("0489-multimedia-canvas", $"annotation/{id}"),
                new TextualBody(value).SetLanguage("en").SetFormat("text/html"), target).SetMotivation("painting"));
        }

        return manifest.AddItem(canvas);
    }

    // ---- 0540-link-for-opening-multiple-canvases (Content State) -----------------------------------

    private static Manifest Recipe0540()
    {
        var manifest = NewManifest("0540-link-for-opening-multiple-canvases", "Two monuments in Rome");
        var canvas1 = NewCanvas("0540-link-for-opening-multiple-canvases", "p1", "The Temple of Vesta Rome", 1464, 2048);
        canvas1.AddAnnotation(new Annotation(Id("0540-link-for-opening-multiple-canvases", "annotation/p0001-image"),
            new ImageResource("https://fixtures.iiif.io/images/Yale/ycba/The_Temple_of_Vesta_Rome.jpg", "image/jpeg").SetHeight(1464).SetWidth(2048), canvas1.Id));

        var canvas2 = NewCanvas("0540-link-for-opening-multiple-canvases", "p2", "The Colosseum", 1302, 2048);
        canvas2.AddAnnotation(new Annotation(Id("0540-link-for-opening-multiple-canvases", "annotation/p0002-image"),
            new ImageResource("https://fixtures.iiif.io/images/Yale/ycba/The_Colosseum_Rome2.jpg", "image/jpeg").SetHeight(1302).SetWidth(2048), canvas2.Id));

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    private static ContentState Recipe0540ContentState()
    {
        var target1 = new ContentStateTarget(Id("0540-link-for-opening-multiple-canvases", "canvas/2"), "Canvas")
            .SetPartOf(Id("0540-link-for-opening-multiple-canvases", "manifest-2.json"), "Manifest");
        var target2 = new ContentStateTarget(Id("0540-link-for-opening-multiple-canvases", "canvas/p2"), "Canvas")
            .SetPartOf(Id("0540-link-for-opening-multiple-canvases", "manifest.json"), "Manifest");
        return new ContentState(target1, target2);
    }

    // ---- 0560-resources-on-a-timeline --------------------------------------------------------------

    private static Manifest Recipe0560()
    {
        var manifest = NewManifest("0560-resources-on-a-timeline", "Rendering Resources Sequentially on a Timeline").AddBehavior(new Behavior("repeat"));
        var canvas = NewCanvas("0560-resources-on-a-timeline", "1", null, 2572, 3764, idOverride: "canvas1").SetDuration(4.0);

        canvas.AddAnnotation(new Annotation(Id("0560-resources-on-a-timeline", "canvas1/annotation/p1a1-image"),
            new ImageResource("https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Northeaster_by_Winslow_Homer_1895/full/max/0/default.jpg", "image/jpeg")
                .SetHeight(2572).SetWidth(3764)
                .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Northeaster_by_Winslow_Homer_1895/", "level1")),
            new AnnotationTarget($"{canvas.Id}#t=0,2")));

        canvas.AddAnnotation(new Annotation(Id("0560-resources-on-a-timeline", "canvas1/annotation/p1a2-image"),
            new ImageResource("https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Winslow_Homer_-_The_Gulf_Stream_-_Metropolitan_Museum_of_Art/full/max/0/default.jpg", "image/jpeg")
                .SetHeight(3540).SetWidth(5886)
                .AddService(new Service(ImageService3, "https://iiif.io/api/image/3.0/example/reference/329817fc8a251a01c393f517d8a17d87-Winslow_Homer_-_The_Gulf_Stream_-_Metropolitan_Museum_of_Art/", "level1")),
            new AnnotationTarget($"{canvas.Id}#t=2,4")));

        return manifest.AddItem(canvas);
    }

    // ---- 0561-text-on-image ----------------------------------------------------------------------

    private static Manifest Recipe0561()
    {
        var manifest = NewManifest("0561-text-on-image", "Visible Text Annotation");
        var canvas = NewCanvas("0561-text-on-image", "canvas", null, 31722, 70399);
        canvas.AddAnnotation(PaintingImage(canvas, "0561-text-on-image", "p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004/full/max/0/default.jpg", "image/jpeg", 31722, 70399,
            "https://iiif.io/api/image/3.0/example/reference/36ca0a3370db128ec984b33d71a1543d-100320001004"));

        var target = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(5500, 12200, 8000, 5000));
        canvas.AddAnnotation(new Annotation(Id("0561-text-on-image", "annotation/p0001-text"),
            new TextualBody("<p style='font-size:1000px; background-color: rgba(16, 16, 16, 0.5); padding:300px'>The koto is to the right, carried in a cloth wrapping.</p>")
                .SetLanguage("en").SetFormat("text/html"), target));
        return manifest.AddItem(canvas);
    }

    // ---- 0599-drag-and-drop (Content State) ---------------------------------------------------------

    private static ContentState Recipe0599()
    {
        var target = new ContentStateTarget(Id("0006-text-language", "manifest.json"), "Manifest");
        return new ContentState(target).SetId(Id("0599-drag-and-drop", "dnd-manifest.json"));
    }

    // ---- Shared helpers -------------------------------------------------------------------------

    private const string GottingenImageId = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg";
    private const string GottingenServiceId = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen";

    private static string Id(string recipe, string path) => $"{Base}/{recipe}/{path}";

    private static string CanvasId(string recipe, string page) => Id(recipe, $"canvas/{page}");

    private static Manifest NewManifest(string recipe, string label, string file = "manifest.json") => new(Id(recipe, file), new Label(label));

    private static Manifest NewManifest(string recipe, Label label, string file = "manifest.json") => new(Id(recipe, file), label);

    private static Manifest NewManifest(string recipe, Label primary, Label secondary) => new Manifest(Id(recipe, "manifest.json"), primary).AddLabel(secondary);

    private static Canvas NewCanvas(string recipe, string page, string? label, int? height, int? width, string? idOverride = null, bool idIsFull = false, string labelLanguage = "en")
    {
        var id = idIsFull ? Id(recipe, page) : idOverride is not null ? Id(recipe, idOverride) : CanvasId(recipe, page);
        var canvas = new Canvas(id, new Label(label ?? "Untitled", label is null ? "none" : labelLanguage), height ?? 1, width ?? 1);
        return canvas;
    }

    private static Canvas NewCanvas(string recipe, string page, string? label, int? height, int? width, string labelOverride, string labelLanguage) =>
        new Canvas(CanvasId(recipe, page), new Label(labelOverride, labelLanguage), height ?? 1, width ?? 1);

    private static Annotation PaintingImage(Canvas canvas, string recipe, string annotationPath, string imageId, string format, int? height, int? width, string? serviceId, bool idIsFull = false)
    {
        var image = new ImageResource(imageId, format);
        if (height is not null)
        {
            image.SetHeight(height.Value);
        }

        if (width is not null)
        {
            image.SetWidth(width.Value);
        }

        if (serviceId is not null)
        {
            image.AddService(new Service(ImageService3, serviceId, "level1"));
        }

        var annotationId = idIsFull ? Id(recipe, annotationPath) : Id(recipe, $"annotation/{annotationPath}");
        return new Annotation(annotationId, image, canvas.Id);
    }

    private static Manifest WithSupplementaryPage(Manifest manifest, Canvas canvas, AnnotationPage page, Annotation annotation)
    {
        page.AddItem(annotation);
        return manifest.AddItem(canvas);
    }
}
