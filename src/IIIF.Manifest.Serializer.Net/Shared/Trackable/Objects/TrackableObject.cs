using IIIF.Manifests.Serializer.Shared.Trackable.AdditionalProperties;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;
using IIIF.Manifests.Serializer.Shared.Trackable.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Objects;

public partial class TrackableObject<TTrackableObject> : Core.TrackableObject, INotifyPropertyChanging, INotifyPropertyChanged
    where TTrackableObject : TrackableObject<TTrackableObject>
{
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
    public event TrackableObjectPropertyChangingEventHandler<TTrackableObject>? TrackableObjectPropertyChanging;
    public event TrackableObjectPropertyChangedEventHandler<TTrackableObject>? TrackableObjectPropertyChanged;

    private static readonly ConcurrentDictionary<Type, Type> TrackableCollectionTypeCache = new();

    // Keyed by member name so the same subscription instance (and therefore the same delegate
    // identity) is reused across every SetElementValue call for that property - a fresh
    // delegate each call would never equal the one actually attached to an earlier value, so
    // DetachChangeHandlers would silently fail to unsubscribe from replaced collections.
    private readonly Dictionary<string, ChangeNotificationSubscription> _changeHandlerSubscriptions = [];

    private ChangeNotificationSubscription GetChangeHandlerSubscription(string memberName)
    {
        if (!_changeHandlerSubscriptions.TryGetValue(memberName, out var subscription))
        {
            var self = (TTrackableObject)this;
            subscription = new ChangeNotificationSubscription(
                (_, e) => self.OnPropertyChanging(memberName, e.ChangeType),
                (_, e) => self.OnPropertyChanged(memberName, e.ChangeType),
                (_, _) => self.OnPropertyChanging(memberName),
                (_, _) => self.OnPropertyChanged(memberName));
            _changeHandlerSubscriptions[memberName] = subscription;
        }

        return subscription;
    }

    protected virtual void OnPropertyChanging([CallerMemberName] string? propertyName = null, CollectionChangeType? changeType = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var args = changeType is null
            ? new TrackableObjectPropertyChangingEventArgs(propertyName)
            : new TrackableObjectPropertyChangingEventArgs(propertyName, changeType.Value);

        PropertyChanging?.Invoke(this, args);
        TrackableObjectPropertyChanging?.Invoke((TTrackableObject)this, args);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null, CollectionChangeType? changeType = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException(nameof(propertyName));

        var args = changeType is not null
            ? new TrackableObjectPropertyChangedEventArgs(propertyName, changeType.Value)
            : new TrackableObjectPropertyChangedEventArgs(propertyName);

        PropertyChanged?.Invoke(this, args);
        TrackableObjectPropertyChanged?.Invoke((TTrackableObject)this, args);
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
        Func<TValue?, TValue?> valueFactory,
        bool isAdditional = false,
        [CallerMemberName] string? memberName = null
    )
    {
        if (target is null) throw new ArgumentNullException(nameof(target));

        if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Member name cannot be null or whitespace.", nameof(memberName));

        if (valueFactory is null) throw new ArgumentNullException(nameof(valueFactory));

        TValue? currentValue = default;
        if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor))
        {
            if (elementDescriptor.Value is TValue typedCurrentValue)
            {
                currentValue = typedCurrentValue;
            }
            else
            {
                try
                {
                    currentValue = (TValue?)elementDescriptor.Value;
                }
                catch (InvalidCastException)
                {
                    // Mirrors GetElementValue's JToken recovery: an additional property that
                    // hasn't been read yet is still a raw JToken from JsonExtensionData, and a
                    // valueFactory-based setter can be the first access - fall back to default
                    // only if conversion genuinely fails, instead of always discarding it.
                    try
                    {
                        var token = elementDescriptor.Value as JToken
                                    ?? JToken.FromObject(elementDescriptor.Value!);
                        currentValue = token.ToObject<TValue>()!;
                    }
                    catch (JsonException)
                    {
                        currentValue = default!;
                    }
                }
            }
        }

        var value = valueFactory(currentValue);

        // In-place TrackableCollection operations already update their item descriptors and raise
        // collection-aware notifications through the handlers attached to the stored instance.
        if (value is ITrackableCollection && ReferenceEquals(currentValue, value))
            return target;

        var subscription = target.GetChangeHandlerSubscription(memberName);

        target.OnPropertyChanging(memberName);

        if (value is null)
        {
            if (elementDescriptor is not null)
            {
                subscription.Detach(elementDescriptor.Value);

                if (elementDescriptor.ModificationType == ModificationType.Added)
                    target.ElementDescriptors.Remove(memberName);
                else
                    elementDescriptor.SetModificationType(ModificationType.Removed);

                target.OnPropertyChanged(memberName);
            }
        }
        else
        {
            var isEnumerable = value is IEnumerable and not string and not JToken;

            if (isEnumerable && value is not ITrackableCollection)
            {
                var trackableCollectionType = TrackableCollectionTypeCache.GetOrAdd(value.GetType(), static valueType =>
                {
                    var elementType = valueType
                                          .GetInterfaces()
                                          .Concat([valueType])
                                          .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                          .Select(t => t.GetGenericArguments()[0])
                                          .FirstOrDefault()
                                      ?? typeof(object);

                    return typeof(TrackableCollection<>).MakeGenericType(elementType);
                });

                var trackableCollection = (ITrackableCollection)Activator.CreateInstance(trackableCollectionType)!;

                foreach (var item in (IEnumerable)value) trackableCollection.Add(item);

                // Cast IBindingList to TValue (which should be compatible with the BindingList<T> type)
                value = (TValue)trackableCollection;
            }

            subscription.Attach(value);

            if (elementDescriptor is not null)
            {
                subscription.Detach(elementDescriptor.Value);

                IElementDescriptor newElementDescriptor = new ElementDescriptor(elementDescriptor.OriginalValue, value, elementDescriptor.IsAdditional);
                var modificationType = elementDescriptor.ModificationType == ModificationType.Added
                    ? ModificationType.Added
                    : Equals(elementDescriptor.OriginalValue, value)
                        ? ModificationType.Unchanged
                        : ModificationType.Changed;
                newElementDescriptor.SetModificationType(modificationType);
                target.ElementDescriptors[memberName] = newElementDescriptor;
            }
            else
            {
                elementDescriptor = new ElementDescriptor(value, isAdditional);
                elementDescriptor.SetModificationType(ModificationType.Added);
                target.ElementDescriptors.Add(memberName, elementDescriptor);
            }

            target.OnPropertyChanged(memberName);
        }

        return target;
    }

    /// <summary>
    ///     Gets an element value with modification and additional flags.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="target">The target trackable object.</param>
    /// <param name="memberName">The name of the member to get.</param>
    /// <param name="modificationType">Output parameter indicating modificationType.</param>
    /// <param name="isAdditional">Output parameter indicating if this is an additional property.</param>
    /// <returns>The element value, or default if not found.</returns>
    private static TValue? GetElementValue<TValue>(
        TTrackableObject target,
        out ModificationType modificationType,
        out bool isAdditional,
        [CallerMemberName] string? memberName = null
    )
    {
        if (target is null) throw new ArgumentNullException(nameof(target));

        if (string.IsNullOrWhiteSpace(memberName)) throw new ArgumentException("Member name cannot be null or whitespace.", nameof(memberName));

        if (target.ElementDescriptors.TryGetValue(memberName, out var elementDescriptor) && elementDescriptor.ModificationType != ModificationType.Removed)
        {
            modificationType = elementDescriptor.ModificationType;
            isAdditional = elementDescriptor.IsAdditional;

            // Safe cast with null handling
            if (elementDescriptor.Value is TValue typedValue) return typedValue;

            // Try to cast if possible, otherwise return default
            try
            {
                return (TValue?)elementDescriptor.Value;
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
                                ?? JToken.FromObject(elementDescriptor.Value!);
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

        modificationType = ModificationType.Unknown;
        isAdditional = false;
        return default;
    }

    private sealed class AdditionalPropertiesDictionary(TrackableObject<TTrackableObject> owner) : IDictionary<string, object?>
    {
        private IEnumerable<KeyValuePair<string, IElementDescriptor>> AdditionalEntries =>
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

        public ICollection<string> Keys => [.. AdditionalEntries.Select(kvp => kvp.Key)];
        public ICollection<object?> Values => [.. AdditionalEntries.Select(kvp => kvp.Value.Value)];
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