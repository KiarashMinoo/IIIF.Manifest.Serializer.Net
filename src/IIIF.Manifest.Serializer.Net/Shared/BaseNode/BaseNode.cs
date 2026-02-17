using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.AccompanyingCanvasProperty;
using IIIF.Manifests.Serializer.Properties.DescriptionPropery;
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
        public const string RelatedJName = "related";
        public const string ProviderJName = "provider";
        public const string AccompanyingCanvasJName = "accompanyingCanvas";
        public const string BehaviorJName = "behavior";


        private List<Label> labels = new List<Label>();
        private readonly List<Description> descriptions = new List<Description>();
        private readonly List<Metadata> metadatas = new List<Metadata>();
        private readonly List<Attribution> attributions = new List<Attribution>();
        private readonly List<SeeAlso> seeAloses = new List<SeeAlso>();
        private readonly List<Within> withins = new List<Within>();
        private readonly List<Rendering> renderings = new List<Rendering>();
        private readonly List<Homepage> homepages = new List<Homepage>();
        private readonly List<Provider> providers = new List<Provider>();
        private readonly List<Behavior> behaviors = new List<Behavior>();


        [JsonProperty(LabelJName)]
        public IReadOnlyCollection<Label> Label => labels.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(DescriptionJName)]
        public IReadOnlyCollection<Description> Description => descriptions.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(MetadataJName)]
        public IReadOnlyCollection<Metadata> Metadata => metadatas.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(AttributionJName)]
        public IReadOnlyCollection<Attribution> Attribution => attributions.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(LogoJName)]
        public Logo Logo { get; private set; }

        [PresentationAPI("2.0")]
        [JsonProperty(ThumbnailJName)]
        public Thumbnail Thumbnail { get; private set; }

        [PresentationAPI("2.0")]
        [JsonProperty(LicenseJName)]
        public License License { get; private set; }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "behavior")]
        [JsonProperty(ViewingHintJName)]
        public ViewingHint ViewingHint { get; private set; }

        [PresentationAPI("2.0")]
        [JsonProperty(RenderingJName)]
        public IReadOnlyCollection<Rendering> Rendering => renderings.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(SeeAlsoJName)]
        public IReadOnlyCollection<SeeAlso> SeeAlso => seeAloses.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(WithinJName)]
        public IReadOnlyCollection<Within> Within => withins.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(HomepageJName)]
        public IReadOnlyCollection<Homepage> Homepage => homepages.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(ProviderJName)]
        public IReadOnlyCollection<Provider> Provider => providers.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(AccompanyingCanvasJName)]
        public AccompanyingCanvas AccompanyingCanvas { get; private set; }

        [PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x. Some values also valid in 2.x as viewingHint.")]
        [JsonProperty(BehaviorJName)]
        public IReadOnlyCollection<Behavior> Behavior => behaviors.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(RelatedJName)]
        public string Related { get; private set; }

        protected internal BaseNode(string id) : base(id)
        {
        }
        public BaseNode(string id, string type) : base(id, type)
        {
        }

        public TBaseNode SetLabel(Label[] labels) => SetPropertyValue(a => a.labels, a => a.Label, this.labels = new List<Label>(labels));
        public TBaseNode AddLabel(Label label) => SetPropertyValue(a => a.labels, a => a.Label, labels.Attach(label));
        public TBaseNode RemoveLabel(Label label) => SetPropertyValue(a => a.labels, a => a.Label, labels.Detach(label));

        public TBaseNode AddDescription(Description description) => SetPropertyValue(a => a.descriptions, a => a.Description, descriptions.Attach(description));
        public TBaseNode RemoveDescription(Description description) => SetPropertyValue(a => a.descriptions, a => a.Description, descriptions.Detach(description));

        public TBaseNode AddMetadata(Metadata metadata) => SetPropertyValue(a => a.metadatas, a => a.Metadata, metadatas.Attach(metadata));
        public TBaseNode RemoveMetadata(Metadata metadata) => SetPropertyValue(a => a.metadatas, a => a.Metadata, metadatas.Detach(metadata));

        public TBaseNode AddAttribution(Attribution attribution) => SetPropertyValue(a => a.attributions, a => a.Attribution, attributions.Attach(attribution));
        public TBaseNode RemoveAttribution(Attribution attribution) => SetPropertyValue(a => a.attributions, a => a.Attribution, attributions.Detach(attribution));

        public TBaseNode AddSeeAlso(SeeAlso seeAlso) => SetPropertyValue(a => a.seeAloses, a => a.SeeAlso, seeAloses.Attach(seeAlso));
        public TBaseNode RemoveSeeAlso(SeeAlso seeAlso) => SetPropertyValue(a => a.seeAloses, a => a.SeeAlso, seeAloses.Detach(seeAlso));

        public TBaseNode AddRendering(Rendering rendering) => SetPropertyValue(a => a.renderings, a => a.Rendering, renderings.Attach(rendering));
        public TBaseNode RemoveRendering(Rendering rendering) => SetPropertyValue(a => a.renderings, a => a.Rendering, renderings.Detach(rendering));

        public TBaseNode AddHomepage(Homepage homepage) => SetPropertyValue(a => a.homepages, a => a.Homepage, homepages.Attach(homepage));
        public TBaseNode RemoveHomepage(Homepage homepage) => SetPropertyValue(a => a.homepages, a => a.Homepage, homepages.Detach(homepage));

        public TBaseNode AddProvider(Provider provider) => SetPropertyValue(a => a.providers, a => a.Provider, providers.Attach(provider));
        public TBaseNode RemoveProvider(Provider provider) => SetPropertyValue(a => a.providers, a => a.Provider, providers.Detach(provider));

        public TBaseNode SetAccompanyingCanvas(AccompanyingCanvas accompanyingCanvas) => SetPropertyValue(a => a.AccompanyingCanvas, accompanyingCanvas);

        public TBaseNode AddBehavior(Behavior behavior) => SetPropertyValue(a => a.behaviors, a => a.Behavior, behaviors.Attach(behavior));
        public TBaseNode RemoveBehavior(Behavior behavior) => SetPropertyValue(a => a.behaviors, a => a.Behavior, behaviors.Detach(behavior));

        public TBaseNode AddWithin(Within within) => SetPropertyValue(a => a.withins, a => a.Within, withins.Attach(within));
        public TBaseNode RemoveWithin(Within within) => SetPropertyValue(a => a.withins, a => a.Within, withins.Detach(within));

        public TBaseNode SetLogo(Logo logo) => SetPropertyValue(a => a.Logo, logo);
        public TBaseNode SetThumbnail(Thumbnail thumbnail) => SetPropertyValue(a => a.Thumbnail, thumbnail);
        public TBaseNode SetLicense(License license) => SetPropertyValue(a => a.License, license);
        public TBaseNode SetViewingHint(ViewingHint viewingHint) => SetPropertyValue(a => a.ViewingHint, viewingHint);
        public TBaseNode SetRelated(string related) => SetPropertyValue(a => a.Related, related);
    }
}