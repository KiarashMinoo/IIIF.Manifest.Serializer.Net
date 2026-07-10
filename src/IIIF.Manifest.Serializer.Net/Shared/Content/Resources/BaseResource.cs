using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources;

public interface IBaseResource
{
    ResourceType? Type { get; }
}

public class BaseResource<TBaseResource> : FormattableItem<TBaseResource>, IBaseResource where TBaseResource : BaseResource<TBaseResource>
{
    ResourceType? IBaseResource.Type => !string.IsNullOrWhiteSpace(base.Type) ? new ResourceType(base.Type) : null;

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