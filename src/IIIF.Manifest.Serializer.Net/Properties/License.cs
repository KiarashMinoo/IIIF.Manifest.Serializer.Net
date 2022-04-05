using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(ValuableItemJsonConverter<License>))]
    public class License : ValuableItem<License>
    {
        public License(string value) : base(value)
        {
        }
    }
}