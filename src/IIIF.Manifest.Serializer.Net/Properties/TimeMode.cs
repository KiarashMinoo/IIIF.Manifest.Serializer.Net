using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF time mode values for temporal media.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[PresentationAPI("3.0", Notes = "Time mode for temporal media in Presentation API 3.0")]
[JsonConverter(typeof(ValuableItemJsonConverter<TimeMode>))]
public class TimeMode(string value) : ValuableItem<TimeMode>(value)
{
    public static TimeMode Trim => new("trim");
    public static TimeMode Scale => new("scale");
    public static TimeMode Loop => new("loop");
}