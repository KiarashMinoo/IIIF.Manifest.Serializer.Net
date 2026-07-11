using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Selectors;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation;

/// <summary>
/// An <see cref="Annotation"/>'s target - the resource (and optionally a specific part of it) the
/// annotation applies to. Serializes as one of the shapes the W3C Annotation Model allows (chosen
/// automatically by <see cref="AnnotationTargetJsonConverter"/>): a bare URI string, a typed
/// resource reference (optionally with a single <see cref="PartOfId"/>), or a full SpecificResource
/// wrapping a <see cref="Selector"/>. Implicitly convertible from <c>string</c> so every existing
/// bare-URI call site keeps compiling unchanged.
/// </summary>
[PresentationAPI("3.0")]
[JsonConverter(typeof(AnnotationTargetJsonConverter))]
public class AnnotationTarget : TrackableObject<AnnotationTarget>
{
    public string SourceId
    {
        get => GetElementValue(x => x.SourceId)!;
        private set => SetElementValue(value);
    }

    public string? SourceType
    {
        get => GetElementValue(x => x.SourceType);
        private set => SetElementValue(value);
    }

    public string? PartOfId
    {
        get => GetElementValue(x => x.PartOfId);
        private set => SetElementValue(value);
    }

    public string? PartOfType
    {
        get => GetElementValue(x => x.PartOfType);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// The SpecificResource wrapper's own id - only meaningful when <see cref="Selector"/> is set.
    /// </summary>
    public string? SpecificResourceId
    {
        get => GetElementValue(x => x.SpecificResourceId);
        private set => SetElementValue(value);
    }

    public ISelector? Selector
    {
        get => GetElementValue(x => x.Selector);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Unofficial community convention (see cookbook recipe 0045-css) for associating a CSS class
    /// with the SpecificResource wrapper.
    /// </summary>
    public string? StyleClass
    {
        get => GetElementValue(x => x.StyleClass);
        private set => SetElementValue(value);
    }

    public AnnotationTarget(string sourceId, string? sourceType = null)
    {
        SourceId = sourceId;
        SourceType = sourceType;
    }

    public static implicit operator AnnotationTarget(string uri) => new(uri);

    public AnnotationTarget SetSelector(ISelector selector)
    {
        Selector = selector;
        return this;
    }

    public AnnotationTarget SetPartOf(string partOfId, string partOfType = "Manifest")
    {
        PartOfId = partOfId;
        PartOfType = partOfType;
        return this;
    }

    public AnnotationTarget SetSpecificResourceId(string specificResourceId)
    {
        SpecificResourceId = specificResourceId;
        return this;
    }

    public AnnotationTarget SetStyleClass(string styleClass)
    {
        StyleClass = styleClass;
        return this;
    }

    public override string ToString() => SourceId;
}
