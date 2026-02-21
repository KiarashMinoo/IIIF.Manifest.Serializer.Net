using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.TileProperty
{
    [JsonConverter(typeof(TileJsonConverter))]
    public class Tile : TrackableObject<Tile>
    {
        public const string WidthJName = "width";
        public const string ScaleFactorsJName = "scaleFactors";

        [JsonProperty(WidthJName)] public int? Width => GetElementValue(x => x.Width);

        [JsonProperty(ScaleFactorsJName)] public IReadOnlyCollection<int> ScaleFactors => GetElementValue(x => x.ScaleFactors) ?? [];

        public Tile SetWidth(int width) => SetElementValue(a => a.Width, width);

        public Tile AddScaleFactor(int scaleFactor) => SetElementValue(a => ScaleFactors, collection => collection.With(scaleFactor));
        public Tile RemoveScaleFactor(int scaleFactor) => SetElementValue(a => ScaleFactors, collection => collection.Without(scaleFactor));
    }
}