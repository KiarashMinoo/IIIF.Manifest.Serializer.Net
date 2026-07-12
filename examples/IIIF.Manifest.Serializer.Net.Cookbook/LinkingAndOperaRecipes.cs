using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Shared.Selectors;
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Recipes 0046-0103: the descriptive linking properties (rendering/homepage/seeAlso), the 2.1
///     vs. 3.0 publishing duality, opera Manifests structured as one Canvas or many, a newspaper
///     Collection with OCR supplementing annotations, multilingual captions, and a supplementary
///     comment on an audio segment. Shares the UCLA playbill Canvas set (0046/0053) and the newspaper
///     issue Manifest shape (0068) as private helpers, since each is only reused within this group.
/// </summary>
internal sealed class LinkingAndOperaRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes()
    {
        return
        [
            new("Recipe 0046: Rendering", Recipe0046),
            new("Recipe 0047: Linking to Structured Metadata (Homepage)", Recipe0047),
            new("Recipe 0053: Linking to Structured Metadata (SeeAlso)", Recipe0053),
            new("Recipe 0057: Presentation API 2.1 vs 3.0", Recipe0057),
            new("Recipe 0064: Book/Opera - One Canvas", Recipe0064),
            new("Recipe 0065: Book/Opera - Multiple Canvases", Recipe0065),
            new("Recipe 0068: Newspaper", Recipe0068),
            new("Recipe 0074: Multiple Language Captions", Recipe0074),
            new("Recipe 0103: Simple Annotation on Audio Segment", Recipe0103)
        ];
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
            "https://iiif.io/api/image/3.0/example/reference/28473c77da3deebe4375c3a50572d9d3-laocoon", true));
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
                $"https://iiif.io/api/image/3.0/example/reference/4ce82cef49fb16798f4c2440307c3d6f-{imageSuffixBase}{n}", true));

            var ocrPage = new AnnotationPage(Id("0068-newspaper", $"{file}-anno_p{n}.json"));
            var ocrTarget = new AnnotationTarget(canvas.Id, "Canvas").SetPartOf(Id("0068-newspaper", file)).SetSelector(FragmentSelector.ForRegion(0, 376, 399, 53));
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
}