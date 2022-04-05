using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(FormatableItemJsonConverter<Logo>))]
    public class Logo : FormatableItem<Logo>
    {
        public Logo(string id) : base(id, "dctypes:Image")
        {
        }
    }
}