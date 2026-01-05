using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.ValuableItem
{
    [JsonConverter(typeof(ValuableItemJsonConverter<>))]
    public class ValuableItem<TValuableItem> : TrackableObject<TValuableItem>
        where TValuableItem : ValuableItem<TValuableItem>
    {
        public string Value { get; }

        public ValuableItem(string value)
        {
            Value = value;
        }
    }
}