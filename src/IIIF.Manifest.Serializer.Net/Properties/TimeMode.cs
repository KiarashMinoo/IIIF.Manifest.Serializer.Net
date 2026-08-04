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
public sealed class TimeMode(string value) : ValuableItem<TimeMode>(value)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private TimeMode() : this(null!)
    {
    }

    public static TimeMode Trim => new("trim");
    public static TimeMode Scale => new("scale");
    public static TimeMode Loop => new("loop");
}