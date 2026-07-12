using System.Text.Json;
using System.Text.Json.Serialization;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;

namespace IIIF.Manifests.Serializer.SystemTextJson;

/// <summary>
///     Bridges <c>System.Text.Json</c> to this SDK's own Newtonsoft-based
///     <see cref="AnnotationCollection" /> read/write logic, so
///     <c>System.Text.Json.JsonSerializer.Serialize(annotationCollection)</c> (and ASP.NET Core's
///     default request/response (de)serialization) produce the same correct IIIF JSON as calling
///     <see cref="IiifSerializer.Serialize(AnnotationCollection)" />/
///     <see cref="IiifSerializer.DeserializeAnnotationCollection" /> directly - with no extra
///     configuration required from the consumer.
/// </summary>
public sealed class AnnotationCollectionSystemTextJsonConverter : JsonConverter<AnnotationCollection>
{
    public override AnnotationCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return IiifSerializer.DeserializeAnnotationCollection(document.RootElement.GetRawText());
    }

    public override void Write(Utf8JsonWriter writer, AnnotationCollection value, JsonSerializerOptions options)
    {
        using var document = JsonDocument.Parse(IiifSerializer.Serialize(value));
        document.RootElement.WriteTo(writer);
    }
}