namespace IIIF.Manifests.Serializer.Shared
{
    public class ModifedMember<TValueType>
    {
        public TValueType OriginalValue { get; }
        public TValueType ModifiedValue { get; }

        internal ModifedMember(TValueType originalValue, TValueType modifiedValue)
        {
            OriginalValue = originalValue;
            ModifiedValue = modifiedValue;
        }
    }

    public class ModifedMember : ModifedMember<object>
    {
        internal ModifedMember(object originalValue, object modifiedValue) : base(originalValue, modifiedValue)
        {
        }
    }
}