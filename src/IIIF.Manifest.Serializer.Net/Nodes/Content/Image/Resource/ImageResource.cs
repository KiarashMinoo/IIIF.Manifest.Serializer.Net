using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    [JsonConverter(typeof(ImageResourceJsonConverter))]
    public class ImageResource : BaseResource<ImageResource>, IDimenssionSupport<ImageResource>
    {
        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; private set; }

        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; private set; }

        public ImageResource(string id, string format) : base(id, "dctypes:Image")
        {
            SetFormat(format);
        }

        public ImageResource SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public ImageResource SetWidth(int width) => SetPropertyValue(a => a.Width, width);
    }
}