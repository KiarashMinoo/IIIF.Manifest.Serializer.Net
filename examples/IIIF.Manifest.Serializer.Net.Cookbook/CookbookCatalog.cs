using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Segment.Selector;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
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

public static class CookbookCatalog
{
    public static IReadOnlyList<ExampleDefinition> GetAll() =>
    [
        new ExampleDefinition("Recipe 0001: Simplest Manifest - Single Image File", CreateSimpleImageManifest),
        new ExampleDefinition("Recipe 0002: Simplest Manifest - Single Audio File", CreateSimpleAudioManifest),
        new ExampleDefinition("Recipe: Simplest Manifest - Single Video File", CreateSimpleVideoManifest),
        new ExampleDefinition("Recipe: Basic IIIF Image Service", CreateDeepZoomManifest),
        new ExampleDefinition("Recipe: Simple Book Manifest", CreateBookManifest),
        new ExampleDefinition("Recipe: Book Paging and Viewing Direction", CreateBookPagingManifest),
        new ExampleDefinition("Recipe: Image Thumbnail and Preview", CreatePreviewManifest),
        new ExampleDefinition("Recipe: Audio with Accompanying Image", CreateAccompanyingAudioManifest),
        new ExampleDefinition("Recipe: Manifest Start Canvas", CreateStartCanvasManifest),
        new ExampleDefinition("Recipe: Chronology and NavDate", CreateChronologyManifest),
        new ExampleDefinition("Recipe: Map Location with navPlace", CreateMapManifest),
        new ExampleDefinition("Recipe: Simple Collection", CreateCollectionManifest),
        new ExampleDefinition("Recipe: Table of Contents", CreateTableOfContentsManifest),
        new ExampleDefinition("Recipe: Spatial Region Annotation", CreateSpatialRegionAnnotationList),
        new ExampleDefinition("Recipe: Transcript Annotation List", CreateTranscriptAnnotationList),
        new ExampleDefinition("Recipe: Captions and Subtitles", CreateCaptionAnnotationList),
        new ExampleDefinition("Recipe: Multiple Images on a Canvas", CreateMultipleImagesCanvasManifest),
        new ExampleDefinition("Recipe: Linked Metadata and Presentation Resources", CreateLinkedMetadataManifest),
        new ExampleDefinition("Recipe: Search, Discovery, and Access Control", CreateAccessControlManifest),
        new ExampleDefinition("Recipe: Layer and AnnotationList", CreateLayerExample)
    ];

    private static Manifest CreateSimpleImageManifest()
    {
        var manifest = CreateManifest("https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json", "Single Image Example");
        var canvas = CreateCanvas("https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1", "p. 1", 1800, 1200);
        canvas.AddImage(CreateImage(
            "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
            canvas.Id,
            "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
            "image/png",
            1800,
            1200));

        manifest.AddSequence(new Sequence("https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/s0").AddCanvas(canvas));
        return manifest;
    }

    private static Manifest CreateSimpleAudioManifest()
    {
        var manifest = CreateManifest("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json", "Simplest Audio Example 1");
        var canvas = CreateCanvas("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas", "Mahler, Symphony No. 3: CD 1", 1, 1).SetDuration(1985.024);
        canvas.AddAudio(new Audio(
            "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation",
            new AudioResource("https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4", "audio/mp4").SetDuration(1985.024),
            canvas.Id));

        manifest.AddSequence(new Sequence("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/s0").AddCanvas(canvas));
        return manifest;
    }

    private static Manifest CreateSimpleVideoManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/video/manifest", "Video Example");
        var canvas = CreateCanvas("https://example.org/cookbook/video/canvas", "Video", 720, 1280).SetDuration(60.5);
        canvas.AddVideo(new Video(
            "https://example.org/cookbook/video/annotation/1",
            new VideoResource("https://example.org/cookbook/video.mp4", "video/mp4").SetHeight(720).SetWidth(1280).SetDuration(60.5),
            canvas.Id));

