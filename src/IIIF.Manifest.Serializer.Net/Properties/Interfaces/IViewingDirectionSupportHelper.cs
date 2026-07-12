using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Interfaces;

public static class IViewingDirectionSupportHelper
{
    public static T SetViewingDirection<T>(this T item, JToken element) where T : BaseNode<T>, IViewingDirectionSupport<T>
    {
        var jViewingDirection = element.TryGetToken(Constants.ViewingDirectionJName);
        var viewingDirection = jViewingDirection?.ToObject<ViewingDirection>();
        if (viewingDirection != null) item.SetViewingDirection(viewingDirection);

        return item;
    }
}