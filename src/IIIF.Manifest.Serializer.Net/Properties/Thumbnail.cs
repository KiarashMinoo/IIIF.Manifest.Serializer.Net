using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
public sealed class Thumbnail : FormattableItem<Thumbnail>, IDimensionSupport<Thumbnail>
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Thumbnail()
    {
    }

    [JsonConstructor]
    public Thumbnail(string id) : base(id, "dctypes:Image")
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

    public Thumbnail SetHeight(int height)
    {
        Height = height;
        return this;
    }

    public Thumbnail SetWidth(int width)
    {
        Width = width;
        return this;
    }
}