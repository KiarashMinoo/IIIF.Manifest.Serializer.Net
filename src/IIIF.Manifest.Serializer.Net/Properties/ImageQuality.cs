using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF Image API quality values.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[ImageAPI("2.0", Notes = "Supported in both Image API 2.x and 3.0")]
[JsonConverter(typeof(ValuableItemJsonConverter<ImageQuality>))]
public sealed class ImageQuality(string value) : ValuableItem<ImageQuality>(value)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private ImageQuality() : this(null!)
    {
    }

    // Standard qualities
    public static ImageQuality Default => new("default");
    public static ImageQuality Color => new("color");
    public static ImageQuality Gray => new("gray");
    public static ImageQuality Bitonal => new("bitonal");
}