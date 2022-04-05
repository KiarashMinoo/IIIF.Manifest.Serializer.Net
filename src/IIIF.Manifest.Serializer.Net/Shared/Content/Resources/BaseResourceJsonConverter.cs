namespace IIIF.Manifests.Serializer.Shared
{
    public class BaseResourceJsonConverter<TBaseResource> : FormatableItemJsonConverter<TBaseResource>
        where TBaseResource : BaseResource<TBaseResource>
    {
    }

    public class BaseResourceJsonConverter : BaseResourceJsonConverter<BaseResource>
    {
    }
}