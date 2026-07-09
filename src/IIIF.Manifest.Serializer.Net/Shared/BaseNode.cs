using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

public class BaseNode<TBaseNode> : BaseItem<TBaseNode> where TBaseNode : BaseNode<TBaseNode>
{
    public const string LabelJName = "label";
    public const string DescriptionJName = "description";
    public const string MetadataJName = "metadata";
    public const string AttributionJName = "attribution";
    public const string LogoJName = "logo";
    public const string ThumbnailJName = "thumbnail";
    public const string LicenseJName = "license";
    public const string ViewingHintJName = "viewingHint";
    public const string RenderingJName = "rendering";
    public const string WithinJName = "within";
    public const string SeeAlsoJName = "seeAlso";
    public const string HomepageJName = "homepage";
    public const string ProviderJName = "provider";
    public const string AccompanyingCanvasJName = "accompanyingCanvas";
    public const string BehaviorJName = "behavior";
    public const string RelatedJName = "related";
    public const string ItemsJName = "items";

    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(DescriptionJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Description> Description
    {
        get => GetElementValue(x => x.Description) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(MetadataJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Metadata> Metadata
    {
        get => GetElementValue(x => x.Metadata) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(AttributionJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Attribution> Attribution
    {
        get => GetElementValue(x => x.Attribution) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(LogoJName)]
    public Logo? Logo
    {
        get => GetElementValue(x => x.Logo);
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(ThumbnailJName)]
    public Thumbnail? Thumbnail
    {
        get => GetElementValue(x => x.Thumbnail);
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(LicenseJName)]
    public License? License
    {
        get => GetElementValue(x => x.License);
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "behavior")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Replaced by behavior.")]
    [JsonProperty(ViewingHintJName)]
    public ViewingHint? ViewingHint
    {
        get => GetElementValue(x => x.ViewingHint);
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(RenderingJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Rendering> Rendering
    {
        get => GetElementValue(x => x.Rendering) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(WithinJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Within> Within
    {
        get => GetElementValue(x => x.Within) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(SeeAlsoJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<SeeAlso> SeeAlso
    {
        get => GetElementValue(x => x.SeeAlso) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(HomepageJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Homepage> Homepage
    {
        get => GetElementValue(x => x.Homepage) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(ProviderJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Provider> Provider
    {
        get => GetElementValue(x => x.Provider) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(AccompanyingCanvasJName)]
    public AccompanyingCanvas? AccompanyingCanvas
    {
        get => GetElementValue(x => x.AccompanyingCanvas);
        private set => SetElementValue(value);
    }

    [PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x. Some values also valid in 2.x as viewingHint.")]
    [JsonProperty(BehaviorJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Behavior> Behavior
    {
        get => GetElementValue(x => x.Behavior) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(RelatedJName)]
    public string? Related
    {
        get => GetElementValue(x => x.Related);
        private set => SetElementValue(value);
    }

    [JsonProperty(ItemsJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<IBaseItem> Items
    {
        get => GetElementValue(x => x.Items) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    protected internal BaseNode(string id) : base(id)
    {
    }

    public BaseNode(string id, string type) : base(id, type)
    {
    }

    public TBaseNode SetLabel(IReadOnlyCollection<Label> labels)
    {
        Label = labels;
        return (TBaseNode)this;
    }

    public TBaseNode AddLabel(Label label)
    {
        return SetLabel(Label.With(label));
    }

    public TBaseNode RemoveLabel(Label label)
    {
        return SetLabel(Label.Without(label));
    }

    public TBaseNode AddDescription(Description description)
    {
        Description = Description.With(description);
        return (TBaseNode)this;
    }

    public TBaseNode RemoveDescription(Description description)
    {
        Description = Description.Without(description);
        return (TBaseNode)this;
    }

    public TBaseNode AddMetadata(Metadata metadata)
    {
        Metadata = Metadata.With(metadata);
        return (TBaseNode)this;
    }

    public TBaseNode RemoveMetadata(Metadata metadata)
    {
        Metadata = Metadata.Without(metadata);
        return (TBaseNode)this;
    }

    public TBaseNode AddAttribution(Attribution attribution) => SetElementValue(a => a.Attribution, (collection) => collection.With(attribution));
    public TBaseNode RemoveAttribution(Attribution attribution) => SetElementValue(a => a.Attribution, (collection) => collection.Without(attribution));

    public TBaseNode AddSeeAlso(SeeAlso seeAlso) => SetElementValue(a => a.SeeAlso, (collection) => collection.With(seeAlso));
    public TBaseNode RemoveSeeAlso(SeeAlso seeAlso) => SetElementValue(a => a.SeeAlso, (collection) => collection.Without(seeAlso));

    public TBaseNode AddRendering(Rendering rendering) => SetElementValue(a => a.Rendering, (collection) => collection.With(rendering));
    public TBaseNode RemoveRendering(Rendering rendering) => SetElementValue(a => a.Rendering, (collection) => collection.Without(rendering));

    public TBaseNode AddHomepage(Homepage homepage) => SetElementValue(a => a.Homepage, (collection) => collection.With(homepage));
    public TBaseNode RemoveHomepage(Homepage homepage) => SetElementValue(a => a.Homepage, (collection) => collection.Without(homepage));

    public TBaseNode AddProvider(Provider provider) => SetElementValue(a => a.Provider, (collection) => collection.With(provider));
    public TBaseNode RemoveProvider(Provider provider) => SetElementValue(a => a.Provider, (collection) => collection.Without(provider));

    public TBaseNode AddBehavior(Behavior behavior) => SetElementValue(a => a.Behavior, (collection) => collection.With(behavior));
    public TBaseNode RemoveBehavior(Behavior behavior) => SetElementValue(a => a.Behavior, (collection) => collection.Without(behavior));

    public TBaseNode AddWithin(Within within) => SetElementValue(a => a.Within, (collection) => collection.With(within));
    public TBaseNode RemoveWithin(Within within) => SetElementValue(a => a.Within, (collection) => collection.Without(within));

    public TBaseNode SetAccompanyingCanvas(AccompanyingCanvas accompanyingCanvas) => SetElementValue(a => a.AccompanyingCanvas, accompanyingCanvas);
    public TBaseNode SetLogo(Logo logo) => SetElementValue(a => a.Logo, logo);
    public TBaseNode SetThumbnail(Thumbnail thumbnail) => SetElementValue(a => a.Thumbnail, thumbnail);
    public TBaseNode SetLicense(License license) => SetElementValue(a => a.License, license);

    [Obsolete("Deprecated in IIIF Presentation API 3.0. Replaced by behavior.")]
    internal TBaseNode SetViewingHint(ViewingHint viewingHint) => SetElementValue(a => a.ViewingHint, viewingHint);

    public TBaseNode SetRelated(string related) => SetElementValue(a => a.Related, related);

    //Items

    public TBaseNode SetItems(IReadOnlyCollection<IBaseItem> items)
    {
        Items = [..items];
        return (TBaseNode)this;
    }

    public TBaseNode SetItem<TItem>(TItem item) where TItem : IBaseItem
    {
        Items = [item];
        return (TBaseNode)this;
    }

    public TBaseNode AddItem<TItem>(TItem item) where TItem : IBaseItem
    {
        Items = Items.With(item);
        return (TBaseNode)this;
    }

    public TBaseNode RemoveItem<TItem>(TItem item) where TItem : IBaseItem
    {
        Items = Items.Without(item);
        return (TBaseNode)this;
    }
}


