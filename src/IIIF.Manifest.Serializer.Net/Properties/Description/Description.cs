using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{

    [JsonConverter(typeof(DescriptionJsonConverter))]
    public class Description : ValuableItem<Description>
    {
        public const string ValueJName = "@value";
        public const string LanguageJName = "@language";

        [JsonProperty(LanguageJName)]
        public string Language { get; private set; }

        public Description(string value) : base(value)
        {
        }

        public Description SetLanguage(string language) => SetPropertyValue(a => a.Language, language);
    }
}