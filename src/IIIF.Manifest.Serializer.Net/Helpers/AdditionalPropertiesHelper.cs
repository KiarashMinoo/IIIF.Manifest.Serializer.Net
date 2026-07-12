using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Helpers;

/// <summary>
///     Extension methods for TrackableObject to manage element descriptors with change tracking.
///     <para>
///         <strong>Note on BindingList Event Handlers:</strong>
///         This helper automatically wraps enumerable values in BindingList instances and subscribes to
///         their ListChanged events. However, event handler unsubscription during element removal is not
///         fully implemented because handler references are not stored. For production use in
///         long-running applications, consider implementing weak event patterns or storing handler
///         references in ElementDescriptor to prevent potential memory leaks.
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
            return target.SetElementValue(value, propertyName);
        }

        /// <summary>
        ///     Retrieves an additional property from ElementDescriptors.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The value if found and is marked as additional, otherwise default.</returns>
        public TValue? GetAdditionalProperty<TValue>(string propertyName)
        {
            return target.GetElementValue<TValue>(propertyName);
        }
    }
}