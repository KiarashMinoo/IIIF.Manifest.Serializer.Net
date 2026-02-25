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

    [JsonConstructor]
    public BaseResource(string id, ResourceType type) : base(id, type.Value)
    {
    }
}

public class BaseResource : BaseResource<BaseResource>
{
    protected internal BaseResource(string id) : base(id)
    {
    }

    [JsonConstructor]
    public BaseResource(string id, ResourceType type) : base(id, type)
    {
    }
}