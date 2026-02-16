using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF time mode values for temporal media.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Time mode for temporal media in Presentation API 3.0")]
    [JsonConverter(typeof(ValuableItemJsonConverter<TimeMode>))]
    public class TimeMode : ValuableItem<TimeMode>
    {
        public TimeMode(string value) : base(value)
        {
        }

        public static TimeMode Trim => new TimeMode("trim");
        public static TimeMode Scale => new TimeMode("scale");
        public static TimeMode Loop => new TimeMode("loop");
    }
}
