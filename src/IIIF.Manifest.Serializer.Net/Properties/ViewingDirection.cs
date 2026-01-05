using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(ValuableItemJsonConverter<ViewingDirection>))]
    public class ViewingDirection : ValuableItem<ViewingDirection>
    {
        public ViewingDirection(string value) : base(value)
        {
        }

        public static ViewingDirection Ltr => new ViewingDirection("left-to-right");
        public static ViewingDirection Rtl => new ViewingDirection("right-to-left");
        public static ViewingDirection Ttb => new ViewingDirection("top-to-bottom");
        public static ViewingDirection Btt => new ViewingDirection("bottom-to-top");
    }
}