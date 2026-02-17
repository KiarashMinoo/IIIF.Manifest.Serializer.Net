using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Attribution>))]
    public class Attribution : ValuableItem<Attribution>
    {
        public Attribution(string value) : base(value)
        {
        }
    }
}