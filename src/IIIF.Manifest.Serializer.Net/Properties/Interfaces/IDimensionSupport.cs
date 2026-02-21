using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Interfaces

{
    public interface IDimensionSupport<out TItem>
        where TItem : BaseItem<TItem>, IDimensionSupport<TItem>
    {
        [JsonProperty(Constants.HeightJName)] int? Height { get; }

        [JsonProperty(Constants.WidthJName)] int? Width { get; }


        TItem SetHeight(int height);
        TItem SetWidth(int width);
    }
}