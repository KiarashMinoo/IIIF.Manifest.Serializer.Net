using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(TileJsonConverter))]
    public class Tile : TrackableObject<Tile>
    {
        public const string WidthJName = "width";
        public const string ScaleFactorsJName = "scaleFactors";

        private readonly List<int> scaleFactors = new List<int>();

        [JsonProperty(WidthJName)]
        public int? Width { get; private set; }

        [JsonProperty(ScaleFactorsJName)]
        public IReadOnlyCollection<int> ScaleFactors => scaleFactors.AsReadOnly();

        public Tile SetWidth(int width) => SetPropertyValue(a => a.Width, width);

        public Tile AddScaleFactor(int scaleFactor) => SetPropertyValue(a => scaleFactors, a => ScaleFactors, scaleFactors.Attach(scaleFactor));
        public Tile RemoveScaleFactor(int scaleFactor) => SetPropertyValue(a => scaleFactors, a => ScaleFactors, scaleFactors.Detach(scaleFactor));
    }
}