using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Image API format values.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [ImageAPI("2.0", Notes = "Supported in both Image API 2.x and 3.0")]
    public class ImageFormat(string value) : ValuableItem<ImageFormat>(value)
    {
        // Common image formats
        public static ImageFormat Jpg => "jpg";
        public static ImageFormat Png => "png";
        public static ImageFormat Gif => "gif";
        public static ImageFormat Webp => "webp";
        public static ImageFormat Tif => "tif";
        public static ImageFormat Jp2 => "jp2";
        public static ImageFormat Pdf => "pdf";
        public static ImageFormat Avif => "avif";
        public static ImageFormat Heic => "heic";

        public static implicit operator ImageFormat(string value) => new(value);
    }
}