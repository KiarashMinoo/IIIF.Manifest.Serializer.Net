using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.DescriptionProperty
{
    [JsonConverter(typeof(DescriptionJsonConverter))]
    public class Description(string value) : ValuableItem<Description>(value)
    {
        public const string ValueJName = "@value";
        public const string LanguageJName = "@language";

        [JsonProperty(LanguageJName)] public string? Language => GetElementValue(x => x.Language);

        public Description SetLanguage(string language) => SetElementValue(a => a.Language, language);
    }
}