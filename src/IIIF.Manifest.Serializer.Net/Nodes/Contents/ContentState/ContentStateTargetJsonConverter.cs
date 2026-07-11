using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
/// Reads/writes the polymorphic Content State 1.0 target shape: a bare URI string, a typed
/// resource reference (<c>{"id","type"}</c>), or a full SpecificResource (<c>{"type":
/// "SpecificResource","source","selector"}</c>) - see <see cref="ContentStateTarget"/>.
/// </summary>
public class ContentStateTargetJsonConverter : JsonConverter<ContentStateTarget>
{
    public override void WriteJson(JsonWriter writer, ContentStateTarget? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        if (value.Selector is null && value.PartOfId is null)
        {
            WriteResourceReference(writer, value.Id, value.ResourceType, null);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("type");
        writer.WriteValue("SpecificResource");
        writer.WritePropertyName("source");
        WriteResourceReference(writer, value.Id, value.ResourceType, (value.PartOfId, value.PartOfType));
        if (value.Selector is not null)
        {
            writer.WritePropertyName("selector");
            serializer.Serialize(writer, value.Selector);
        }
        writer.WriteEndObject();
    }

    private static void WriteResourceReference(JsonWriter writer, string id, string? resourceType, (string? Id, string? Type)? partOf)
    {
        if (resourceType is null && partOf is null)
        {
            writer.WriteValue(id);
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("id");
        writer.WriteValue(id);
        if (resourceType is not null)
        {
            writer.WritePropertyName("type");
            writer.WriteValue(resourceType);
        }

        if (partOf?.Id is not null)
        {
            writer.WritePropertyName("partOf");
            writer.WriteStartArray();
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue(partOf.Value.Id);
            writer.WritePropertyName("type");
            writer.WriteValue(partOf.Value.Type ?? "Manifest");
            writer.WriteEndObject();
            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }

    public override ContentStateTarget? ReadJson(JsonReader reader, Type objectType, ContentStateTarget? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.String)
        {
            return new ContentStateTarget(token.ToString());
        }

        var obj = (JObject)token;
        if ((string?)obj["type"] == "SpecificResource")
        {
            var target = ReadResourceReference(obj["source"]);
            if (obj["selector"] is { } selectorToken)
            {
                target.SetSelector(selectorToken.ToObject<ContentStateFragmentSelector>(serializer)!);
            }

            return target;
        }

        return ReadResourceReference(obj);
    }

    private static ContentStateTarget ReadResourceReference(JToken? token)
    {
        if (token is null)
        {
            throw new JsonSerializationException("Content State target is missing a resource reference.");
        }

        if (token.Type == JTokenType.String)
        {
            return new ContentStateTarget(token.ToString());
        }

        var obj = (JObject)token;
        var id = (string?)obj["id"] ?? throw new JsonSerializationException("Content State target resource is missing an id.");
        var target = new ContentStateTarget(id, (string?)obj["type"]);

        if (obj["partOf"]?.FirstOrDefault() is { } partOf)
        {
            target.SetPartOf((string?)partOf["id"] ?? string.Empty, (string?)partOf["type"] ?? "Manifest");
        }

        return target;
    }
}
