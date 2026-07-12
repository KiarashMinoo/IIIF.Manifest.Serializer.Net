using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
///     Low-level JSON helpers (language-map building/reading, required-property lookups, @-prefix
///     renaming) shared across every other <c>IiifSerializer</c> partial file.
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
        foreach (var group in descriptions.GroupBy(x => x.Language ?? "none")) map[group.Key] = new JArray(group.Select(x => x.Value));

        return map;
    }

    private static JToken BuildLabelLanguageMapToken(IEnumerable<Label> labels)
    {
        var map = new JObject();
        foreach (var group in labels.GroupBy(x => x.Language ?? "none")) map[group.Key] = new JArray(group.Select(x => x.Value));

        return map;
    }

    private static List<Description> ReadDescriptions(JToken? token)
    {
        if (token is JObject languageMap)
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Select(value => prop.Name == "none" ? new Description(value) : new Description(value).SetLanguage(prop.Name)))
                .ToList();

        if (token is JArray array) return array.Values<string>().OfType<string>().Select(x => new Description(x)).ToList();

        var stringValue = (string?)token;
        return string.IsNullOrWhiteSpace(stringValue) ? [] : [new Description(stringValue)];
    }

    private static void WriteLanguageMap(JObject obj, string name, IEnumerable<string> values)
    {
        var array = new JArray(values.Where(x => !string.IsNullOrWhiteSpace(x)));
        if (array.Count > 0) obj[name] = new JObject { ["none"] = array };
    }

    private static IReadOnlyCollection<Label> ReadLabels(JToken? token)
    {
        if (token is JObject languageMap)
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => prop.Name == "none" ? new Label(x) : new Label(x, prop.Name)))
                .ToList();

        if (token is JArray array) return array.Values<string>().OfType<string>().Select(x => new Label(x)).ToList();

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

    /// <summary>
    ///     Writes every property an extension package (navPlace, TextGranularity, ...) attached via
    ///     the additional-properties mechanism (<see cref="IAdditionalPropertiesSupport{T}" />) back
    ///     into the V3 <paramref name="obj" /> being built. The hand-rolled V3 writer builds its
    ///     <see cref="JObject" /> field-by-field rather than going through Newtonsoft's automatic
    ///     property serialization, so it never reaches the <c>[JsonExtensionData]</c> bridge that
    ///     makes extension data survive a plain <c>JsonConvert.SerializeObject</c> call - without
    ///     this, an extension property set via e.g. <c>SetNavPlace</c>/<c>SetTextGranularity</c>
    ///     would silently vanish from <see cref="IiifSerializer" />'s V3 output. Safe by
    ///     construction: only <c>IsAdditional</c>-flagged <see cref="ElementDescriptor" /> entries
    ///     are written, and no core-modeled property is ever marked additional.
    /// </summary>
    private static void WriteV3AdditionalProperties<TTrackableObject>(TrackableObject<TTrackableObject> node, JObject obj)
        where TTrackableObject : TrackableObject<TTrackableObject>
    {
        foreach (var kvp in node.ElementDescriptors.Where(x => x.Value.IsAdditional))
            obj[kvp.Key] = kvp.Value.Value is JToken token ? token : JToken.FromObject(kvp.Value.Value!, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));
    }

    /// <summary>
    ///     The read-side counterpart of <see cref="WriteV3AdditionalProperties{TTrackableObject}" />:
    ///     if <paramref name="key" /> is present on <paramref name="obj" />, stores it as a raw,
    ///     additional-flagged <see cref="ElementDescriptor" /> so the corresponding extension
    ///     package's getter (e.g. <c>NavPlace</c>/<c>TextGranularity</c>) can read it back via
    ///     lazy on-first-access conversion, the same as if it had arrived through the
    ///     <c>[JsonExtensionData]</c> bridge. Named per-key rather than a generic "sweep every
    ///     unrecognized property" pass because only this SDK's own approved extensions
    ///     (navPlace/Georeference/TextGranularity) are in scope - see SDK_VERSIONING_GUIDE.md Round
    ///     12; a fully generic sweep risks miscategorizing a not-yet-hand-rolled core V3 property.
    /// </summary>
    private static void ReadV3AdditionalProperty<TTrackableObject>(JObject obj, TrackableObject<TTrackableObject> node, string key)
        where TTrackableObject : TrackableObject<TTrackableObject>
    {
        if (obj[key] is { } token) node.ElementDescriptors[key] = new ElementDescriptor(token, true);
    }
}