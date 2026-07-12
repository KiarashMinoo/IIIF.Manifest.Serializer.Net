using System;
using System.Collections.Generic;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Properties.Services.Auth2;
using IIIF.Manifests.Serializer.Properties.Services.Search;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;

namespace IIIF.Manifests.Serializer.Examples;

public sealed record ExampleDefinition(string Title, Func<object> Build);

public static class DemoCatalog
{
    public static IReadOnlyList<ExampleDefinition> GetAll()
    {
        return
        [
            new ExampleDefinition("Wellcome-style search and access manifest", CreateSearchManifest),
            new ExampleDefinition("Wellcome-style overlaid search results", CreateOverlaidSearchResults),
            new ExampleDefinition("Nationalmuseum-style deep zoom manifest", CreateDeepZoomManifest),
            new ExampleDefinition("Stanford-style paged book manifest", CreatePagedBookManifest),
            new ExampleDefinition("Biblissima-style reunification of a separated object", CreateReunificationExample),
            new ExampleDefinition("Harvard-style canvas annotation example", CreateAnnotationExample),
            new ExampleDefinition("Education/storytelling annotation tour", CreateStorytellingAnnotationExample),
            new ExampleDefinition("Guided annotation tour (Range-based)", CreateGuidedTourExample),
            new ExampleDefinition("Mixed-media multi-canvas object (audio/video/PDF)", CreateMixedMediaObjectExample),
            new ExampleDefinition("Map-based place example", CreateMapExample)
        ];
    }

    private static Manifest CreateSearchManifest()
    {
        var manifest = new Manifest("https://example.org/demo/search/manifest", new Label("Search Demo"));
        var canvas = new Canvas("https://example.org/demo/search/canvas/1", new Label("Page 1"), 1200, 900);
        canvas.AddAnnotation(new Annotation(
            "https://example.org/demo/search/annotation/1",
            new ImageResource("https://example.org/demo/search/image.jpg", "image/jpeg").SetHeight(1200).SetWidth(900),
            canvas.Id));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/demo/search/sequence/normal");
        manifest.AddService(new SearchService("http://iiif.io/api/search/2/context.json", "https://example.org/demo/search/service", "http://iiif.io/api/search/0/search").AddService(new AutoCompleteService("http://iiif.io/api/search/2/context.json",
            "https://example.org/demo/search/autocomplete", "http://iiif.io/api/search/0/autocomplete")));
        manifest.AddService(new ContentStateService("http://iiif.io/api/content-state/1/context.json", "https://example.org/demo/content-state", "http://iiif.io/api/content-state/v1/state"));
        var accessTokenService = new AuthAccessTokenService2("https://example.org/demo/auth/token");
        var accessService = new AuthAccessService2("https://example.org/demo/auth/login", "active", accessTokenService)
            .SetLabel("Login").SetHeading("Sign in").SetNote("Protected content").SetConfirmLabel("Continue")
            .SetLogoutService(new AuthLogoutService2("https://example.org/demo/auth/logout", "Logout"));
        manifest.AddService(new AuthProbeService2("https://example.org/demo/auth/probe", accessService));
        return manifest;
    }

    /// <summary>
    ///     Distinct from <see cref="CreateSearchManifest" />: that demo carries only the
    ///     search/autocomplete service *descriptors* a viewer would use to issue a query. This one
    ///     is the actual search *response* a Wellcome-style overlay would render - hit-highlighted
    ///     Annotations with text-quote selectors targeting canvas regions, paged like a real
    ///     Content Search API 2.0 result set.
    /// </summary>
    private static SearchResponse CreateOverlaidSearchResults()
    {
        var hit1 = new SearchHitAnnotation(
                new SearchHitTarget("https://example.org/demo/search/canvas/1#xywh=100,100,250,20",
                    [new SearchTextQuoteSelector("lighthouse").SetPrefix("a view of the ").SetSuffix(" from the harbor")]))
            .SetId("https://example.org/demo/search/annotation/match-1");

        var hit2 = new SearchHitAnnotation(
                new SearchHitTarget("https://example.org/demo/search/canvas/2#xywh=300,150,180,30",
                    [new SearchTextQuoteSelector("lighthouse keeper").SetPrefix("the ").SetSuffix(" recorded the weather daily")]))
            .SetId("https://example.org/demo/search/annotation/match-2");

        return new SearchResponse("https://example.org/demo/search/service/search?q=lighthouse")
            .SetAnnotations(new SearchHitAnnotationPage([hit1, hit2]))
            .SetPartOf(new SearchAnnotationCollectionRef("https://example.org/demo/search/results").SetTotal(2))
            .SetStartIndex(0);
    }

