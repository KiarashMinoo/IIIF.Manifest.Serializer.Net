using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.ValuableItem;

[JsonConverter(typeof(ValuableItemJsonConverter<>))]
public class ValuableItem<TValuableItem> : TrackableObject<TValuableItem>
    where TValuableItem : ValuableItem<TValuableItem>
{
    public ValuableItem(string value)
    {
        Value = value;
    }

    public virtual string Value
    {
        get { return GetElementValue(x => x.Value)!; }
        private set => SetElementValue(value);
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && (ReferenceEquals(obj, this) || (obj is ValuableItem<TValuableItem> other && Equals(other)));
    }

    protected virtual bool Equals(ValuableItem<TValuableItem>? other)
    {
        return other is not null && other.Value.Equals(Value);
    }

    public override int GetHashCode()
    {
        return !string.IsNullOrWhiteSpace(Value) ? Value.GetHashCode() : 0;
    }

    public static bool operator ==(ValuableItem<TValuableItem>? left, ValuableItem<TValuableItem>? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(ValuableItem<TValuableItem>? left, ValuableItem<TValuableItem>? right)
    {
        return !(left == right);
    }
}