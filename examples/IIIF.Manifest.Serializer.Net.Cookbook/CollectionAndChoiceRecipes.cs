using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0027-0033: alternative Range page ordering, per-canvas metadata, multi-volume works
///     (both as a Collection of Manifests and as a single bound Manifest with Ranges), the simplest
///     Collection, and an image Choice.
/// </summary>
internal sealed class CollectionAndChoiceRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0027: Alternative Page Order", Recipe0027),
            new("Recipe 0029: Metadata Anywhere", Recipe0029),
            new("Recipe 0030: Multi-Volume Work (Collection)", Recipe0030),
            new("Recipe 0031: Bound Multi-Volume Work (Single Manifest)", Recipe0031),
            new("Recipe 0032: Simple Collection", Recipe0032),
            new("Recipe 0033: Image Choice", Recipe0033)
        ];
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
        for (var n = 1; n <= 4; n++) physical.AddCanvasReference(CanvasId("0027-alternative-page-order", $"p{n}"));

        var authorIntended = new Structure(Id("0027-alternative-page-order", "range/r2"), new Label("Author-intended sequence")).AddBehavior(new Behavior("sequence"));
        foreach (var n in new[] { 2, 3, 4, 1 }) authorIntended.AddCanvasReference(CanvasId("0027-alternative-page-order", $"p{n}"));

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
        var pages = new[]
        {
            ("1_frontcover", "Front cover"), ("2_insidefrontcover", "Inside front cover"), ("3_titlepage1", "Vol. 1 title page"), ("4_titlepage1_verso", "Vol. 1 title page (verso)"), ("5_titlepage2", "Vol. 2 title page"),
            ("6_titlepage2_verso", "Vol. 2 title page (verso)")
        };
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
}