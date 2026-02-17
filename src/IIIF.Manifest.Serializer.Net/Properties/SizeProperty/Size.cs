using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.SizeProperty
{
    [JsonConverter(typeof(SizeJsonConverter))]
    public class Size : TrackableObject<Size>
    {
        public const string WidthJName = "width";
        public const string HeightJName = "height";
        [JsonProperty(WidthJName)]
        public int Width { get; private set; }
        [JsonProperty(HeightJName)]
        public int Height { get; private set; }
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
