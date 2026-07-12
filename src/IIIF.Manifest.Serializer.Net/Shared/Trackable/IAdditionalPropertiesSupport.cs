using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public interface IAdditionalPropertiesSupport<TAdditionalPropertiesSupport>
    where TAdditionalPropertiesSupport : IAdditionalPropertiesSupport<TAdditionalPropertiesSupport>
{
    //Setters

    TAdditionalPropertiesSupport SetElementValue<TValue>(
        Func<TValue, TValue?> valueFactory,
        [CallerMemberName] string? memberName = null
    );

    TAdditionalPropertiesSupport SetElementValue<TValue>(
        TValue? value,
        [CallerMemberName] string? memberName = null
    )
    {
        return SetElementValue<TValue>(_ => value, memberName);
    }

    TAdditionalPropertiesSupport SetElementValue<TValue>(
        Expression<Func<TAdditionalPropertiesSupport, TValue>> expression,
        Func<TValue, TValue?> valueFactory
    );

    TAdditionalPropertiesSupport SetElementValue<TValue>(
        Expression<Func<TAdditionalPropertiesSupport, TValue>> expression,
        TValue? value
    )
    {
        return SetElementValue(expression, _ => value);
    }

    //Getters

    TValue? GetElementValue<TValue>(
        out bool isModified,
        [CallerMemberName] string? memberName = null
    );

    TValue? GetElementValue<TValue>(
        Expression<Func<TAdditionalPropertiesSupport, TValue>> expression,
        out bool isModified
    );

    TValue? GetElementValue<TValue>(
        [CallerMemberName] string? memberName = null
    )
    {
        return GetElementValue<TValue>(out _, memberName);
    }

    TValue? GetElementValue<TValue>(
        Expression<Func<TAdditionalPropertiesSupport, TValue>> expression
    )
    {
        return GetElementValue(expression, out _);
    }
}