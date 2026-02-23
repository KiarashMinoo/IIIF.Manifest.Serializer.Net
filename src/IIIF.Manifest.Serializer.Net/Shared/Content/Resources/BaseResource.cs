using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources
{
    public class BaseResource<TBaseResource> : FormattableItem<TBaseResource> where TBaseResource : BaseResource<TBaseResource>
    {
        protected internal BaseResource(string id) : base(id)
        {
        }

        [JsonConstructor]
        public BaseResource(string id, string type) : base(id, type)
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
    }
}