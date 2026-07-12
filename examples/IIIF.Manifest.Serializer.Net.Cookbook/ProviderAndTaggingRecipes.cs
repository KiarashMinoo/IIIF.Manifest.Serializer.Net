using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0230-0283: NavDate on a Collection, Canvas thumbnails for A/V and image content, the
///     Provider descriptive property, navPlace on individual Canvases, tagging an external Dataset
///     resource, a non-rectangular SVG selector, annotating a whole Canvas, embedded vs. referenced
///     AnnotationPages, and a missing-image placeholder pattern.
/// </summary>
internal sealed class ProviderAndTaggingRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0230: NavDate on Collection and Manifest", Recipe0230),
            new("Recipe 0232: Thumbnail on a Canvas (Video)", Recipe0232Av),
            new("Recipe 0232: Thumbnail on a Canvas (Image)", Recipe0232Image),
            new("Recipe 0234: Provider", Recipe0234),
            new("Recipe 0240: navPlace on Canvases", Recipe0240),
            new("Recipe 0258: Tagging an External Resource", Recipe0258),
            new("Recipe 0261: Non-Rectangular Commenting", Recipe0261),
            new("Recipe 0266: Annotating the Whole Canvas", Recipe0266),
            new("Recipe 0269: Embedded or Referenced Annotations", Recipe0269),
            new("Recipe 0283: Missing Image", Recipe0283)
        ];
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
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-87691274-1986", true));
        m1986.AddItem(c1986);

        var m1987 = new Manifest(Id("0230-navdate", "navdate_map_1-manifest.json"), new Label("1987 Chesapeake and Ohio Canal, Washington, D.C., Maryland, West Virginia, official map and guide"))
            .SetNavDate(DateTime.Parse("1987-01-01T00:00:00Z").ToUniversalTime());
        var c1987 = new Canvas(Id("0230-navdate", "navdate_map_1-manifest.json/canvas/p1"), new Label("1987 Map, recto and verso, with a date of publication"), 7072, 5212);
        c1987.AddAnnotation(PaintingImage(c1987, "0230-navdate", "navdate_map_1-manifest.json/annotation/p0001-image",
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674/full/max/0/default.jpg", "image/jpeg", 7072, 5212,
            "https://iiif.io/api/image/3.0/example/reference/43153e2ec7531f14dd1c9b2fc401678a-88695674", true));
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
        canvas.AddAnnotation(PaintingImage(canvas, "0266-full-canvas-annotation", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, true));

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
        canvas.AddAnnotation(PaintingImage(canvas, "0269-embedded-or-referenced-annotations", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, true));

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
}