using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0299-0346: cropping an image region, linking an externally-referenced AnnotationPage
///     back to its Manifest, a paginated AnnotationCollection (both the Manifest side and the
///     standalone document), navPlace/navDate together on a Collection's members, a per-layer
///     annotation on one item of a Choice, and a multilingual Choice used as an annotation body.
/// </summary>
internal sealed class AnnotationCollectionRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0299: Cropping an Image (Region)", Recipe0299),
            new("Recipe 0306: Linking Annotations to Manifests", Recipe0306),
            new("Recipe 0306: Referenced AnnotationPage", Recipe0306AnnotationPage),
            new("Recipe 0309: Annotation Collection", Recipe0309),
            new("Recipe 0309: Standalone AnnotationCollection Document", Recipe0309Collection),
            new("Recipe 0318: navPlace and navDate on a Collection", Recipe0318),
            new("Recipe 0326: Annotating a Choice of Images", Recipe0326),
            new("Recipe 0346: Multilingual Annotation Body", Recipe0346)
        ];
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
        canvas.AddAnnotation(PaintingImage(canvas, "0306-linking-annotations-to-manifests", "canvas-1/annopage-1/anno-1", GottingenImageId, "image/jpeg", 3024, 4032, GottingenServiceId, true));

        // The recipe's annotationpage.json is a separate, externally-referenced resource (see
        // Recipe0306AnnotationPage) - the canvas itself only carries the {id,type} stub.
        canvas.AddAnnotationPageReference(new AnnotationPage(Id("0306-linking-annotations-to-manifests", "annotationpage.json")));
        return manifest.AddItem(canvas);
    }

    private static AnnotationPage Recipe0306AnnotationPage()
    {
        var target = new AnnotationTarget(Id("0306-linking-annotations-to-manifests", "canvas-1"), "Canvas")
            .SetPartOf(Id("0306-linking-annotations-to-manifests", "manifest.json"))
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
        var page1 = new AnnotationPage(p1Id).AddPartOf(new PartOf(collectionId, "AnnotationCollection")).SetNext(p2Id);
        canvas1.AddAnnotationPageReference(page1);

        var canvas2 = new Canvas(Id("0309-annotation-collection", "canvas/p2"), new Label("p. 2", "none"), 5000, 3602);
        canvas2.AddAnnotation(PaintingImage(canvas2, "0309-annotation-collection", "p2",
            "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p2/full/max/0/default.jpg", "image/jpeg", null, null,
            "https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-newspaper-p2"));
        var page2 = new AnnotationPage(p2Id).AddPartOf(new PartOf(collectionId, "AnnotationCollection")).SetPrev(p1Id);
        canvas2.AddAnnotationPageReference(page2);

        return manifest.AddItem(canvas1).AddItem(canvas2);
    }

    private static AnnotationCollection Recipe0309Collection()
    {
        return new AnnotationCollection(Id("0309-annotation-collection", "anno_coll.json"), new Label("Newspaper layout markup"))
            .SetTotal(8)
            .SetFirst(Id("0309-annotation-collection", "anno_p1.json"))
            .SetLast(Id("0309-annotation-collection", "anno_p2.json"));
    }

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
}