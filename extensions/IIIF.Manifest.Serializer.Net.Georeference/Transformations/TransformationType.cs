using IIIF.Manifests.Serializer.Shared.ValuableItem;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

public class TransformationType(string value) : ValuableItem<TransformationType>(value)
{
    /// <summary>
    /// 1st, 2nd or 3rd order polynomial transformation
    /// Options: order
    /// </summary>
    public static TransformationType Polynomial => new TransformationType("polynomial");

    /// <summary>
    /// Thin plate spline transformation, also known as rubber sheeting
    /// Options: N/A
    /// </summary>
    public static TransformationType ThinPlateSpline => new TransformationType("thinPlateSpline");
}