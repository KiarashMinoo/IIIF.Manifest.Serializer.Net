using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(FormatableItemJsonConverter<Thumbnail>))]
    public class Thumbnail : FormatableItem<Thumbnail>
    {
        public Thumbnail(string id) : base(id, "dctypes:Image")
        {
        }
    }
}