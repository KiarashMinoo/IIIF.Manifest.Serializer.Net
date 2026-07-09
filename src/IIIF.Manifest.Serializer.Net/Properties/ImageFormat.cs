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
    [JsonConverter(typeof(ValuableItemJsonConverter<ImageFormat>))]
    public class ImageFormat(string value) : ValuableItem<ImageFormat>(value)
    {
        // Common image formats
        public static ImageFormat Jpg => "image/jpeg";
        public static ImageFormat Png => "image/png";
        public static ImageFormat Gif => "image/gif";
        public static ImageFormat Webp => "image/webp";
        public static ImageFormat Tif => "image/tiff";
        public static ImageFormat Jp2 => "image/jp2";
        public static ImageFormat Pdf => "application/pdf";
        public static ImageFormat Avif => "image/avif";
        public static ImageFormat Heic => "image/heic";

        public static implicit operator ImageFormat(string value) => new(value);
    }
}
