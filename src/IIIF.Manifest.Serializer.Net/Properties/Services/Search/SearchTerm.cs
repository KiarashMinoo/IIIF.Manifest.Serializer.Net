using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Search;

/// <summary>
/// One suggestion in a <see cref="TermPageResponse"/> - the autocomplete response body. Only
/// <see cref="Value"/> is required; a bare <c>{"value": "..."}</c> is a valid minimal Term.
/// </summary>
[SearchAPI("2.0")]
public class SearchTerm : TrackableObject<SearchTerm>
{
    public const string TypeJName = "type";
    public const string ValueJName = "value";
    public const string TotalJName = "total";
    public const string LabelJName = "label";
    public const string LanguageJName = "language";
    public const string ServiceJName = "service";

    [JsonProperty(TypeJName)]
    public string? Type
    {
        get => GetElementValue(x => x.Type);
        private set => SetElementValue(value);
    }

    [JsonProperty(ValueJName)]
    public string Value
    {
        get => GetElementValue(x => x.Value)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(TotalJName)]
    public int? Total
    {
        get => GetElementValue(x => x.Total);
        private set => SetElementValue(value);
    }

    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(LanguageJName)]
    public string? Language
    {
        get => GetElementValue(x => x.Language);
        private set => SetElementValue(value);
    }

    [JsonProperty(ServiceJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<IBaseService> Service
    {
        get => GetElementValue(x => x.Service) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public SearchTerm(string value)
    {
        Value = value;
    }

    public SearchTerm SetType(string type)
    {
        Type = type;
        return this;
    }

    public SearchTerm SetTotal(int total)
    {
        Total = total;
        return this;
    }

    public SearchTerm SetLabel(string label) => SetElementValue(x => x.Label, (IReadOnlyCollection<Label>)[new Label(label)]);

    public SearchTerm SetLanguage(string language)
    {
        Language = language;
        return this;
    }

    public SearchTerm AddService(IBaseService service)
    {
        Service = Service.With(service);
        return this;
    }
}
