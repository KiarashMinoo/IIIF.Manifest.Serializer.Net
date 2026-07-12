using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF Image API feature values.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[ImageAPI("2.0", Notes = "Supported in both Image API 2.x and 3.0. Feature names consistent across versions.")]
[JsonConverter(typeof(ValuableItemJsonConverter<ImageFeature>))]
public class ImageFeature(string value) : ValuableItem<ImageFeature>(value)
{
    // Region features
    public static ImageFeature RegionByPx => new("regionByPx");
    public static ImageFeature RegionByPct => new("regionByPct");
    public static ImageFeature RegionSquare => new("regionSquare");

    // Size features
    public static ImageFeature SizeByH => new("sizeByH");
    public static ImageFeature SizeByW => new("sizeByW");
    public static ImageFeature SizeByPct => new("sizeByPct");
    public static ImageFeature SizeByWh => new("sizeByWh");
    public static ImageFeature SizeByConfinedWh => new("sizeByConfinedWh");
    public static ImageFeature SizeUpscaling => new("sizeUpscaling");

    // Rotation features
    public static ImageFeature RotationBy90s => new("rotationBy90s");
    public static ImageFeature RotationArbitrary => new("rotationArbitrary");
    public static ImageFeature Mirroring => new("mirroring");

    // Other features
    public static ImageFeature BaseUriRedirect => new("baseUriRedirect");
    public static ImageFeature Cors => new("cors");
    public static ImageFeature JsonldMediaType => new("jsonldMediaType");
    public static ImageFeature ProfileLinkHeader => new("profileLinkHeader");
    public static ImageFeature CanonicalLinkHeader => new("canonicalLinkHeader");
}