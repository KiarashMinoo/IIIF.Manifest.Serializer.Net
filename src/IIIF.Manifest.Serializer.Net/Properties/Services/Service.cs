using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Services
{
    /// <summary>
    /// IIIF Image API Service descriptor - provides info about image server capabilities.
    /// </summary>
    [ImageAPI("2.0", Notes = "Service descriptor for Image API. Properties vary between 2.x and 3.0.")]
    public class Service : BaseItem<Service>, IDimensionSupport<Service>, IBaseService
    {
        public const string ProtocolJName = "protocol";
        public const string TilesJName = "tiles";
        public const string SizesJName = "sizes";
        public const string MaxWidthJName = "maxWidth";
        public const string MaxHeightJName = "maxHeight";
        public const string MaxAreaJName = "maxArea";
        public const string RightsJName = "rights";
        public const string PreferredFormatsJName = "preferredFormats";
        public const string ExtraFormatsJName = "extraFormats";
        public const string ExtraQualitiesJName = "extraQualities";
        public const string ExtraFeaturesJName = "extraFeatures";

        /// <summary>
        /// Fixed per spec (both 2.x and 3.0). Read-only - a service that isn't compliant with the
        /// Image API protocol has no reason to carry this descriptor at all.
        /// </summary>
        [ImageAPI("2.0")]
        [JsonProperty(ProtocolJName)]
        public string Protocol => "http://iiif.io/api/image";

        [ImageAPI("2.0")]
        [JsonProperty(IBaseService.ProfileJName)]
        public string Profile
        {
            get => GetElementValue(x => x.Profile)!;
            private set => SetElementValue(value);
        }

        [ImageAPI("2.0")]
        [JsonProperty(Constants.HeightJName)]
        public int? Height
        {
            get => GetElementValue(x => x.Height);
            private set => SetElementValue(value);
        }

        [ImageAPI("2.0")]
        [JsonProperty(Constants.WidthJName)]
        public int? Width
        {
            get => GetElementValue(x => x.Width);
            private set => SetElementValue(value);
        }

        [ImageAPI("2.0")]
        [JsonProperty(TilesJName)]
        public IReadOnlyCollection<Tile> Tiles
        {
            get => GetElementValue(x => x.Tiles) ?? [];
            private set => SetElementValue(value);
        }

        [ImageAPI("2.0")]
        [JsonProperty(SizesJName)]
        public IReadOnlyCollection<Size> Sizes
        {
            get => GetElementValue(x => x.Sizes) ?? [];
            private set => SetElementValue(value);
        }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxWidthJName)]
        public int? MaxWidth
        {
            get => GetElementValue(x => x.MaxWidth);
            private set => SetElementValue(value);
        }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxHeightJName)]
        public int? MaxHeight
        {
            get => GetElementValue(x => x.MaxHeight);
            private set => SetElementValue(value);
        }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(MaxAreaJName)]
        public long? MaxArea
        {
            get => GetElementValue(x => x.MaxArea);
            private set => SetElementValue(value);
        }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(RightsJName)]
        public Rights? Rights
        {
            get => GetElementValue(x => x.Rights);
            private set => SetElementValue(value);
        }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(PreferredFormatsJName)]
        public IReadOnlyCollection<ImageFormat> PreferredFormats
        {
            get => GetElementValue(x => x.PreferredFormats) ?? [];
            private set => SetElementValue(value);
        }

        [ImageAPI("2.0", Notes = "Additional formats beyond the profile's defaults.")]
        [JsonProperty(ExtraFormatsJName)]
        public IReadOnlyCollection<ImageFormat> ExtraFormats
        {
            get => GetElementValue(x => x.ExtraFormats) ?? [];
            private set => SetElementValue(value);
        }

        [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
        [JsonProperty(ExtraQualitiesJName)]
        public IReadOnlyCollection<ImageQuality> ExtraQualities
        {
            get => GetElementValue(x => x.ExtraQualities) ?? [];
            private set => SetElementValue(value);
        }

        [JsonProperty(ExtraFeaturesJName)]
        public IReadOnlyCollection<ImageFeature> ExtraFeatures
        {
            get => GetElementValue(x => x.ExtraFeatures) ?? [];
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        private Service(string id, string profile) : base(id, "ImageService3")
        {
            SetElementValue(x => x.Profile, profile);
        }

        /// <summary>
        /// Creates an Image API service descriptor. Defaults to <c>type: "ImageService3"</c> -
        /// call <see cref="AsImageService2"/> when authoring a 2.x-oriented document, since this
        /// class's <c>type</c> is genuinely version-dependent (unlike other services, which map to
        /// exactly one spec version) and can't be inferred from context alone.
        /// </summary>
        public Service(string context, string id, string profile) : base(id, "ImageService3", context)
        {
            SetElementValue(x => x.Profile, profile);
        }

        public Service AsImageService2() => SetType("ImageService2");
        public Service AsImageService3() => SetType("ImageService3");

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

        // Value object overloads for extra formats
        public Service AddExtraFormat(ImageFormat format) => SetElementValue(a => a.ExtraFormats, collection => collection.With(format));
        public Service AddExtraFormat(string format) => AddExtraFormat(new ImageFormat(format));
        public Service RemoveExtraFormat(ImageFormat format) => SetElementValue(a => a.ExtraFormats, collection => collection.Without(format));

        // Value object overloads for extra qualities
        public Service AddExtraQuality(ImageQuality quality) => SetElementValue(a => a.ExtraQualities, collection => collection.With(quality));
        public Service AddExtraQuality(string quality) => AddExtraQuality(new ImageQuality(quality));
        public Service RemoveExtraQuality(ImageQuality quality) => SetElementValue(a => a.ExtraQualities, collection => collection.With(quality));

        // Value object overloads for extra features
        public Service AddExtraFeature(ImageFeature feature) => SetElementValue(a => a.ExtraFeatures, collection => collection.With(feature));
        public Service AddExtraFeature(string feature) => AddExtraFeature(new ImageFeature(feature));
        public Service RemoveExtraFeature(ImageFeature feature) => SetElementValue(a => a.ExtraFeatures, collection => collection.Without(feature));

        /// <summary>
        /// Serializes as a standalone <c>info.json</c> document - unprefixed <c>id</c>/<c>type</c>
        /// (per spec, distinct from the <c>@id</c>/<c>@type</c> this class uses when embedded
        /// inline in a Presentation resource's "service" property for 2.x-era backwards
        /// compatibility). <c>@context</c> stays prefixed either way.
        /// </summary>
        public string ToInfoJson()
        {
            var obj = JObject.Parse(Serialize());
            if (obj["@id"] is { } idToken)
            {
                obj.Remove("@id");
                obj["id"] = idToken;
            }

            if (obj["@type"] is { } typeToken)
            {
                obj.Remove("@type");
                obj["type"] = typeToken;
            }

            return obj.ToString(Formatting.Indented);
        }
    }
}