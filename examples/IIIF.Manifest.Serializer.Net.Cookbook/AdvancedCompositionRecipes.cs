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
using static IIIF.Manifests.Serializer.Net.Cookbook.RecipeBuilders;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
/// Recipes 0377-0599: multi-body annotations (image detail plus text), a Choice of
/// audio/video formats, reusing another Manifest's Canvas verbatim, standalone Content State
/// documents (a canvas region, opening multiple canvases, drag-and-drop), a multimedia Canvas
/// combining image/video/text at different times and regions, timeline-sequenced resources with
/// a repeat behavior, and visible text painted directly onto an image.
/// </summary>
internal sealed class AdvancedCompositionRecipes : IRecipeSet
{
    public IEnumerable<ExampleDefinition> GetRecipes() =>
    [
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
}
