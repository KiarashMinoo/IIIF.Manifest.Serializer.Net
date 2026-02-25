using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

/// <summary>
/// Extension methods for adding navPlace geographic location support to IIIF resources.
/// Per the navPlace spec, navPlace can be used with: Collection, Manifest, Range, Canvas.
/// Other types of resource must not have the navPlace property.
/// </summary>
public static class TransformationExtensions
{
    extension<TNode>(TNode node) where TNode : BaseNode<TNode>, IAdditionalPropertiesSupport<TNode>
    {
        /// <summary>
        /// Set the navPlace property on this Canvas.
        /// </summary>
        public TNode SetTransformation(Transformation navPlace)
        {
            return node.SetAdditionalProperty(Transformation.TransformationJName, navPlace);
        }

        /// <summary>
        /// Get the navPlace property from this Canvas.
        /// </summary>
        public Transformation? Transformation => node.GetAdditionalProperty<TNode, Transformation>(Transformation.TransformationJName);
    }
}