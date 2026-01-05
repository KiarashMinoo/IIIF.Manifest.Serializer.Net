using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Trackable
{
    [JsonConverter(typeof(TrackableObjectJsonConverter<>))]
    public class TrackableObject<TTrackableObject> : INotifyPropertyChanging, INotifyPropertyChanged
        where TTrackableObject : TrackableObject<TTrackableObject>
    {
        [JsonIgnore]
        private readonly Dictionary<string, ModifedMember> modifiedProperties = new Dictionary<string, ModifedMember>();

        [JsonIgnore]
        public IReadOnlyDictionary<string, ModifedMember> ModifiedProperties => modifiedProperties;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected virtual void OnPropertyChanging(string propertyName) => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

        private TTrackableObject SetPropertyValue<TValue>(TTrackableObject target, Expression<Func<TTrackableObject, TValue>> expresison, string memberName, TValue value)
        {
            object GetValue(MemberInfo memberInfo)
            {
                if (memberInfo is PropertyInfo propertyInfo)
                    return propertyInfo.GetValue(target, null);

                if (memberInfo is FieldInfo fieldInfo)
                    return fieldInfo.GetValue(target);

                return null;
            }

            void SetValue(MemberInfo memberInfo, object val)
            {
                if (memberInfo is PropertyInfo propertyInfo)
                    propertyInfo.SetValue(target, val, null);

                if (memberInfo is FieldInfo fieldInfo)
                    fieldInfo.SetValue(target, val);
            }

            if (expresison.Body is MemberExpression memberSelectorExpression)
            {
                if (string.IsNullOrEmpty(memberName))
                    memberName = memberSelectorExpression.Member.Name;

                OnPropertyChanging(memberName);

                object oldData = GetValue(memberSelectorExpression.Member);
                SetValue(memberSelectorExpression.Member, value);

                OnPropertyChanged(memberName);

                if (modifiedProperties.ContainsKey(memberName))
                {
                    var last = modifiedProperties[memberName];
                    modifiedProperties[memberName] = new ModifedMember(last.OriginalValue, value);
                }
                else
                    modifiedProperties.Add(memberName, new ModifedMember(oldData, value));
            }

            return target;
        }
        private TTrackableObject SetPropertyValue<TValue>(TTrackableObject target, Expression<Func<TTrackableObject, TValue>> expresison, Expression<Func<TTrackableObject, TValue>> memberNameExpression, TValue value)
        {
            return memberNameExpression.Body is MemberExpression memberNameSelectorExpression
                ? SetPropertyValue(target, expresison, memberNameSelectorExpression.Member.Name, value)
                : SetPropertyValue(target, expresison, (string)null, value);
        }
        //protected internal TTrackableObject SetPropertyValue<TValue>(Expression<Func<TTrackableObject, TValue>> expresison, string memberName, TValue value)
        //    => SetPropertyValue((TTrackableObject)this, expresison, memberName, value);
        protected internal TTrackableObject SetPropertyValue<TValue>(Expression<Func<TTrackableObject, TValue>> expresison, Expression<Func<TTrackableObject, TValue>> memberNameExpression, TValue value)
            => SetPropertyValue((TTrackableObject)this, expresison, memberNameExpression, value);
        protected internal TTrackableObject SetPropertyValue<TValue>(Expression<Func<TTrackableObject, TValue>> expresison, TValue value)
            => SetPropertyValue((TTrackableObject)this, expresison, (string)null, value);

        internal void ClearModifiedProperties() => modifiedProperties.Clear();
    }
}