using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Text Granularity Extension - indicates the level of text granularity for annotations.
    /// Used to specify whether text content represents a page, line, word, etc.
    /// </summary>
    [TextGranularityExtension("1.0", Notes = "Text Granularity Extension for indicating OCR/text granularity levels.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<TextGranularity>))]
    public class TextGranularity : ValuableItem<TextGranularity>
    {
        public TextGranularity(string value) : base(value)
        {
        }

        // Text granularity levels as defined in the IIIF Text Granularity Extension
        public static TextGranularity Page => new TextGranularity("page");
        public static TextGranularity Block => new TextGranularity("block");
        public static TextGranularity Paragraph => new TextGranularity("paragraph");
        public static TextGranularity Line => new TextGranularity("line");
        public static TextGranularity Word => new TextGranularity("word");
        public static TextGranularity Glyph => new TextGranularity("glyph");
    }
}