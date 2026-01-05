using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Service
{
    [JsonConverter(typeof(ServiceJsonConverter))]
    public class Service : BaseItem<Service>, IDimenssionSupport<Service>
    {
        public const string ProfileJName = "profile";
        public const string TilesJName = "tiles";

        private readonly List<Tile.Tile> tiles = new List<Tile.Tile>();


        [JsonProperty(ProfileJName)]
        public string Profile { get; }

        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; private set; }

        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; private set; }

        [JsonProperty(TilesJName)]
        public IReadOnlyCollection<Tile.Tile> Tiles => tiles.AsReadOnly();

        public Service(string context, string id, string profile) : base(id, string.Empty, context)
        {
            Profile = profile;
        }

        public Service SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public Service SetWidth(int width) => SetPropertyValue(a => a.Width, width);

        public Service AddTile(Tile.Tile tile) => SetPropertyValue(a => a.tiles, a => a.Tiles, tiles.Attach(tile));
        public Service RemoveTile(Tile.Tile tile) => SetPropertyValue(a => a.tiles, a => a.Tiles, tiles.Detach(tile));
    }
}