    private static Manifest CreateDeepZoomManifest()
    {
        var manifest = new Manifest("https://example.org/demo/deep-zoom/manifest", new Label("Deep Zoom Demo"));
        manifest.AddHomepage(new Homepage("https://example.org/demo/object", "Object page"));
        manifest.SetThumbnail(new Thumbnail("https://example.org/demo/deep-zoom/thumb.jpg"));
        manifest.AddProvider(new Provider("https://example.org/demo/institution", "Museum"));

        var canvas = new Canvas("https://example.org/demo/deep-zoom/canvas/1", new Label("Image"), 4000, 3000);
        var imageResource = new ImageResource("https://example.org/demo/deep-zoom/image/full/full/0/default.jpg", "image/jpeg").SetHeight(4000).SetWidth(3000);
        imageResource.AddService(new Service("http://iiif.io/api/image/2/context.json", "https://example.org/demo/image-service", "http://iiif.io/api/image/2/level2.json").AsImageService2().SetHeight(4000).SetWidth(3000));
        canvas.AddAnnotation(new Annotation("https://example.org/demo/deep-zoom/annotation/1", imageResource, canvas.Id));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/demo/deep-zoom/sequence/normal");
        return manifest;
    }

    private static Manifest CreatePagedBookManifest()
    {
        var manifest = new Manifest("https://example.org/demo/book/manifest", new Label("Paged Book Demo"));
        manifest.SetViewingDirection(ViewingDirection.Ltr);
        manifest.AddBehavior(Behavior.Paged);
        manifest.SetStart("https://example.org/demo/book/canvas/2");
        manifest.SetSequenceId("https://example.org/demo/book/sequence/normal");

        for (var page = 1; page <= 4; page++)
        {
            var canvas = new Canvas($"https://example.org/demo/book/canvas/{page}", new Label($"Page {page}"), 2000, 1400);
            canvas.AddAnnotation(new Annotation($"https://example.org/demo/book/annotation/{page}", new ImageResource($"https://example.org/demo/book/page-{page}.jpg", "image/jpeg").SetHeight(2000).SetWidth(1400), canvas.Id));
            manifest.AddItem(canvas);
        }

        return manifest;
    }

    private static Collection CreateReunificationExample()
    {
        // Inspired by Biblissima's separated-object reunification pattern: a physical object
        // (e.g. a manuscript split across two institutions) is represented as leaves of separate
        // Manifests, each preserving its own source institution's Provider/Homepage/SeeAlso so a
        // viewer can browse the reunified whole while still crediting/linking back to whichever
        // institution actually holds each part.
        var parisManifest = new Manifest("https://gallica.bnf.fr/demo/reunification/manifest", new Label("Manuscript, Part 1 (Paris)"))
            .AddProvider(new Provider("https://www.bnf.fr", "Bibliothèque nationale de France").AddHomepage(new Homepage("https://www.bnf.fr")))
            .AddHomepage(new Homepage("https://gallica.bnf.fr/demo/reunification/object-page", "View at Gallica"))
            .AddSeeAlso(new SeeAlso("https://gallica.bnf.fr/demo/reunification/record.xml").SetFormat("application/xml").SetType("Dataset"));
        parisManifest.AddItem(new Canvas("https://gallica.bnf.fr/demo/reunification/canvas/1", new Label("Leaf 1r"), 3000, 2200));
        parisManifest.SetSequenceId("https://gallica.bnf.fr/demo/reunification/sequence/normal");

        var vaticanManifest = new Manifest("https://digi.vatlib.it/demo/reunification/manifest", new Label("Manuscript, Part 2 (Vatican)"))
            .AddProvider(new Provider("https://www.vatlib.it", "Biblioteca Apostolica Vaticana").AddHomepage(new Homepage("https://www.vatlib.it")))
            .AddHomepage(new Homepage("https://digi.vatlib.it/demo/reunification/object-page", "View at DigiVatLib"))
            .AddSeeAlso(new SeeAlso("https://digi.vatlib.it/demo/reunification/record.xml").SetFormat("application/xml").SetType("Dataset"));
        vaticanManifest.AddItem(new Canvas("https://digi.vatlib.it/demo/reunification/canvas/1", new Label("Leaf 2r"), 3000, 2200));
        vaticanManifest.SetSequenceId("https://digi.vatlib.it/demo/reunification/sequence/normal");

        var collection = new Collection("https://example.org/demo/reunification/collection", new Label("Reunified Manuscript"))
            .AddHomepage(new Homepage("https://example.org/demo/reunification/about", "About this reunification project"));
        collection.AddBehavior(Behavior.MultiPart);
        collection.AddItem(parisManifest);
        collection.AddItem(vaticanManifest);
        return collection;
    }

