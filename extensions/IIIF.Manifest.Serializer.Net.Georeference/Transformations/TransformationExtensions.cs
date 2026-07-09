using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

/// <summary>
/// Extension methods for adding Georeference transformation support to IIIF resources.
/// Per the Georeference extension spec, transformation describes how resourceCoords map onto
/// real-world georeferenced coordinates (e.g. via a polynomial or thin-plate-spline transform).
/// </summary>
public static class TransformationExtensions
{
    extension<TNode>(TNode node) where TNode : BaseNode<TNode>, IAdditionalPropertiesSupport<TNode>
    {
        /// <summary>
        /// Set the transformation property on this resource.
        /// </summary>
        public TNode SetTransformation(Transformation transformation)
        {
            return node.SetAdditionalProperty(Transformation.TransformationJName, transformation);
        }

        /// <summary>
        /// Get the transformation property from this resource.
        /// </summary>
        [GeoreferenceExtension("3.0")]
        public Transformation? Transformation => node.GetAdditionalProperty<TNode, Transformation>(Transformation.TransformationJName);
    }
}