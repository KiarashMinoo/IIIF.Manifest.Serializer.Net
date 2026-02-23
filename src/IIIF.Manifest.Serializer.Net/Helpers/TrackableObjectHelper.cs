using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Helpers;

/// <summary>
/// Extension methods for TrackableObject to manage element descriptors with change tracking.
/// 
/// <para>
/// <strong>Note on BindingList Event Handlers:</strong>
/// This helper automatically wraps enumerable values in BindingList instances and subscribes to
/// their ListChanged events. However, event handler unsubscription during element removal is not
/// fully implemented because handler references are not stored. For production use in
/// long-running applications, consider implementing weak event patterns or storing handler
/// references in ElementDescriptor to prevent potential memory leaks.
/// </para>
/// </summary>
public static class TrackableObjectHelper
{
    /// <param name="target">The target trackable object.</param>
    /// <typeparam name="TTrackableObject">The type of the trackable object.</typeparam>
    extension<TTrackableObject>(TTrackableObject target) where TTrackableObject : TrackableObject<TTrackableObject>
    {
        /// <summary>
        /// Sets an element value using a factory function that transforms the existing value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="valueFactory">A factory function that receives the current value and returns the new value.</param>
        /// <param name="isAdditional">Whether this is an additional property not defined in the IIIF spec.</param>
        /// <param name="memberName">The name of the member to set.</param>
        /// <returns>The trackable object for fluent chaining.</returns>
        internal TTrackableObject SetElementValue<TValue>(Func<TValue, TValue?> valueFactory,
            bool isAdditional = false,
            [CallerMemberName] string? memberName = null
        )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException("Member name cannot be null or whitespace.", nameof(memberName));
            }

            if (valueFactory is null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            TValue currentValue = default!;
            if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
            {
                // Safe cast with proper null handling
                try
                {
                    currentValue = (TValue)elementDescriptor.Value;
                }
                catch (InvalidCastException)
                {
                    // If cast fails, use default value
                    currentValue = default!;
                }
            }

            var value = valueFactory(currentValue);

            target.OnPropertyChanging(memberName);

            if (value is null)
            {
                if (elementDescriptor is not null)
                {
                    // Unsubscribe from event if it's a binding list
                    if (elementDescriptor.Value is IBindingList oldBindingList)
                    {
                        oldBindingList.ListChanged -= BindingListOnListChanged;
                    }

                    if (target.ElementDescriptors.Remove(memberName))
                    {
                        target.OnPropertyChanged(memberName);
                    }
                }
            }
            else
            {
                // Check if value is an enumerable (but not a string) that needs to be wrapped in a BindingList
                var valueType = value.GetType();
                var isEnumerable = value is IEnumerable and not string;

                if (isEnumerable && valueType.IsGenericType)
                {
                    // Check if it's IEnumerable<T> or List<T> or similar collection
                    if (typeof(IEnumerable<>).IsAssignableFrom(valueType))
                    {
                        var elementType = valueType.GetGenericArguments()[0];
                        var bindingListType = typeof(BindingList<>).MakeGenericType(elementType);
                        var bindingList = (IBindingList)Activator.CreateInstance(bindingListType)!;

                        foreach (var item in (IEnumerable)value)
                        {
                            bindingList.Add(item);
                        }

                        bindingList.ListChanged += BindingListOnListChanged;

                        // Cast IBindingList to TValue (which should be compatible with the BindingList<T> type)
                        value = (TValue)bindingList;
                    }
                }

                if (elementDescriptor is not null)
                {
                    // Unsubscribe from old binding list event
                    if (elementDescriptor.Value is IBindingList oldBindingList)
                    {
                        oldBindingList.ListChanged -= BindingListOnListChanged;
                    }

                    target.ElementDescriptors[memberName] = new ElementDescriptor(elementDescriptor, value);
                }
                else
                {
                    target.ElementDescriptors.Add(memberName, new ElementDescriptor(value, isAdditional));
                }

                target.OnPropertyChanged(memberName);
            }

            return target;

            void BindingListOnListChanged(object sender, ListChangedEventArgs e)
            {
                target.OnPropertyChanged(memberName, e.ListChangedType);
            }
        }

