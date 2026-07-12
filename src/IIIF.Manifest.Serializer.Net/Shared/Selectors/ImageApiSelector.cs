using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Selectors;

/// <summary>
///     An "ImageApiSelector" (IIIF Presentation API Annex) - selects a region/size/rotation/quality/
///     format of an Image API resource using the same syntax as an Image API request URI's parameters.
///     All fields are optional; only the ones actually constraining the selection need be present.
/// </summary>
public class ImageApiSelector : TrackableObject<ImageApiSelector>, ISelector
{
    public const string TypeJName = "type";
    public const string RegionJName = "region";
    public const string SizeJName = "size";
    public const string RotationJName = "rotation";
    public const string QualityJName = "quality";
    public const string FormatJName = "format";

    [JsonConstructor]
    public ImageApiSelector()
    {
        Type = "ImageApiSelector";
    }

    [JsonProperty(RegionJName)]
    public string? Region
    {
        get => GetElementValue(x => x.Region);
        private set => SetElementValue(value);
    }

    [JsonProperty(SizeJName)]
    public string? Size
    {
        get => GetElementValue(x => x.Size);
        private set => SetElementValue(value);
    }

    [JsonProperty(RotationJName)]
    public string? Rotation
    {
        get => GetElementValue(x => x.Rotation);
        private set => SetElementValue(value);
    }

    [JsonProperty(QualityJName)]
    public string? Quality
    {
        get => GetElementValue(x => x.Quality);
        private set => SetElementValue(value);
    }

    [JsonProperty(FormatJName)]
    public string? Format
    {
        get => GetElementValue(x => x.Format);
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "ImageApiSelector";
        private set => SetElementValue(value);
    }

    public ImageApiSelector SetRegion(string region)
    {
        Region = region;
        return this;
    }

    public ImageApiSelector SetRegion(int x, int y, int width, int height)
    {
        return SetRegion($"{x},{y},{width},{height}");
    }

    public ImageApiSelector SetSize(string size)
    {
        Size = size;
        return this;
    }

    public ImageApiSelector SetRotation(string rotation)
    {
        Rotation = rotation;
        return this;
    }

    public ImageApiSelector SetQuality(string quality)
    {
        Quality = quality;
        return this;
    }

    public ImageApiSelector SetFormat(string format)
    {
        Format = format;
        return this;
    }
}