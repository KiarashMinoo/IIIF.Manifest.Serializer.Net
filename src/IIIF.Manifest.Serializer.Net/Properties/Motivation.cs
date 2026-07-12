using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF Presentation API motivation values for annotations.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[PresentationAPI("2.0", Notes = "Motivation values. sc:painting in 2.x, painting in 3.0.")]
[JsonConverter(typeof(ValuableItemJsonConverter<Motivation>))]
public class Motivation(string value) : ValuableItem<Motivation>(value)
{
    // IIIF Presentation motivations
    public static Motivation Painting => new("painting");
    public static Motivation Supplementing => new("supplementing");

    // Web Annotation motivations
    public static Motivation Commenting => new("commenting");
    public static Motivation Describing => new("describing");
    public static Motivation Tagging => new("tagging");
    public static Motivation Classifying => new("classifying");
    public static Motivation Linking => new("linking");
    public static Motivation Identifying => new("identifying");
    public static Motivation Bookmarking => new("bookmarking");
    public static Motivation Highlighting => new("highlighting");
    public static Motivation Editing => new("editing");
    public static Motivation Replying => new("replying");
    public static Motivation Assessing => new("assessing");
    public static Motivation Moderating => new("moderating");
    public static Motivation Questioning => new("questioning");

    // IIIF 2.0 compatibility (sc: prefix)
    public static Motivation ScPainting => new("sc:painting");
}