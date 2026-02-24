using System.Collections.Generic;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

public class BaseItem<TBaseItem> : TrackableObject<TBaseItem>, IContextSupport where TBaseItem : BaseItem<TBaseItem>
{
    public const string DefaultContext = "http://iiif.io/api/presentation/2/context.json";
    public const string ContextJName = "@context";
    public const string IdJName = "@id";
    public const string TypeJName = "@type";
    public const string ServiceJName = "service";

    [JsonProperty(IdJName)]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    string IContextSupport.Context => Context.ElementAt(0);

    [JsonProperty(ContextJName, ItemConverterType = typeof(ObjectArrayJsonConverter))]
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

    [JsonProperty(ServiceJName, ItemConverterType = typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<IBaseService> Service
    {
        get => GetElementValue(x => x.Service) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    protected internal BaseItem(string id)
    {
        Id = id;
        Context = [DefaultContext];
    }

    public BaseItem(string id, string type) : this(id)
    {
        Type = type;
    }

    protected internal BaseItem(string id, string type, string context) : this(id, type)
    {
        Context = [context];
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

    internal TBaseItem SetType(string type)
    {
        Type = type;
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