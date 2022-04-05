using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Nodes
{
    [JsonConverter(typeof(CanvasJsonConverter))]
    public class Canvas : BaseNode<Canvas>, IDimenssionSupport<Canvas>
    {
        public const string ImagesJName = "images";
        public const string OtherContentsJName = "otherContent";

        private readonly List<Image> images = new List<Image>();
        private readonly List<OtherContent> otherContents = new List<OtherContent>();


        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; }

        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; }

        [JsonProperty(ImagesJName)]
        public IReadOnlyCollection<Image> Images => images.AsReadOnly();

        [JsonProperty(OtherContentsJName)]
        public IReadOnlyCollection<OtherContent> OtherContents => otherContents.AsReadOnly();

        public Canvas(string id, Label label, int height, int width) : base(id, "sc:Canvas")
        {
            SetLabel(new[] { label });
            Height = height;
            Width = width;
        }

        public Canvas AddImage(Image image) => SetPropertyValue(a => a.images, a => a.Images, images.Attach(image));
        public Canvas AddOtherContent(OtherContent otherContent) => SetPropertyValue(a => a.otherContents, a => a.OtherContents, otherContents.Attach(otherContent));

        public Canvas SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public Canvas SetWidth(int width) => SetPropertyValue(a => a.Width, width);
    }
}