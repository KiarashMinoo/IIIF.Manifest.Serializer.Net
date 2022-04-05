using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(ValuableItemJsonConverter<Label>))]
    public class Label : ValuableItem<Label>
    {
        public Label(string value) : base(value)
        {
        }
    }
}