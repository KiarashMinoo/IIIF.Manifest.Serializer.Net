using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;

/// <summary>
///     IIIF Video resource for A/V content.
/// </summary>
public sealed class VideoResource : BaseResource<VideoResource>, IDimensionSupport<VideoResource>
{
    public const string DurationJName = "duration";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private VideoResource()
    {
    }

    public VideoResource(string id, string format) : base(id, ResourceType.Video)
    {
        SetFormat(format);
    }

    [JsonProperty(DurationJName)]
    public double? Duration
    {
        get => GetElementValue(x => x.Duration);
        private set => SetElementValue(value);
    }

    [JsonProperty(Constants.HeightJName)]
    public int? Height
    {
        get => GetElementValue(x => x.Height);
        private set => SetElementValue(value);
    }

    [JsonProperty(Constants.WidthJName)]
    public int? Width
    {
        get => GetElementValue(x => x.Width);
        private set => SetElementValue(value);
    }

    public VideoResource SetHeight(int height)
    {
        Height = height;
        return this;
    }

    public VideoResource SetWidth(int width)
    {
        Width = width;
        return this;
    }

    public VideoResource SetDuration(double duration)
    {
        Duration = duration;
        return this;
    }
}