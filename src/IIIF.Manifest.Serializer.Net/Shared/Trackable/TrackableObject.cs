using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using IIIF.Manifests.Serializer.Helpers;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Trackable
{
    [JsonConverter(typeof(TrackableObjectJsonConverter<>))]
    public class TrackableObject<TTrackableObject> : INotifyPropertyChanging, INotifyPropertyChanged
        where TTrackableObject : TrackableObject<TTrackableObject>
    {
        [JsonIgnore] internal readonly Dictionary<string, ElementDescriptor> ElementDescriptors = [];

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        protected internal virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected internal virtual void OnPropertyChanging(string propertyName) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

        protected TTrackableObject SetElementValue<TValue>(
            string memberName,
            Func<TValue, TValue?> valueFactory,
            bool isAdditional = false
        ) => ((TTrackableObject)this).SetElementValue<TTrackableObject, TValue>(memberName, valueFactory, isAdditional);

        protected TTrackableObject SetElementValue<TValue>(
            string memberName,
            TValue? value,
            bool isAdditional = false
        ) => ((TTrackableObject)this).SetElementValue<TTrackableObject, TValue>(memberName, value, isAdditional);

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
            string memberName,
            out bool isModified,
            out bool isAdditional
        ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(memberName, out isModified, out isAdditional);

        protected TValue? GetElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression,
            out bool isModified,
            out bool isAdditional
        ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(expression, out isModified, out isAdditional);

        protected TValue? GetElementValue<TValue>(
            string memberName
        ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(memberName);

        protected TValue? GetElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression
        ) => ((TTrackableObject)this).GetElementValue<TTrackableObject, TValue>(expression);
    }
}