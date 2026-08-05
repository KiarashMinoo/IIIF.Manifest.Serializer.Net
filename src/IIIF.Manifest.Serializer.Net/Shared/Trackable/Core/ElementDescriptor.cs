namespace IIIF.Manifests.Serializer.Shared.Trackable.Core;

public interface IElementDescriptor
{
    object? OriginalValue { get; }
    object? ModifiedValue { get; }
    object? Value => ModifiedValue ?? OriginalValue;
    bool IsAdditional { get; }
    ModificationType ModificationType { get; }

    internal void SetModificationType(ModificationType modificationType);
}

public interface IElementDescriptor<out TValueType> : IElementDescriptor
{
    new TValueType? OriginalValue { get; }
    new TValueType? ModifiedValue { get; }
    new TValueType? Value => ModifiedValue ?? OriginalValue;
}

public struct ElementDescriptor : IElementDescriptor
{
    public object? OriginalValue { get; }
    public object? ModifiedValue { get; }
    public bool IsAdditional { get; }
    public ModificationType ModificationType { get; private set; } = ModificationType.Unchanged;

    internal ElementDescriptor(object? value, bool isAdditional = false)
    {
        OriginalValue = value;
        IsAdditional = isAdditional;
    }

    internal ElementDescriptor(object? originalValue, object modifiedValue, bool isAdditional = false) : this(originalValue)
    {
        ModifiedValue = modifiedValue;
        IsAdditional = isAdditional;
        SetModificationType(ModificationType.Changed);
    }

    internal ElementDescriptor(ElementDescriptor elementDescriptor, object modifiedValue)
        : this(elementDescriptor.OriginalValue, modifiedValue, elementDescriptor.IsAdditional)
    {
    }

    private void SetModificationType(ModificationType modificationType) => ModificationType = modificationType;

    void IElementDescriptor.SetModificationType(ModificationType modificationType)
        => SetModificationType(modificationType);
}

public readonly struct ElementDescriptor<TValueType> : IElementDescriptor<TValueType>
{
    private readonly IElementDescriptor _elementDescriptor;

    object? IElementDescriptor.OriginalValue => _elementDescriptor.OriginalValue;
    public TValueType? OriginalValue => (TValueType?)_elementDescriptor.OriginalValue;

    object? IElementDescriptor.ModifiedValue => _elementDescriptor.ModifiedValue;
    public TValueType? ModifiedValue => (TValueType?)_elementDescriptor.ModifiedValue;

    public TValueType? Value => (TValueType?)_elementDescriptor.Value;

    public bool IsAdditional => _elementDescriptor.IsAdditional;
    public ModificationType ModificationType => _elementDescriptor.ModificationType;

    internal ElementDescriptor(TValueType? originalValue, bool isAdditional = false)
    {
        _elementDescriptor = new ElementDescriptor(originalValue!, isAdditional);
    }

    internal ElementDescriptor(TValueType? originalValue, TValueType modifiedValue, bool isAdditional = false) : this(originalValue)
    {
        _elementDescriptor = new ElementDescriptor(originalValue!, modifiedValue!, isAdditional);
    }

    internal ElementDescriptor(ElementDescriptor<TValueType> elementDescriptor, TValueType modifiedValue)
        : this(elementDescriptor.OriginalValue, modifiedValue, elementDescriptor.IsAdditional)
    {
    }

    void IElementDescriptor.SetModificationType(ModificationType modificationType)
        => _elementDescriptor.SetModificationType(modificationType);
}