using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(ValuableItemJsonConverter<Attribution>))]
    public class Attribution : ValuableItem<Attribution>
    {
        public Attribution(string value) : base(value)
        {
        }
    }
}