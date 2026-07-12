using System.Text.Json;
using System.Text.Json.Serialization;
using IIIF.Manifests.Serializer.Nodes;

namespace IIIF.Manifests.Serializer.SystemTextJson;

/// <summary>
///     Bridges <c>System.Text.Json</c> to this SDK's own Newtonsoft-based <see cref="Manifest" />
///     read/write logic, so <c>System.Text.Json.JsonSerializer.Serialize(manifest)</c> (and ASP.NET
///     Core's default request/response (de)serialization) produce the same correct, version-aware IIIF
///     JSON as calling <see cref="IiifSerializer.Serialize(Manifest)" />/
///     <see cref="IiifSerializer.DeserializeManifest" /> directly - with no extra configuration required
///     from the consumer.
/// </summary>
public sealed class ManifestSystemTextJsonConverter : JsonConverter<Manifest>
{
    public override Manifest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return IiifSerializer.DeserializeManifest(document.RootElement.GetRawText());
    }

    public override void Write(Utf8JsonWriter writer, Manifest value, JsonSerializerOptions options)
    {
        using var document = JsonDocument.Parse(IiifSerializer.Serialize(value));
        document.RootElement.WriteTo(writer);
    }
}