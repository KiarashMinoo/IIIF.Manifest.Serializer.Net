using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources;

[JsonConverter(typeof(BaseResourceJsonConverter))]
public interface IBaseResource
{
    ResourceType? Type { get; }

    /// <summary>
    ///     Default empty for resources with no service concept (TextualBody, Choice,
    ///     SpecificResource) - automatically satisfied without any code change by every
    ///     <see cref="BaseResource{TBaseResource}" />-derived type, which already exposes this exact
    ///     member via its inherited <c>BaseItem.Service</c>. Lets IiifSerializer's hand-rolled
    ///     WriteV3Resource rebuild a spec-correct, always-array "service" regardless of which concrete
    ///     resource type it's writing, without a per-type switch.
    /// </summary>
    IReadOnlyCollection<IBaseService> Service => [];
}

public class BaseResource<TBaseResource> : FormattableItem<TBaseResource>, IBaseResource where TBaseResource : BaseResource<TBaseResource>
{
    public const string LabelJName = "label";

    protected internal BaseResource(string id) : base(id)
    {
    }

    // Newtonsoft's constructor-parameter matching binds a raw JSON string value directly to this
    // ctor without applying ResourceType's own JsonConverter, so a ResourceType-typed
    // JsonConstructor throws InvalidCastException when deserializing a bare BaseResource. Take
    // the raw string instead (matching how the inherited "@type" property is itself a string)
    // and keep the ResourceType overload as a plain, non-JsonConstructor convenience.
    [JsonConstructor]
    public BaseResource(string id, string type) : base(id, type)
    {
    }

    public BaseResource(string id, ResourceType type) : this(id, type.Value)
    {
    }

    // Per spec §3 Structural Properties table, a Content resource (Image/Sound/Video/Text) may
    // carry its own "label" - used e.g. by Choice-body items to name each alternative
    // (cookbook recipes 0033-choice/0434-choice-av, "Natural Light" vs "X-Ray", "MP3" vs "FLAC").
    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    ResourceType? IBaseResource.Type => !string.IsNullOrWhiteSpace(base.Type) ? new ResourceType(base.Type) : null;

    public TBaseResource SetLabel(IReadOnlyCollection<Label> labels)
    {
        Label = labels;
        return (TBaseResource)this;
    }

    public TBaseResource SetLabel(Label label)
    {
        return SetLabel([label]);
    }
}

public class BaseResource : BaseResource<BaseResource>
{
    protected internal BaseResource(string id) : base(id)
    {
    }

    [JsonConstructor]
    public BaseResource(string id, string type) : base(id, type)
    {
    }

    public BaseResource(string id, ResourceType type) : base(id, type)
    {
    }
}