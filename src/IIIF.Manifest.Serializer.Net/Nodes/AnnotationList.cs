using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    /// <summary>
    /// IIIF Presentation API 2.0 AnnotationList resource.
    /// Represents an ordered list of annotations on a canvas.
    /// </summary>
    /// <remarks>
    /// Deprecated in IIIF Presentation API 3.0. Use AnnotationPage instead.
    /// </remarks>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0",
        ReplacedBy = "AnnotationPage", Notes = "AnnotationLists replaced by AnnotationPage in API 3.0")]
    public class AnnotationList(string id) : BaseNode<AnnotationList>(id, "sc:AnnotationList")
    {
        public const string ResourcesJName = "resources";

        /// <summary>
        /// The annotations in this list.
        /// </summary>
        [JsonProperty(ResourcesJName)]
        public IReadOnlyCollection<IBaseResource> Resources
        {
            get => GetElementValue(x => x.Resources) ?? [];
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Reference to the Layer this AnnotationList belongs to.
        /// </summary>
        [JsonProperty(WithinJName)]
        public string? WithinLayer
        {
            get => GetElementValue(x => x.WithinLayer);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public AnnotationList(string id, Label label) : this(id) => AddLabel(label);

        public AnnotationList AddResource<TResource>(TResource annotation) where TResource : IBaseResource
        {
            Resources = Resources.With(annotation);
            return this;
        }

        public AnnotationList RemoveResource<TResource>(TResource annotation) where TResource : IBaseResource
        {
            Resources = Resources.Without(annotation);
            return this;
        }

        public AnnotationList SetWithinLayer(string layerId)
        {
            WithinLayer = layerId;
            return this;
        }
    }
}