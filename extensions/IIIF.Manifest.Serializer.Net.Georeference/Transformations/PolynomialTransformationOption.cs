using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions.Transformations;

public class PolynomialTransformationOption : TrackableObject<PolynomialTransformationOption>
{
    [JsonProperty("order")]
    public long Order
    {
        get => GetElementValue(x => x.Order);
        private set => SetElementValue(value);
    }

    public PolynomialTransformationOption SetOrder(long order)
    {
        return SetElementValue(a => a.Order, order);
    }
}