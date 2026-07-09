using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;

namespace IIIF.Manifests.Serializer.Extensions.ResourceCoords;

/// <summary>
/// Extension methods for adding Georeference resourceCoords support to a navPlace Feature's
/// properties. Per the Georeference extension spec, resourceCoords pins a Feature to specific
/// pixel/frame coordinates on the resource being georeferenced.
/// </summary>
public static class ResourceCoordExtensions
{
    public const string ResourceCoordsJName = "resourceCoords";

    extension(FeatureProperties featureProperties)
    {
        /// <summary>
        /// Set the resourceCoords property on these Feature properties.
        /// </summary>
        public FeatureProperties SetResourceCoords(IReadOnlyCollection<double> resourceCoords)
        {
            return featureProperties.SetAdditionalProperty(ResourceCoordsJName, resourceCoords);
        }

        /// <summary>
        /// Get the resourceCoords property from these Feature properties.
        /// </summary>
        [GeoreferenceExtension("3.0")]
        public IReadOnlyCollection<double>? ResourceCoords => featureProperties.GetAdditionalProperty<FeatureProperties, double[]>(ResourceCoordsJName);
    }
}