using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(FormatableItemJsonConverter<SeeAlso>))]
    public class SeeAlso : FormatableItem<SeeAlso>
    {
        public SeeAlso(string id) : base(id)
        {
        }
    }
}