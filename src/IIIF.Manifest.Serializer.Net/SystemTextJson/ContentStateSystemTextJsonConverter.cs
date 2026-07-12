using System;
using System.Text.Json;
using IIIF.Manifests.Serializer.Shared.Trackable;
using ContentStateDocument = IIIF.Manifests.Serializer.Nodes.Contents.ContentState.ContentState;

namespace IIIF.Manifests.Serializer.SystemTextJson;

/// <summary>
/// Bridges <c>System.Text.Json</c> to this SDK's own Newtonsoft-based Content State 1.0 read/write
/// logic, so <c>System.Text.Json.JsonSerializer.Serialize(contentState)</c> (and ASP.NET Core's
/// default request/response (de)serialization) produce the same correct JSON as calling
/// <c>contentState.Serialize()</c>/<c>TrackableObject.Parse&lt;ContentState&gt;</c> directly - with
/// no extra configuration required from the consumer. Content State has no legacy shape, so unlike
/// Manifest/Collection/AnnotationCollection this doesn't go through <see cref="IiifSerializer"/>'s
/// version dispatch at all.
/// </summary>
public sealed class ContentStateSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<ContentStateDocument>
{
    public override ContentStateDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return TrackableObject.Parse<ContentStateDocument>(document.RootElement.GetRawText());
    }

    public override void Write(Utf8JsonWriter writer, ContentStateDocument value, JsonSerializerOptions options)
    {
        using var document = JsonDocument.Parse(value.Serialize());
        document.RootElement.WriteTo(writer);
    }
}
