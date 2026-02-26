using System.Collections;
using System.Reflection;
using IIIF.Manifests.Serializer.Shared.Trackable;
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
                    return TrackableObject.JsonSerializerSettings.NullValueHandling == NullValueHandling.Include || property.NullValueHandling == NullValueHandling.Include;
                case string str:
                    return TrackableObject.JsonSerializerSettings.NullValueHandling == NullValueHandling.Include || property.NullValueHandling == NullValueHandling.Include || !string.IsNullOrWhiteSpace(str);
                case IEnumerable enumerable:
                {
                    if (TrackableObject.JsonSerializerSettings.DefaultValueHandling != DefaultValueHandling.Include && property.DefaultValueHandling != DefaultValueHandling.Include)
                    {
                        var enumerator = enumerable.GetEnumerator();
                        using var _ = enumerator as IDisposable;
                        if (!enumerator.MoveNext())
                        {
                            return false;
                        }
                    }

                    break;
                }
            }

            return true;
        };

        return property;
    }
}