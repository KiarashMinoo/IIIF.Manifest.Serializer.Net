using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

/// <summary>
/// Base type for services defined by specs that postdate the Presentation 3.0 "no @ prefix" convention
/// (Auth 2.0, Content Search 2.0, Change Discovery 1.0, Content State 1.0) - unlike <see cref="BaseItem{TBaseItem}"/>,
/// whose @id/@type prefixing matches Presentation 2.x/3.0 resources that exist in both conventions.
/// </summary>
public class UnprefixedBaseItem<TBaseItem> : TrackableObject<TBaseItem>, IBaseItem, IContextSupport
    where TBaseItem : UnprefixedBaseItem<TBaseItem>
{
    public const string DefaultContext = "http://iiif.io/api/presentation/3/context.json";
    public const string ContextJName = "@context";
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string ServiceJName = "service";

    [JsonProperty(IdJName)]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    string IContextSupport.Context => Context.ElementAt(0);

    [JsonProperty(ContextJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<string> Context
    {
        get => GetElementValue(x => x.Context) ?? [DefaultContext];
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string? Type
    {
        get => GetElementValue(x => x.Type);
        private set => SetElementValue(value);
    }

    [JsonProperty(ServiceJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<IBaseService> Service
    {
        get => GetElementValue(x => x.Service) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    protected internal UnprefixedBaseItem(string? id)
    {
        // Nullable id: Auth 2.0's AuthAccessService2 must omit id entirely for the "external"
        // profile - SetElementValue already treats a null value as "no element", so Id simply
        // reads back as null (and NullValueHandling.Ignore omits it on write) rather than throwing.
        Id = id!;
        Context = [DefaultContext];
    }

    public UnprefixedBaseItem(string? id, string type) : this(id)
    {
        Type = type;
    }

    protected internal UnprefixedBaseItem(string? id, string type, string context) : this(id, type)
    {
        Context = [context];
    }

    internal TBaseItem SetType(string type)
    {
        Type = type;
        return (TBaseItem)this;
    }

    public TBaseItem SetContext(IReadOnlyCollection<string> context)
    {
        Context = context;
        return (TBaseItem)this;
    }

    public TBaseItem SetContext(string context)
    {
        return SetContext([context]);
    }

    public TBaseItem AddContext(string context)
    {
        return SetContext(Context.With(context));
    }

    public TBaseItem RemoveContext(string context)
    {
        return SetContext(Context.Without(context));
    }

    public TBaseItem SetService(IReadOnlyCollection<IBaseService> services)
    {
        Service = [..services];
        return (TBaseItem)this;
    }

    public TBaseItem SetService<TService>(TService service) where TService : IBaseService
    {
        Service = [service];
        return (TBaseItem)this;
    }

    public TBaseItem AddService<TService>(TService service) where TService : IBaseService
    {
        Service = Service.With(service);
        return (TBaseItem)this;
    }

    public TBaseItem RemoveService<TService>(TService service) where TService : IBaseService
    {
        Service = Service.Without(service);
        return (TBaseItem)this;
    }
}
