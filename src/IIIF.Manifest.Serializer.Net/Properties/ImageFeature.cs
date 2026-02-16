using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Image API feature values.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [ImageAPI("2.0", Notes = "Supported in both Image API 2.x and 3.0. Feature names consistent across versions.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<ImageFeature>))]
    public class ImageFeature : ValuableItem<ImageFeature>
    {
        public ImageFeature(string value) : base(value)
        {
        }

        // Region features
        public static ImageFeature RegionByPx => new ImageFeature("regionByPx");
        public static ImageFeature RegionByPct => new ImageFeature("regionByPct");
        public static ImageFeature RegionSquare => new ImageFeature("regionSquare");

        // Size features
        public static ImageFeature SizeByH => new ImageFeature("sizeByH");
        public static ImageFeature SizeByW => new ImageFeature("sizeByW");
        public static ImageFeature SizeByPct => new ImageFeature("sizeByPct");
        public static ImageFeature SizeByWh => new ImageFeature("sizeByWh");
        public static ImageFeature SizeByConfinedWh => new ImageFeature("sizeByConfinedWh");
        public static ImageFeature SizeUpscaling => new ImageFeature("sizeUpscaling");

        // Rotation features
        public static ImageFeature RotationBy90s => new ImageFeature("rotationBy90s");
        public static ImageFeature RotationArbitrary => new ImageFeature("rotationArbitrary");
        public static ImageFeature Mirroring => new ImageFeature("mirroring");

        // Other features
        public static ImageFeature BaseUriRedirect => new ImageFeature("baseUriRedirect");
        public static ImageFeature Cors => new ImageFeature("cors");
        public static ImageFeature JsonldMediaType => new ImageFeature("jsonldMediaType");
        public static ImageFeature ProfileLinkHeader => new ImageFeature("profileLinkHeader");
        public static ImageFeature CanonicalLinkHeader => new ImageFeature("canonicalLinkHeader");
    }
}
