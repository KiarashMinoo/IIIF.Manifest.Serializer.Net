using IIIF.Manifests.Serializer.Shared.Trackable.Objects;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Selectors;

/// <summary>
///     A W3C "SvgSelector" - selects an arbitrary (typically non-rectangular) region via inline SVG
///     markup. The generic, core-library counterpart to the Georeference extension's
///     <c>GeoreferenceSvgSelector</c> (which stays in that extension package rather than depending on
///     it here, matching this SDK's established per-API small-type-duplication precedent - see e.g.
///     Auth/Discovery/Search's separate <c>*ResourceReference</c> types).
/// </summary>
public sealed class SvgSelector : TrackableObject<SvgSelector>, ISelector
{
    public const string TypeJName = "type";
    public const string ValueJName = "value";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private SvgSelector()
    {
    }

    [JsonConstructor]
    public SvgSelector(string value)
    {
        Type = "SvgSelector";
        Value = value;
    }

    [JsonProperty(ValueJName)]
    public string Value
    {
        get => GetElementValue(x => x.Value)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "SvgSelector";
        private set => SetElementValue(value);
    }
}