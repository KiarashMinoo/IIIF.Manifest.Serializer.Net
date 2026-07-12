using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;

/// <summary>
///     The W3C Web Annotation Model's "TextualBody" - an inline string value (never dereferenceable,
///     so unlike other <see cref="IBaseResource" /> resources it has no <c>id</c>) used as an
///     <see cref="Nodes.Contents.Annotation.Annotation" />'s body for commenting/tagging/transcribing
///     motivations (cookbook recipes 0019/0021/0103/0135/0258/0261/0269/0306/0326/0346/0464/0489/0561,
///     among others). Distinct from the legacy 2.x <c>cnt:ContentAsText</c> shape modeled by
///     <see cref="Embedded.Resource.EmbeddedContentResource" />.
/// </summary>
[PresentationAPI("3.0")]
public class TextualBody : TrackableObject<TextualBody>, IBaseResource
{
    public const string TypeJName = "type";
    public const string ValueJName = "value";
    public const string FormatJName = "format";
    public const string LanguageJName = "language";

    [JsonConstructor]
    public TextualBody(string value)
    {
        Type = "TextualBody";
        Value = value;
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "TextualBody";
        private set => SetElementValue(value);
    }

    [JsonProperty(ValueJName)]
    public string Value
    {
        get => GetElementValue(x => x.Value)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(FormatJName)]
    public string? Format
    {
        get => GetElementValue(x => x.Format);
        private set => SetElementValue(value);
    }

    [JsonProperty(LanguageJName)]
    public string? Language
    {
        get => GetElementValue(x => x.Language);
        private set => SetElementValue(value);
    }

    ResourceType? IBaseResource.Type => new(Type);

    public TextualBody SetFormat(string format)
    {
        Format = format;
        return this;
    }

    public TextualBody SetLanguage(string language)
    {
        Language = language;
        return this;
    }
}