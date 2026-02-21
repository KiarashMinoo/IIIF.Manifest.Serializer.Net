using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Transformation parameters for georeferencing.
/// </summary>
public class Transformation
{
    /// <summary>
    /// Transformation type (e.g., "polynomial", "helmert").
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    /// <summary>
    /// Transformation parameters as an array of coefficients.
    /// </summary>
    [JsonProperty("options")]
    public object Options { get; set; }

    public Transformation(string type, object options)
    {
        Type = type;
        Options = options;
    }
}