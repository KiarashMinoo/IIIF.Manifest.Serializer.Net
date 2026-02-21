using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A ground control point relating image pixels to geographic coordinates.
/// </summary>
public class GroundControlPoint
{
    /// <summary>
    /// Image coordinates (x, y) in pixels.
    /// </summary>
    [JsonProperty("image")]
    public double[] Image { get; set; }

    /// <summary>
    /// Geographic coordinates (longitude, latitude) or (easting, northing).
    /// </summary>
    [JsonProperty("world")]
    public double[] World { get; set; }

    public GroundControlPoint(double imageX, double imageY, double worldX, double worldY)
    {
        Image = new[] { imageX, imageY };
        World = new[] { worldX, worldY };
    }
}