    private static AnnotationList CreateAnnotationExample()
    {
        var annotationList = new AnnotationList("https://example.org/demo/annotations/list", new Label("Annotations"));
        annotationList.AddResource(new SegmentResource("https://example.org/demo/annotations/1", ResourceType.Image).SetFull(new BaseResource("https://example.org/demo/annotations/1/full", ResourceType.Image)));
        return annotationList;
    }

    /// <summary>
    ///     Distinct from <see cref="CreateAnnotationExample" /> (a legacy 2.x AnnotationList, no
    ///     3.0 equivalent): this is a modern 3.0-native educational narrative - multiple rich,
    ///     multilingual commenting Annotations on a single Canvas, the kind of "curator's tour of
    ///     one object" storytelling pattern seen on museum education sites.
    /// </summary>
    private static Manifest CreateStorytellingAnnotationExample()
    {
        var manifest = new Manifest("https://example.org/demo/storytelling/manifest", new Label("The Painter's Studio: A Closer Look"));
        var canvas = new Canvas("https://example.org/demo/storytelling/canvas/1", new Label("The Painter's Studio"), 4000, 3000);
        canvas.AddAnnotation(new Annotation(
            "https://example.org/demo/storytelling/annotation/painting",
            new ImageResource("https://example.org/demo/storytelling/image.jpg", "image/jpeg").SetHeight(4000).SetWidth(3000),
            canvas.Id));

        var mirrorDetail = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(2400, 900, 500, 700));
        canvas.AddAnnotation(new Annotation("https://example.org/demo/storytelling/annotation/mirror-en",
                new TextualBody("Notice the mirror in the background - a common device artists used to expand the sense of space in the studio.").SetLanguage("en"), mirrorDetail)
            .SetMotivation("commenting"));
        canvas.AddAnnotation(new Annotation("https://example.org/demo/storytelling/annotation/mirror-fr",
                new TextualBody("Remarquez le miroir à l'arrière-plan, un procédé souvent utilisé pour élargir l'espace de l'atelier.").SetLanguage("fr"), mirrorDetail)
            .SetMotivation("commenting"));

