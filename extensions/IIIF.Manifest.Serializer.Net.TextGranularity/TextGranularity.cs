using System;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Text granularity levels for IIIF Text Granularity extension.
    /// Defines the level of text segmentation available in OCR or text content.
    /// </summary>
    [JsonConverter(typeof(TextGranularityJsonConverter))]
    public class TextGranularity : ValuableItem<TextGranularity>
    {
        /// <summary>
        /// Character-level text granularity for individual character positioning.
        /// </summary>
        public static readonly TextGranularity Character = new TextGranularity("character");

        /// <summary>
        /// Word-level text granularity for word-based operations.
        /// </summary>
        public static readonly TextGranularity Word = new TextGranularity("word");

        /// <summary>
        /// Line-level text granularity for line-based text display.
        /// </summary>
        public static readonly TextGranularity Line = new TextGranularity("line");

        /// <summary>
        /// Block-level text granularity for paragraph or region text.
        /// </summary>
        public static readonly TextGranularity Block = new TextGranularity("block");

        /// <summary>
        /// Page-level text granularity for full page text content.
        /// </summary>
        public static readonly TextGranularity Page = new TextGranularity("page");

        private TextGranularity(string value) : base(value)
        {
        }

        /// <summary>
        /// Parse a string value to a TextGranularity instance.
        /// </summary>
        public static TextGranularity Parse(string value)
        {
            return value?.ToLowerInvariant() switch
            {
                "character" => Character,
                "word" => Word,
                "line" => Line,
                "block" => Block,
                "page" => Page,
                _ => throw new ArgumentException($"Unknown text granularity: {value}", nameof(value))
            };
        }

        /// <summary>
        /// Try to parse a string value to a TextGranularity instance.
        /// </summary>
        public static bool TryParse(string value, out TextGranularity result)
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