using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Description;
using IIIF.Manifests.Serializer.Properties.Metadata;
using IIIF.Manifests.Serializer.Properties.Rendering;
using IIIF.Manifests.Serializer.Properties.Within;
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
        public const string RelatedJName = "related";


        private List<Label> labels = new List<Label>();
        private readonly List<Description> descriptions = new List<Description>();
        private readonly List<Metadata> metadatas = new List<Metadata>();
        private readonly List<Attribution> attributions = new List<Attribution>();
        private readonly List<SeeAlso> seeAloses = new List<SeeAlso>();
        private readonly List<Within> withins = new List<Within>();


        [JsonProperty(LabelJName)]
        public IReadOnlyCollection<Label> Label => labels.AsReadOnly();

        [JsonProperty(DescriptionJName)]
        public IReadOnlyCollection<Description> Description => descriptions.AsReadOnly();

        [JsonProperty(MetadataJName)]
        public IReadOnlyCollection<Metadata> Metadata => metadatas.AsReadOnly();

        [JsonProperty(AttributionJName)]
        public IReadOnlyCollection<Attribution> Attribution => attributions.AsReadOnly();

        [JsonProperty(LogoJName)]
        public Logo Logo { get; private set; }

        [JsonProperty(ThumbnailJName)]
        public Thumbnail Thumbnail { get; private set; }

        [JsonProperty(LicenseJName)]
        public License License { get; private set; }

        [JsonProperty(ViewingHintJName)]
        public string ViewingHint { get; private set; }

        [JsonProperty(RenderingJName)]
        public Rendering Rendering { get; private set; }

        [JsonProperty(SeeAlsoJName)]
        public IReadOnlyCollection<SeeAlso> SeeAlso => seeAloses.AsReadOnly();

        [JsonProperty(WithinJName)]
        public IReadOnlyCollection<Within> Within => withins.AsReadOnly();

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

        public TBaseNode AddWithin(Within within) => SetPropertyValue(a => a.withins, a => a.Within, withins.Attach(within));
        public TBaseNode RemoveWithin(Within within) => SetPropertyValue(a => a.withins, a => a.Within, withins.Detach(within));

        public TBaseNode SetLogo(Logo logo) => SetPropertyValue(a => a.Logo, logo);
        public TBaseNode SetThumbnail(Thumbnail thumbnail) => SetPropertyValue(a => a.Thumbnail, thumbnail);
        public TBaseNode SetLicense(License license) => SetPropertyValue(a => a.License, license);
        public TBaseNode SetViewingHint(string viewingHint) => SetPropertyValue(a => a.ViewingHint, viewingHint);
        public TBaseNode SetRendering(Rendering rendering) => SetPropertyValue(a => a.Rendering, rendering);
        public TBaseNode SetRelated(string related) => SetPropertyValue(a => a.Related, related);
    }
}