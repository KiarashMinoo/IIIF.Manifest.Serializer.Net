using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

public class CoordinateItemJsonConverter : JsonConverter<CoordinateItem>
{
    public override void WriteJson(JsonWriter writer, CoordinateItem? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();

        if (value.Coordinates.Count == 0)
        {
            writer.WriteValue(value.Longitude);
            writer.WriteValue(value.Latitude);

            if (value.Altitude > 0)
            {
                writer.WriteValue(value.Altitude);
            }
        }
        else
        {
            foreach (var coordinate in value.Coordinates)
            {
                serializer.Serialize(writer, coordinate);
            }
        }

        writer.WriteEndArray();
    }

    public override CoordinateItem ReadJson(JsonReader reader, Type objectType, CoordinateItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        existingValue = new CoordinateItem();

        List<double> items = new(3);
        if (reader.TokenType == JsonToken.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                    break;

                if (reader.TokenType == JsonToken.StartArray)
                {
                    existingValue.AddCoordinate(serializer.Deserialize<CoordinateItem>(reader)!);
                }
                else
                {
                    items.Add(Convert.ToDouble(reader.Value));
                }
            }
        }

        existingValue = items.Count switch
        {
            > 0 => items.Count switch
            {
                > 2 => new CoordinateItem(items[0], items[1], items[2]),
                _ => new CoordinateItem(items[0], items[1])
            },
            _ => existingValue
        };

        return existingValue;
    }
}