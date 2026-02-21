using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.AccompanyingCanvasProperty;
using IIIF.Manifests.Serializer.Properties.DescriptionProperty;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.ProviderProperty;
using IIIF.Manifests.Serializer.Properties.RenderingProperty;
using IIIF.Manifests.Serializer.Properties.WithinProperty;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.BaseNode
{
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

        [JsonProperty(LabelJName)] public IReadOnlyCollection<Label> Label => GetElementValue(x => x.Label) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(DescriptionJName)]
        public IReadOnlyCollection<Description> Description => GetElementValue(x => x.Description) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(MetadataJName)]
        public IReadOnlyCollection<Metadata> Metadata => GetElementValue(x => x.Metadata) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(AttributionJName)]
        public IReadOnlyCollection<Attribution> Attribution => GetElementValue(x => x.Attribution) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(LogoJName)]
        public Logo? Logo => GetElementValue(x => x.Logo);

        [PresentationAPI("2.0")]
        [JsonProperty(ThumbnailJName)]
        public Thumbnail? Thumbnail => GetElementValue(x => x.Thumbnail);

        [PresentationAPI("2.0")]
        [JsonProperty(LicenseJName)]
        public License? License => GetElementValue(x => x.License);

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "behavior")]
        [JsonProperty(ViewingHintJName)]
        public ViewingHint? ViewingHint => GetElementValue(x => x.ViewingHint);

        [PresentationAPI("2.0")]
        [JsonProperty(RenderingJName)]
        public IReadOnlyCollection<Rendering> Rendering => GetElementValue(x => x.Rendering) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(WithinJName)]
        public IReadOnlyCollection<Within> Within => GetElementValue(x => x.Within) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(SeeAlsoJName)]
        public IReadOnlyCollection<SeeAlso> SeeAlso => GetElementValue(x => x.SeeAlso) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(HomepageJName)]
        public IReadOnlyCollection<Homepage> Homepage => GetElementValue(x => x.Homepage) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(ProviderJName)]
        public IReadOnlyCollection<Provider> Provider => GetElementValue(x => x.Provider) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(AccompanyingCanvasJName)]
        public AccompanyingCanvas? AccompanyingCanvas => GetElementValue(x => x.AccompanyingCanvas);

        [PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x. Some values also valid in 2.x as viewingHint.")]
        [JsonProperty(BehaviorJName)]
        public IReadOnlyCollection<Behavior> Behavior => GetElementValue(x => x.Behavior) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(RelatedJName)]
        public string? Related => GetElementValue(x => x.Related);

        protected internal BaseNode(string id) : base(id)
        {
        }

        public BaseNode(string id, string type) : base(id, type)
        {
        }

        public TBaseNode SetLabel(Label[] labels) => SetElementValue(a => a.Label, _ => [..labels]);
        public TBaseNode AddLabel(Label label) => SetElementValue(a => a.Label, labels => labels.With(label));
        public TBaseNode RemoveLabel(Label label) => SetElementValue(a => a.Label, labels => labels.Without(label));

        public TBaseNode AddDescription(Description description) => SetElementValue(a => a.Description, collection => collection.With(description));
        public TBaseNode RemoveDescription(Description description) => SetElementValue(a => a.Description, collection => collection.Without(description));

        public TBaseNode AddMetadata(Metadata metadata) => SetElementValue(a => a.Metadata, (collection) => collection.With(metadata));
        public TBaseNode RemoveMetadata(Metadata metadata) => SetElementValue(a => a.Metadata, (collection) => collection.Without(metadata));

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
        public TBaseNode SetViewingHint(ViewingHint viewingHint) => SetElementValue(a => a.ViewingHint, viewingHint);
        public TBaseNode SetRelated(string related) => SetElementValue(a => a.Related, related);
    }
}