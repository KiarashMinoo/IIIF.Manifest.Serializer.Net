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
    public class ImageFormat : ValuableItem<ImageFormat>
    {
        public ImageFormat(string value) : base(value)
        {
        }

        // Common image formats
        public static ImageFormat Jpg => new ImageFormat("jpg");
        public static ImageFormat Png => new ImageFormat("png");
        public static ImageFormat Gif => new ImageFormat("gif");
        public static ImageFormat Webp => new ImageFormat("webp");
        public static ImageFormat Tif => new ImageFormat("tif");
        public static ImageFormat Jp2 => new ImageFormat("jp2");
        public static ImageFormat Pdf => new ImageFormat("pdf");
        public static ImageFormat Avif => new ImageFormat("avif");
        public static ImageFormat Heic => new ImageFormat("heic");
    }
}
