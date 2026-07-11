using System;
using System.Diagnostics.CodeAnalysis;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Text granularity levels for IIIF Text Granularity extension.
    /// Defines the level of text segmentation available in OCR or text content.
    /// </summary>
    [JsonConverter(typeof(ValuableItemJsonConverter<TextGranularity>))]
    [TextGranularityExtension("3.0")]
    public class TextGranularity : ValuableItem<TextGranularity>
    {
        public const string TextGranularityJName = "textGranularity";

        private TextGranularity(string value) : base(value)
        {
        }

        /// <summary>
        /// A page in a paginated document.
        /// </summary>
        public static readonly TextGranularity Page = new TextGranularity("page");

        /// <summary>
        /// An arbitrary region of text.
        /// </summary>
        public static readonly TextGranularity Block = new TextGranularity("block");

        /// <summary>
        /// A paragraph.
        /// </summary>
        public static readonly TextGranularity Paragraph = new TextGranularity("paragraph");

        /// <summary>
        /// A topographic line.
        /// </summary>
        public static readonly TextGranularity Line = new TextGranularity("line");

        /// <summary>
        /// A single word.
        /// </summary>
        public static readonly TextGranularity Word = new TextGranularity("word");

        /// <summary>
        /// A single glyph or symbol.
        /// </summary>
        public static readonly TextGranularity Glyph = new TextGranularity("glyph");

        /// <summary>
        /// Parse a string value to a TextGranularity instance.
        /// </summary>
        public static TextGranularity Parse(string value)
        {
            return value?.ToLowerInvariant() switch
            {
                "page" => Page,
                "block" => Block,
                "paragraph" => Paragraph,
                "line" => Line,
                "word" => Word,
                "glyph" => Glyph,
                _ => throw new ArgumentException($"Unknown text granularity: {value}", nameof(value))
            };
        }

        /// <summary>
        /// Try to parse a string value to a TextGranularity instance.
        /// </summary>
        public static bool TryParse(string value, [MaybeNullWhen(false)] out TextGranularity result)
        {
            try
            {
                result = Parse(value);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}