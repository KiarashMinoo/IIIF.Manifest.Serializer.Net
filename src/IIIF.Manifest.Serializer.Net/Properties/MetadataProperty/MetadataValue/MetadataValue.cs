using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.MetadataProperty.MetadataValue;

[JsonConverter(typeof(ValuableItemJsonConverter<MetadataValue>))]
public sealed class MetadataValue(string value) : ValuableItem<MetadataValue>(value)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private MetadataValue() : this(null!)
    {
    }

    public const string ValueJName = "@value";
    public const string LanguageJName = "@language";

    public MetadataValue(string value, string language) : this(value)
    {
        Language = language;
    }

    [JsonProperty(ValueJName)] public override string Value => base.Value;

    [JsonProperty(LanguageJName)]
    public string? Language
    {
        get => GetElementValue(x => x.Language);
        private set => SetElementValue(value);
    }

    public MetadataValue SetValue(string value)
    {
        return SetElementValue(a => a.Value, value);
    }
}