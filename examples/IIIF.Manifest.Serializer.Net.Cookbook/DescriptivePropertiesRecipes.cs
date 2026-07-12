using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Selectors;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0117-0229: descriptive properties beyond the basics - Manifest thumbnails, multi-value
///     metadata, point/geo selectors, the navPlace extension, manifest-level start canvas, an external
///     caption file, and a behavior-driven Range/Structure with per-Range thumbnails.
/// </summary>
internal sealed class DescriptivePropertiesRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0117: Add a Thumbnail to a Manifest", Recipe0117),
            new("Recipe 0118: Multiple Values in Metadata", Recipe0118),
            new("Recipe 0135: Annotating a Point in a Canvas", Recipe0135),
            new("Recipe 0139: Geolocate a Canvas Fragment", Recipe0139),
            new("Recipe 0154: Simple navPlace Extension", Recipe0154),
            new("Recipe 0202: Manifest Start Canvas", Recipe0202),
            new("Recipe 0219: Using a Caption File", Recipe0219),
            new("Recipe 0229: Table of Contents with Behavior", Recipe0229)
        ];
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
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", true));

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
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", true));

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
            if (n == 2) startCanvas = canvas;
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
}