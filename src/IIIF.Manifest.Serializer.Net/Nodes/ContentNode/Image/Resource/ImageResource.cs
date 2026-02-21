using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource
{
    [JsonConverter(typeof(ImageResourceJsonConverter))]
    public class ImageResource : BaseResource<ImageResource>, IDimensionSupport<ImageResource>
    {
        [JsonProperty(Constants.HeightJName)] public int? Height => GetElementValue(x => x.Height);

        [JsonProperty(Constants.WidthJName)] public int? Width => GetElementValue(x => x.Width);

        public ImageResource(string id, string format) : base(id, "dctypes:Image")
        {
            SetFormat(format);
        }

        public ImageResource(string id, ImageFormat format) : this(id, format.Value)
        {
        }

        public ImageResource SetHeight(int height) => SetElementValue(a => a.Height, height);
        public ImageResource SetWidth(int width) => SetElementValue(a => a.Width, width);
    }
}