using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.SizeProperty
{
    [JsonConverter(typeof(SizeJsonConverter))]
    public class Size : TrackableObject<Size>
    {
        public const string WidthJName = "width";
        public const string HeightJName = "height";

        [JsonProperty(WidthJName)] public int Width => GetElementValue(x => x.Width);
        [JsonProperty(HeightJName)] public int Height => GetElementValue(x => x.Height);

        public Size(int width, int height)
        {
            SetElementValue(x => x.Width, width);
            SetElementValue(x => x.Height, height);
        }
    }
}