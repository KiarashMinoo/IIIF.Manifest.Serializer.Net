using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

public class PolynomialTransformationOption : TrackableObject<PolynomialTransformationOption>
{
    public long Order
    {
        get => GetElementValue(x => x.Order);
        private set => SetElementValue(value);
    }
}