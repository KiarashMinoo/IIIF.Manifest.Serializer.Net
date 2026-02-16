using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Presentation API 2.0 viewing hint.
    /// Provides hints to the client application about how to present the resource.
    /// </summary>
    /// <remarks>
    /// In IIIF Presentation API 3.0, viewingHint is replaced by behavior.
    /// </remarks>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
        ReplacedBy = "behavior", Notes = "viewingHint renamed to behavior in API 3.0")]
    [JsonConverter(typeof(ValuableItemJsonConverter<ViewingHint>))]
    public class ViewingHint : ValuableItem<ViewingHint>
    {
        public ViewingHint(string value) : base(value)
        {
        }

        /// <summary>
        /// Valid on Collection, Manifest. Each canvas represents a separate page/view.
        /// Implies that viewers should show one or few pages at a time.
        /// </summary>
        public static ViewingHint Paged => new ViewingHint("paged");

        /// <summary>
        /// Valid on Collection, Manifest, Range. The resource should be treated as a continuous item.
        /// For example, a scroll that should be displayed linearly.
        /// </summary>
        public static ViewingHint Continuous => new ViewingHint("continuous");

        /// <summary>
        /// Valid on Collection. Individual objects within the collection are not connected.
        /// </summary>
        public static ViewingHint Individuals => new ViewingHint("individuals");

        /// <summary>
        /// Valid on Canvas, Range. Canvases should be displayed simultaneously as facing pages.
        /// </summary>
        public static ViewingHint FacingPages => new ViewingHint("facing-pages");

        /// <summary>
        /// Valid on Canvas. A Canvas that should not be displayed by default.
        /// </summary>
        public static ViewingHint NonPaged => new ViewingHint("non-paged");

        /// <summary>
        /// Valid on Canvas. The first canvas in a paged manifest, usually shown alone.
        /// </summary>
        public static ViewingHint Top => new ViewingHint("top");

        /// <summary>
        /// Valid on Collection, Manifest. The constituent objects are split over multiple files.
        /// </summary>
        public static ViewingHint MultiPart => new ViewingHint("multi-part");
    }
}

