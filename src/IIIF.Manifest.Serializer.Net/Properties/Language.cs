using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF language values based on BCP 47 tags.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[PresentationAPI("2.0", Notes = "Language tags used in both 2.x and 3.0 for internationalized strings.")]
[JsonConverter(typeof(ValuableItemJsonConverter<Language>))]
public class Language(string value) : ValuableItem<Language>(value)
{
    // Common languages
    public static Language English => new("en");
    public static Language EnglishUs => new("en-US");
    public static Language EnglishGb => new("en-GB");
    public static Language French => new("fr");
    public static Language German => new("de");
    public static Language Spanish => new("es");
    public static Language Italian => new("it");
    public static Language Portuguese => new("pt");
    public static Language Dutch => new("nl");
    public static Language Russian => new("ru");
    public static Language Chinese => new("zh");
    public static Language Japanese => new("ja");
    public static Language Korean => new("ko");
    public static Language Arabic => new("ar");
    public static Language Hebrew => new("he");
    public static Language Latin => new("la");
    public static Language Greek => new("el");
    public static Language None => new("none");
}