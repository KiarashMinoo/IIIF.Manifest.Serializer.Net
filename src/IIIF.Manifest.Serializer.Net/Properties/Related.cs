using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(FormatableItemJsonConverter<>))]
    public class Related : FormatableItem<Related>
    {
        public Related(string id) : base(id)
        {
        }
    }
}