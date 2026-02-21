using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.AnnotationListNode
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
    [JsonConverter(typeof(AnnotationListJsonConverter))]
    public class AnnotationList(string id) : BaseNode<AnnotationList>(id, "sc:AnnotationList")
    {
        public const string ResourcesJName = "resources";

        /// <summary>
        /// The annotations in this list.
        /// </summary>
        [JsonProperty(ResourcesJName)]
        public IReadOnlyCollection<object> Resources => GetElementValue(x => x.Resources) ?? [];

        /// <summary>
        /// Reference to the Layer this AnnotationList belongs to.
        /// </summary>
        [JsonProperty(WithinJName)]
        public string? WithinLayer => GetElementValue(x => x.WithinLayer);

        public AnnotationList(string id, Label label) : this(id) => AddLabel(label);

        public AnnotationList AddResource(object annotation) =>
            SetElementValue(a => a.Resources, collection => collection.With(annotation));

        public AnnotationList RemoveResource(object annotation) =>
            SetElementValue(a => a.Resources, collection => collection.Without(annotation));

        public AnnotationList SetWithinLayer(string layerId) =>
            SetElementValue(a => a.WithinLayer, layerId);
    }
}