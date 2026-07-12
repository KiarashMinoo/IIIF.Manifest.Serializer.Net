using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Selectors;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources;

/// <summary>
///     A W3C "SpecificResource" used as an <see cref="Nodes.Contents.Annotation.Annotation" />'s
///     <c>body</c> - wraps another resource (<see cref="Source" />, dispatched polymorphically via
///     <see cref="BaseResourceJsonConverter" />, e.g. an <c>ImageResource</c>) together with a
///     <see cref="Selector" /> that crops/selects part of it (cookbook recipe 0299-region: an
///     <c>ImageApiSelector</c> cropping an embedded Image resource). Distinct from
///     <see cref="Nodes.Contents.Annotation.AnnotationTarget" />, which wraps a plain resource
///     *reference* (id/type/partOf) rather than a full embeddable resource.
/// </summary>
[PresentationAPI("3.0")]
public class SpecificResource : TrackableObject<SpecificResource>, IBaseResource
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string SourceJName = "source";
    public const string SelectorJName = "selector";
    public const string StyleClassJName = "styleClass";

    [JsonConstructor]
    public SpecificResource(IBaseResource source)
    {
        Type = "SpecificResource";
        Source = source;
    }

    [JsonProperty(IdJName)]
    public string? Id
    {
        get => GetElementValue(x => x.Id);
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "SpecificResource";
        private set => SetElementValue(value);
    }

    [JsonProperty(SourceJName)]
    public IBaseResource Source
    {
        get => GetElementValue(x => x.Source)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(SelectorJName)]
    public ISelector? Selector
    {
        get => GetElementValue(x => x.Selector);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Unofficial community convention (see cookbook recipe 0045-css) for associating a CSS class
    ///     with this wrapper.
    /// </summary>
    [JsonProperty(StyleClassJName)]
    public string? StyleClass
    {
        get => GetElementValue(x => x.StyleClass);
        private set => SetElementValue(value);
    }

    ResourceType? IBaseResource.Type => new(Type);

    public SpecificResource SetId(string id)
    {
        Id = id;
        return this;
    }

    public SpecificResource SetSelector(ISelector selector)
    {
        Selector = selector;
        return this;
    }

    public SpecificResource SetStyleClass(string styleClass)
    {
        StyleClass = styleClass;
        return this;
    }
}