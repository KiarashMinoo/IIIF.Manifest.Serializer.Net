using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

public class FormattableItem<TFormattableItem> : BaseItem<TFormattableItem>
    where TFormattableItem : FormattableItem<TFormattableItem>
{
    public const string FormatJName = "format";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through <see cref="BaseItem{TBaseItem}.Id" />-taking overloads.
    /// </summary>
    protected internal FormattableItem()
    {
    }

    [JsonConstructor]
    protected internal FormattableItem(string id) : base(id)
    {
    }

    public FormattableItem(string id, string type) : base(id, type)
    {
    }

    [JsonProperty(FormatJName)]
    public string? Format
    {
        get => GetElementValue(x => x.Format);
        private set => SetElementValue(value);
    }

    public TFormattableItem SetFormat(string format)
    {
        Format = format;
        return (TFormattableItem)this;
    }
}