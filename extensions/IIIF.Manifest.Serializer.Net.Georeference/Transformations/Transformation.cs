using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

/// <summary>
/// Transformation parameters for georeferencing.
/// </summary>
public class Transformation : TrackableObject<Transformation>
{
    public const string TransformationJName = "transformation";

    /// <summary>
    /// Transformation type (e.g., "polynomial", "thinPlateSpline").
    /// </summary>
    [JsonProperty("type")]
    public TransformationType Type
    {
        get => GetElementValue<TransformationType>(x => x.Type)!;
        private set => SetElementValue(value);
    }

    public Transformation(TransformationType type)
    {
        Type = type;
    }
}