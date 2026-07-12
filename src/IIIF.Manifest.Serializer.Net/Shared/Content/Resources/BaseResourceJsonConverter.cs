using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources;

/// <summary>
///     JSON converter for deserializing <see cref="Nodes.Contents.Annotation.Annotation" />'s
///     polymorphic <c>body</c> (declared as <see cref="IBaseResource" />) into the correct concrete
///     resource type by inspecting <c>@type</c>/<c>type</c>. Without this, only <c>IiifSerializer</c>'s
///     hand-built V3 Canvas reader could resolve a body's concrete type; a standalone
///     <c>Annotation</c> (e.g. a Content Search 2.0 result) round-tripped through plain
///     <c>JsonConvert</c>/<c>TrackableObject.Parse</c> would throw trying to instantiate the
///     <see cref="IBaseResource" /> interface directly.
/// </summary>
public class BaseResourceJsonConverter : JsonConverter<IBaseResource>
{
    private const string TypeJName = "@type";

    // Same recursion-guard need as ServiceJsonConverter: IBaseResource carries this converter so
    // the Body property (declared IBaseResource) resolves it, but every implementer (ImageResource
    // etc.) also inherits it - reading/writing a concrete leaf type here must skip this converter
    // for the leaf type itself, or ToObject<TResource>()/serializer.Serialize would re-enter this
    // method (recursing on read, silently writing null via ReferenceLoopHandling.Ignore on write).
    // Public so extension-assembly IBaseResource implementers (e.g. navPlace's Feature, cookbook
    // recipe 0139) can register themselves with ResourceTypeRegistry using the same safe serializer
    // rather than duplicating the recursion-guard contract resolver.
    public static readonly JsonSerializer LeafSerializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        Formatting = TrackableObject.JsonSerializerSettings.Formatting,
        NullValueHandling = TrackableObject.JsonSerializerSettings.NullValueHandling,
        DefaultValueHandling = TrackableObject.JsonSerializerSettings.DefaultValueHandling,
        ReferenceLoopHandling = TrackableObject.JsonSerializerSettings.ReferenceLoopHandling,
        ContractResolver = new LeafContractResolver()
    });

    public override IBaseResource? ReadJson(JsonReader reader, Type objectType, IBaseResource? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;

        var obj = JObject.Load(reader);
        var typeValue = (string?)obj[TypeJName] ?? (string?)obj["type"];

        return typeValue switch
        {
            "dctypes:Image" or "Image" => obj.ToObject<ImageResource>(LeafSerializer),
            "dctypes:Sound" or "Sound" => obj.ToObject<AudioResource>(LeafSerializer),
            "dctypes:MovingImage" or "Video" => obj.ToObject<VideoResource>(LeafSerializer),
            "cnt:ContentAsText" or "Text" => obj.ToObject<EmbeddedContentResource>(LeafSerializer),
            "TextualBody" => obj.ToObject<TextualBody>(LeafSerializer),
            "Choice" => obj.ToObject<Choice>(LeafSerializer),
            "SpecificResource" => obj.ToObject<SpecificResource>(LeafSerializer),
            not null when ResourceTypeRegistry.TryCreate(typeValue, obj) is { } registered => registered,
            _ => obj.ToObject<BaseResource>(LeafSerializer)
        };
    }

    public override void WriteJson(JsonWriter writer, IBaseResource? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        // value's runtime type also inherits this converter via the interface attribute, so
        // serializer.Serialize(writer, value) would re-enter this method. Use LeafSerializer to
        // write the concrete object's own properties instead of recursing back into this converter.
        LeafSerializer.Serialize(writer, value);
    }

    private sealed class LeafContractResolver : IIIFJsonContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            var converter = base.ResolveContractConverter(objectType);
            return converter is BaseResourceJsonConverter && objectType != typeof(IBaseResource) ? null : converter;
        }
    }
}