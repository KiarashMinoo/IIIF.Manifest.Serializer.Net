using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Objects;

public partial class TrackableObject<TTrackableObject>
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    protected TValue? GetElementValue<TValue>(
        out ModificationType modificationType,
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>((TTrackableObject)this, out modificationType, out _, memberName);
    }

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out ModificationType modificationType
    )
    {
        return GetElementValue<TValue>(out modificationType, GetMemberName(expression));
    }

    protected TValue? GetElementValue<TValue>(
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>(out _, memberName);
    }

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression
    )
    {
        return GetElementValue(expression, out _);
    }
}