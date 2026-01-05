using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources
{
    [JsonConverter(typeof(BaseResourceJsonConverter<>))]
    public class BaseResource<TBaseResource> : FormatableItem<TBaseResource> where TBaseResource : BaseResource<TBaseResource>
    {
        protected internal BaseResource(string id) : base(id)
        {
        }

        public BaseResource(string id, string type) : base(id, type)
        {
        }
    }

    [JsonConverter(typeof(BaseResourceJsonConverter))]
    public class BaseResource : BaseResource<BaseResource>
    {
        protected internal BaseResource(string id) : base(id)
        {
        }

        public BaseResource(string id, string type) : base(id, type)
        {
        }
    }
}