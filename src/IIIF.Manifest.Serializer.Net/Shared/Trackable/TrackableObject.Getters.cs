using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public partial class TrackableObject<TTrackableObject>
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    protected TValue? GetElementValue<TValue>(
        out bool isModified,
        [CallerMemberName] string? memberName = null
    ) => GetElementValue<TValue>((TTrackableObject)this, out isModified, out _, memberName);

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out bool isModified
    ) => GetElementValue<TValue>(out isModified, GetMemberName(expression));

    protected TValue? GetElementValue<TValue>(
        [CallerMemberName] string? memberName = null
    ) => GetElementValue<TValue>(out _, memberName);

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression
    ) => GetElementValue(expression, out _);
}