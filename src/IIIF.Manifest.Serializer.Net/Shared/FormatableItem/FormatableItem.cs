using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.FormatableItem
{
    [JsonConverter(typeof(FormatableItemJsonConverter<>))]
    public class FormatableItem<TFormatableItem> : BaseItem<TFormatableItem>
        where TFormatableItem : FormatableItem<TFormatableItem>
    {
        public const string FormatJName = "format";

        [JsonProperty(FormatJName)]
        public string Format { get; private set; }

        protected internal FormatableItem(string id) : base(id)
        {
        }

        public FormatableItem(string id, string type) : base(id, type)
        {
        }

        public TFormatableItem SetFormat(string format) => SetPropertyValue(a => a.Format, format);
    }
}