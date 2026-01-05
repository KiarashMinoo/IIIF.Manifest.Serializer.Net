using IIIF.Manifests.Serializer.Shared.FormatableItem;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources
{
    public class BaseResourceJsonConverter<TBaseResource> : FormatableItemJsonConverter<TBaseResource>
        where TBaseResource : BaseResource<TBaseResource>
    {
    }

    public class BaseResourceJsonConverter : BaseResourceJsonConverter<BaseResource>
    {
    }
}