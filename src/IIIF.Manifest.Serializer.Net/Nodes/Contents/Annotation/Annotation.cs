using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation;

/// <summary>
///     IIIF Presentation API 3.0 Web Annotation. Represents a single annotation on a Canvas
///     (typically painting an Image/Audio/Video resource). This is the 3.0-native replacement for
///     the 2.x Image/Audio/Video/OtherContent annotation wrapper classes.
/// </summary>
[PresentationAPI("3.0", Notes = "Web Annotation model. In 2.x, this concept was split across Canvas.images/audio/video wrappers.")]
public class Annotation : BaseItem<Annotation>
{
    public const string MotivationJName = "motivation";
    public const string BodyJName = "body";
    public const string TargetJName = "target";
    public const string StylesheetJName = "stylesheet";
    public const string TimeModeJName = "timeMode";

    // Newtonsoft's constructor-parameter matching binds by wire-name first ("target" is
    // Targets' JsonProperty name), so the [JsonConstructor] parameter must be the plural
    // collection type to match what it actually resolves - a scalar-typed parameter named
    // "target" here caused Newtonsoft to pass a deserialized List<AnnotationTarget> into a
    // parameter expecting a single AnnotationTarget, corrupting every plain JsonConvert round
    // trip of any Annotation (a real bug caught by the full test suite, not reasoning alone).
    [JsonConstructor]
    private Annotation(string id, IReadOnlyCollection<IBaseResource> body, IReadOnlyCollection<AnnotationTarget> target) : base(id, "Annotation")
    {
        Bodies = body;
        Targets = target;
        Motivation = "painting";
    }

    public Annotation(string id, IBaseResource body, AnnotationTarget target)
        : this(id, (IReadOnlyCollection<IBaseResource>)[body], (IReadOnlyCollection<AnnotationTarget>)[target])
    {
    }

    [JsonProperty(MotivationJName)]
    public string Motivation
    {
        get => GetElementValue(x => x.Motivation) ?? "painting";
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     3.0-native storage - the W3C Annotation Model allows <c>body</c> to be a single value
    ///     or an array (cookbook recipes 0022/0103/0258/0377: a comment paired with a linked
    ///     resource, or an image plus text overlay, as sibling bodies rather than Choice
    ///     alternatives). <see cref="Body" /> is a computed single-value convenience over this
    ///     collection.
    /// </summary>
    [JsonProperty(BodyJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<IBaseResource> Bodies
    {
        get => GetElementValue(x => x.Bodies) ?? [];
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     The first (and typically only) body. Setting this replaces the entire
    ///     <see cref="Bodies" /> collection with a single entry - use <see cref="AddBody" /> to
    ///     build a multi-body annotation instead.
    /// </summary>
    [JsonIgnore]
    public IBaseResource Body
    {
        get => Bodies.First();
        private set => Bodies = [value];
    }

    /// <summary>
    ///     3.0-native storage - the W3C Annotation Model allows <c>target</c> to be a single value
    ///     or an array (cookbook recipes 0540/0599: a link annotation opening several canvases at
    ///     once). <see cref="Target" /> is a computed single-value convenience over this collection.
    /// </summary>
    [JsonProperty(TargetJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<AnnotationTarget> Targets
    {
        get => GetElementValue(x => x.Targets) ?? [];
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     The first (and typically only) target. Setting this replaces the entire
    ///     <see cref="Targets" /> collection with a single entry - use <see cref="AddTarget" /> to
    ///     build a multi-target annotation instead.
    /// </summary>
    [JsonIgnore]
    public AnnotationTarget Target
    {
        get => Targets.First();
        private set => Targets = [value];
    }

    /// <summary>
    ///     Unofficial community convention (see cookbook recipe 0045-css) linking an external CSS
    ///     stylesheet whose classes the annotation's/its resources' <c>styleClass</c> refer to.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Unofficial CSS-extension convention, cookbook recipe 0045.")]
    [JsonProperty(StylesheetJName)]
    public string? Stylesheet
    {
        get => GetElementValue(x => x.Stylesheet);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Spec §4.5 - how a temporal-media body's own duration relates to its target's duration
    ///     when the two differ (trim/scale/loop). No cookbook recipe exercises this directly, but
    ///     it is a genuine Presentation 3.0 Annotation property, not extension-territory.
    /// </summary>
    [PresentationAPI("3.0")]
    [JsonProperty(TimeModeJName)]
    public TimeMode? TimeMode
    {
        get => GetElementValue(x => x.TimeMode);
        private set => SetElementValue(value);
    }

    public Annotation SetMotivation(string motivation)
    {
        Motivation = motivation;
        return this;
    }

    public Annotation AddTarget(AnnotationTarget target)
    {
        Targets = Targets.With(target);
        return this;
    }

    public Annotation AddBody(IBaseResource body)
    {
        Bodies = Bodies.With(body);
        return this;
    }

    public Annotation SetStylesheet(string stylesheet)
    {
        Stylesheet = stylesheet;
        return this;
    }

    public Annotation SetTimeMode(TimeMode timeMode)
    {
        TimeMode = timeMode;
        return this;
    }
}