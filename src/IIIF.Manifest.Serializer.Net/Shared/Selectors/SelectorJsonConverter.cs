using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Selectors;

/// <summary>
///     Dispatches a polymorphic <see cref="ISelector" /> to the correct concrete type by inspecting
///     <c>type</c>. Same recursion-guard need as <c>ServiceJsonConverter</c>/<c>BaseResourceJsonConverter</c>:
///     <see cref="ISelector" /> carries this converter so element-typed properties resolve it, but every
///     implementer inherits it too, so reading/writing a concrete leaf type must skip this converter
///     for the leaf type itself.
/// </summary>
public class SelectorJsonConverter : JsonConverter<ISelector>
{
    private static readonly JsonSerializer LeafSerializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        Formatting = TrackableObject.JsonSerializerSettings.Formatting,
        NullValueHandling = TrackableObject.JsonSerializerSettings.NullValueHandling,
        DefaultValueHandling = TrackableObject.JsonSerializerSettings.DefaultValueHandling,
        ReferenceLoopHandling = TrackableObject.JsonSerializerSettings.ReferenceLoopHandling,
        ContractResolver = new LeafContractResolver()
    });

    public override ISelector? ReadJson(JsonReader reader, Type objectType, ISelector? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var obj = JObject.Load(reader);
        return (string?)obj["type"] switch
        {
            "FragmentSelector" => obj.ToObject<FragmentSelector>(LeafSerializer),
            "PointSelector" => obj.ToObject<PointSelector>(LeafSerializer),
            "ImageApiSelector" => obj.ToObject<ImageApiSelector>(LeafSerializer),
            "SvgSelector" => obj.ToObject<SvgSelector>(LeafSerializer),
            _ => throw new JsonSerializationException($"Unknown selector type '{obj["type"]}'.")
        };
    }

    public override void WriteJson(JsonWriter writer, ISelector? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        LeafSerializer.Serialize(writer, value);
    }

    private sealed class LeafContractResolver : IIIFJsonContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            var converter = base.ResolveContractConverter(objectType);
            return converter is SelectorJsonConverter && objectType != typeof(ISelector) ? null : converter;
        }
    }
}