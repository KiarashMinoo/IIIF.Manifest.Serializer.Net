using System;
using System.Linq.Expressions;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Helpers;

public static class TrackableObjectHelper
{
    extension<TTrackableObject>(TTrackableObject target) where TTrackableObject : TrackableObject<TTrackableObject>
    {
        internal TTrackableObject SetElementValue<TValue>(
            string memberName,
            Func<TValue, TValue?> valueFactory,
            bool isAdditional = false
        )
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            target.OnPropertyChanging(memberName);

            if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
            {
                var value = valueFactory((TValue)elementDescriptor.Value);
                if (value is null)
                {
                    target.ElementDescriptors.Remove(memberName);
                }
                else
                {
                    target.ElementDescriptors[memberName] = new ElementDescriptor(elementDescriptor, value);
                }

                target.OnPropertyChanged(memberName);
            }
            else
            {
                var value = valueFactory(default!);
                if (value is not null)
                {
                    target.ElementDescriptors.Add(memberName, new ElementDescriptor(value, isAdditional));
                    target.OnPropertyChanged(memberName);
                }
            }

            return target;
        }

        internal TTrackableObject SetElementValue<TValue>(
            string memberName,
            TValue? value,
            bool isAdditional = false
        )
        {
            return target.SetElementValue<TTrackableObject, TValue>(memberName, _ => value, isAdditional);
        }

        internal TTrackableObject SetElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression,
            Func<TValue, TValue?> valueFactory,
            bool isAdditional = false
        )
        {
            if (expression.Body is not MemberExpression memberNameSelectorExpression)
                throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));

            return target.SetElementValue(memberNameSelectorExpression.Member.Name, valueFactory, isAdditional);
        }

        internal TTrackableObject SetElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression,
            TValue? value,
            bool isAdditional = false
        )
        {
            if (expression.Body is not MemberExpression memberNameSelectorExpression)
                throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));

            return target.SetElementValue(memberNameSelectorExpression.Member.Name, value, isAdditional);
        }

        public TTrackableObject SetAdditionalElementValue<TValue>(
            string memberName,
            Func<TValue, TValue?> valueFactory
        ) => target.SetElementValue(memberName, valueFactory, isAdditional: true);

        public TTrackableObject SetAdditionalElementValue<TValue>(
            string memberName,
            TValue? value
        ) => target.SetElementValue(memberName, value, isAdditional: true);

        public TTrackableObject SetAdditionalElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression,
            Func<TValue, TValue?> valueFactory
        ) => target.SetElementValue(expression, valueFactory, isAdditional: true);

        public TTrackableObject SetAdditionalElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression,
            TValue? value
        ) => target.SetElementValue(expression, value, isAdditional: true);

        public TValue? GetElementValue<TValue>(
            string memberName,
            out bool isModified,
            out bool isAdditional
        )
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
            {
                isModified = elementDescriptor.IsModified;
                isAdditional = elementDescriptor.IsAdditional;
                return (TValue)elementDescriptor.Value;
            }

            isModified = false;
            isAdditional = false;
            return default;
        }

        public TValue? GetElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression,
            out bool isModified,
            out bool isAdditional
        )
        {
            if (expression.Body is not MemberExpression memberNameSelectorExpression)
                throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));

            return target.GetElementValue<TTrackableObject, TValue>(memberNameSelectorExpression.Member.Name, out isModified, out isAdditional);
        }

        public TValue? GetElementValue<TValue>(
            string memberName
        ) => target.GetElementValue<TTrackableObject, TValue>(memberName, out _, out _);

        public TValue? GetElementValue<TValue>(
            Expression<Func<TTrackableObject, TValue>> expression
        ) => target.GetElementValue<TTrackableObject, TValue>(expression, out _, out _);
    }
}