        manifest.AddSequence(new Sequence("https://example.org/cookbook/video/sequence/normal").AddCanvas(canvas));
        return manifest;
    }

    private static Manifest CreateDeepZoomManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/deep-zoom/manifest", "Deep Zoom Example");
        manifest.SetThumbnail(new Thumbnail("https://example.org/cookbook/deep-zoom/thumb.jpg"));

        var canvas = CreateCanvas("https://example.org/cookbook/deep-zoom/canvas", "Deep Zoom", 3000, 2000);
        var image = CreateImage(
            "https://example.org/cookbook/deep-zoom/annotation/1",
            canvas.Id,
            "https://example.org/cookbook/deep-zoom/full/full/0/default.jpg",
            "image/jpeg",
            3000,
            2000);

        var service = new Service(
            "http://iiif.io/api/image/2/context.json",
            "https://example.org/iiif/image-service",
            "http://iiif.io/api/image/2/level2.json")
            .SetHeight(3000)
            .SetWidth(2000)
            .AddTile(new Tile().SetWidth(512).AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4))
            .AddSize(new Size(200, 300))
            .AddSize(new Size(400, 600))
            .SetMaxWidth(2000)
            .SetMaxHeight(3000)
            .SetMaxArea(6000000);

        image.Resource.AddService(service);
        canvas.AddImage(image);
        manifest.AddSequence(new Sequence("https://example.org/cookbook/deep-zoom/sequence/normal").AddCanvas(canvas));
        return manifest;
    }

    private static Manifest CreateBookManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/book/manifest", "Book Example");
        var sequence = new Sequence("https://example.org/cookbook/book/sequence/normal");

        for (var index = 1; index <= 3; index++)
        {
            var canvas = CreateCanvas($"https://example.org/cookbook/book/canvas/p{index}", $"Page {index}", 2000, 1400);
            canvas.AddImage(CreateImage(
                $"https://example.org/cookbook/book/annotation/p{index:D4}-image",
                canvas.Id,
                $"https://example.org/cookbook/book/images/page{index}.jpg",
                "image/jpeg",
                2000,
                1400));
            sequence.AddCanvas(canvas);
        }

        manifest.AddSequence(sequence);
        return manifest;
    }

    private static Manifest CreateBookPagingManifest()
    {
        var manifest = CreateBookManifest();
        manifest.SetViewingDirection(ViewingDirection.Ltr);
        manifest.SetViewingHint(ViewingHint.Paged);
        manifest.Sequences.First().SetViewingDirection(ViewingDirection.Ltr).SetStartCanvas(new StartCanvas("https://example.org/cookbook/book/canvas/p2"));
        return manifest;
    }

    private static Manifest CreatePreviewManifest()
    {
        var manifest = CreateSimpleImageManifest();
        manifest.SetThumbnail(new Thumbnail("https://example.org/cookbook/preview/thumb.jpg"));
        manifest.SetRelated("https://example.org/cookbook/preview/full-view");
        return manifest;
    }

    private static Manifest CreateAccompanyingAudioManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/audio/manifest", "Audio with Accompanying Image");
        var imageCanvas = CreateCanvas("https://example.org/cookbook/audio/canvas/image", "Cover", 1000, 1000);
        imageCanvas.AddImage(CreateImage(
            "https://example.org/cookbook/audio/annotation/image",
            imageCanvas.Id,
            "https://example.org/cookbook/audio/cover.jpg",
            "image/jpeg",
            1000,
            1000));

        var audioCanvas = CreateCanvas("https://example.org/cookbook/audio/canvas/audio", "Audio", 1, 1).SetDuration(180.0);
        audioCanvas.AddAudio(new Audio(
            "https://example.org/cookbook/audio/annotation/audio",
            new AudioResource("https://example.org/cookbook/audio/track.mp3", "audio/mpeg").SetDuration(180.0),
            audioCanvas.Id));

        manifest.SetAccompanyingCanvas(new AccompanyingCanvas(imageCanvas.Id));
        manifest.AddSequence(new Sequence("https://example.org/cookbook/audio/sequence/normal").AddCanvas(audioCanvas).AddCanvas(imageCanvas));
        return manifest;
    }

    private static Manifest CreateStartCanvasManifest()
    {
        var manifest = CreateBookManifest();
        manifest.Sequences.First().SetStartCanvas(new StartCanvas("https://example.org/cookbook/book/canvas/p3"));
        return manifest;
    }

    private static Manifest CreateChronologyManifest()
    {
        var manifest = CreateBookManifest();
        manifest.SetNavDate(new DateTime(1890, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        manifest.AddMetadata(new Metadata("Publication date", "1890", "en"));
        return manifest;
    }

    private static Manifest CreateMapManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/map/manifest", "Map Example");
        manifest.SetNavPlace(
            new NavPlace("https://example.org/cookbook/map/navplace")
                .AddFeature(
                    new Feature("https://example.org/cookbook/map/feature/1")
                        .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(40.7128, -74.0060)))
                        .SetProperties(new FeatureProperties().AddLabel(new Label("New York")))));
        manifest.AddSequence(new Sequence("https://example.org/cookbook/map/sequence/normal").AddCanvas(CreateCanvas("https://example.org/cookbook/map/canvas", "Map", 1000, 1000)));
        return manifest;
    }

    private static Collection CreateCollectionManifest()
    {
        var collection = new Collection("https://example.org/cookbook/collection", new Label("Collection Example"));
        collection.AddManifest("https://example.org/cookbook/book/manifest");
        collection.AddManifest("https://example.org/cookbook/audio/manifest");
        collection.AddCollection(new Collection("https://example.org/cookbook/collection/sub", new Label("Sub Collection")));
        collection.SetViewingHint(ViewingHint.Individuals);
        return collection;
    }

    private static Manifest CreateTableOfContentsManifest()
    {
        var manifest = CreateBookManifest();
        var structure = new Structure("https://example.org/cookbook/book/range/chapters", new Label("Chapters"));
        structure.AddCanvas("https://example.org/cookbook/book/canvas/p1");
        structure.AddCanvas("https://example.org/cookbook/book/canvas/p2");
        structure.AddCanvas("https://example.org/cookbook/book/canvas/p3");
        manifest.AddStructure(structure);
        return manifest;
    }

    private static AnnotationList CreateSpatialRegionAnnotationList()
    {
        var annotationList = new AnnotationList("https://example.org/cookbook/annotation-list/regions", new Label("Regions"));
        annotationList.AddResource(
            new SegmentResource("https://example.org/cookbook/annotation/region-1", ResourceType.Image)
                .SetFull(new BaseResource("https://example.org/cookbook/annotation/region-1/full", ResourceType.Image)));
        return annotationList;
    }

    private static AnnotationList CreateTranscriptAnnotationList()
    {
        var annotationList = new AnnotationList("https://example.org/cookbook/annotation-list/transcript", new Label("Transcript"));
        annotationList.AddResource(new EmbeddedContentResource("<p>Transcript text</p>", "en"));
        return annotationList;
    }

    private static AnnotationList CreateCaptionAnnotationList()
    {
        var annotationList = new AnnotationList("https://example.org/cookbook/annotation-list/captions", new Label("Captions"));
        annotationList.AddResource(new EmbeddedContentResource("WEBVTT\n\n00:00.000 --> 00:05.000\nCaption text", "en"));
        return annotationList;
    }

    private static Manifest CreateMultipleImagesCanvasManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/composite/manifest", "Composite Image Example");
        var canvas = CreateCanvas("https://example.org/cookbook/composite/canvas", "Composite", 1200, 1800);
        canvas.AddImage(CreateImage("https://example.org/cookbook/composite/annotation/1", canvas.Id, "https://example.org/cookbook/composite/image-1.jpg", "image/jpeg", 1200, 900));
        canvas.AddImage(CreateImage("https://example.org/cookbook/composite/annotation/2", canvas.Id, "https://example.org/cookbook/composite/image-2.jpg", "image/jpeg", 1200, 900));
        manifest.AddSequence(new Sequence("https://example.org/cookbook/composite/sequence/normal").AddCanvas(canvas));
        return manifest;
    }

    private static Manifest CreateLinkedMetadataManifest()
    {
        var manifest = CreateManifest("https://example.org/cookbook/metadata/manifest", "Linked Metadata Example");
        manifest.AddMetadata(new Metadata("Author", "Jane Doe", "en"));
        manifest.AddMetadata(new Metadata("Author", "Jean Dupont", "fr"));
        manifest.AddHomepage(new Homepage("https://example.org/cookbook/object", "Object page"));
        manifest.AddRendering(new Rendering("https://example.org/cookbook/object.pdf", "PDF"));
        manifest.SetLicense(new License("https://creativecommons.org/licenses/by/4.0/"));
        manifest.SetRelated("https://example.org/cookbook/related");
        manifest.AddSeeAlso(new SeeAlso("https://example.org/cookbook/record.json").SetFormat("application/json"));
        return manifest;
    }

    private static Manifest CreateAccessControlManifest()
    {
        var manifest = CreateSimpleImageManifest();
        manifest.AddService(new SearchService("http://iiif.io/api/search/2/context.json", "https://example.org/search", "http://iiif.io/api/search/0/search").AddService(new AutoCompleteService("http://iiif.io/api/search/2/context.json", "https://example.org/search/autocomplete", "http://iiif.io/api/search/0/autocomplete")));
        manifest.AddService(new ContentStateService("http://iiif.io/api/content-state/1/context.json", "https://example.org/content-state", "http://iiif.io/api/content-state/v1/state"));
        manifest.AddService(new AuthService2("https://example.org/auth/login", "http://iiif.io/api/auth/2/login").SetLabel("Login").SetHeading("Sign in").SetNote("Restricted content").SetConfirmLabel("Continue"));
        manifest.AddService(new DiscoveryService("http://iiif.io/api/discovery/1/context.json", "https://example.org/discovery", "http://iiif.io/api/discovery/1/search"));
        return manifest;
    }

    private static Layer CreateLayerExample()
    {
        var layer = new Layer("https://example.org/cookbook/layer", new Label("OCR Layer"));
        layer.AddOtherContent("https://example.org/cookbook/annotation-list/transcript");
        return layer;
    }

    private static Manifest CreateManifest(string id, string label)
    {
        return new Manifest(id, new Label(label));
    }

    private static Canvas CreateCanvas(string id, string label, int height, int width)
    {
        return new Canvas(id, new Label(label), height, width);
    }

    private static Image CreateImage(string annotationId, string canvasId, string resourceId, string format, int height, int width)
    {
        var resource = new ImageResource(resourceId, format).SetHeight(height).SetWidth(width);
        return new Image(annotationId, resource, canvasId);
    }
}







