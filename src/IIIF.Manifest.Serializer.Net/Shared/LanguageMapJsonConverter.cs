using System;
using System.Collections.Generic;
using System.Linq;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared;

/// <summary>
/// Reads/writes an <see cref="IReadOnlyCollection{Label}"/> as a proper IIIF language map
/// (<c>{"none": ["..."]}</c>) rather than the bare-string/array shape <see cref="ObjectArrayJsonConverter"/>
/// produces for <c>BaseNode.Label</c> (that bare shape only becomes a real language map when passed
/// through <c>IiifSerializer</c>'s hand-built V3 writer). Auth 2.0's label-shaped fields
/// (label/heading/note/confirmLabel/errorHeading/errorNote) are embedded services, not top-level
/// Manifest/Canvas/etc. containers, so they never go through that writer and need to be spec-correct
/// on their own. Reads leniently: accepts a full language map, a bare array, or a bare string.
/// </summary>
public class LanguageMapJsonConverter : JsonConverter<IReadOnlyCollection<Label>>
{
    public override void WriteJson(JsonWriter writer, IReadOnlyCollection<Label>? value, JsonSerializer serializer)
    {
        var values = value?.Select(x => x.Value).Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? [];
        if (values.Count == 0)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("none");
        writer.WriteStartArray();
        foreach (var value1 in values)
        {
            writer.WriteValue(value1);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public override IReadOnlyCollection<Label>? ReadJson(JsonReader reader, Type objectType, IReadOnlyCollection<Label>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        return token.Type switch
        {
            JTokenType.Null => null,
            JTokenType.String => [new Label(token.ToString())],
            JTokenType.Array => token.Select(v => new Label(v.ToString())).ToList(),
            JTokenType.Object => ((JObject)token).Properties()
                .SelectMany(p => p.Value is JArray arr ? arr.Select(v => v.ToString()) : [p.Value.ToString()])
                .Select(v => new Label(v))
                .ToList(),
            _ => null
        };
    }
}
