namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class ElementDescriptor<TValueType> : IDisposable
{
    private readonly bool _hasModifiedValue;

    public TValueType OriginalValue { get; }
    public TValueType? ModifiedValue { get; }
    public bool IsAdditional { get; }
    public TValueType Value => _hasModifiedValue ? ModifiedValue! : OriginalValue;
    public ModificationType ModificationType { get; private set; } = ModificationType.Unchanged;

    internal ElementDescriptor(TValueType originalValue, bool isAdditional = false)
    {
        OriginalValue = originalValue;
        IsAdditional = isAdditional;
    }

    internal ElementDescriptor(TValueType originalValue, TValueType modifiedValue, bool isAdditional = false) : this(originalValue)
    {
        ModifiedValue = modifiedValue;
        _hasModifiedValue = true;
        IsAdditional = isAdditional;
        SetModificationType(ModificationType.Changed);
    }

    internal ElementDescriptor(ElementDescriptor<TValueType> elementDescriptor, TValueType modifiedValue)
        : this(elementDescriptor.OriginalValue, modifiedValue, elementDescriptor.IsAdditional)
    {
    }

    internal void SetModificationType(ModificationType modificationType)
    {
        ModificationType = modificationType;
    }

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class ElementDescriptor : ElementDescriptor<object>
{
    internal ElementDescriptor(object originalValue, bool isAdditional = false) : base(originalValue, isAdditional)
    {
    }

    internal ElementDescriptor(object originalValue, object modifiedValue, bool isAdditional = false) : base(originalValue, modifiedValue, isAdditional)
    {
    }

    internal ElementDescriptor(ElementDescriptor<object> elementDescriptor, object modifiedValue) : base(elementDescriptor, modifiedValue)
    {
    }
}