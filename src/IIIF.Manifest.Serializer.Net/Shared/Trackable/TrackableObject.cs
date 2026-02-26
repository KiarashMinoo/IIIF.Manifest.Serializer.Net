using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObject
{
    protected static JsonSerializerSettings JsonSerializerSettings { get; } = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new IIIFJsonContractResolver()
    };

    public string Serialize() => JsonConvert.SerializeObject(this, JsonSerializerSettings);

    public static TTrackableObject Parse<TTrackableObject>(string json)
        where TTrackableObject : TrackableObject
        => !TryParse<TTrackableObject>(json, out var trackableObject)
            ? throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json))
            : trackableObject;

    public static bool TryParse<TTrackableObject>(string json, [MaybeNullWhen(false)] out TTrackableObject trackableObject)
        where TTrackableObject : TrackableObject
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            trackableObject = null;
            return false;
        }

        trackableObject = JsonConvert.DeserializeObject<TTrackableObject>(json, JsonSerializerSettings);
        return trackableObject is not null;
    }
}

public partial class TrackableObject<TTrackableObject> : TrackableObject, INotifyPropertyChanging, INotifyPropertyChanged
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    [JsonIgnore] internal readonly Dictionary<string, ElementDescriptor> ElementDescriptors = [];

    public event PropertyChangingEventHandler? PropertyChanging;
    public event TrackableObjectPropertyChangingEventHandler? TrackableObjectPropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;
    public event TrackableObjectPropertyChangedEventHandler? TrackableObjectPropertyChanged;

    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null, bool isList = false)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        TrackableObjectPropertyChangingEventArgs args = new(propertyName, isList);

        PropertyChanging?.Invoke(this, args);
        TrackableObjectPropertyChanging?.Invoke(this, args);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null, ListChangedType? listChangedType = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var args = listChangedType is not null
            ? new TrackableObjectPropertyChangedEventArgs(propertyName, listChangedType.Value)
            : new TrackableObjectPropertyChangedEventArgs(propertyName);

        PropertyChanged?.Invoke(this, args);
        TrackableObjectPropertyChanged?.Invoke(this, args);
    }

    private string GetMemberName<TValue>(Expression<Func<TTrackableObject, TValue>> expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (expression.Body is not MemberExpression memberNameSelectorExpression)
        {
            throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));
        }

        return memberNameSelectorExpression.Member.Name;
    }

    /// <summary>
    /// Sets an element value using a factory function that transforms the existing value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="target">The target trackable object.</param>
    /// <param name="valueFactory">A factory function that receives the current value and returns the new value.</param>
    /// <param name="isAdditional">Whether this is an additional property not defined in the IIIF spec.</param>
    /// <param name="memberName">The name of the member to set.</param>
    /// <returns>The trackable object for fluent chaining.</returns>
    private static TTrackableObject SetElementValue<TValue>(
        TTrackableObject target,
        Func<TValue, TValue?> valueFactory,
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

            if (isEnumerable)
            {
                var elementType = valueType
                                      .GetInterfaces()
                                      .Concat([valueType])
                                      .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                      .Select(t => t.GetGenericArguments()[0])
                                      .FirstOrDefault()
                                  ?? typeof(object);

                var bindingListType = typeof(BindingList<>).MakeGenericType(elementType);
                var bindingList = (IBindingList)Activator.CreateInstance(bindingListType)!;

                foreach (var item in (IEnumerable)value)
                {
                    bindingList.Add(item);
                }

                // Cast IBindingList to TValue (which should be compatible with the BindingList<T> type)
                value = (TValue)bindingList;
            }

            (value as IBindingList)?.ListChanged += BindingListOnListChanged;
            (value as INotifyPropertyChanging)?.PropertyChanging += NotifyPropertyChangingOnPropertyChanging;
            (value as INotifyPropertyChanged)?.PropertyChanged += NotifyPropertyChangedOnPropertyChanged;

            if (elementDescriptor is not null)
            {
                // Unsubscribe from old element events
                (elementDescriptor.Value as IBindingList)?.ListChanged -= BindingListOnListChanged;
                (elementDescriptor.Value as INotifyPropertyChanging)?.PropertyChanging -= NotifyPropertyChangingOnPropertyChanging;
                (elementDescriptor.Value as INotifyPropertyChanged)?.PropertyChanged -= NotifyPropertyChangedOnPropertyChanged;

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

        void NotifyPropertyChangingOnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            target.OnPropertyChanged(memberName);
        }

        void NotifyPropertyChangedOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            target.OnPropertyChanged(memberName);
        }
    }

    /// <summary>
    /// Gets an element value with modification and additional flags.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="target">The target trackable object.</param>
    /// <param name="memberName">The name of the member to get.</param>
    /// <param name="isModified">Output parameter indicating if the value has been modified.</param>
    /// <param name="isAdditional">Output parameter indicating if this is an additional property.</param>
    /// <returns>The element value, or default if not found.</returns>
    private static TValue? GetElementValue<TValue>(
        TTrackableObject target,
        out bool isModified,
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
}