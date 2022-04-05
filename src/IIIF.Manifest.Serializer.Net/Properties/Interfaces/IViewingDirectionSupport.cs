using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties
{
    public interface IViewingDirectionSupport<TNode>
        where TNode : BaseNode<TNode>, IViewingDirectionSupport<TNode>
    {
        [JsonProperty(Constants.ViewingDirectionJName)]
        ViewingDirection ViewingDirection { get; }

        TNode SetViewingDirection(ViewingDirection viewingDirection);
    }

    public static class IViewingDirectionSupportHelper
    {
        public static T SetViewingDirection<T>(this T item, JToken element) where T : BaseNode<T>, IViewingDirectionSupport<T>
        {
            var jViewingDirection = element.TryGetToken(Constants.ViewingDirectionJName);
            if (jViewingDirection != null)
                item.SetViewingDirection(jViewingDirection.ToObject<ViewingDirection>());

            return item;
        }
    }
}