using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF Image API profile/compliance level values.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[ImageAPI("2.0", Notes = "Profile values for Image API compliance levels. Format changed between 2.x (URLs) and 3.0 (keywords).")]
[JsonConverter(typeof(ValuableItemJsonConverter<Profile>))]
public class Profile(string value) : ValuableItem<Profile>(value)
{
    // IIIF Image API 3.0 profiles
    public static Profile Level0 => new("level0");
    public static Profile Level1 => new("level1");
    public static Profile Level2 => new("level2");

    // IIIF Image API 2.x profiles (full URLs)
    public static Profile ImageApi2Level0 => new("http://iiif.io/api/image/2/level0.json");
    public static Profile ImageApi2Level1 => new("http://iiif.io/api/image/2/level1.json");
    public static Profile ImageApi2Level2 => new("http://iiif.io/api/image/2/level2.json");

    // IIIF Auth profiles
    public static Profile AuthLogin => new("http://iiif.io/api/auth/1/login");
    public static Profile AuthClickthrough => new("http://iiif.io/api/auth/1/clickthrough");
    public static Profile AuthKiosk => new("http://iiif.io/api/auth/1/kiosk");
    public static Profile AuthExternal => new("http://iiif.io/api/auth/1/external");
    public static Profile AuthToken => new("http://iiif.io/api/auth/1/token");
    public static Profile AuthLogout => new("http://iiif.io/api/auth/1/logout");
}