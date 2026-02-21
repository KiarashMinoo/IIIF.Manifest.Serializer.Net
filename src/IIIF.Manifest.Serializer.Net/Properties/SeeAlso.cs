using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(FormatableItemJsonConverter<SeeAlso>))]
    public class SeeAlso : FormattableItem<SeeAlso>
    {
        public SeeAlso(string id) : base(id)
        {
        }
    }
}