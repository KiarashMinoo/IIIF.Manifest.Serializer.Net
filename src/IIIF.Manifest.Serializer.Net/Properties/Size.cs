using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    public class Size : TrackableObject<Size>
    {
        public const string WidthJName = "width";
        public const string HeightJName = "height";

        [JsonProperty(WidthJName)]
        public int Width
        {
            get => GetElementValue(x => x.Width);
            private set => SetElementValue(value);
        }

        [JsonProperty(HeightJName)]
        public int Height
        {
            get => GetElementValue(x => x.Height);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}