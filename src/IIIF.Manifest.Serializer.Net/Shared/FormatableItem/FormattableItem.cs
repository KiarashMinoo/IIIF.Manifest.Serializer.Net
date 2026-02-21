using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.FormatableItem
{
    [JsonConverter(typeof(FormatableItemJsonConverter<>))]
    public class FormattableItem<TFormatableItem> : BaseItem<TFormatableItem>
        where TFormatableItem : FormattableItem<TFormatableItem>
    {
        public const string FormatJName = "format";

        [JsonProperty(FormatJName)] public string? Format => GetElementValue(x => x.Format);

        protected internal FormattableItem(string id) : base(id)
        {
        }

        public FormattableItem(string id, string type) : base(id, type)
        {
        }

        public TFormatableItem SetFormat(string format) => SetElementValue(a => a.Format, format);
    }
}