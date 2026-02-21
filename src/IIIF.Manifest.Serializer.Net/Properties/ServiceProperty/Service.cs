using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Properties.SizeProperty;
using IIIF.Manifests.Serializer.Properties.TileProperty;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    /// <summary>
    /// IIIF Image API Service descriptor - provides info about image server capabilities.
    /// </summary>
    [ImageAPI("2.0", Notes = "Service descriptor for Image API. Properties vary between 2.x and 3.0.")]
    [JsonConverter(typeof(ServiceJsonConverter))]
    public class Service : BaseItem<Service>, IDimensionSupport<Service>, IBaseService
    {
        public const string TilesJName = "tiles";
        public const string SizesJName = "sizes";
        public const string MaxWidthJName = "maxWidth";
        public const string MaxHeightJName = "maxHeight";
        public const string MaxAreaJName = "maxArea";
        public const string RightsJName = "rights";
        public const string PreferredFormatsJName = "preferredFormats";
        public const string ExtraQualitiesJName = "extraQualities";
        public const string ExtraFeaturesJName = "extraFeatures";

        [ImageAPI("2.0")]
        [JsonProperty(IBaseService.ProfileJName)]
        public string Profile => GetElementValue(x => x.Profile)!;

        [ImageAPI("2.0")]
        [JsonProperty(Constants.HeightJName)]
        public int? Height => GetElementValue(x => x.Height);

        [ImageAPI("2.0")]
        [JsonProperty(Constants.WidthJName)]
        public int? Width => GetElementValue(x => x.Width);

        [ImageAPI("2.0")]
        [JsonProperty(TilesJName)]
        public IReadOnlyCollection<Tile> Tiles => GetElementValue(x => x.Tiles) ?? [];

        [ImageAPI("2.0")]
        [JsonProperty(SizesJName)]
        public IReadOnlyCollection<Size> Sizes => GetElementValue(x => x.Sizes) ?? [];

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxWidthJName)]
        public int? MaxWidth => GetElementValue(x => x.MaxWidth);

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxHeightJName)]
        public int? MaxHeight => GetElementValue(x => x.MaxHeight);

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxAreaJName)]
        public long? MaxArea => GetElementValue(x => x.MaxArea);

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(RightsJName)]
        public Rights? Rights => GetElementValue(x => x.Rights);

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(PreferredFormatsJName)]
        public IReadOnlyCollection<ImageFormat> PreferredFormats => GetElementValue(x => x.PreferredFormats) ?? [];

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(ExtraQualitiesJName)]
        public IReadOnlyCollection<ImageQuality> ExtraQualities => GetElementValue(x => x.ExtraQualities) ?? [];

        [JsonProperty(ExtraFeaturesJName)] public IReadOnlyCollection<ImageFeature> ExtraFeatures => GetElementValue(x => x.ExtraFeatures) ?? [];

        public Service(string context, string id, string profile) : base(id, string.Empty, context)
        {
            SetElementValue(x => x.Profile, profile);
        }

        public Service SetHeight(int height) => SetElementValue(a => a.Height, height);
        public Service SetWidth(int width) => SetElementValue(a => a.Width, width);

        public Service AddTile(Tile tile) => SetElementValue(a => a.Tiles, collection => collection.With(tile));
        public Service RemoveTile(Tile tile) => SetElementValue(a => a.Tiles, collection => collection.Without(tile));

        public Service AddSize(Size size) => SetElementValue(a => a.Sizes, collection => collection.With(size));
        public Service RemoveSize(Size size) => SetElementValue(a => a.Sizes, collection => collection.Without(size));

        public Service SetMaxWidth(int maxWidth) => SetElementValue(a => a.MaxWidth, maxWidth);
        public Service SetMaxHeight(int maxHeight) => SetElementValue(a => a.MaxHeight, maxHeight);
        public Service SetMaxArea(long maxArea) => SetElementValue(a => a.MaxArea, maxArea);

        public Service SetRights(string rights) => SetElementValue(a => a.Rights, new Rights(rights));
        public Service SetRights(Rights rights) => SetElementValue(a => a.Rights, rights);

        // Value object overloads for preferred formats
        public Service AddPreferredFormat(ImageFormat format) => SetElementValue(a => a.PreferredFormats, collection => collection.With(format));
        public Service AddPreferredFormat(string format) => AddPreferredFormat(new ImageFormat(format));
        public Service RemovePreferredFormat(ImageFormat format) => SetElementValue(a => a.PreferredFormats, collection => collection.Without(format));

        // Value object overloads for extra qualities
        public Service AddExtraQuality(ImageQuality quality) => SetElementValue(a => a.ExtraQualities, collection => collection.With(quality));
        public Service AddExtraQuality(string quality) => AddExtraQuality(new ImageQuality(quality));
        public Service RemoveExtraQuality(ImageQuality quality) => SetElementValue(a => a.ExtraQualities, collection => collection.With(quality));

        // Value object overloads for extra features
        public Service AddExtraFeature(ImageFeature feature) => SetElementValue(a => a.ExtraFeatures, collection => collection.With(feature));
        public Service AddExtraFeature(string feature) => AddExtraFeature(new ImageFeature(feature));
        public Service RemoveExtraFeature(ImageFeature feature) => SetElementValue(a => a.ExtraFeatures, collection => collection.Without(feature));
    }
}