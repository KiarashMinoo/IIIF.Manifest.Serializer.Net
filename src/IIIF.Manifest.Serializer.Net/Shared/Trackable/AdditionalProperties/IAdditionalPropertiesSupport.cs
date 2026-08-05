using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;

namespace IIIF.Manifests.Serializer.Shared.Trackable.AdditionalProperties;

public interface IAdditionalPropertiesSupport<TAdditionalPropertiesSupport>
    where TAdditionalPropertiesSupport : IAdditionalPropertiesSupport<TAdditionalPropertiesSupport>
{
    //Setters

    TAdditionalPropertiesSupport SetElementValue<TValue>(
        Func<TValue?, TValue?> valueFactory,
        [CallerMemberName] string? memberName = null
    );

    TAdditionalPropertiesSupport SetElementValue<TValue>(
        Expression<Func<TAdditionalPropertiesSupport, TValue>> expression,
        Func<TValue?, TValue?> valueFactory
    );

    //Getters

    TValue? GetElementValue<TValue>(
        out ModificationType modificationType,
        [CallerMemberName] string? memberName = null
    );

    TValue? GetElementValue<TValue>(
        Expression<Func<TAdditionalPropertiesSupport, TValue>> expression,
        out ModificationType modificationType
    );
}