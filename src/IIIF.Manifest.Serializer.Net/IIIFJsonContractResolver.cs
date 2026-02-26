using System.Collections;
using System.Reflection;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IIIF.Manifests.Serializer;

public class IIIFJsonContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        property.ShouldSerialize = o =>
        {
            var value = property.ValueProvider?.GetValue(o);

            switch (value)
            {
                case null:
                    return false;
                case string str:
                    return !string.IsNullOrWhiteSpace(str);
                case IEnumerable enumerable:
                {
                    var enumerator = enumerable.GetEnumerator();
                    using var _ = enumerator as IDisposable;
                    if (!enumerator.MoveNext())
                    {
                        return false;
                    }

                    break;
                }
            }

            return true;
        };

        return property;
    }
}