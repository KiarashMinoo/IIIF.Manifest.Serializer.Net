using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
public sealed class Rendering : FormattableItem<Rendering>
{
    public const string LabelJName = "label";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Rendering()
    {
    }

    [JsonConstructor]
    public Rendering(string id, string label) : base(id)
    {
        SetElementValue(x => x.Label, label);
    }

    [JsonProperty(LabelJName)]
    public string Label
    {
        get => GetElementValue(x => x.Label)!;
        private set => SetElementValue(value);
    }
}