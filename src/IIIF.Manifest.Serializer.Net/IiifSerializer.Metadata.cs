using System.Linq;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.MetadataProperty.MetadataValue;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
    private static JObject WriteV3Metadata(Metadata metadata)
    {
        return new JObject
        {
            ["label"] = BuildLanguageMapToken([metadata.Label]),
            ["value"] = BuildLanguageMapValueToken(metadata.Value)
        };
    }

    private static JToken BuildLanguageMapValueToken(IEnumerable<MetadataValue> values)
    {
        var map = new JObject();
        foreach (var group in values.GroupBy(x => x.Language ?? "none"))
        {
            map[group.Key] = new JArray(group.Select(x => x.Value));
        }

        return map;
    }

    private static Metadata ReadV3Metadata(JObject obj)
    {
        var label = ReadLabels(obj["label"]).FirstOrDefault()?.Value ?? string.Empty;
        var metadata = new Metadata(label, string.Empty);
        var values = ReadMetadataValues(obj["value"]);
        if (values.Count > 0)
        {
            metadata.ResetValue(values[0]);
            foreach (var value in values.Skip(1))
            {
                metadata.AddValue(value);
            }
        }

        return metadata;
    }

    private static List<MetadataValue> ReadMetadataValues(JToken? token)
    {
        if (token is JObject languageMap)
        {
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Select(value => prop.Name == "none" ? new MetadataValue(value) : new MetadataValue(value, prop.Name)))
                .ToList();
        }

        var value = (string?)token;
        return string.IsNullOrWhiteSpace(value) ? [] : [new MetadataValue(value)];
    }
}
