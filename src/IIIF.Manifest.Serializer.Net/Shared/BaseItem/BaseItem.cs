using IIIF.Manifests.Serializer.Properties.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.BaseItem
{
    [JsonConverter(typeof(BaseItemJsonConverter<>))]
    public class BaseItem<TBaseItem> : TrackableObject<TBaseItem> where TBaseItem : BaseItem<TBaseItem>
    {
        public const string DefaultContext = "http://iiif.io/api/presentation/2/context.json";
        public const string ContextJName = "@context";
        public const string IdJName = "@id";
        public const string TypeJName = "@type";
        public const string ServiceJName = "service";

        [JsonProperty(ContextJName)]
        public string Context { get; private set; }

        [JsonProperty(IdJName)]
        public string Id { get; }

        [JsonProperty(TypeJName)]
        public string Type { get; private set; }

        [JsonProperty(ServiceJName)]
        public Service Service { get; private set; }

        protected internal BaseItem(string id)
        {
            Id = id;
            Context = DefaultContext;
        }

        public BaseItem(string id, string type) : this(id) => SetType(type);

        protected internal BaseItem(string id, string type, string context) : this(id, type) => Context = context;

        internal TBaseItem SetType(string type) => SetPropertyValue(a => a.Type, type);
        public TBaseItem SetService(Service service) => SetPropertyValue(a => a.Service, service);
    }
}