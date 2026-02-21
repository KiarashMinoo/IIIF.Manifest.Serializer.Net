using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.CollectionNode;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Nodes.StructureNode;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Extension methods for adding navPlace geographic location support to IIIF resources.
/// Per the navPlace spec, navPlace can be used with: Collection, Manifest, Range, Canvas.
/// Other types of resource must not have the navPlace property.
/// </summary>
public static class NavPlaceExtensions
{
    extension(Canvas canvas)
    {
        /// <summary>
        /// Set the navPlace property on this Canvas.
        /// </summary>
        public Canvas SetNavPlace(NavPlace navPlace)
        {
            return canvas.SetAdditionalElementValue(NavPlace.NavPlaceJName, navPlace);
        }

        /// <summary>
        /// Get the navPlace property from this Canvas.
        /// </summary>
        public NavPlace? GetNavPlace()
        {
            return canvas.GetAdditionalElementValue<Canvas, NavPlace>(NavPlace.NavPlaceJName);
        }
    }

    extension(Manifest manifest)
    {
        /// <summary>
        /// Set the navPlace property on this Manifest.
        /// </summary>
        public Manifest SetNavPlace(NavPlace navPlace)
        {
            return manifest.SetAdditionalElementValue(NavPlace.NavPlaceJName, navPlace);
        }

        /// <summary>
        /// Get the navPlace property from this Manifest.
        /// </summary>
        public NavPlace? GetNavPlace()
        {
            return manifest.GetAdditionalElementValue<Manifest, NavPlace>(NavPlace.NavPlaceJName);
        }
    }

    extension(Structure structure)
    {
        /// <summary>
        /// Set the navPlace property on this Structure (Range).
        /// </summary>
        public Structure SetNavPlace(NavPlace navPlace)
        {
            return structure.SetAdditionalElementValue(NavPlace.NavPlaceJName, navPlace);
        }

        /// <summary>
        /// Get the navPlace property from this Structure (Range).
        /// </summary>
        public NavPlace? GetNavPlace()
        {
            return structure.GetAdditionalElementValue<Structure, NavPlace>(NavPlace.NavPlaceJName);
        }
    }

    extension(Collection collection)
    {
        /// <summary>
        /// Set the navPlace property on this Collection.
        /// </summary>
        public Collection SetNavPlace(NavPlace navPlace)
        {
            return collection.SetAdditionalElementValue(NavPlace.NavPlaceJName, navPlace);
        }

        /// <summary>
        /// Get the navPlace property from this Collection.
        /// </summary>
        public NavPlace? GetNavPlace()
        {
            return collection.GetAdditionalElementValue<Collection, NavPlace>(NavPlace.NavPlaceJName);
        }
    }
}