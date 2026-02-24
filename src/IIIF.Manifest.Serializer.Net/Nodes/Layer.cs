using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
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
    public class Layer(string id) : BaseNode<Layer>(id, "sc:Layer")
    {
        public const string OtherContentJName = "otherContent";

        /// <summary>
        /// References to AnnotationLists that belong to this layer.
        /// </summary>
        [JsonProperty(OtherContentJName)]
        public IReadOnlyCollection<string> OtherContent
        {
            get => GetElementValue(x => x.OtherContent) ?? [];
            private set => SetElementValue(value);
        }

        public Layer(string id, Label label) : this(id) => AddLabel(label);

        public Layer AddOtherContent(string annotationListId)
        {
            OtherContent = OtherContent.With(annotationListId);
            return this;
        }

        public Layer RemoveOtherContent(string annotationListId)
        {
            OtherContent = OtherContent.Without(annotationListId);
            return this;
        }
    }
}