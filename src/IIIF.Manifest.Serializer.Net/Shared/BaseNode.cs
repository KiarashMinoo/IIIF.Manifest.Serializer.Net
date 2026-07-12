using System;
using System.Linq;
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

    /// <summary>
    /// Legacy (2.x) view of <see cref="Summary"/> (the 3.0-native storage) - a straight rename,
    /// unlike <see cref="Attribution"/>/<see cref="RequiredStatement"/> which also restructured.
    /// </summary>
    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "summary")]
    [JsonProperty(DescriptionJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Description> Description
    {
        get => Summary;
        private set => Summary = value;
    }

    /// <summary>
    /// 3.0-native replacement for <see cref="Description"/>. Deliberately <see cref="JsonIgnoreAttribute"/>d,
    /// same reasoning as <see cref="RequiredStatement"/>: the hand-built V3 reader/writer in
    /// IiifSerializer already reads/writes it explicitly.
    /// </summary>
    public const string SummaryJName = "summary";

    [PresentationAPI("3.0", Notes = "Replaces description from API 2.x. Straight rename, no structural change.")]
    [JsonIgnore]
    public IReadOnlyCollection<Description> Summary
    {
        get => GetElementValue(x => x.Summary) ?? [];
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

    /// <summary>
    /// Legacy (2.x) view of the attribution text. Computed from <see cref="RequiredStatement"/>
    /// (the 3.0-native storage) — a structural change, not just a rename: 2.x attribution has no
    /// label, so the legacy view discards <see cref="RequiredStatement.Label"/> and reads back
    /// only the value entries.
    /// </summary>
    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "requiredStatement")]
    [JsonProperty(AttributionJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Attribution> Attribution
    {
        get => (RequiredStatement?.Value ?? []).Select(x => new Attribution(x.Value)).ToList();
        private set => RequiredStatement = value is { Count: > 0 }
            ? new RequiredStatement([new Label("Attribution")], value.Select(x => new Description(x.Value)).ToList())
            : null;
    }

    /// <summary>
    /// 3.0-native replacement for <see cref="Attribution"/>. No 2.x equivalent shape (2.x
    /// attribution carries no label). Deliberately <see cref="JsonIgnoreAttribute"/>d, same
    /// reasoning as <see cref="BaseNode{TBaseNode}.Items"/>: the hand-built V3 reader/writer in
    /// IiifSerializer already reads/writes it explicitly, so generic reflection-based
    /// serialization must never touch it too, or it leaks into legacy JSON via plain JsonConvert.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces attribution from API 2.x.")]
    [JsonIgnore]
    public RequiredStatement? RequiredStatement
    {
        get => GetElementValue(x => x.RequiredStatement);
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

    /// <summary>
    /// Legacy (2.x) view of the rights statement. Computed from <see cref="Rights"/> (the
    /// 3.0-native storage) — rename only, per the confirmed spec mapping.
    /// </summary>
    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "rights")]
    [JsonProperty(LicenseJName)]
    public License? License
    {
        get => Rights is not null ? new License(Rights.Value) : null;
        private set => Rights = value is not null ? new Rights(value.Value) : null;
    }

    /// <summary>
    /// 3.0-native replacement for <see cref="License"/>. Rename only (confirmed against spec).
    /// Deliberately <see cref="JsonIgnoreAttribute"/>d - see <see cref="RequiredStatement"/>.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces license from API 2.x.")]
    [JsonIgnore]
    public Rights? Rights
    {
        get => GetElementValue(x => x.Rights);
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

    /// <summary>
    /// Legacy (2.x) view of parent references. Computed from <see cref="PartOf"/> (the
    /// 3.0-native storage) — 2.x within has no type, so it defaults to "Manifest" when
    /// converting a legacy value into <see cref="PartOf"/>.
    /// </summary>
    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "partOf")]
    [JsonProperty(WithinJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Within> Within
    {
        get => PartOf.Select(x => new Within(x.Id)).ToList();
        private set => PartOf = (value ?? []).Select(x => new global::IIIF.Manifests.Serializer.Properties.PartOf(x.Id, "Manifest")).ToList();
    }

    /// <summary>
    /// 3.0-native replacement for <see cref="Within"/>. Restructured to an object array with
    /// id/type (confirmed against spec). Deliberately <see cref="JsonIgnoreAttribute"/>d - see
    /// <see cref="RequiredStatement"/>.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces within from API 2.x.")]
    [JsonIgnore]
    public IReadOnlyCollection<global::IIIF.Manifests.Serializer.Properties.PartOf> PartOf
    {
        get => GetElementValue(x => x.PartOf) ?? [];
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

    /// <summary>
    /// 3.0-only; no 2.x equivalent shape (2.x uses <see cref="ViewingHint"/> instead). Deliberately
    /// <see cref="JsonIgnoreAttribute"/>d - same reasoning as <see cref="RequiredStatement"/>:
    /// <c>IiifSerializer</c> already reads/writes it explicitly via a shared V3 helper (covering
    /// Manifest/Collection/Canvas/Range), so generic reflection-based serialization must never
    /// touch it too, or it leaks into legacy V2.x JSON via plain JsonConvert.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x. Some values also valid in 2.x as viewingHint.")]
    [JsonIgnore]
    public IReadOnlyCollection<Behavior> Behavior
    {
        get => GetElementValue(x => x.Behavior) ?? [];
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Legacy (2.x) view of the related resource link. Computed from <see cref="Homepage"/>
    /// (the 3.0-native storage) — confirmed via the 3.0 change log: "the related property was
    /// renamed to homepage with more specific semantics". Setting this replaces the whole
    /// Homepage collection with a single entry, matching related's historically singular shape.
    /// </summary>
    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "homepage")]
    [JsonProperty(RelatedJName)]
    public string? Related
    {
        get => Homepage.FirstOrDefault()?.Id;
        private set => Homepage = value is not null ? [new Homepage(value)] : [];
    }

    /// <summary>
    /// 3.0-native primary storage for this node's child items (Canvas/Range references on a Manifest,
    /// AnnotationPage/Annotation on a Canvas, etc). Deliberately <see cref="JsonIgnoreAttribute"/>d: every
    /// IIIF version has a different JSON shape for "child items", so no generic reflection-based serializer
    /// should ever touch this property directly. Version-aware readers/writers (see IiifSerializer) read and
    /// write it explicitly instead.
    /// </summary>
    [JsonIgnore]
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

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "summary")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddSummary instead.")]
    public TBaseNode AddDescription(Description description)
    {
        Description = Description.With(description);
        return (TBaseNode)this;
    }

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "summary")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use RemoveSummary instead.")]
    public TBaseNode RemoveDescription(Description description)
    {
        Description = Description.Without(description);
        return (TBaseNode)this;
    }

    public TBaseNode SetSummary(IReadOnlyCollection<Description> summary)
    {
        Summary = summary;
        return (TBaseNode)this;
    }

    public TBaseNode AddSummary(Description summary)
    {
        Summary = Summary.With(summary);
        return (TBaseNode)this;
    }

    public TBaseNode RemoveSummary(Description summary)
    {
        Summary = Summary.Without(summary);
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

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "requiredStatement")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use SetRequiredStatement instead.")]
    public TBaseNode AddAttribution(Attribution attribution)
    {
        Attribution = Attribution.With(attribution);
        return (TBaseNode)this;
    }

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "requiredStatement")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use SetRequiredStatement instead.")]
    public TBaseNode RemoveAttribution(Attribution attribution)
    {
        Attribution = Attribution.Without(attribution);
        return (TBaseNode)this;
    }

    public TBaseNode SetRequiredStatement(RequiredStatement requiredStatement) => SetElementValue(a => a.RequiredStatement, requiredStatement);

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

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "partOf")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddPartOf instead.")]
    public TBaseNode AddWithin(Within within)
    {
        Within = Within.With(within);
        return (TBaseNode)this;
    }

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "partOf")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddPartOf instead.")]
    public TBaseNode RemoveWithin(Within within)
    {
        Within = Within.Without(within);
        return (TBaseNode)this;
    }

    public TBaseNode AddPartOf(global::IIIF.Manifests.Serializer.Properties.PartOf partOf)
    {
        PartOf = PartOf.With(partOf);
        return (TBaseNode)this;
    }

    public TBaseNode RemovePartOf(global::IIIF.Manifests.Serializer.Properties.PartOf partOf)
    {
        PartOf = PartOf.Without(partOf);
        return (TBaseNode)this;
    }

    public TBaseNode SetAccompanyingCanvas(AccompanyingCanvas accompanyingCanvas) => SetElementValue(a => a.AccompanyingCanvas, accompanyingCanvas);
    public TBaseNode SetLogo(Logo logo) => SetElementValue(a => a.Logo, logo);
    public TBaseNode SetThumbnail(Thumbnail thumbnail) => SetElementValue(a => a.Thumbnail, thumbnail);

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "rights")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use SetRights instead.")]
    public TBaseNode SetLicense(License license)
    {
        License = license;
        return (TBaseNode)this;
    }

    public TBaseNode SetRights(Rights rights) => SetElementValue(a => a.Rights, rights);

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "behavior")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Replaced by behavior.")]
    public TBaseNode SetViewingHint(ViewingHint viewingHint) => SetElementValue(a => a.ViewingHint, viewingHint);

    [PresentationAPI("2.0", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "homepage")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Use AddHomepage instead.")]
    public TBaseNode SetRelated(string related)
    {
        Related = related;
        return (TBaseNode)this;
    }

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



