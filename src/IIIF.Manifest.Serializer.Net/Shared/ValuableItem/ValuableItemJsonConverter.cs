using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.ValuableItem;

/// <summary>
///     JSON converter for ValuableItem types that serializes/deserializes as simple string values.
/// </summary>
/// <typeparam name="TValuableItem">The type of ValuableItem to convert.</typeparam>
public sealed class ValuableItemJsonConverter<TValuableItem> : JsonConverter<TValuableItem>
    where TValuableItem : ValuableItem<TValuableItem>
{
    // Resolved and compiled once per closed TValuableItem (Label, Description, Rights, ...) instead
    // of re-resolving the (string) constructor via Activator.CreateInstance on every value read -
    // this type is on the hot path for nearly every IIIF resource (label/summary/rights/etc).
    private static readonly Func<string, TValuableItem> Factory = CreateFactory();

    private static Func<string, TValuableItem> CreateFactory()
    {
        var constructor = typeof(TValuableItem).GetConstructor(
                               BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                               null,
                               [typeof(string)],
                               null)
                           ?? throw new InvalidOperationException($"{typeof(TValuableItem)} has no (string) constructor.");

        var parameter = Expression.Parameter(typeof(string), "value");
        return Expression.Lambda<Func<string, TValuableItem>>(Expression.New(constructor, parameter), parameter).Compile();
    }

    /// <summary>
    ///     Writes a ValuableItem to JSON as a simple string value.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The ValuableItem to serialize.</param>
    /// <param name="serializer">The JSON serializer.</param>
    public override void WriteJson(JsonWriter writer, TValuableItem? value, JsonSerializer serializer)
    {
        if (value != null && !string.IsNullOrEmpty(value.Value))
            writer.WriteValue(value.Value);
        else
            writer.WriteNull();
    }

    /// <summary>
    ///     Reads a string value from JSON and creates a ValuableItem instance.
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
        if (reader.TokenType == JsonToken.Null) return null;

        // Read the string value
        var stringValue = reader.Value?.ToString();

        if (string.IsNullOrEmpty(stringValue)) return null;

        return Factory(stringValue);
    }
}