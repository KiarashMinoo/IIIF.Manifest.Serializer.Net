using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Interfaces
{
    public interface IViewingDirectionSupport<TNode>
        where TNode : BaseNode<TNode>, IViewingDirectionSupport<TNode>
    {
        [JsonProperty(Constants.ViewingDirectionJName)]
        ViewingDirection ViewingDirection { get; }

        TNode SetViewingDirection(ViewingDirection viewingDirection);
    }
}