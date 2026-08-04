using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF viewing direction values.
///     Specifies the direction in which a set of Canvases should be displayed.
/// </summary>
[PresentationAPI("2.0", Notes = "Supported in both 2.x and 3.0")]
[JsonConverter(typeof(ValuableItemJsonConverter<ViewingDirection>))]
public sealed class ViewingDirection(string value) : ValuableItem<ViewingDirection>(value)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private ViewingDirection() : this(null!)
    {
    }

    public static ViewingDirection Ltr => new("left-to-right");
    public static ViewingDirection Rtl => new("right-to-left");
    public static ViewingDirection Ttb => new("top-to-bottom");
    public static ViewingDirection Btt => new("bottom-to-top");
}