using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Shared.Trackable
{
    public class ElementDescriptor<TValueType>
    {
        public TValueType OriginalValue { get; }
        public TValueType? ModifiedValue { get; }
        public TValueType Value => ModifiedValue ?? OriginalValue;
        public bool IsModified => ModifiedValue is not null && !EqualityComparer<TValueType>.Default.Equals(OriginalValue, ModifiedValue);
        public bool IsAdditional { get; }

        internal ElementDescriptor(TValueType originalValue, bool isAdditional = false)
        {
            OriginalValue = originalValue;
            IsAdditional = isAdditional;
        }

        internal ElementDescriptor(TValueType originalValue, TValueType modifiedValue) : this(originalValue)
        {
            ModifiedValue = modifiedValue;
        }

        internal ElementDescriptor(ElementDescriptor<TValueType> elementDescriptor, TValueType modifiedValue) : this(elementDescriptor.OriginalValue, modifiedValue)
        {
        }
    }

    public class ElementDescriptor : ElementDescriptor<object>
    {
        internal ElementDescriptor(object originalValue, bool isAdditional = false) : base(originalValue, isAdditional)
        {
        }

        internal ElementDescriptor(object originalValue, object modifiedValue) : base(originalValue, modifiedValue)
        {
        }

        internal ElementDescriptor(ElementDescriptor<object> elementDescriptor, object modifiedValue) : base(elementDescriptor, modifiedValue)
        {
        }
    }
}