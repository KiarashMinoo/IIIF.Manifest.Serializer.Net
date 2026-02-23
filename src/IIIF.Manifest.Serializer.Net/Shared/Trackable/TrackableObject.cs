using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.Helpers;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObject<TTrackableObject> : INotifyPropertyChanging, INotifyPropertyChanged
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    [JsonIgnore] internal readonly Dictionary<string, ElementDescriptor> ElementDescriptors = [];

    public event PropertyChangingEventHandler? PropertyChanging;
    public event TrackableObjectPropertyChangingEventHandler? TrackableObjectPropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event TrackableObjectPropertyChangedEventHandler? TrackableObjectPropertyChanged;

    protected internal virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null, bool isList = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        TrackableObjectPropertyChangingEventArgs args = new(propertyName, isList);

        PropertyChanging?.Invoke(this, args);
        TrackableObjectPropertyChanging?.Invoke(this, args);
    }

    protected internal virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null, ListChangedType? listChangedType = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var args = listChangedType is not null
            ? new TrackableObjectPropertyChangedEventArgs(propertyName, listChangedType.Value)
            : new TrackableObjectPropertyChangedEventArgs(propertyName);

        PropertyChanged?.Invoke(this, args);
        TrackableObjectPropertyChanged?.Invoke(this, args);
    }

    protected TTrackableObject SetElementValue<TValue>(
        Func<TValue, TValue?> valueFactory,
        bool isAdditional = false,
        [CallerMemberName] string? memberName = null
    ) => ((TTrackableObject)this).SetElementValue<TTrackableObject, TValue>(valueFactory, isAdditional, memberName);

    protected TTrackableObject SetElementValue<TValue>(
        TValue? value,
        bool isAdditional = false,
        [CallerMemberName] string? memberName = null
    ) => ((TTrackableObject)this).SetElementValue<TTrackableObject, TValue>(value, isAdditional, memberName);

    protected TTrackableObject SetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        Func<TValue, TValue?> valueFactory,
        bool isAdditional = false
    ) => ((TTrackableObject)this).SetElementValue<TTrackableObject, TValue>(expression, valueFactory, isAdditional);

    protected TTrackableObject SetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        TValue? value,
        bool isAdditional = false
    ) => ((TTrackableObject)this).SetElementValue<TTrackableObject, TValue>(expression, value, isAdditional);

    protected TValue? GetElementValue<TValue>(
        out bool isModified,
        out bool isAdditional,
        [CallerMemberName] string? memberName = null
    ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(out isModified, out isAdditional, memberName);

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out bool isModified,
        out bool isAdditional
    ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(expression, out isModified, out isAdditional);

    protected TValue? GetElementValue<TValue>(
        [CallerMemberName] string? memberName = null
    ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(memberName);

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression
    ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(expression);
}