using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

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
}