using System;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.ValuableItem;

/// <summary>
/// JSON converter for ValuableItem types that serializes/deserializes as simple string values.
/// </summary>
/// <typeparam name="TValuableItem">The type of ValuableItem to convert.</typeparam>
public class ValuableItemJsonConverter<TValuableItem> : JsonConverter<TValuableItem>
    where TValuableItem : ValuableItem<TValuableItem>
{
    /// <summary>
    /// Writes a ValuableItem to JSON as a simple string value.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The ValuableItem to serialize.</param>
    /// <param name="serializer">The JSON serializer.</param>
    public override void WriteJson(JsonWriter writer, TValuableItem? value, JsonSerializer serializer)
    {
        if (value != null && !string.IsNullOrEmpty(value.Value))
        {
            writer.WriteValue(value.Value);
        }
        else
        {
            writer.WriteNull();
        }
    }

    /// <summary>
    /// Reads a string value from JSON and creates a ValuableItem instance.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="objectType">The type of object to deserialize.</param>
    /// <param name="existingValue">The existing value being replaced.</param>
    /// <param name="hasExistingValue">Whether an existing value is present.</param>
    /// <param name="serializer">The JSON serializer.</param>
    /// <returns>A ValuableItem instance, or null if the token is null.</returns>
    public override TValuableItem? ReadJson(JsonReader reader, Type objectType, TValuableItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Handle null tokens
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        // Read the string value
        var stringValue = reader.Value?.ToString();

        if (string.IsNullOrEmpty(stringValue))
        {
            return null;
        }

        // Create instance using the string constructor
        return (TValuableItem)Activator.CreateInstance(typeof(TValuableItem), stringValue)!;
    }
}