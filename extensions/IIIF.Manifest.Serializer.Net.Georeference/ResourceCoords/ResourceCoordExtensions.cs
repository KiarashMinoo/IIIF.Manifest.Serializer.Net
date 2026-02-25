using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;

namespace IIIF.Manifests.Serializer.Extensions.ResourceCoords;

/// <summary>
/// Extension methods for adding navPlace geographic location support to IIIF resources.
/// Per the navPlace spec, navPlace can be used with: Collection, Manifest, Range, Canvas.
/// Other types of resource must not have the navPlace property.
/// </summary>
public static class ResourceCoordExtensions
{
    public const string ResourceCoordsJName = "resourceCoords";

    extension(FeatureProperties featureProperties)
    {
        /// <summary>
        /// Set the navPlace property on this Canvas.
        /// </summary>
        public FeatureProperties SetResourceCoords(IReadOnlyCollection<double> resourceCoords)
        {
            return featureProperties.SetAdditionalProperty(ResourceCoordsJName, resourceCoords);
        }

        /// <summary>
        /// Get the navPlace property from this Canvas.
        /// </summary>
        public IReadOnlyCollection<double>? ResourceCoords => featureProperties.GetAdditionalProperty<FeatureProperties, double[]>(ResourceCoordsJName);
    }
}