using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public partial class TrackableObject<TTrackableObject> : IAdditionalPropertiesSupport<TTrackableObject>
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    TTrackableObject IAdditionalPropertiesSupport<TTrackableObject>.SetElementValue<TValue>(
        Func<TValue, TValue?> valueFactory,
        string? memberName
    ) where TValue : default
    {
        return SetAdditionalProperty(valueFactory, memberName);
    }

    TTrackableObject IAdditionalPropertiesSupport<TTrackableObject>.SetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        Func<TValue, TValue?> valueFactory
    ) where TValue : default
    {
        return SetAdditionalProperty(valueFactory, GetMemberName(expression));
    }

    TValue? IAdditionalPropertiesSupport<TTrackableObject>.GetElementValue<TValue>(
        out bool isModified,
        string? memberName
    ) where TValue : default
    {
        return GetAdditionalProperty<TValue>(out isModified, memberName);
    }

    TValue? IAdditionalPropertiesSupport<TTrackableObject>.GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out bool isModified
    ) where TValue : default
    {
        return GetAdditionalProperty<TValue>(out isModified, GetMemberName(expression));
    }
    //Setters

    private TTrackableObject SetAdditionalProperty<TValue>(Func<TValue, TValue?> valueFactory, [CallerMemberName] string? memberName = null)
    {
        return SetElementValue((TTrackableObject)this, valueFactory, true, memberName);
    }

    //Getters

    private TValue? GetAdditionalProperty<TValue>(out bool isModified, string? memberName)
    {
        var rtn = GetElementValue<TValue>((TTrackableObject)this, out isModified, out var isAdditional, memberName);

        return !isAdditional
            ? throw new InvalidOperationException("The specified member is not an additional property.")
            : rtn;
    }
}