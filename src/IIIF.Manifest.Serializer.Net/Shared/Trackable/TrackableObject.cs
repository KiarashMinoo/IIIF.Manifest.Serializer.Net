using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObject
{
    protected internal static JsonSerializerSettings JsonSerializerSettings { get; } = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new IIIFJsonContractResolver()
    };

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, JsonSerializerSettings);
    }

    public static TTrackableObject Parse<TTrackableObject>(string json)
        where TTrackableObject : TrackableObject
    {
        return !TryParse<TTrackableObject>(json, out var trackableObject)
            ? throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json))
            : trackableObject;
    }

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

    /// <summary>
    ///     Bridges "additional" (extension) ElementDescriptors to Newtonsoft's JsonExtensionData
    ///     mechanism, so properties set via <see cref="IAdditionalPropertiesSupport{TAdditionalPropertiesSupport}" />
    ///     (e.g. the navPlace/Georeference/TextGranularity extension packages) actually survive a
    ///     JSON round-trip instead of only existing in-memory. Newtonsoft calls the getter once per
    ///     serialize/deserialize and both enumerates it (write) and calls Add on it (read); since this
    ///     wrapper always proxies the same underlying ElementDescriptors, a fresh instance each call
    ///     behaves identically to a cached one.
    /// </summary>
    [JsonExtensionData]
    private IDictionary<string, object?> AdditionalPropertiesData => new AdditionalPropertiesDictionary(this);

    public event PropertyChangedEventHandler? PropertyChanged;

    public event PropertyChangingEventHandler? PropertyChanging;
    public event TrackableObjectPropertyChangingEventHandler? TrackableObjectPropertyChanging;
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
        if (expression is null) throw new ArgumentNullException(nameof(expression));

        if (expression.Body is not MemberExpression memberNameSelectorExpression) throw new ArgumentException("The member name expression must be a member access expression.", nameof(expression));

        return memberNameSelectorExpression.Member.Name;
    }

    /// <summary>
    ///     Sets an element value using a factory function that transforms the existing value.
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
        if (target is null) throw new ArgumentNullException(nameof(target));

        if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Member name cannot be null or whitespace.", nameof(memberName));

        if (valueFactory is null) throw new ArgumentNullException(nameof(valueFactory));

        TValue currentValue = default!;
        if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
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

        var value = valueFactory(currentValue);

        target.OnPropertyChanging(memberName);

        if (value is null)
        {
            if (elementDescriptor is not null)
            {
                // Unsubscribe from event if it's a binding list
                if (elementDescriptor.Value is IBindingList oldBindingList) oldBindingList.ListChanged -= BindingListOnListChanged;

                if (target.ElementDescriptors.Remove(memberName)) target.OnPropertyChanged(memberName);
            }
        }
        else
        {
            // Check if value is an enumerable (but not a string) that needs to be wrapped in a BindingList.
            // JToken (JObject/JArray) is deliberately excluded even though it implements IEnumerable:
            // it only ever reaches here as a raw, not-yet-typed value read via the JsonExtensionData
            // bridge for additional/extension properties, and must be stored as an atomic scalar so it
            // can be lazily converted to its real type on first typed access (see GetElementValue).
            var valueType = value.GetType();
            var isEnumerable = value is IEnumerable and not string and not JToken;

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

                foreach (var item in (IEnumerable)value) bindingList.Add(item);

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
    ///     Gets an element value with modification and additional flags.
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
        if (target is null) throw new ArgumentNullException(nameof(target));

        if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Member name cannot be null or whitespace.", nameof(memberName));

        if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
        {
            isModified = elementDescriptor.IsModified;
            isAdditional = elementDescriptor.IsAdditional;

            // Safe cast with null handling
            if (elementDescriptor.Value is TValue typedValue) return typedValue;

            // Try to cast if possible, otherwise return default
            try
            {
                return (TValue)elementDescriptor.Value;
            }
            catch (InvalidCastException)
            {
                // Additional properties round-trip through JsonExtensionData as raw JTokens or
                // (for simple scalars) raw CLR primitives - Newtonsoft has no type information
                // for an unmapped key. Convert lazily on first typed access, applying whatever
                // JsonConverter TValue itself declares (e.g. ValuableItemJsonConverter), and
                // cache the result so this only happens once.
                try
                {
                    var token = elementDescriptor.Value as JToken
                                ?? JToken.FromObject(elementDescriptor.Value);
                    var converted = token.ToObject<TValue>();
                    target.ElementDescriptors[memberName] = new ElementDescriptor(converted!, elementDescriptor.IsAdditional);
                    return converted;
                }
                catch (JsonException)
                {
                    return default;
                }
            }
        }

        isModified = false;
        isAdditional = false;
        return default;
    }

    private sealed class AdditionalPropertiesDictionary(TrackableObject<TTrackableObject> owner) : IDictionary<string, object?>
    {
        private IEnumerable<KeyValuePair<string, ElementDescriptor>> AdditionalEntries =>
            owner.ElementDescriptors.Where(kvp => kvp.Value.IsAdditional);

        public object? this[string key]
        {
            get => AdditionalEntries.FirstOrDefault(kvp => kvp.Key == key).Value?.Value;
            set => Add(key, value);
        }

        public void Add(string key, object? value)
        {
            owner.SetElementValue(value, true, key);
        }

        public void Add(KeyValuePair<string, object?> item)
        {
            Add(item.Key, item.Value);
        }

        public bool ContainsKey(string key)
        {
            return AdditionalEntries.Any(kvp => kvp.Key == key);
        }

        public bool TryGetValue(string key, out object? value)
        {
            var match = AdditionalEntries.FirstOrDefault(kvp => kvp.Key == key);
            value = match.Value?.Value;
            return match.Value is not null;
        }

        public ICollection<string> Keys => AdditionalEntries.Select(kvp => kvp.Key).ToList();
        public ICollection<object?> Values => AdditionalEntries.Select(kvp => (object?)kvp.Value.Value).ToList();
        public int Count => AdditionalEntries.Count();
        public bool IsReadOnly => false;

        public bool Contains(KeyValuePair<string, object?> item)
        {
            return ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        {
            foreach (var kvp in this) array[arrayIndex++] = kvp;
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            return AdditionalEntries.Select(kvp => new KeyValuePair<string, object?>(kvp.Key, kvp.Value.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(string key)
        {
            throw new NotSupportedException("Additional properties cannot be removed through the extension-data view.");
        }

        public bool Remove(KeyValuePair<string, object?> item)
        {
            throw new NotSupportedException("Additional properties cannot be removed through the extension-data view.");
        }

        public void Clear()
        {
            throw new NotSupportedException("Additional properties cannot be cleared through the extension-data view.");
        }
    }
}