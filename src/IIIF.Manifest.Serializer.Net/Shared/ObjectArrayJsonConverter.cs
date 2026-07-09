using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared;

public class ObjectArrayJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IEnumerable).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var elementType = objectType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType)!;

        if (reader.TokenType == JsonToken.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

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
            while (enumerator.MoveNext())
            {
                arrayList.Add(enumerator.Current);
            }

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

            foreach (var item in arrayList)
            {
                serializer.Serialize(writer, item);
            }

            writer.WriteEndArray();
        }
        else
        {
            serializer.Serialize(writer, value);
        }
    }
}
