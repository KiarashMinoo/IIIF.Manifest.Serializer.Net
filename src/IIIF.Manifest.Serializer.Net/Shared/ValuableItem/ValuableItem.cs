using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.ValuableItem
{
    [JsonConverter(typeof(ValuableItemJsonConverter<>))]
    public class ValuableItem<TValuableItem> : TrackableObject<TValuableItem>
        where TValuableItem : ValuableItem<TValuableItem>
    {
        public string Value => GetElementValue(x => x.Value)!;

        public ValuableItem(string value)
        {
            SetElementValue(x => x.Value, value);
        }
    }
}