using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A ground control point relating image pixels to geographic coordinates.
/// </summary>
public class GroundControlPoint : TrackableObject<GroundControlPoint>
{
    /// <summary>
    /// Image coordinates (x, y) in pixels.
    /// </summary>
    [JsonProperty("image")]
    public IReadOnlyCollection<double> Image => GetElementValue(x => x.Image) ?? [];

    /// <summary>
    /// Geographic coordinates (longitude, latitude) or (easting, northing).
    /// </summary>
    [JsonProperty("world")]
    public IReadOnlyCollection<double> World => GetElementValue(x => x.World) ?? [];

    public GroundControlPoint(double imageX, double imageY, double worldX, double worldY)
    {
        SetElementValue(x => x.Image, [imageX, imageY]);
        SetElementValue(x => x.World, [worldX, worldY]);
    }
}