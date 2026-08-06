using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable.Objects;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

public sealed class Tile : TrackableObject<Tile>
{
    public const string WidthJName = "width";
    public const string HeightJName = "height";
    public const string ScaleFactorsJName = "scaleFactors";

    /// <summary>
    ///     Also the type's normal fluent-builder entry point (<c>new Tile().SetWidth(...)</c>) - kept
    ///     <c>public</c> rather than private since it's genuine application-facing API, not just an
    ///     EF Core/ORM materialization backdoor.
    /// </summary>
    public Tile()
    {
    }

    [JsonProperty(WidthJName)]
    public int? Width
    {
        get => GetElementValue(x => x.Width);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Optional per spec - defaults to <see cref="Width" /> when omitted.
    /// </summary>
    [JsonProperty(HeightJName)]
    public int? Height
    {
        get => GetElementValue(x => x.Height);
        private set => SetElementValue(value);
    }

    [JsonProperty(ScaleFactorsJName)]
    public IReadOnlyCollection<int> ScaleFactors
    {
        get => GetElementValue(x => x.ScaleFactors);
        private set => SetElementValue(value);
    }

    public Tile SetWidth(int width)
    {
        Width = width;
        return this;
    }

    public Tile SetHeight(int height)
    {
        Height = height;
        return this;
    }

    public Tile AddScaleFactor(int scaleFactor)
    {
        ScaleFactors = ScaleFactors.With(scaleFactor);
        return this;
    }

    public Tile RemoveScaleFactor(int scaleFactor)
    {
        ScaleFactors = ScaleFactors.Without(scaleFactor);
        return this;
    }
}