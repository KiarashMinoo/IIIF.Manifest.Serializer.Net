using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

public class FormattableItem<TFormattableItem> : BaseItem<TFormattableItem>
    where TFormattableItem : FormattableItem<TFormattableItem>
{
    public const string FormatJName = "format";

    [JsonProperty(FormatJName)]
    public string? Format
    {
        get => GetElementValue(x => x.Format);
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    protected internal FormattableItem(string id) : base(id)
    {
    }

    public FormattableItem(string id, string type) : base(id, type)
    {
    }

    public TFormattableItem SetFormat(string format)
    {
        Format = format;
        return (TFormattableItem)this;
    }
}