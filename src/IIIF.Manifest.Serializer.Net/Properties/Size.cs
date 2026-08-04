using IIIF.Manifests.Serializer.Shared.Trackable.Objects;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

public sealed class Size : TrackableObject<Size>
{
    public const string WidthJName = "width";
    public const string HeightJName = "height";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Size()
    {
    }

    [JsonConstructor]
    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }

    [JsonProperty(WidthJName)]
    public int Width
    {
        get => GetElementValue(x => x.Width);
        private set => SetElementValue(value);
    }

    [JsonProperty(HeightJName)]
    public int Height
    {
        get => GetElementValue(x => x.Height);
        private set => SetElementValue(value);
    }
}