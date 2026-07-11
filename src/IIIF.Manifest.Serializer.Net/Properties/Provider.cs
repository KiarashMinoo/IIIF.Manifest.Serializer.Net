using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
/// IIIF Provider property - describes the organization providing the resource (a W3C
/// Organization/Agent), optionally with its own homepage/logo/seeAlso (cookbook recipes
/// 0027-alternative-page-order, 0068-newspaper, 0234-provider). 3.0-only despite this class's
/// pre-existing "2.0" tag - 2.x had no Agent/provider concept at all.
/// </summary>
[PresentationAPI("3.0", Notes = "3.0-only. No 2.x equivalent (2.x had no Agent/provider concept).")]
public class Provider : FormattableItem<Provider>
{
    public const string LabelJName = "label";
    public const string HomepageJName = "homepage";
    public const string LogoJName = "logo";
    public const string SeeAlsoJName = "seeAlso";

    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(HomepageJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Homepage> Homepage
    {
        get => GetElementValue(x => x.Homepage) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(LogoJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<Logo> Logo
    {
        get => GetElementValue(x => x.Logo) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(SeeAlsoJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<SeeAlso> SeeAlso
    {
        get => GetElementValue(x => x.SeeAlso) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public Provider(string id) : base(id, "Agent")
    {
    }

    public Provider(string id, Label label) : this(id)
    {
        Label = [label];
    }

    public Provider(string id, string label) : this(id, new Label(label))
    {
    }

    public Provider SetLabel(IReadOnlyCollection<Label> label)
    {
        Label = label;
        return this;
    }

    public Provider AddHomepage(Homepage homepage)
    {
        Homepage = Homepage.With(homepage);
        return this;
    }

    public Provider AddLogo(Logo logo)
    {
        Logo = Logo.With(logo);
        return this;
    }

    public Provider AddSeeAlso(SeeAlso seeAlso)
    {
        SeeAlso = SeeAlso.With(seeAlso);
        return this;
    }
}
