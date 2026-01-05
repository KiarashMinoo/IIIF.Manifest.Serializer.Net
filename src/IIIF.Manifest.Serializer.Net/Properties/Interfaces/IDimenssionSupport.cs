using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Interfaces

{
    public interface IDimenssionSupport<TItem>
        where TItem : BaseItem<TItem>, IDimenssionSupport<TItem>
    {
        [JsonProperty(Constants.HeightJName)]
        int? Height { get; }

        [JsonProperty(Constants.WidthJName)]
        int? Width { get; }


        TItem SetHeight(int height);
        TItem SetWidth(int width);
    }

    public static class IDimenssionSupportHelper
    {
        public static T SetHeight<T>(this T item, JToken element) where T : BaseItem<T>, IDimenssionSupport<T>
        {
            var jHeight = element.TryGetToken(Constants.HeightJName);
            if (jHeight != null)
                item.SetHeight(jHeight.Value<int>());

            return item;
        }

        public static T SetWidth<T>(this T item, JToken element) where T : BaseItem<T>, IDimenssionSupport<T>
        {
            var jWidth = element.TryGetToken(Constants.WidthJName);
            if (jWidth != null)
                item.SetWidth(jWidth.Value<int>());

            return item;
        }
    }
}