using System.Linq;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Low-level JSON helpers (language-map building/reading, required-property lookups, @-prefix
/// renaming) shared across every other <c>IiifSerializer</c> partial file.
/// </summary>
public static partial class IiifSerializer
{
    private static JToken BuildLanguageMapToken(IEnumerable<string> values)
    {
        var list = values.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        return new JObject { ["none"] = new JArray(list) };
    }

    private static JToken BuildDescriptionLanguageMapToken(IEnumerable<Description> descriptions)
    {
        var map = new JObject();
        foreach (var group in descriptions.GroupBy(x => x.Language ?? "none"))
        {
            map[group.Key] = new JArray(group.Select(x => x.Value));
        }

        return map;
    }

    private static JToken BuildLabelLanguageMapToken(IEnumerable<Label> labels)
    {
        var map = new JObject();
        foreach (var group in labels.GroupBy(x => x.Language ?? "none"))
        {
            map[group.Key] = new JArray(group.Select(x => x.Value));
        }

        return map;
    }

    private static List<Description> ReadDescriptions(JToken? token)
    {
        if (token is JObject languageMap)
        {
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Select(value => prop.Name == "none" ? new Description(value) : new Description(value).SetLanguage(prop.Name)))
                .ToList();
        }

        if (token is JArray array)
        {
            return array.Values<string>().OfType<string>().Select(x => new Description(x)).ToList();
        }

        var stringValue = (string?)token;
        return string.IsNullOrWhiteSpace(stringValue) ? [] : [new Description(stringValue)];
    }

    private static void WriteLanguageMap(JObject obj, string name, IEnumerable<string> values)
    {
        var array = new JArray(values.Where(x => !string.IsNullOrWhiteSpace(x)));
        if (array.Count > 0)
        {
            obj[name] = new JObject { ["none"] = array };
        }
    }

    private static IReadOnlyCollection<Label> ReadLabels(JToken? token)
    {
        if (token is JObject languageMap)
        {
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => prop.Name == "none" ? new Label(x) : new Label(x, prop.Name)))
                .ToList();
        }

        if (token is JArray array)
        {
            return array.Values<string>().OfType<string>().Select(x => new Label(x)).ToList();
        }

        var value = (string?)token;
        return string.IsNullOrWhiteSpace(value) ? [] : [new Label(value)];
    }

    private static IEnumerable<string> ReadStringArray(JToken? token)
    {
        return token switch
        {
            JArray array => array.Values<string>().OfType<string>().Where(x => !string.IsNullOrWhiteSpace(x)),
            _ when !string.IsNullOrWhiteSpace((string?)token) => [(string)token!],
            _ => []
        };
    }

    private static string ReadRequiredString(JObject obj, string name)
    {
        return (string?)obj[name] ?? throw new JsonSerializationException($"Required property '{name}' is missing.");
    }

    private static void Rename(JObject obj, string oldName, string newName)
    {
        if (obj[oldName] is not null)
        {
            obj[newName] = obj[oldName];
            obj.Remove(oldName);
        }
    }
}
