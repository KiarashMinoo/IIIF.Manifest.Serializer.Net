using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF language values based on BCP 47 tags.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Language tags used in both 2.x and 3.0 for internationalized strings.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Language>))]
    public class Language : ValuableItem<Language>
    {
        public Language(string value) : base(value)
        {
        }

        // Common languages
        public static Language English => new Language("en");
        public static Language EnglishUs => new Language("en-US");
        public static Language EnglishGb => new Language("en-GB");
        public static Language French => new Language("fr");
        public static Language German => new Language("de");
        public static Language Spanish => new Language("es");
        public static Language Italian => new Language("it");
        public static Language Portuguese => new Language("pt");
        public static Language Dutch => new Language("nl");
        public static Language Russian => new Language("ru");
        public static Language Chinese => new Language("zh");
        public static Language Japanese => new Language("ja");
        public static Language Korean => new Language("ko");
        public static Language Arabic => new Language("ar");
        public static Language Hebrew => new Language("he");
        public static Language Latin => new Language("la");
        public static Language Greek => new Language("el");
        public static Language None => new Language("none");
    }
}
