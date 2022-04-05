using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(FormatableItemJsonConverter<>))]
    public class Related : FormatableItem<Related>
    {
        public Related(string id) : base(id)
        {
        }
    }
}