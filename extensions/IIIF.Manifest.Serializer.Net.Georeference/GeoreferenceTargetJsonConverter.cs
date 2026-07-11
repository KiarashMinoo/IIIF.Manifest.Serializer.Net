using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Reads/writes the polymorphic Georeference Annotation target shape: a bare Canvas/Image Service
/// URI, a full resource object (<c>{"id","type","height","width"}</c>), or a SpecificResource
/// wrapping a <see cref="GeoreferenceSvgSelector"/> - see <see cref="GeoreferenceTarget"/>.
/// </summary>
public class GeoreferenceTargetJsonConverter : JsonConverter<GeoreferenceTarget>
{
    public override void WriteJson(JsonWriter writer, GeoreferenceTarget? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        if (value.Selector is null)
        {
            WriteResource(writer, value.SourceId, value.SourceType, value.SourceHeight, value.SourceWidth);
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
        WriteResource(writer, value.SourceId, value.SourceType, value.SourceHeight, value.SourceWidth);
        writer.WritePropertyName("selector");
        serializer.Serialize(writer, value.Selector);
        writer.WriteEndObject();
    }

    private static void WriteResource(JsonWriter writer, string id, string? type, int? height, int? width)
    {
        if (type is null && height is null && width is null)
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

        if (height is not null)
        {
            writer.WritePropertyName("height");
            writer.WriteValue(height.Value);
        }

        if (width is not null)
        {
            writer.WritePropertyName("width");
            writer.WriteValue(width.Value);
        }

        writer.WriteEndObject();
    }

    public override GeoreferenceTarget? ReadJson(JsonReader reader, Type objectType, GeoreferenceTarget? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);

        if (token.Type == JTokenType.String)
        {
            return new GeoreferenceTarget(token.ToString());
        }

        var obj = (JObject)token;
        if ((string?)obj["type"] == "SpecificResource")
        {
            var target = ReadResource(obj["source"]);
            if ((string?)obj["id"] is { } specificResourceId)
            {
                target.SetSpecificResourceId(specificResourceId);
            }

            if (obj["selector"] is { } selectorToken)
            {
                target.SetSelector(selectorToken.ToObject<GeoreferenceSvgSelector>(serializer)!);
            }

            return target;
        }

        return ReadResource(obj);
    }

    private static GeoreferenceTarget ReadResource(JToken? token)
    {
        if (token is null)
        {
            throw new JsonSerializationException("Georeference Annotation target is missing a resource reference.");
        }

        if (token.Type == JTokenType.String)
        {
            return new GeoreferenceTarget(token.ToString());
        }

        var obj = (JObject)token;
        var id = (string?)obj["id"] ?? throw new JsonSerializationException("Georeference Annotation target resource is missing an id.");
        var target = new GeoreferenceTarget(id, (string?)obj["type"]);

        var height = (int?)obj["height"];
        var width = (int?)obj["width"];
        if (height is not null && width is not null)
        {
            target.SetSourceDimensions(height.Value, width.Value);
        }

        return target;
    }
}
