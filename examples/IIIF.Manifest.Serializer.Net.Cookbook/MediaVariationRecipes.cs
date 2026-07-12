using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0035-0045: variations on how images are painted onto a Canvas - a foldout sequence
///     with a non-paged behavior, a composite of a full image plus a cropped detail, an Image API
///     rotation delivered either as a service parameter or via CSS, and CSS-styled annotations.
/// </summary>
internal sealed class MediaVariationRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0035: Foldouts", Recipe0035),
            new("Recipe 0036: Composition From Multiple Images", Recipe0036),
            new("Recipe 0040: Image Rotation Service", Recipe0040Service),
            new("Recipe 0040: Image Rotation via CSS", Recipe0040Css),
            new("Recipe 0045: Simple Annotation with CSS Styling", Recipe0045)
        ];
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
            if (behavior is not null) canvas.AddBehavior(new Behavior(behavior));

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
        var canvas = NewCanvas("0036-composition-from-multiple-images", "p1", null, 5412, 7216, "f. 033v-034r [Chilpéric Ier tue Galswinthe, se remarie et est assassiné]", "none");
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
}