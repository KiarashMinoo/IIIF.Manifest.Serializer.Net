using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;

public sealed class ImageResource : BaseResource<ImageResource>, IDimensionSupport<ImageResource>
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private ImageResource()
    {
    }

    [JsonConstructor]
    private ImageResource(string id, string format) : base(id, ResourceType.Image)
    {
        SetFormat(format);
    }

    public ImageResource(string id, ImageFormat format) : this(id, format.Value)
    {
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

    public ImageResource SetHeight(int height)
    {
        Height = height;
        return this;
    }

    public ImageResource SetWidth(int width)
    {
        Width = width;
        return this;
    }
}