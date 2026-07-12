using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public partial class TrackableObject<TTrackableObject>
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    protected TTrackableObject SetElementValue<TValue>(
        Func<TValue, TValue?> valueFactory,
        bool isAdditional = false,
        [CallerMemberName] string? memberName = null
    )
    {
        return SetElementValue((TTrackableObject)this, valueFactory, isAdditional, memberName);
    }

    protected TTrackableObject SetElementValue<TValue>(
        TValue? value,
        bool isAdditional = false,
        [CallerMemberName] string? memberName = null
    )
    {
        return SetElementValue<TValue>(_ => value, isAdditional, memberName);
    }

    protected TTrackableObject SetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        Func<TValue, TValue?> valueFactory,
        bool isAdditional = false
    )
    {
        return SetElementValue(valueFactory, isAdditional, GetMemberName(expression));
    }

    protected TTrackableObject SetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        TValue? value,
        bool isAdditional = false
    )
    {
        return SetElementValue(expression, _ => value, isAdditional);
    }
}