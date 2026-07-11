using System;
using System.Linq;
using IIIF.Manifests.Serializer.Shared.Selectors;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation;

/// <summary>
/// Reads/writes the polymorphic <see cref="AnnotationTarget"/> shape: a bare URI string, a typed
/// resource reference (optionally with a single <c>partOf</c>), or a full SpecificResource wrapping
/// a <see cref="ISelector"/> and/or a <c>styleClass</c> (recipe 0045-css).
/// </summary>
public class AnnotationTargetJsonConverter : JsonConverter<AnnotationTarget>
{
    public override void WriteJson(JsonWriter writer, AnnotationTarget? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        if (value.Selector is null && value.StyleClass is null)
        {
            WriteResourceReference(writer, value.SourceId, value.SourceType, (value.PartOfId, value.PartOfType));
            return;
        }

        writer.WriteStartObject();
        if (value.SpecificResourceId is not null)
        {
            writer.WritePropertyName("id");
            writer.WriteValue(value.SpecificResourceId);
        }

        writer.WritePropertyName("type");
        writer.WriteValue("SpecificResource");
        writer.WritePropertyName("source");
        WriteResourceReference(writer, value.SourceId, value.SourceType, (value.PartOfId, value.PartOfType));

        if (value.StyleClass is not null)
        {
            writer.WritePropertyName("styleClass");
            writer.WriteValue(value.StyleClass);
        }

        if (value.Selector is not null)
        {
            writer.WritePropertyName("selector");
            serializer.Serialize(writer, value.Selector);
        }

        writer.WriteEndObject();
    }

    private static void WriteResourceReference(JsonWriter writer, string id, string? type, (string? Id, string? Type) partOf)
    {
        if (type is null && partOf.Id is null)
        {
            writer.WriteValue(id);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("id");
        writer.WriteValue(id);
        if (type is not null)
        {
            writer.WritePropertyName("type");
            writer.WriteValue(type);
        }

        if (partOf.Id is not null)
        {
            writer.WritePropertyName("partOf");
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(partOf.Id);
            writer.WritePropertyName("type");
            writer.WriteValue(partOf.Type ?? "Manifest");
            writer.WriteEndObject();
            writer.WriteEndArray();
            writer.WriteEndObject();
            return;
        }

        writer.WriteEndObject();
    }

    public override AnnotationTarget? ReadJson(JsonReader reader, Type objectType, AnnotationTarget? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.Null)
        {
            return null;
        }

        if (token.Type == JTokenType.String)
        {
            return new AnnotationTarget(token.ToString());
        }

        var obj = (JObject)token;
        if ((string?)obj["type"] == "SpecificResource")
        {
            var target = ReadResourceReference(obj["source"]);
            if ((string?)obj["id"] is { } specificResourceId)
            {
                target.SetSpecificResourceId(specificResourceId);
            }

            if ((string?)obj["styleClass"] is { } styleClass)
            {
                target.SetStyleClass(styleClass);
            }

            if (obj["selector"] is { } selectorToken)
            {
                target.SetSelector(selectorToken.ToObject<ISelector>(serializer)!);
            }

            return target;
        }

        return ReadResourceReference(obj);
    }

    private static AnnotationTarget ReadResourceReference(JToken? token)
    {
        if (token is null || token.Type == JTokenType.Null)
        {
            throw new JsonSerializationException("Annotation target is missing a resource reference.");
        }

        if (token.Type == JTokenType.String)
        {
            return new AnnotationTarget(token.ToString());
        }

        var obj = (JObject)token;
        var id = (string?)obj["id"] ?? throw new JsonSerializationException("Annotation target resource is missing an id.");
        var target = new AnnotationTarget(id, (string?)obj["type"]);

        if (obj["partOf"]?.FirstOrDefault() is { } partOf)
        {
            target.SetPartOf((string?)partOf["id"] ?? string.Empty, (string?)partOf["type"] ?? "Manifest");
        }

        return target;
    }
}
