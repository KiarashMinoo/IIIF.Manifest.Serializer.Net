using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[JsonConverter(typeof(ValuableItemJsonConverter<Description>))]
public sealed class Description(string value) : ValuableItem<Description>(value)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Description() : this(null!)
    {
    }

    public const string ValueJName = "@value";
    public const string LanguageJName = "@language";

    [JsonProperty(ValueJName)] public override string Value => base.Value;

    [JsonProperty(LanguageJName)]
    public string? Language
    {
        get => GetElementValue(x => x.Language);
        private set => SetElementValue(value);
    }

    public Description SetLanguage(string language)
    {
        Language = language;
        return this;
    }
}