        /// <summary>
        /// Sets an element value directly.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The new value to set.</param>
        /// <param name="isAdditional">Whether this is an additional property not defined in the IIIF spec.</param>
        /// <param name="memberName">The name of the member to set.</param>
        /// <returns>The trackable object for fluent chaining.</returns>
        internal TTrackableObject SetElementValue<TValue>(TValue? value,
            bool isAdditional = false,
            [CallerMemberName] string? memberName = null
        )
        {
            return target.SetElementValue<TTrackableObject, TValue>(_ => value, isAdditional, memberName);
        }

        /// <summary>
        /// Sets an element value using a lambda expression and factory function.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">Expression that identifies the member to set.</param>
        /// <param name="valueFactory">A factory function that receives the current value and returns the new value.</param>
        /// <param name="isAdditional">Whether this is an additional property not defined in the IIIF spec.</param>
        /// <returns>The trackable object for fluent chaining.</returns>
        internal TTrackableObject SetElementValue<TValue>(Expression<Func<TTrackableObject, TValue>> expression,
            Func<TValue, TValue?> valueFactory,
            bool isAdditional = false
        )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is not MemberExpression memberNameSelectorExpression)
            {
                throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));
            }

            return target.SetElementValue(valueFactory, isAdditional, memberNameSelectorExpression.Member.Name);
        }

        /// <summary>
        /// Sets an element value using a lambda expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">Expression that identifies the member to set.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="isAdditional">Whether this is an additional property not defined in the IIIF spec.</param>
        /// <returns>The trackable object for fluent chaining.</returns>
        internal TTrackableObject SetElementValue<TValue>(Expression<Func<TTrackableObject, TValue>> expression,
            TValue? value,
            bool isAdditional = false
        )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is not MemberExpression memberNameSelectorExpression)
            {
                throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));
            }

            return target.SetElementValue(value, isAdditional, memberNameSelectorExpression.Member.Name);
        }

        /// <summary>
        /// Gets an element value with modification and additional flags.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="memberName">The name of the member to get.</param>
        /// <param name="isModified">Output parameter indicating if the value has been modified.</param>
        /// <param name="isAdditional">Output parameter indicating if this is an additional property.</param>
        /// <returns>The element value, or default if not found.</returns>
        public TValue? GetElementValue<TValue>(out bool isModified,
            out bool isAdditional,
            [CallerMemberName] string? memberName = null
        )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException("Member name cannot be null or whitespace.", nameof(memberName));
            }

            if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
            {
                isModified = elementDescriptor.IsModified;
                isAdditional = elementDescriptor.IsAdditional;

                // Safe cast with null handling
                if (elementDescriptor.Value is TValue typedValue)
                {
                    return typedValue;
                }

                // Try to cast if possible, otherwise return default
                try
                {
                    return (TValue)elementDescriptor.Value;
                }
                catch (InvalidCastException)
                {
                    return default;
                }
            }

            isModified = false;
            isAdditional = false;
            return default;
        }

        /// <summary>
        /// Gets an element value using a lambda expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">Expression that identifies the member to get.</param>
        /// <param name="isModified">Output parameter indicating if the value has been modified.</param>
        /// <param name="isAdditional">Output parameter indicating if this is an additional property.</param>
        /// <returns>The element value, or default if not found.</returns>
        public TValue? GetElementValue<TValue>(Expression<Func<TTrackableObject, TValue>> expression,
            out bool isModified,
            out bool isAdditional
        )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.Body is not MemberExpression memberNameSelectorExpression)
            {
                throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));
            }

            return target.GetElementValue<TTrackableObject, TValue>(out isModified, out isAdditional, memberNameSelectorExpression.Member.Name);
        }

        /// <summary>
        /// Gets an element value by name.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="memberName">The name of the member to get.</param>
        /// <returns>The element value, or default if not found.</returns>
        public TValue? GetElementValue<TValue>([CallerMemberName] string? memberName = null
        ) => target.GetElementValue<TTrackableObject, TValue>(out _, out _, memberName);

        /// <summary>
        /// Gets an element value using a lambda expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">Expression that identifies the member to get.</param>
        /// <returns>The element value, or default if not found.</returns>
        public TValue? GetElementValue<TValue>(Expression<Func<TTrackableObject, TValue>> expression
        ) => target.GetElementValue(expression, out _, out _);
    }
}