using IIIF.Manifests.Serializer.Shared.Service;
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

        [JsonProperty(ContextJName)] public string Context => GetElementValue(a => a.Context) ?? DefaultContext;

        [JsonProperty(IdJName)] public string Id => GetElementValue(a => a.Id)!;

        [JsonProperty(TypeJName)] public string? Type => GetElementValue(x => x.Type);

        [JsonProperty(ServiceJName)] public IBaseService? Service => GetElementValue(x => x.Service);

        protected internal BaseItem(string id)
        {
            SetElementValue(x => x.Id, id);
            SetElementValue(x => x.Context, DefaultContext);
        }

        public BaseItem(string id, string type) : this(id) => SetType(type);

        protected internal BaseItem(string id, string type, string context) : this(id, type)
        {
            SetElementValue(x => x.Context, context);
        }

        internal TBaseItem SetType(string type) => SetElementValue(a => a.Type, type);

        public TBaseItem SetService<TService>(TService service) where TService : IBaseService
        {
            return SetElementValue(a => a.Service, service);
        }
    }
}