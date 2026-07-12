using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF Presentation API behavior values.
///     Replaces viewingHint in API 3.0 with expanded capabilities.
/// </summary>
[PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x. Some values also valid in 2.x as viewingHint.")]
[JsonConverter(typeof(ValuableItemJsonConverter<Behavior>))]
public class Behavior : ValuableItem<Behavior>
{
    public Behavior(string value) : base(value)
    {
    }

    // Layout behaviors
    public static Behavior Paged => new("paged");
    public static Behavior Continuous => new("continuous");
    public static Behavior Individuals => new("individuals");
    public static Behavior Unordered => new("unordered");

    // Canvas behaviors
    public static Behavior FacingPages => new("facing-pages");
    public static Behavior NonPaged => new("non-paged");

    // Range behaviors
    public static Behavior Sequence => new("sequence");
    public static Behavior ThumbnailNav => new("thumbnail-nav");
    public static Behavior NoNav => new("no-nav");

    // Temporal behaviors
    public static Behavior AutoAdvance => new("auto-advance");
    public static Behavior NoAutoAdvance => new("no-auto-advance");
    public static Behavior Repeat => new("repeat");
    public static Behavior NoRepeat => new("no-repeat");

    // Collection behaviors
    public static Behavior MultiPart => new("multi-part");
    public static Behavior Together => new("together");

    // Annotation behaviors
    public static Behavior Hidden => new("hidden");
}