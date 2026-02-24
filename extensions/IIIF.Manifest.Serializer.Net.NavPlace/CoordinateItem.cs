using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions;

public class CoordinateItem : TrackableObject<CoordinateItem>, ICoordinateItemSupport<CoordinateItem>
{
    public double? Longitude
    {
        get => GetElementValue(x => x.Longitude);
        private set => SetElementValue(value);
    }

    public double? Latitude
    {
        get => GetElementValue(x => x.Latitude);
        private set => SetElementValue(value);
    }

    public double? Altitude
    {
        get => GetElementValue(x => x.Altitude);
        private set => SetElementValue(value);
    }

    public IReadOnlyCollection<CoordinateItem> Coordinates
    {
        get => GetElementValue(x => x.Coordinates) ?? [];
        private set => SetElementValue(value);
    }

    public CoordinateItem(CoordinateItem[] coordinateItems)
    {
        SetElementValue(x => x.Coordinates, [..coordinateItems]);
    }

    public CoordinateItem(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }

    public CoordinateItem(double longitude, double latitude, double altitude) : this(longitude, latitude)
    {
        Altitude = altitude;
    }

    public CoordinateItem SetCoordinates(IReadOnlyCollection<CoordinateItem> coordinates)
    {
        Coordinates = coordinates;
        return this;
    }

    public CoordinateItem AddCoordinate(CoordinateItem coordinate)
    {
        Coordinates = Coordinates.With(coordinate);
        return this;
    }

    public CoordinateItem RemoveAddCoordinate(CoordinateItem coordinate)
    {
        Coordinates = Coordinates.Without(coordinate);
        return this;
    }
}