using System;
using System.Collections;
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

        var arrayList = new ArrayList();

        if (reader.TokenType == JsonToken.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                var item = serializer.Deserialize(reader, elementType);
                arrayList.Add(item!);
            }
        }
        else
        {
            var item = serializer.Deserialize(reader, elementType);
            arrayList.Add(item!);
        }

        return arrayList;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is IEnumerable enumerable)
        {
            var arrayList = new ArrayList();

            var enumerator = enumerable.GetEnumerator();
            using var _ = enumerator as IDisposable;
            while (enumerator.MoveNext())
            {
                arrayList.Add(enumerator.Current);
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