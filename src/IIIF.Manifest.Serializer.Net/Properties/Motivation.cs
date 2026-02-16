using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Presentation API motivation values for annotations.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Motivation values. sc:painting in 2.x, painting in 3.0.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Motivation>))]
    public class Motivation : ValuableItem<Motivation>
    {
        public Motivation(string value) : base(value)
        {
        }

        // IIIF Presentation motivations
        public static Motivation Painting => new Motivation("painting");
        public static Motivation Supplementing => new Motivation("supplementing");

        // Web Annotation motivations
        public static Motivation Commenting => new Motivation("commenting");
        public static Motivation Describing => new Motivation("describing");
        public static Motivation Tagging => new Motivation("tagging");
        public static Motivation Classifying => new Motivation("classifying");
        public static Motivation Linking => new Motivation("linking");
        public static Motivation Identifying => new Motivation("identifying");
        public static Motivation Bookmarking => new Motivation("bookmarking");
        public static Motivation Highlighting => new Motivation("highlighting");
        public static Motivation Editing => new Motivation("editing");
        public static Motivation Replying => new Motivation("replying");
        public static Motivation Assessing => new Motivation("assessing");
        public static Motivation Moderating => new Motivation("moderating");
        public static Motivation Questioning => new Motivation("questioning");

        // IIIF 2.0 compatibility (sc: prefix)
        public static Motivation ScPainting => new Motivation("sc:painting");
    }
}
