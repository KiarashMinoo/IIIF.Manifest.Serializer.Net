using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(BaseItemJsonConverter<StartCanvas>))]
    public class StartCanvas : BaseItem<StartCanvas>
    {
        public StartCanvas(string id) : base(id)
        {
        }
    }
}