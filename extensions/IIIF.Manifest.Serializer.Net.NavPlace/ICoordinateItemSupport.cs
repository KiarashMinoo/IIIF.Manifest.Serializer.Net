namespace IIIF.Manifests.Serializer.Extensions;

public interface ICoordinateItemSupport<out TCoordinateItemSupport>
    where TCoordinateItemSupport : ICoordinateItemSupport<TCoordinateItemSupport>
{
    IReadOnlyCollection<CoordinateItem> Coordinates { get; }

    TCoordinateItemSupport SetCoordinates(IReadOnlyCollection<CoordinateItem> coordinates);
    TCoordinateItemSupport AddCoordinate(CoordinateItem coordinate);
    TCoordinateItemSupport RemoveAddCoordinate(CoordinateItem coordinate);
}