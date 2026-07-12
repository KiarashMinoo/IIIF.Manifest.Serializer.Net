using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

public class PolynomialTransformation : Transformation
{
    public PolynomialTransformation(PolynomialTransformationOption options) : base(TransformationType.Polynomial)
    {
        Options = options;
    }

    /// <summary>
    ///     Transformation parameters as an array of coefficients.
    /// </summary>
    [JsonProperty("options")]
    public PolynomialTransformationOption Options
    {
        get => GetElementValue<PolynomialTransformationOption>()!;
        private set => SetElementValue(value);
    }
}