using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

/// <summary>
///     Extension methods for adding Georeference transformation support to IIIF resources.
///     Per the Georeference extension spec (§3.6), <c>transformation</c> belongs on the GeoJSON
///     FeatureCollection that is a <see cref="GeoreferenceAnnotation" />'s <c>body</c> (a
///     <see cref="NavPlace" /> instance) - it does not belong on arbitrary Presentation resources.
///     Constrained to <see cref="TrackableObject{TTrackableObject}" /> rather than the narrower
///     <c>BaseNode&lt;TNode&gt;</c> this used previously, since <see cref="NavPlace" /> is a
///     <c>BaseItem</c> (not a <c>BaseNode</c>) and otherwise couldn't reach this extension at all.
/// </summary>
public static class TransformationExtensions
{
    extension<TNode>(TNode node) where TNode : TrackableObject<TNode>, IAdditionalPropertiesSupport<TNode>
    {
        /// <summary>
        ///     Get the transformation property from this resource.
        /// </summary>
        [GeoreferenceExtension("3.0")]
        public Transformation? Transformation => node.GetAdditionalProperty<TNode, Transformation>(Transformation.TransformationJName);

        /// <summary>
        ///     Set the transformation property on this resource.
        /// </summary>
        public TNode SetTransformation(Transformation transformation)
        {
            return node.SetAdditionalProperty(Transformation.TransformationJName, transformation);
        }
    }
}