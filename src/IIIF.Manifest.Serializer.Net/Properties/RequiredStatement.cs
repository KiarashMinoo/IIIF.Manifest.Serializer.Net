using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF Presentation API 3.0 requiredStatement - a single {label, value} pair. Replaces the
///     2.x bare-string/language-map "attribution" property (a structural change, not a rename:
///     2.x attribution carries no label, so the legacy view synthesizes one).
/// </summary>
[PresentationAPI("3.0", Notes = "Replaces attribution from API 2.x. Structural change: attribution has no label, requiredStatement does.")]
public class RequiredStatement : TrackableObject<RequiredStatement>
{
    public const string LabelJName = "label";
    public const string ValueJName = "value";

    [JsonConstructor]
    public RequiredStatement(IReadOnlyCollection<Label> label, IReadOnlyCollection<Description> value)
    {
        Label = label;
        Value = value;
    }

    public RequiredStatement(Label label, Description value) : this([label], [value])
    {
    }

    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(ValueJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Description> Value
    {
        get => GetElementValue(x => x.Value) ?? [];
        private set => SetElementValue(value);
    }
}