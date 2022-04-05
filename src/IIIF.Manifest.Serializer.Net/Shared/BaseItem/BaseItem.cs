using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared
{
    [JsonConverter(typeof(BaseItemJsonConverter<>))]
    public class BaseItem<TBaseItem> : TrackableObject<TBaseItem> where TBaseItem : BaseItem<TBaseItem>
    {
        public const string ContextJName = "@context";
        public const string IdJName = "@id";
        public const string TypeJName = "@type";
        public const string ServiceJName = "service";

        [JsonProperty(ContextJName)]
        public string Context { get; } = "http://iiif.io/api/presentation/2/context.json";

        [JsonProperty(IdJName)]
        public string Id { get; }

        [JsonProperty(TypeJName)]
        public string Type { get; private set; }

        [JsonProperty(ServiceJName)]
        public Service Service { get; private set; }

        protected internal BaseItem(string id) => Id = id;
        public BaseItem(string id, string type) : this(id) => SetType(type);

        internal TBaseItem SetType(string type) => SetPropertyValue(a => a.Type, type);
        public TBaseItem SetService(Service service) => SetPropertyValue(a => a.Service, service);
    }
}