using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Selectors;

/// <summary>
/// A W3C "PointSelector" - selects either a spatial point (<see cref="X"/>/<see cref="Y"/>, pixel
/// coordinates on an image) or a temporal point (<see cref="T"/>, seconds into an AV recording).
/// The cookbook uses the same <c>type</c> value for both shapes; only the populated fields differ.
/// </summary>
public class PointSelector : TrackableObject<PointSelector>, ISelector
{
    public const string TypeJName = "type";
    public const string XJName = "x";
    public const string YJName = "y";
    public const string TJName = "t";

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "PointSelector";
        private set => SetElementValue(value);
    }

    [JsonProperty(XJName)]
    public int? X
    {
        get => GetElementValue(x => x.X);
        private set => SetElementValue(value);
    }

    [JsonProperty(YJName)]
    public int? Y
    {
        get => GetElementValue(x => x.Y);
        private set => SetElementValue(value);
    }

    [JsonProperty(TJName)]
    public double? T
    {
        get => GetElementValue(x => x.T);
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    private PointSelector()
    {
        Type = "PointSelector";
    }

    public static PointSelector ForSpatialPoint(int x, int y) => new() { X = x, Y = y };

    public static PointSelector ForTemporalPoint(double t) => new() { T = t };
}
