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
    public class AnnotationList : BaseNode<AnnotationList>
    {
        public const string ResourcesJName = "resources";
        
        private readonly List<object> resources = new List<object>();

        /// <summary>
        /// The annotations in this list.
        /// </summary>
        [JsonProperty(ResourcesJName)]
        public IReadOnlyCollection<object> Resources => resources.AsReadOnly();

        /// <summary>
        /// Reference to the Layer this AnnotationList belongs to.
        /// </summary>
        [JsonProperty(WithinJName)]
        public string WithinLayer { get; private set; }

        public AnnotationList(string id) : base(id, "sc:AnnotationList")
        {
        }

        public AnnotationList(string id, Label label) : this(id) => AddLabel(label);

        public AnnotationList AddResource(object annotation) => 
            SetPropertyValue(a => a.resources, a => a.Resources, resources.Attach(annotation));
        
        public AnnotationList RemoveResource(object annotation) => 
            SetPropertyValue(a => a.resources, a => a.Resources, resources.Detach(annotation));

        public AnnotationList SetWithinLayer(string layerId) => 
            SetPropertyValue(a => a.WithinLayer, layerId);
    }
}

