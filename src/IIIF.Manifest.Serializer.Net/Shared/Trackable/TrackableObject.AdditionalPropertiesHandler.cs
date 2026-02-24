using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public partial class TrackableObject<TTrackableObject> : IAdditionalPropertiesSupport<TTrackableObject>
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    //Setters

    private TTrackableObject SetAdditionalProperty<TValue>(Func<TValue, TValue?> valueFactory, [CallerMemberName] string? memberName = null)
    {
        return SetElementValue((TTrackableObject)this, valueFactory, true, memberName);
    }

    TTrackableObject IAdditionalPropertiesSupport<TTrackableObject>.SetElementValue<TValue>(
        Func<TValue, TValue?> valueFactory,
        string? memberName
    ) where TValue : default => SetAdditionalProperty(valueFactory, memberName);

    TTrackableObject IAdditionalPropertiesSupport<TTrackableObject>.SetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        Func<TValue, TValue?> valueFactory
    ) where TValue : default => SetAdditionalProperty(valueFactory, GetMemberName(expression));

    //Getters

    private TValue? GetAdditionalProperty<TValue>(out bool isModified, string? memberName)
    {
        var rtn = GetElementValue<TValue>((TTrackableObject)this, out isModified, out var isAdditional, memberName);

        return !isAdditional
            ? throw new InvalidOperationException("The specified member is not an additional property.")
            : rtn;
    }

    TValue? IAdditionalPropertiesSupport<TTrackableObject>.GetElementValue<TValue>(
        out bool isModified,
        string? memberName
    ) where TValue : default => GetAdditionalProperty<TValue>(out isModified, memberName);

    TValue? IAdditionalPropertiesSupport<TTrackableObject>.GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out bool isModified
    ) where TValue : default => GetAdditionalProperty<TValue>(out isModified, GetMemberName(expression));
}