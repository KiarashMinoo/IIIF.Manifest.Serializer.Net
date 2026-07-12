using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
///     A W3C "TextQuoteSelector" - identifies matched text within a search result Annotation's body by
///     the exact matched text plus surrounding context, used by <see cref="SearchHitAnnotation" />.
/// </summary>
[SearchAPI("2.0")]
public class SearchTextQuoteSelector : TrackableObject<SearchTextQuoteSelector>
{
    public const string TypeJName = "type";
    public const string PrefixJName = "prefix";
    public const string ExactJName = "exact";
    public const string SuffixJName = "suffix";

    [JsonConstructor]
    public SearchTextQuoteSelector(string exact)
    {
        Type = "TextQuoteSelector";
        Exact = exact;
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "TextQuoteSelector";
        private set => SetElementValue(value);
    }

    [JsonProperty(PrefixJName)]
    public string? Prefix
    {
        get => GetElementValue(x => x.Prefix);
        private set => SetElementValue(value);
    }

    [JsonProperty(ExactJName)]
    public string Exact
    {
        get => GetElementValue(x => x.Exact)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(SuffixJName)]
    public string? Suffix
    {
        get => GetElementValue(x => x.Suffix);
        private set => SetElementValue(value);
    }

    public SearchTextQuoteSelector SetPrefix(string prefix)
    {
        Prefix = prefix;
        return this;
    }

    public SearchTextQuoteSelector SetSuffix(string suffix)
    {
        Suffix = suffix;
        return this;
    }
}