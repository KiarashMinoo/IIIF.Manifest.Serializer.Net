using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF viewing direction values.
    /// Specifies the direction in which a set of Canvases should be displayed.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Supported in both 2.x and 3.0")]
    public class ViewingDirection(string value) : ValuableItem<ViewingDirection>(value)
    {
        public static ViewingDirection Ltr => new ViewingDirection("left-to-right");
        public static ViewingDirection Rtl => new ViewingDirection("right-to-left");
        public static ViewingDirection Ttb => new ViewingDirection("top-to-bottom");
        public static ViewingDirection Btt => new ViewingDirection("bottom-to-top");
    }
}