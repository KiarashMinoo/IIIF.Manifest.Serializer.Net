using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Image API profile/compliance level values.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [ImageAPI("2.0", Notes = "Profile values for Image API compliance levels. Format changed between 2.x (URLs) and 3.0 (keywords).")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Profile>))]
    public class Profile : ValuableItem<Profile>
    {
        public Profile(string value) : base(value)
        {
        }

        // IIIF Image API 3.0 profiles
        public static Profile Level0 => new Profile("level0");
        public static Profile Level1 => new Profile("level1");
        public static Profile Level2 => new Profile("level2");

        // IIIF Image API 2.x profiles (full URLs)
        public static Profile ImageApi2Level0 => new Profile("http://iiif.io/api/image/2/level0.json");
        public static Profile ImageApi2Level1 => new Profile("http://iiif.io/api/image/2/level1.json");
        public static Profile ImageApi2Level2 => new Profile("http://iiif.io/api/image/2/level2.json");

        // IIIF Auth profiles
        public static Profile AuthLogin => new Profile("http://iiif.io/api/auth/1/login");
        public static Profile AuthClickthrough => new Profile("http://iiif.io/api/auth/1/clickthrough");
        public static Profile AuthKiosk => new Profile("http://iiif.io/api/auth/1/kiosk");
        public static Profile AuthExternal => new Profile("http://iiif.io/api/auth/1/external");
        public static Profile AuthToken => new Profile("http://iiif.io/api/auth/1/token");
        public static Profile AuthLogout => new Profile("http://iiif.io/api/auth/1/logout");
    }
}
