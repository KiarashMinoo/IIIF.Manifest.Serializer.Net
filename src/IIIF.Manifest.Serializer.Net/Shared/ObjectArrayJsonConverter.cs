using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

public sealed class ObjectArrayJsonConverter : JsonConverter
{
    // objectType -> (elementType, compiled List<elementType> constructor), resolved once per closed
    // generic array-property type instead of re-running GetGenericArguments/MakeGenericType/
    // Activator.CreateInstance reflection on every deserialize.
    private static readonly ConcurrentDictionary<Type, (Type ElementType, Func<IList> NewList)> ListFactoryCache = new();

    private static (Type ElementType, Func<IList> NewList) GetListFactory(Type objectType)
    {
        return ListFactoryCache.GetOrAdd(objectType, static type =>
        {
            var elementType = type.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(elementType);
            var newList = Expression.Lambda<Func<IList>>(Expression.New(listType)).Compile();
            return (elementType, newList);
        });
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(IEnumerable).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var (elementType, newList) = GetListFactory(objectType);
        var list = newList();

        // An explicit JSON null means "no values" - an empty collection, not a single null
        // element. Without this, downstream code that maps/derives from the collection (e.g. a
        // computed legacy view) can throw a NullReferenceException on that phantom element.
        if (reader.TokenType == JsonToken.Null) return list;

        if (reader.TokenType == JsonToken.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray) break;

                var item = serializer.Deserialize(reader, elementType);
                list.Add(item!);
            }
        }
        else
        {
            var item = serializer.Deserialize(reader, elementType);
            list.Add(item!);
        }

        return list;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        var isEnumerable = value is IEnumerable and not string;
        if (isEnumerable)
        {
            var arrayList = new ArrayList();

            var enumerator = ((IEnumerable)value).GetEnumerator();
            using var _ = enumerator as IDisposable;
            while (enumerator.MoveNext()) arrayList.Add(enumerator.Current);

            if (arrayList.Count == 0)
            {
                writer.WriteNull();
                return;
            }

            if (arrayList.Count == 1)
            {
                serializer.Serialize(writer, arrayList[0]);
                return;
            }

            writer.WriteStartArray();

            foreach (var item in arrayList) serializer.Serialize(writer, item);

            writer.WriteEndArray();
        }
        else
        {
            serializer.Serialize(writer, value);
        }
    }
}