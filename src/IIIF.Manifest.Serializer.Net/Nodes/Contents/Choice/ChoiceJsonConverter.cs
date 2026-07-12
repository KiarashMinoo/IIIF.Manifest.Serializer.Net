using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Choice;

public class ChoiceJsonConverter : JsonConverter<Choice>
{
    public override void WriteJson(JsonWriter writer, Choice? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("type");
        writer.WriteValue("Choice");
        writer.WritePropertyName("items");
        writer.WriteStartArray();
        foreach (var item in value.Items) serializer.Serialize(writer, item);
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public override Choice? ReadJson(JsonReader reader, Type objectType, Choice? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        if (token.Type == JTokenType.Null) return null;

        var obj = (JObject)token;
        var items = (obj["items"] as JArray ?? [])
            .Select(item => item.ToObject<IBaseResource>(serializer))
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        return new Choice(items);
    }
}