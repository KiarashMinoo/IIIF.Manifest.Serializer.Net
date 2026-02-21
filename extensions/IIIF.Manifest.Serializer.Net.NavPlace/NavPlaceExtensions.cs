using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.CanvasNode;

namespace IIIF.Manifests.Serializer.Extensions;

public static class NavPlaceExtensions
{
    extension(Canvas collection)
    {
        public Canvas SetNavPlace(NavPlace navPlace)
        {
            return collection.SetAdditionalElementValue(NavPlace.NavPlaceName, navPlace);
        }

        public NavPlace? GetNavPlace()
        {
            return collection.GetAdditionalElementValue<Canvas, NavPlace>(NavPlace.NavPlaceName);
        }
    }
}