using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.OtherContent;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes;

/// <summary>
///     IIIF Canvas resource - a virtual container representing a page or view.
/// </summary>
[PresentationAPI("2.0", Notes = "Core resource. In 3.0, images/otherContent replaced by items (AnnotationPage) / annotations.")]
public class Canvas : BaseNode<Canvas>, IDimensionSupport<Canvas>
{
    public const string ImagesJName = "images";
    public const string OtherContentsJName = "otherContent";
    public const string DurationJName = "duration";
    public const string AnnotationsJName = "annotations";
    public const string PlaceholderCanvasJName = "placeholderCanvas";

    public Canvas(string id, Label label, int height, int width) : base(id, "sc:Canvas")
    {
        AddLabel(label);
        Height = height;
        Width = width;
    }

    /// <summary>
    ///     Duration in seconds for time-based media (A/V content).
    /// </summary>
    [PresentationAPI("2.1", Notes = "Added in 2.1 for A/V support. Also in 3.0.")]
    [JsonProperty(DurationJName)]
    public double? Duration
    {
        get => GetElementValue(x => x.Duration);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Legacy (2.x) view of the painting annotations on this Canvas whose body is an image resource.
    ///     Computed from <see cref="BaseNode{TBaseNode}.Items" /> (the 3.0-native storage); not itself the
    ///     backing store. Prefer <see cref="AddAnnotation" /> for new code.
    /// </summary>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [JsonProperty(ImagesJName)]
    public IReadOnlyCollection<Image> Images
    {
        get => GetPaintingAnnotations().Where(x => x.Body is ImageResource).Select(ToImage).ToList();
        private set => ReplacePaintingAnnotations(x => x.Body is ImageResource, (value ?? []).Select(ToAnnotation));
    }

    /// <summary>
    ///     Legacy (2.x) view of the painting annotations on this Canvas whose body is an audio resource.
    ///     Computed from <see cref="BaseNode{TBaseNode}.Items" /> (the 3.0-native storage); not itself the
    ///     backing store. Prefer <see cref="AddAnnotation" /> for new code.
    /// </summary>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [JsonProperty(nameof(Audios))]
    public IReadOnlyCollection<Audio> Audios
    {
        get => GetPaintingAnnotations().Where(x => x.Body is AudioResource).Select(ToAudio).ToList();
        private set => ReplacePaintingAnnotations(x => x.Body is AudioResource, (value ?? []).Select(ToAnnotation));
    }

    /// <summary>
    ///     Legacy (2.x) view of the painting annotations on this Canvas whose body is a video resource.
    ///     Computed from <see cref="BaseNode{TBaseNode}.Items" /> (the 3.0-native storage); not itself the
    ///     backing store. Prefer <see cref="AddAnnotation" /> for new code.
    /// </summary>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [JsonProperty(nameof(Videos))]
    public IReadOnlyCollection<Video> Videos
    {
        get => GetPaintingAnnotations().Where(x => x.Body is VideoResource).Select(ToVideo).ToList();
        private set => ReplacePaintingAnnotations(x => x.Body is VideoResource, (value ?? []).Select(ToAnnotation));
    }

    /// <summary>
    ///     3.0-native reference to external annotation lists on this Canvas. Replaces 2.x otherContent.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces otherContent from API 2.x.")]
    [JsonProperty(AnnotationsJName)]
    public IReadOnlyCollection<AnnotationPage> Annotations
    {
        get => GetElementValue(x => x.Annotations) ?? [];
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Legacy (2.x) view of external annotation list references. Computed from <see cref="Annotations" />.
    /// </summary>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "annotations")]
    [JsonProperty(OtherContentsJName)]
    public IReadOnlyCollection<OtherContent> OtherContents
    {
        get => Annotations.Select(ToOtherContent).ToList();
        private set => Annotations = (value ?? []).Select(ToAnnotationPageReference).ToList();
    }

    /// <summary>
    ///     A full Canvas shown before this Canvas's own content is available/rendered - e.g. a
    ///     poster frame for a video (cookbook recipe 0013-placeholderCanvas). 3.0-only; there is
    ///     no 2.x equivalent shape.
    /// </summary>
    [PresentationAPI("3.0")]
    [JsonProperty(PlaceholderCanvasJName)]
    public Canvas? PlaceholderCanvas
    {
        get => GetElementValue(x => x.PlaceholderCanvas);
        private set => SetElementValue(value);
    }

    private AnnotationPage? PrimaryAnnotationPage => Items.OfType<AnnotationPage>().FirstOrDefault();

    [PresentationAPI("2.0")]
    [JsonProperty(Constants.HeightJName)]
    public int? Height
    {
        get => GetElementValue(x => x.Height);
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(Constants.WidthJName)]
    public int? Width
    {
        get => GetElementValue(x => x.Width);
        private set => SetElementValue(value);
    }

    public Canvas SetPlaceholderCanvas(Canvas placeholderCanvas)
    {
        PlaceholderCanvas = placeholderCanvas;
        return this;
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct an Annotation with an ImageResource body and use AddAnnotation instead.")]
    public Canvas AddImage(Image image)
    {
        AddAnnotationCore(ToAnnotation(image));
        return this;
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct an Annotation with an AudioResource body and use AddAnnotation instead.")]
    public Canvas AddAudio(Audio audio)
    {
        AddAnnotationCore(ToAnnotation(audio));
        return this;
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct an Annotation with a VideoResource body and use AddAnnotation instead.")]
    public Canvas AddVideo(Video video)
    {
        AddAnnotationCore(ToAnnotation(video));
        return this;
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "annotations")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddAnnotationPageReference instead.")]
    public Canvas AddOtherContent(OtherContent otherContent)
    {
        Annotations = Annotations.With(ToAnnotationPageReference(otherContent));
        return this;
    }

    public Canvas SetDuration(double duration)
    {
        Duration = duration;
        return this;
    }

    /// <summary>
    ///     Adds a Web Annotation (e.g. painting an Image/Audio/Video resource) to this Canvas's primary
    ///     AnnotationPage. This is the 3.0-preferred way to attach content to a Canvas.
    /// </summary>
    public Canvas AddAnnotation(Annotation annotation)
    {
        AddAnnotationCore(annotation);
        return this;
    }

    /// <summary>
    ///     Adds a reference to an external AnnotationPage (3.0 replacement for otherContent).
    /// </summary>
    public Canvas AddAnnotationPageReference(AnnotationPage annotationPage)
    {
        Annotations = Annotations.With(annotationPage);
        return this;
    }

    private IEnumerable<Annotation> GetPaintingAnnotations()
    {
        return PrimaryAnnotationPage?.Items.OfType<Annotation>() ?? [];
    }

    private void AddAnnotationCore(Annotation annotation)
    {
        ReplacePaintingAnnotations(x => x.Id == annotation.Id, [annotation]);
    }

    private void ReplacePaintingAnnotations(Func<Annotation, bool> matches, IEnumerable<Annotation> replacements)
    {
        var existingPage = PrimaryAnnotationPage;
        var kept = existingPage?.Items.OfType<Annotation>().Where(x => !matches(x)) ?? [];
        var merged = kept.Concat(replacements).ToList();

        if (merged.Count == 0)
        {
            if (existingPage is not null) RemoveItem(existingPage);

            return;
        }

        var page = existingPage ?? new AnnotationPage($"{Id}/page");
        page.SetItems(merged);

        if (existingPage is null) AddItem(page);
    }

    private static Image ToImage(Annotation annotation)
    {
        return new Image(annotation.Id, (ImageResource)annotation.Body, annotation.Target.SourceId);
    }

    private static Audio ToAudio(Annotation annotation)
    {
        return new Audio(annotation.Id, (AudioResource)annotation.Body, annotation.Target.SourceId);
    }

    private static Video ToVideo(Annotation annotation)
    {
        return new Video(annotation.Id, (VideoResource)annotation.Body, annotation.Target.SourceId);
    }

    private static OtherContent ToOtherContent(AnnotationPage page)
    {
        return new OtherContent(page.Id);
    }

    private static Annotation ToAnnotation(Image image)
    {
        return new Annotation(image.Id, image.Resource, image.On);
    }

    private static Annotation ToAnnotation(Audio audio)
    {
        return new Annotation(audio.Id, audio.Resource, audio.On);
    }

    private static Annotation ToAnnotation(Video video)
    {
        return new Annotation(video.Id, video.Resource, video.On);
    }

    private static AnnotationPage ToAnnotationPageReference(OtherContent otherContent)
    {
        return new AnnotationPage(otherContent.Id);
    }
}