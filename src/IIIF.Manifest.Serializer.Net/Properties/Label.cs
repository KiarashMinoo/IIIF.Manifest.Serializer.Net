using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     A single label value, optionally language-tagged (cookbook recipes 0006/0010/0011 and many
///     others carry a label in more than one language, e.g. <c>{"en":["..."],"fr":["..."]}</c>).
///     <see cref="Language" /> is not touched by <see cref="ValuableItemJsonConverter{Label}" /> (it
///     only ever reads/writes the bare string) - it exists purely so IiifSerializer's own
///     language-map grouping logic (label is otherwise a hand-rolled "@context"-free JSON shape,
///     not generically reflected) can group entries by language instead of dumping everything
///     under "none".
/// </summary>
[JsonConverter(typeof(ValuableItemJsonConverter<Label>))]
public class Label : ValuableItem<Label>
{
    public Label(string value) : base(value)
    {
    }

    public Label(string value, string language) : this(value)
    {
        Language = language;
    }

    public string? Language
    {
        get => GetElementValue(x => x.Language);
        private set => SetElementValue(value);
    }

    public Label SetLanguage(string language)
    {
        Language = language;
        return this;
    }
}