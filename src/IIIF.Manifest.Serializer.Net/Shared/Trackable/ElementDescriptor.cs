using System;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Shared.Trackable
{
    public class ElementDescriptor<TValueType> : IDisposable
    {
        public TValueType OriginalValue { get; }
        public TValueType? ModifiedValue { get; }
        public bool IsAdditional { get; }
        public TValueType Value => ModifiedValue ?? OriginalValue;
        public bool IsModified => ModifiedValue is not null && !EqualityComparer<TValueType>.Default.Equals(OriginalValue, ModifiedValue);

        internal ElementDescriptor(TValueType originalValue, bool isAdditional = false)
        {
            OriginalValue = originalValue;
            IsAdditional = isAdditional;
        }

        internal ElementDescriptor(TValueType originalValue, TValueType modifiedValue, bool isAdditional = false) : this(originalValue)
        {
            ModifiedValue = modifiedValue;
            IsAdditional = isAdditional;
        }

        internal ElementDescriptor(ElementDescriptor<TValueType> elementDescriptor, TValueType modifiedValue)
            : this(elementDescriptor.OriginalValue, modifiedValue, elementDescriptor.IsAdditional)
        {
        }

        public void Dispose()
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
}