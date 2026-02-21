using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Transformation parameters for georeferencing.
/// </summary>
public class Transformation : TrackableObject<Transformation>
{
    /// <summary>
    /// Transformation type (e.g., "polynomial", "helmert").
    /// </summary>
    [JsonProperty("type")]
    public string Type => GetElementValue<string>(x => x.Type)!;

    /// <summary>
    /// Transformation parameters as an array of coefficients.
    /// </summary>
    [JsonProperty("options")]
    public object Options => GetElementValue<object>(x => x.Options)!;

    public Transformation(string type, object options)
    {
        SetElementValue(x => x.Type, type);
        SetElementValue(x => x.Options, options);
    }
}