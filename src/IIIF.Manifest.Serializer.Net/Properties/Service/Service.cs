using System.Collections.Generic;
using System.Linq;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Service
{
    /// <summary>
    /// IIIF Image API Service descriptor - provides info about image server capabilities.
    /// </summary>
    [ImageAPI("2.0", Notes = "Service descriptor for Image API. Properties vary between 2.x and 3.0.")]
    [JsonConverter(typeof(ServiceJsonConverter))]
    public class Service : BaseItem<Service>, IDimenssionSupport<Service>
    {
        public const string ProfileJName = "profile";
        public const string TilesJName = "tiles";
        public const string SizesJName = "sizes";
        public const string MaxWidthJName = "maxWidth";
        public const string MaxHeightJName = "maxHeight";
        public const string MaxAreaJName = "maxArea";
        public const string RightsJName = "rights";
        public const string PreferredFormatsJName = "preferredFormats";
        public const string ExtraQualitiesJName = "extraQualities";
        public const string ExtraFeaturesJName = "extraFeatures";

        private readonly List<Tile.Tile> tiles = new List<Tile.Tile>();
        private readonly List<Size.Size> sizes = new List<Size.Size>();
        private readonly List<ImageFormat> preferredFormats = new List<ImageFormat>();
        private readonly List<ImageQuality> extraQualities = new List<ImageQuality>();
        private readonly List<ImageFeature> extraFeatures = new List<ImageFeature>();


        [ImageAPI("2.0")]
        [JsonProperty(ProfileJName)]
        public string Profile { get; }

        [ImageAPI("2.0")]
        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; private set; }

        [ImageAPI("2.0")]
        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; private set; }

        [ImageAPI("2.0")]
        [JsonProperty(TilesJName)]
        public IReadOnlyCollection<Tile.Tile> Tiles => tiles.AsReadOnly();

        [ImageAPI("2.0")]
        [JsonProperty(SizesJName)]
        public IReadOnlyCollection<Size.Size> Sizes => sizes.AsReadOnly();

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxWidthJName)]
        public int? MaxWidth { get; private set; }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxHeightJName)]
        public int? MaxHeight { get; private set; }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxAreaJName)]
        public long? MaxArea { get; private set; }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(RightsJName)]
        public Rights Rights { get; private set; }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(PreferredFormatsJName)]
        public IReadOnlyCollection<ImageFormat> PreferredFormats => preferredFormats.AsReadOnly();

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(ExtraQualitiesJName)]
        public IReadOnlyCollection<ImageQuality> ExtraQualities => extraQualities.AsReadOnly();

        [JsonProperty(ExtraFeaturesJName)]
        public IReadOnlyCollection<ImageFeature> ExtraFeatures => extraFeatures.AsReadOnly();

        public Service(string context, string id, string profile) : base(id, string.Empty, context)
        {
            Profile = profile;
        }

        public Service SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public Service SetWidth(int width) => SetPropertyValue(a => a.Width, width);

        public Service AddTile(Tile.Tile tile) => SetPropertyValue(a => a.tiles, a => a.Tiles, tiles.Attach(tile));
        public Service RemoveTile(Tile.Tile tile) => SetPropertyValue(a => a.tiles, a => a.Tiles, tiles.Detach(tile));

        public Service AddSize(Size.Size size) => SetPropertyValue(a => a.sizes, a => a.Sizes, sizes.Attach(size));
        public Service RemoveSize(Size.Size size) => SetPropertyValue(a => a.sizes, a => a.Sizes, sizes.Detach(size));

        public Service SetMaxWidth(int maxWidth) => SetPropertyValue(a => a.MaxWidth, maxWidth);
        public Service SetMaxHeight(int maxHeight) => SetPropertyValue(a => a.MaxHeight, maxHeight);
        public Service SetMaxArea(long maxArea) => SetPropertyValue(a => a.MaxArea, maxArea);
        
        public Service SetRights(string rights) => SetPropertyValue(a => a.Rights, new Rights(rights));
        public Service SetRights(Rights rights) => SetPropertyValue(a => a.Rights, rights);

        // Value object overloads for preferred formats
        public Service AddPreferredFormat(ImageFormat format) => SetPropertyValue(a => a.preferredFormats, a => a.PreferredFormats, preferredFormats.Attach(format));
        public Service AddPreferredFormat(string format) => AddPreferredFormat(new ImageFormat(format));
        public Service RemovePreferredFormat(ImageFormat format) => SetPropertyValue(a => a.preferredFormats, a => a.PreferredFormats, preferredFormats.Detach(format));

        // Value object overloads for extra qualities
        public Service AddExtraQuality(ImageQuality quality) => SetPropertyValue(a => a.extraQualities, a => a.ExtraQualities, extraQualities.Attach(quality));
        public Service AddExtraQuality(string quality) => AddExtraQuality(new ImageQuality(quality));
        public Service RemoveExtraQuality(ImageQuality quality) => SetPropertyValue(a => a.extraQualities, a => a.ExtraQualities, extraQualities.Detach(quality));

        // Value object overloads for extra features
        public Service AddExtraFeature(ImageFeature feature) => SetPropertyValue(a => a.extraFeatures, a => a.ExtraFeatures, extraFeatures.Attach(feature));
        public Service AddExtraFeature(string feature) => AddExtraFeature(new ImageFeature(feature));
        public Service RemoveExtraFeature(ImageFeature feature) => SetPropertyValue(a => a.extraFeatures, a => a.ExtraFeatures, extraFeatures.Detach(feature));
    }
}