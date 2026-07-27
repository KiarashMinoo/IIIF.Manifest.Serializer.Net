using IIIF.Manifests.Serializer.Shared.Trackable.AdditionalProperties;

namespace IIIF.Manifests.Serializer.Helpers;

/// <summary>
///     Extension methods for storing/retrieving "additional" (extension/unmapped) JSON properties on any
///     <see cref="IAdditionalPropertiesSupport{T}" /> implementer, backed by the same
///     <c>ElementDescriptors</c>/change-tracking storage as ordinary model properties.
///     <para>
///         Both methods call straight through to the interface's core <c>SetElementValue</c>/
///         <c>GetElementValue</c> members (implemented by <c>TrackableObject&lt;T&gt;</c>), which wrap a raw
///         enumerable value in <c>TrackableCollection&lt;T&gt;</c> and attach/detach its change-notification
///         handlers via the shared, per-property-cached
///         <c>Core.ChangeNotificationSubscription</c> - so unsubscription on replacement is handled
///         correctly, not a caveat callers need to work around.
///     </para>
/// </summary>
public static class AdditionalPropertiesHelper
{
    extension<TAdditionalPropertiesSupport>(TAdditionalPropertiesSupport target) where TAdditionalPropertiesSupport : IAdditionalPropertiesSupport<TAdditionalPropertiesSupport>
    {
        /// <summary>
        ///     Stores an additional property (unhandled/undefined) directly into ElementDescriptors with isAdditional=true.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyName">The name of the property to store.</param>
        /// <param name="value">The value to store.</param>
        /// <returns>The trackable object for fluent chaining.</returns>
        public TAdditionalPropertiesSupport SetAdditionalProperty<TValue>(string propertyName, TValue? value)
        {
            return target.SetElementValue<TValue>(_ => value, propertyName);
        }

        /// <summary>
        ///     Retrieves an additional property from ElementDescriptors.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The value if found and is marked as additional, otherwise default.</returns>
        public TValue? GetAdditionalProperty<TValue>(string propertyName)
        {
            return target.GetElementValue<TValue>(out _, propertyName);
        }
    }
}