using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.LayerNode
{
    /// <summary>
    /// IIIF Presentation API 2.0 Layer resource.
    /// Represents an ordered list of AnnotationLists that should be rendered together.
    /// </summary>
    /// <remarks>
    /// Deprecated in IIIF Presentation API 3.0. Use AnnotationCollection instead.
    /// </remarks>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0",
        ReplacedBy = "AnnotationCollection", Notes = "Layers removed in API 3.0")]
    [JsonConverter(typeof(LayerJsonConverter))]
    public class Layer(string id) : BaseNode<Layer>(id, "sc:Layer")
    {
        public const string OtherContentJName = "otherContent";

        /// <summary>
        /// References to AnnotationLists that belong to this layer.
        /// </summary>
        [JsonProperty(OtherContentJName)]
        public IReadOnlyCollection<string> OtherContent => GetElementValue(x => x.OtherContent) ?? [];

        public Layer(string id, Label label) : this(id) => AddLabel(label);

        public Layer AddOtherContent(string annotationListId) =>
            SetElementValue(a => a.OtherContent, collection => collection.With(annotationListId));

        public Layer RemoveOtherContent(string annotationListId) =>
            SetElementValue(a => a.OtherContent, collection => collection.Without(annotationListId));
    }
}