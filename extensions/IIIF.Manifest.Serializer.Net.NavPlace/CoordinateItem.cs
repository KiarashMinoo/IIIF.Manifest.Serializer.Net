using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions;

public class CoordinateItem : TrackableObject<CoordinateItem>
{
    public double? X => GetElementValue(x => x.X);
    public double? Y => GetElementValue(x => x.Y);
    public IReadOnlyCollection<CoordinateItem> Subitems => GetElementValue(x => x.Subitems) ?? [];

    public CoordinateItem(CoordinateItem[] coordinateItems)
    {
        SetElementValue(x => x.Subitems, [..coordinateItems]);
    }

    public CoordinateItem(double x, double y)
    {
        SetElementValue(a => a.X, x);
        SetElementValue(a => a.Y, y);
    }

    public CoordinateItem AddSubitem(CoordinateItem item) => SetElementValue(a => a.Subitems, labels => labels.With(item));
    public CoordinateItem RemoveSubitem(CoordinateItem item) => SetElementValue(a => a.Subitems, labels => labels.Without(item));
}