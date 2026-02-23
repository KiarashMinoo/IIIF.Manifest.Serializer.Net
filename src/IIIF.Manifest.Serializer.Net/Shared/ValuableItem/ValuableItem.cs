using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.ValuableItem
{
    [JsonConverter(typeof(ValuableItemJsonConverter<>))]
    public class ValuableItem<TValuableItem> : TrackableObject<TValuableItem>
        where TValuableItem : ValuableItem<TValuableItem>
    {
        public virtual string Value
        {
            get { return GetElementValue(x => x.Value)!; }
            private set => SetElementValue(value);
        }

        public ValuableItem(string value)
        {
            Value = value;
        }
    }
}