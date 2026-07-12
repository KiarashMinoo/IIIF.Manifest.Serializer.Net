using System.Text.Json;
using System.Text.Json.Serialization;
using IIIF.Manifests.Serializer.Nodes;

namespace IIIF.Manifests.Serializer.SystemTextJson;

/// <summary>
///     Bridges <c>System.Text.Json</c> to this SDK's own Newtonsoft-based <see cref="Collection" />
///     read/write logic, so <c>System.Text.Json.JsonSerializer.Serialize(collection)</c> (and ASP.NET
///     Core's default request/response (de)serialization) produce the same correct, version-aware IIIF
///     JSON as calling <see cref="IiifSerializer.Serialize(Collection)" />/
///     <see cref="IiifSerializer.DeserializeCollection" /> directly - with no extra configuration
///     required from the consumer.
/// </summary>
public sealed class CollectionSystemTextJsonConverter : JsonConverter<Collection>
{
    public override Collection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return IiifSerializer.DeserializeCollection(document.RootElement.GetRawText());
    }

    public override void Write(Utf8JsonWriter writer, Collection value, JsonSerializerOptions options)
    {
        using var document = JsonDocument.Parse(IiifSerializer.Serialize(value));
        document.RootElement.WriteTo(writer);
    }
}