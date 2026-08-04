using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
public sealed class Within(string id) : BaseItem<Within>(id)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Within() : this(null!)
    {
    }

    public const string LabelJName = "label";

    [JsonProperty(LabelJName)]
    public string? Label
    {
        get => GetElementValue(x => x.Label);
        private set => SetElementValue(value);
    }

    public Within SetLabel(string label)
    {
        Label = label;
        return this;
    }
}