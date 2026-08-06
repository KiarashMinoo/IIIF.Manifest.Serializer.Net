using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Objects;

public partial class TrackableObject<TTrackableObject>
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    //By MemberName - Out ModificationType

    protected TValue? GetElementValue<TValue>(
        out ModificationType modificationType,
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>((TTrackableObject)this, out modificationType, out _, memberName);
    }

    protected TValue GetElementValue<TValue>(
        out ModificationType modificationType,
        TValue defaultValue,
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>(out modificationType, memberName) ?? defaultValue;
    }

    //By Expression - Out ModificationType

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out ModificationType modificationType
    )
    {
        return GetElementValue<TValue>(out modificationType, GetMemberName(expression));
    }

    protected TValue GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        out ModificationType modificationType,
        TValue defaultValue
    )
    {
        return GetElementValue(expression, out modificationType) ?? defaultValue;
    }

    protected IReadOnlyCollection<TValue> GetElementValue<TValue>(
        Expression<Func<TTrackableObject, IReadOnlyCollection<TValue>>> expression,
        out ModificationType modificationType
    )
    {
        return GetOrInitializeElementCollection<TValue>(GetMemberName(expression), out modificationType);
    }

    protected IReadOnlyCollection<TValue> GetElementValue<TValue>(
        Expression<Func<TTrackableObject, IReadOnlyCollection<TValue>>> expression,
        out ModificationType modificationType,
        IReadOnlyCollection<TValue> defaultValue
    )
    {
        return GetElementValue<IReadOnlyCollection<TValue>>(out modificationType, GetMemberName(expression)) ?? defaultValue;
    }

    //By MemberName

    protected TValue? GetElementValue<TValue>(
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>(out _, memberName);
    }

    protected TValue GetElementValue<TValue>(
        TValue defaultValue,
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>(memberName) ?? defaultValue;
    }

    //By Expression

    protected TValue? GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression
    )
    {
        return GetElementValue(expression, out _);
    }

    protected IReadOnlyCollection<TValue> GetElementValue<TValue>(
        Expression<Func<TTrackableObject, IReadOnlyCollection<TValue>>> expression
    )
    {
        return GetOrInitializeElementCollection<TValue>(GetMemberName(expression), out _);
    }

    /// <summary>
    ///     Backs every unset collection-typed property getter. A naive `?? new TrackableCollection&lt;T&gt;()`
    ///     fallback hands back a fresh, disconnected instance on every call - anything that grabs that
    ///     reference and mutates it in place (e.g. EF Core's relationship-fixup adding materialized related
    ///     rows directly to a navigation collection, instead of always going through the property setter)
    ///     silently loses the mutation, because the next read constructs yet another empty instance. This
    ///     persists the lazily-created collection into <see cref="Core.TrackableObject.ElementDescriptors" />
    ///     the first time it's read, so every later read (and any in-place mutation in between) sees the
    ///     same instance. It's recorded as <see cref="ModificationType.Unchanged" /> and attached to the
    ///     member's change-notification subscription directly - bypassing the normal setter's Added/Changed
    ///     bookkeeping and property-changed events - so merely reading an untouched property never marks
    ///     the owning object as changed.
    /// </summary>
    private IReadOnlyCollection<TValue> GetOrInitializeElementCollection<TValue>(string memberName, out ModificationType modificationType)
    {
        var existing = GetElementValue<IReadOnlyCollection<TValue>>(out modificationType, memberName);
        if (existing is not null) return existing;

        var collection = new TrackableCollection<TValue>();
        GetChangeHandlerSubscription(memberName).Attach(collection);
        ((TTrackableObject)this).ElementDescriptors[memberName] = new ElementDescriptor(collection);
        modificationType = ModificationType.Unchanged;
        return collection;
    }

    protected TValue GetElementValue<TValue>(
        Expression<Func<TTrackableObject, TValue>> expression,
        TValue defaultValue
    )
    {
        return GetElementValue(expression, out _) ?? defaultValue;
    }
}