        var easelDetail = new AnnotationTarget(canvas.Id).SetSelector(FragmentSelector.ForRegion(500, 1200, 900, 1400));
        canvas.AddAnnotation(new Annotation("https://example.org/demo/storytelling/annotation/easel",
                new TextualBody("The easel and half-finished canvas hint at a work still in progress.").SetLanguage("en"), easelDetail)
            .SetMotivation("describing"));

        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/demo/storytelling/sequence/normal");
        return manifest;
    }

    /// <summary>
    ///     A "guided tour" pattern: an ordered <see cref="Structure" />/Range groups a curated
    ///     subset of canvases (not necessarily every canvas, and not necessarily in reading order)
    ///     into a wayfinding sequence a viewer's UI can step through - distinct from
    ///     <see cref="CreateStorytellingAnnotationExample" />, which is about rich annotation
    ///     *content*, not the tour *structure* itself.
    /// </summary>
    private static Manifest CreateGuidedTourExample()
    {
        var manifest = new Manifest("https://example.org/demo/tour/manifest", new Label("Highlights Tour: Ancient Pottery Collection"));

        var stops = new (string Id, string Label)[]
        {
            ("1", "Stop 1: The Amphora"),
            ("2", "Stop 2: The Krater"),
            ("3", "Stop 3: The Kylix")
        };

        var tour = new Structure("https://example.org/demo/tour/range/highlights", new Label("Highlights Tour"));
        foreach (var (id, label) in stops)
        {
            var canvasId = $"https://example.org/demo/tour/canvas/{id}";
            var canvas = new Canvas(canvasId, new Label(label), 2400, 1800);
            canvas.AddAnnotation(new Annotation($"https://example.org/demo/tour/annotation/{id}",
                new ImageResource($"https://example.org/demo/tour/image-{id}.jpg", "image/jpeg").SetHeight(2400).SetWidth(1800), canvasId));
            manifest.AddItem(canvas);
            tour.AddCanvasReference(canvasId);
        }

        manifest.AddStructure(tour);
        manifest.SetSequenceId("https://example.org/demo/tour/sequence/normal");
        return manifest;
    }

    /// <summary>
    ///     Covers "3D/audio/video/PDF resource references where represented by IIIF structures":
    ///     audio and video paint directly onto their own canvases (native 3.0 body types), while a
    ///     PDF companion document and an (external, non-IIIF) 3D model viewer are linked via
    ///     <c>rendering</c> - the spec-correct way to reference a non-IIIF-native resource format
    ///     from a Manifest, since neither PDF nor 3D mesh formats have a Presentation API body type
    ///     of their own.
    /// </summary>
    private static Manifest CreateMixedMediaObjectExample()
    {
        var manifest = new Manifest("https://example.org/demo/mixed-media/manifest", new Label("Object with Audio Guide, Video Walkthrough, and 3D Model"));

        var imageCanvas = new Canvas("https://example.org/demo/mixed-media/canvas/image", new Label("Photograph"), 3000, 2000);
        imageCanvas.AddAnnotation(new Annotation("https://example.org/demo/mixed-media/annotation/image",
            new ImageResource("https://example.org/demo/mixed-media/photo.jpg", "image/jpeg").SetHeight(3000).SetWidth(2000), imageCanvas.Id));
        manifest.AddItem(imageCanvas);

        var audioCanvas = new Canvas("https://example.org/demo/mixed-media/canvas/audio", new Label("Audio Guide"), 1, 1).SetDuration(180.0);
        audioCanvas.AddAnnotation(new Annotation("https://example.org/demo/mixed-media/annotation/audio",
            new AudioResource("https://example.org/demo/mixed-media/guide.mp3", "audio/mp3").SetDuration(180.0), audioCanvas.Id));
        manifest.AddItem(audioCanvas);

        var videoCanvas = new Canvas("https://example.org/demo/mixed-media/canvas/video", new Label("Video Walkthrough"), 1920, 1080).SetDuration(90.0);
        videoCanvas.AddAnnotation(new Annotation("https://example.org/demo/mixed-media/annotation/video",
            new VideoResource("https://example.org/demo/mixed-media/walkthrough.mp4", "video/mp4").SetHeight(1080).SetWidth(1920).SetDuration(90.0), videoCanvas.Id));
        manifest.AddItem(videoCanvas);

        manifest.AddRendering(new Rendering("https://example.org/demo/mixed-media/catalog-entry.pdf", "Catalog entry (PDF)").SetFormat("application/pdf"));
        manifest.AddRendering(new Rendering("https://example.org/demo/mixed-media/model.glb", "3D model (external viewer)").SetFormat("model/gltf-binary"));

        manifest.SetSequenceId("https://example.org/demo/mixed-media/sequence/normal");
        return manifest;
    }

    private static Manifest CreateMapExample()
    {
        var manifest = new Manifest("https://example.org/demo/map/manifest", new Label("Map Demo"));
        manifest.SetNavPlace(new NavPlace("https://example.org/demo/map/navplace").AddFeature(new Feature("https://example.org/demo/map/feature/1").SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.9857, 40.7484)))
            .SetProperties(new FeatureProperties().AddLabel(new Label("New York")))));
        manifest.AddItem(new Canvas("https://example.org/demo/map/canvas/1", new Label("Map"), 1000, 1000));
        manifest.SetSequenceId("https://example.org/demo/map/sequence/normal");
        return manifest;
    }
}